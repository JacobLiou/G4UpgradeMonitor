using Sofar.CommunicationLib.Service.AppModels;
using Sofar.ProtocolLibs.FirmwareInfo;
using Sofar.ProtocolLibs.Modbus.Message.Sofar;
using Sofar.ProtocolLibs.Utils.CRC;
using System.Diagnostics;

namespace Sofar.CommunicationLib.Service.FileTransfer
{
    public class G4UpgradeService : ServiceBase
    {
        private class G4UpgradeContext
        {
            public byte[] SlavesList;
            public G4UpgradeConfig Config;
            public IProgress<G4UpgradeProgressInfo> PgReporter;
            public Dictionary<byte, G4UpgradeProgressInfo> DevPgInfoDict;
            public byte[] FirmwareBytes;
            public SofarPackageInfoV2? SofarPackInfo;
            public BinFirmwareInfoV3? BinPackInfo = null;
            public bool IsSofarMode;
        }

        public Task G4FirmwareUpgradeAsync(IEnumerable<byte> slaves, string firmwarePath, G4UpgradeConfig config,
            CancellationToken cancellationToken, IProgress<G4UpgradeProgressInfo> pgReporter)
        {
            // if (!_longRunningEvent.WaitOne(100))
            // {
            //     throw new InvalidOperationException("Another Modbus long-running task is in progress.");
            // }

            if (_modbusClient == null)
            {
                throw new InvalidOperationException("No Modbus Connection.");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    G4FirmwareUpgrade(slaves, firmwarePath, config, cancellationToken, pgReporter);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw;
                }
                finally
                {
                    // _longRunningEvent.Set();
                }
            }, TaskCreationOptions.LongRunning);
        }

        // 广播升级单个sofar包：(请求固件下发->下发固件->查询位图->补包)->(总校验->启动升级->查询进度)；
        // 单播升级单个sofar包：(启动固件下发->下发固件(每包即时校验/重发))->(总校验->启动升级->查询进度)
        private void G4FirmwareUpgrade(IEnumerable<byte> slaves, string firmwarePath, G4UpgradeConfig config,
            CancellationToken cancellationToken, IProgress<G4UpgradeProgressInfo> pgReporter)
        {
            #region 固件导入

            SofarPackageInfoV2? sofarPackInfo = null;
            BinFirmwareInfoV3? binPackInfo = null;
            string firmwareName = "";
            byte[] firmwareBytes = null;

            bool isSofar = false;
            if (firmwarePath.EndsWith(".sofar"))
            {
                isSofar = true;
                firmwareBytes = File.ReadAllBytes(firmwarePath);
                sofarPackInfo = new SofarPackageInfoV2(firmwareBytes.TakeLast(SofarPackageInfoV2.SIGNATURE_SIZE).ToArray());
                firmwareName = sofarPackInfo.PackageName;
            }
            else if (firmwarePath.EndsWith(".bin"))
            {
                binPackInfo = new BinFirmwareInfoV3(firmwareBytes.TakeLast(BinFirmwareInfoV3.SIGNATURE_SIZE).ToArray());
                firmwareName = binPackInfo.ProjectName;
            }
            else
            {
                throw new ArgumentException("The firmware path is invalid.");
            }

            uint crc32 = CRCHelper.ComputeCrc32(firmwareBytes, CRCHelper.Crc32Method.CRC32);

            #endregion 固件导入

            #region 升级流程上下文

            var slavesList = slaves.ToArray();
            var devPgInfoDict = new Dictionary<byte, G4UpgradeProgressInfo>();    // 进度信息字典，当某个slave中途失败，须将其移出
            foreach (byte slave in slavesList)
            {
                G4UpgradeProgressInfo devPgInfo = new()
                {
                    Slave = slave,
                    FileType = config.IsSofarOrBin ? FirmwareFileType.None : binPackInfo!.FirmwareFileType,
                    ChipRole = config.IsSofarOrBin ? FirmwareChipRole.None : binPackInfo!.FirmwareChipRole,
                    Progress = 0,
                    Stage = G4UpgradeStage.None,
                    Message = null,
                };
                devPgInfoDict[slave] = devPgInfo;
            }

            G4UpgradeContext context = new G4UpgradeContext()
            {
                SlavesList = slavesList,
                Config = config,
                PgReporter = pgReporter,
                DevPgInfoDict = devPgInfoDict,
                SofarPackInfo = sofarPackInfo,
                BinPackInfo = binPackInfo,
                FirmwareBytes = firmwareBytes,
                IsSofarMode = isSofar,
            };

            #endregion 升级流程上下文

            try
            {
                #region <1> 请求固件下发

                int breakpoint = 0;
                G4UgStep_RequestToSendFirmware(context, cancellationToken, firmwareName,
                    firmwareBytes.Length, crc32, out breakpoint);

                #endregion <1> 请求固件下发

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1000);

                #region <2> 下发固件

                G4UgStep_SendFirmwareData(context, cancellationToken, firmwareBytes, crc32, breakpoint);

                #endregion <2> 下发固件

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1500);

                var resendFinishedList = new SortedSet<byte>();

                if (!config.IsBroadcast)
                    goto SkipResending;

                #region <3> loop{查询位图->补包}

                int resendRetry = 0;
                do
                {
                    #region <3.1> 查询位图

                    G4UgStep_ReadFirmBitmap(context, cancellationToken, firmwareBytes, crc32, ref resendFinishedList, out SortedSet<int> lostPacks);

                    #endregion <3.1> 查询位图

                    if (resendFinishedList.Count == devPgInfoDict.Count)
                    {
                        break;
                    }
                    Thread.Sleep(500);

                    #region <3.2> 广播补包

                    G4UgStep_BroadcastLostPacks(context, cancellationToken, firmwareBytes, crc32, resendFinishedList, lostPacks);

                    #endregion <3.2> 广播补包

                    Thread.Sleep(500);
                    resendRetry++;
                } while (resendRetry < config.ResendLostsMaxRetries + 1);

                // 报告补包失败的设备
                foreach (var slave in slavesList)
                {
                    if (!devPgInfoDict.ContainsKey(slave) || resendFinishedList.Contains(slave))
                        continue;
                    devPgInfoDict[slave].Failed = true;
                    devPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                    devPgInfoDict[slave].Message = $"Failed to patch the bitmap after {config.ResendLostsMaxRetries} tries.";
                    pgReporter.Report(devPgInfoDict[slave]);
                    devPgInfoDict.Remove(slave);
                }

                #endregion <3> loop{查询位图->补包}

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(500);

SkipResending:

#region <4> 总校验

                G4UgStep_VerifyFirmware(context, cancellationToken, crc32);

                #endregion <4> 总校验

                /*此后步骤，有的设备或已启动升级，因此不再支持中途取消*/

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(500);

                #region <5.a> 设置定时升级

                if (config.UpgradeTime != null)
                {
                    G4UgStep_SetUpgradeAlarm(context);
                    return;
                }

                #endregion <5.a> 设置定时升级

                #region <5.b> 启动升级

                G4UgStep_StartUpgrade(context);

                #endregion <5.b> 启动升级

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(500);

                // goto Exit;  // Debug: 直接不查进度

                #region <6> 轮询进度

                G4UgStep_CheckProgress(context, cancellationToken);

                #endregion <6> 轮询进度
            }
            catch (OperationCanceledException oce)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    foreach (var slave in slavesList)
                    {
                        if (!devPgInfoDict.ContainsKey(slave))
                            continue;

                        devPgInfoDict[slave].Stage = G4UpgradeStage.Cancelled;
                        devPgInfoDict[slave].Message = "The upgrading task was cancelled.";
                        devPgInfoDict[slave].Failed = true;
                        pgReporter.Report(devPgInfoDict[slave]);
                        devPgInfoDict.Remove(slave);
                    }
                }
            }
        }

        private void G4UgStep_RequestToSendFirmware(in G4UpgradeContext ctx, CancellationToken cancellationToken,
                                            string firmwareName, int firmwareSize, uint crc32, out int breakpoint)
        {
            breakpoint = 0;
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 请求发送固件");

                    WriteFileStartResponse? response;
                    response = ctx.Config.UseNew5001 ?
                        _modbusClient.G4StartWritingFile(slave, firmwareName, (uint)firmwareSize, crc32) :
                        _modbusClient.G4StartWritingFile_Old(slave, firmwareName, (uint)firmwareSize, crc32);

                    if (response != null && (response.ResultCode == 0 || response.ResultCode == 1))
                    {
                        // 请求成功
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.RequestToSendFile;
                        ctx.DevPgInfoDict[slave].Message = "Ready to send the firmware.";
                        ctx.DevPgInfoDict[slave].Progress = 100;
                        if (response.ResultCode == 1)
                        {
                            breakpoint = (int)response.BreakPoint;
                        }
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.RequestToSendFile;
                        ctx.DevPgInfoDict[slave].Message = "Failed to request sending the firmware.";
                        if (response != null)
                            ctx.DevPgInfoDict[slave].Message += $" Result Code: {response.ResultCode}";
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                        break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(500);
            }
        }

        private void G4UgStep_SendFirmwareData(in G4UpgradeContext ctx, CancellationToken cancellationToken,
                                            byte[] firmwareBytes, uint crc32, int breakpoint)
        {
            breakpoint = !ctx.Config.IsBroadcast && ctx.Config.ResumeFromBreakPoint ? breakpoint : 0;
            int firmwareByteCount = firmwareBytes.Length - breakpoint;
            int sendCount = firmwareByteCount / ctx.Config.SendingSegmentSize +
                            (firmwareByteCount % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);

            long t1 = 0, t2 = 0;

            for (int i = 0; i < sendCount; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentSize = ctx.Config.SendingSegmentSize;
                if (currentSize > firmwareByteCount - i * ctx.Config.SendingSegmentSize)
                    currentSize = firmwareByteCount - i * ctx.Config.SendingSegmentSize;
                bool result = false;
                int errorCode = -1;
                if (ctx.Config.IsBroadcast) // 广播发包
                {
                    for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                    {
                        _logger.Information($"广播发送固件");
                        result = G4BroadcastFileData(firmwareBytes.Skip(i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray(),
                            (uint)(i * ctx.Config.SendingSegmentSize), crc32);
                        if (result) break;
                        Thread.Sleep(ctx.Config.SendingInterval);
                    }

                    Thread.Sleep(ctx.Config.SendingInterval);
                }
                else // 单播发包
                {
                    foreach (var slave in ctx.SlavesList)
                    {
                        if (!ctx.DevPgInfoDict.ContainsKey(slave))
                            continue;
                        for (int retry = 0; retry < ctx.Config.ResendLostsMaxRetries; retry++)
                        {
                            _logger.Information($"设备{slave}, 发送固件");

                            // t1 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            //
                            // _logger.Debug($"Sending Interval: {t1 - t2}ms");

                            var response = _modbusClient.G4WriteFileData(slave, firmwareBytes.Skip(breakpoint + i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray(),
                               (uint)(i * ctx.Config.SendingSegmentSize), crc32);

                            // t2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            //
                            // _logger.Debug($"Sending Duration: {t2 - t1}ms");

                            if (response != null)
                            {
                                if (response.ResultCode == 0)
                                {
                                    result = true;
                                    break;
                                }
                                else
                                {
                                    errorCode = response.ResultCode;
                                }
                            }
                            Thread.Sleep(ctx.Config.SendingInterval);
                        }

                        Thread.Sleep(ctx.Config.SendingInterval);
                    }
                }

                // 周期更新进度
                if (i % ctx.Config.SendPackReportIntervals == 0
                    || i + 1 == sendCount || result == false)
                {
                    foreach (var slave in ctx.SlavesList)
                    {
                        if (!ctx.DevPgInfoDict.ContainsKey(slave))
                            continue;

                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.SendingFile;

                        if (result == false)
                        {
                            ctx.DevPgInfoDict[slave].Failed = true;
                            ctx.DevPgInfoDict[slave].Message = "Failed to send package.";
                            if (errorCode > 0)
                                ctx.DevPgInfoDict[slave].Message += $" Result Code: {errorCode}";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                            ctx.DevPgInfoDict.Remove(slave);
                        }
                        else
                        {
                            var pgValue = (i + 1.0) / sendCount * 100;
                            ctx.DevPgInfoDict[slave].Progress = (int)Math.Round(pgValue, 0);
                            ctx.DevPgInfoDict[slave].Message = "Sending...";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        }
                    }
                }
            }
        }

        // 下发固件数据段（广播）
        private bool G4BroadcastFileData(byte[] fileSegmentBytes, uint offset, uint fileCrc32)
        {
            var requestEntity = new WriteFileDataRequest(fileSegmentBytes, offset, fileCrc32);
            return _modbusClient.ModbusTryBroadcast(requestEntity);
        }

        private void G4UgStep_ReadFirmBitmap(in G4UpgradeContext ctx, CancellationToken cancellationToken,
                                            byte[] firmwareBytes, uint crc32, ref SortedSet<byte> resendFinishedList, out SortedSet<int> lostPacks)
        {
            int totalBits = firmwareBytes.Length / ctx.Config.SendingSegmentSize + (firmwareBytes.Length % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);
            int totalBitmapBytes = totalBits / 8 + (totalBits % 8 == 0 ? 0 : 1);
            int queryCount = totalBitmapBytes / ctx.Config.BitmapSegmentSize +
                             (totalBitmapBytes % ctx.Config.BitmapSegmentSize == 0 ? 0 : 1);  // 查一位图所需要的查询次数

            lostPacks = new SortedSet<int>();

            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave) || resendFinishedList.Contains(slave))
                    continue;

                bool noLost = true;
                bool queryResult = true;
                for (int i = 0; i < queryCount; i++)  // 查一个设备的位图
                {
                    int currentSegmentIdx = i;
                    int currentSegmentSize = ctx.Config.BitmapSegmentSize;
                    if (currentSegmentSize > totalBitmapBytes - i * ctx.Config.BitmapSegmentSize)
                        currentSegmentSize = totalBitmapBytes - i * ctx.Config.BitmapSegmentSize;

                    for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                    {
                        byte[]? bitmapBytes = null;
                        _logger.Information($"设备{slave}, 查询位图");

                        if (ctx.Config.Use5108)
                        {
                            var response = _modbusClient.G4ReadFileBitmap(slave, (uint)currentSegmentIdx, (ushort)currentSegmentSize);
                            if (response != null && response.BitmapSegmentSize == currentSegmentSize)
                                bitmapBytes = response.BitmapBytes;
                        }
                        else
                        {
                            var response = _modbusClient.G4ReadFileBitmap_Old(slave, (uint)currentSegmentIdx, (ushort)currentSegmentSize);
                            if (response != null && response.BitmapSegmentSize == currentSegmentSize)
                                bitmapBytes = response.BitmapBytes;
                        }

                        if (bitmapBytes != null)
                        {
                            int currentBitCount = ctx.Config.BitmapSegmentSize * 8;
                            if (currentBitCount > totalBits - i * ctx.Config.BitmapSegmentSize * 8)
                                currentBitCount = totalBits - i * ctx.Config.BitmapSegmentSize * 8;
                            int currentBitOffset = i * ctx.Config.BitmapSegmentSize * 8;

                            // 合并各个设备的位图
                            for (int p = 0; p < bitmapBytes.Length; p++)
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    if (p * 8 + k < currentBitCount &&
                                        (bitmapBytes[p] & 0x80 >> k) != 0x80 >> k)
                                    {
                                        int lostPackIdx = currentBitOffset + p * 8 + k;
                                        lostPacks.Add(lostPackIdx);
                                        noLost = false;   // 发生丢包
                                    }
                                }
                            }
                            break;
                        }
                        else
                        {
                            noLost = false;
                            if (retry == ctx.Config.RequestMaxTries - 1)
                            {
                                ctx.DevPgInfoDict[slave].Failed = true;
                                ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                                ctx.DevPgInfoDict[slave].Message = "Failed to check the bitmap.";
                                ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                                ctx.DevPgInfoDict.Remove(slave);
                                queryResult = false;
                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }

                    if (!queryResult)
                    {
                        break;
                    }

                    Thread.Sleep(500);
                }

                if (noLost)
                {
                    // 对包齐全的设备不再复查
                    ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                    ctx.DevPgInfoDict[slave].Progress = 100;
                    ctx.DevPgInfoDict[slave].Message = "All packs of the firmware have been received by the device.";
                    resendFinishedList.Add(slave);
                    ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                }

                Thread.Sleep(300);

                if (ctx.SlavesList.Length > 1 && resendFinishedList.Count != ctx.DevPgInfoDict.Count)
                {
                    G4UgStep_KeepAlive(ctx, cancellationToken, firmwareBytes, crc32);
                    Thread.Sleep(100);
                }
            }
        }

        private void G4UgStep_KeepAlive(in G4UpgradeContext ctx, CancellationToken cancellationToken, byte[] firmwareBytes, uint crc32)
        {
            /*广播升级查询位图过程中，插播发送第一包固件使设备保持于升级状态*/
            bool result = false;
            for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
            {
                _logger.Information($"广播补包(保活包)");
                int packIdx = 0;
                int currentSize = ctx.Config.SendingSegmentSize;
                if (currentSize > firmwareBytes.Length - packIdx * ctx.Config.SendingSegmentSize)
                    currentSize = firmwareBytes.Length - packIdx * ctx.Config.SendingSegmentSize;
                result = G4BroadcastFileData(firmwareBytes.Skip(packIdx * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray(),
                    (uint)(packIdx * ctx.Config.SendingSegmentSize), crc32);
                if (result) break;
                Thread.Sleep(ctx.Config.SendingInterval * 2);
            }

            if (!result)
            {
                foreach (var slave in ctx.SlavesList)
                {
                    if (!ctx.DevPgInfoDict.ContainsKey(slave))
                        continue;

                    ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                    ctx.DevPgInfoDict[slave].Failed = true;
                    ctx.DevPgInfoDict[slave].Message = "Failed to broadcast the keep-alive packs.";
                    ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                    ctx.DevPgInfoDict.Remove(slave);
                }
            }
        }

        private void G4UgStep_BroadcastLostPacks(in G4UpgradeContext ctx, CancellationToken cancellationToken,
                                            byte[] firmwareBytes, uint crc32, in SortedSet<byte> resendFinishedList, in SortedSet<int> lostPacks)
        {
            int lostPacksCnt = 0;
            foreach (int packIdx in lostPacks)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentSize = ctx.Config.SendingSegmentSize;
                if (currentSize > firmwareBytes.Length - packIdx * ctx.Config.SendingSegmentSize)
                    currentSize = firmwareBytes.Length - packIdx * ctx.Config.SendingSegmentSize;

                bool result = false;
                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"广播补包");
                    result = G4BroadcastFileData(firmwareBytes.Skip(packIdx * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray(),
                        (uint)(packIdx * ctx.Config.SendingSegmentSize), crc32);
                    if (result) break;
                    Thread.Sleep(ctx.Config.SendingInterval * 2);
                }

                if (lostPacksCnt % ctx.Config.SendPackReportIntervals == 0
                    || lostPacksCnt + 1 == lostPacks.Count || result == false)  // 报告补包中的设备
                {
                    foreach (var slave in ctx.SlavesList)
                    {
                        if (!ctx.DevPgInfoDict.ContainsKey(slave) || resendFinishedList.Contains(slave))
                            continue;

                        if (result)
                        {
                            var pgValue = (lostPacksCnt + 1.0) / lostPacks.Count * 100;
                            ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                            ctx.DevPgInfoDict[slave].Progress = (int)Math.Round(pgValue, 0);
                            ctx.DevPgInfoDict[slave].Message = "Resending the lost packs...";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.ResendingLostPacks;
                            ctx.DevPgInfoDict[slave].Failed = true;
                            ctx.DevPgInfoDict[slave].Message = "Failed to resend the lost packs.";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                            ctx.DevPgInfoDict.Remove(slave);
                        }
                    }
                }

                lostPacksCnt++;
                Thread.Sleep(ctx.Config.SendingInterval);
            }
        }

        private void G4UgStep_VerifyFirmware(in G4UpgradeContext ctx, CancellationToken cancellationToken, uint crc32)
        {
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 固件校验");

                    var response = _modbusClient.G4VerifyTransferredFile(slave, crc32);
                    if (response != null && response.FileCrc32 == crc32)
                    {
                        // 校验成功
                        ctx.DevPgInfoDict[slave].Message = "Successfully verified the firmware.";
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.Verification;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Message = "Failed to verify the firmware. ";
                        if (response != null)
                            ctx.DevPgInfoDict[slave].Message += $" CRC32 Check Error: [Slave/Local:{response.FileCrc32.ToString("X8")}/{crc32.ToString("X8")}]";

                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.Verification;
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                    }
                    Thread.Sleep(200);
                }
                Thread.Sleep(200);
            }
        }

        private void G4UgStep_SetUpgradeAlarm(in G4UpgradeContext ctx)
        {
            if (ctx.Config.UpgradeTime == null)
            {
                return;
            }

            foreach (var slave in ctx.SlavesList)
            {
                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    var response = _modbusClient.G4SetUpgradeTime(slave, ctx.Config.UpgradeTime.Value);
                    if (response != null)
                    {
                        // 设置成功
                        ctx.DevPgInfoDict[slave].Message = "Set upgrade time successfully.";
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.SetAlarm;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }
                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Message = "Failed to set upgrade time.";
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.SetAlarm;
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                        break;
                    }
                    Thread.Sleep(200);
                }
                Thread.Sleep(200);
            }
        }

        private void G4UgStep_StartUpgrade(in G4UpgradeContext ctx)
        {
            foreach (var slave in ctx.SlavesList)
            {
                // if (cancellationToken.IsCancellationRequested) // 此后步骤不可取消

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 启动升级");

                    var response = _modbusClient.G4StartUpgrade(slave);
                    if (response != null)
                    {
                        // 启动成功
                        ctx.DevPgInfoDict[slave].Message = "Start upgrade successfully.";
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.StartUpgrade;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Message = "Failed to start upgrade.";
                        ctx.DevPgInfoDict[slave].Stage = G4UpgradeStage.StartUpgrade;
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                        break;
                    }
                    Thread.Sleep(200);
                }
                Thread.Sleep(200);
            }
        }

        private void G4UgStep_CheckProgress(in G4UpgradeContext ctx, CancellationToken cancellationToken)
        {
            var progressDictTree = new Dictionary<byte, (long RefreshTime, Dictionary<ushort, int> ProgressDict)>();
            int refreshTimeoutSeconds = 40;

            if (ctx.IsSofarMode)
            {
                var subFirmwareInfos = ctx.SofarPackInfo.SubFirmwareInfos;
                foreach (var slave in ctx.SlavesList)
                {
                    if (!ctx.DevPgInfoDict.ContainsKey(slave))
                        continue;
                    var subDict = new Dictionary<ushort, int>();
                    foreach (var fwInfo in subFirmwareInfos)
                    {
                        ushort id = (ushort)((byte)fwInfo.FirmwareFileType << 8 | (byte)fwInfo.FirmwareChipRole);
                        int progress = 0;
                        subDict[id] = progress;
                    }
                    long refreshTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    progressDictTree[slave] = new(refreshTime, subDict);
                }
            }
            else
            {
                foreach (var slave in ctx.SlavesList)
                {
                    if (!ctx.DevPgInfoDict.ContainsKey(slave))
                        continue;
                    var subDict = new Dictionary<ushort, int>();
                    ushort id = (ushort)((byte)ctx.BinPackInfo.FirmwareFileType << 8 | (byte)ctx.BinPackInfo.FirmwareChipRole);
                    byte progress = 0;
                    subDict[id] = progress;
                    long refreshTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    progressDictTree[slave] = new(refreshTime, subDict);
                }
            }

            var checkProgressInfo = new G4UpgradeProgressInfo();

            do
            {
                foreach (var slave in ctx.SlavesList)
                {
                    if (!progressDictTree.ContainsKey(slave))
                        continue;

                    checkProgressInfo.Slave = slave;

                    var currentDict = progressDictTree[slave].ProgressDict;
                    long prevRefreshTime = progressDictTree[slave].RefreshTime;

                    bool isFrozen = true;
                    foreach (var kv in currentDict)
                    {
                        ushort id = kv.Key;
                        int prevProgress = kv.Value;

                        FirmwareFileType fileType = (FirmwareFileType)(id >> 8);
                        FirmwareChipRole chipRole = (FirmwareChipRole)(id & 0xFF);
                        checkProgressInfo.FileType = fileType;
                        checkProgressInfo.ChipRole = chipRole;

                        _logger.Information($"设备{slave}, 查询进度");
                        var response = _modbusClient.G4CheckUpgradeStatus_Progress(slave, (byte)fileType, (byte)chipRole);
                        if (response != null
                            && response.FileType == (byte)fileType
                            && response.ChipRole == (byte)chipRole
                            && response.Progress <= 100)
                        {
                            int pgValue = response.Progress;
                            if (pgValue == 100)
                            {
                                isFrozen = false;
                                checkProgressInfo.Stage = G4UpgradeStage.Finished;
                                checkProgressInfo.Failed = false;
                                checkProgressInfo.Progress = pgValue;
                                checkProgressInfo.Message = $"Upgrade of [{fileType}:{chipRole}] finished.";
                                ctx.PgReporter.Report(checkProgressInfo);
                                currentDict.Remove(id);
                            }
                            else if (pgValue >= prevProgress)
                            {
                                checkProgressInfo.Stage = G4UpgradeStage.CheckProgress;
                                checkProgressInfo.Failed = false;
                                checkProgressInfo.Progress = pgValue;
                                checkProgressInfo.Message = $"[0x{(byte)fileType:X2}:0x{(byte)chipRole:X2}] is upgrading...({pgValue}%)";
                                ctx.PgReporter.Report(checkProgressInfo);

                                if (pgValue > prevProgress)
                                {
                                    isFrozen = false;
                                    currentDict[id] = (byte)pgValue;
                                    long refreshTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                    progressDictTree[slave] = new(refreshTime, currentDict);
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }

                    if (isFrozen && currentDict.Count > 0 && prevRefreshTime > 0
                        && DateTimeOffset.Now.ToUnixTimeMilliseconds() - prevRefreshTime > refreshTimeoutSeconds * 1000)
                    {
                        checkProgressInfo.Stage = G4UpgradeStage.CheckProgress;
                        checkProgressInfo.Failed = true;
                        checkProgressInfo.FileType = FirmwareFileType.None;
                        checkProgressInfo.ChipRole = FirmwareChipRole.None;
                        checkProgressInfo.Message =
                            $"The upgrading progress of [Slave{slave}] has been frozen for over {refreshTimeoutSeconds} seconds.";
                        ctx.PgReporter.Report(checkProgressInfo);
                        progressDictTree.Remove(slave);
                        break;
                    }
                    // Thread.Sleep(500);
                }

                if (progressDictTree.Count == 0 ||
                    progressDictTree.All(dict => dict.Value.ProgressDict.Count == 0))
                {
                    break;
                }
                Thread.Sleep(3000);
            } while (true);
        }
    }
}