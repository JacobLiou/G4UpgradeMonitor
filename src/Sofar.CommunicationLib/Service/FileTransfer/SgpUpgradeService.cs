using Sofar.CommunicationLib.Service.AppModels;
using Sofar.ProtocolLibs.FirmwareInfo;
using Sofar.ProtocolLibs.SofarSGP.Info;
using Sofar.ProtocolLibs.Utils.CRC;
using System.Diagnostics;

namespace Sofar.CommunicationLib.Service.FileTransfer
{
    public class SgpUpgradeService : ServiceBase
    {
        #region 升级任务

        private class SgpUpgradeContext
        {
            public byte[] SlavesList;
            public SgpUpgradeConfig Config;
            public IProgress<SgpUpgradeProgressInfo> PgReporter;
            public Dictionary<byte, SgpUpgradeProgressInfo> DevPgInfoDict;
            public List<byte[]> FirmwaresDataList;
            public SofarPackageInfoV2? SofarPackInfo = null;
            public List<BinFirmwareInfoV3>? BinInfoList = null;
            public bool IsSofarMode;
        }

        public Task SgpFirmwareUpgradeAsync(byte[] slaves, string[] firmwaresPath, SgpUpgradeConfig config,
            CancellationToken cancellationToken, IProgress<SgpUpgradeProgressInfo> pgReporter)
        {
            // if (!_longRunningEvent.WaitOne(100))
            // {
            //     throw new InvalidOperationException("Another Modbus long-running task is in progress.");
            // }

            if (_modbusClient == null)
            {
                throw new InvalidOperationException("No Modbus Connection.");
            }

            if (config.UpgradeTime != null && (config.UpgradeTime.Value.Year < 2000 || config.UpgradeTime.Value.Year > 2099))
            {
                throw new ArgumentException("The year value of upgrade time must be between 2000 and 2099.");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    SgpFirmwareUpgrade(slaves, firmwaresPath, config, cancellationToken, pgReporter);
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

        // 单个固件广播升级：握手->{请求固件下发->下发固件->{查询位图->补包}->总校验}->启动升级->查询进度；
        // 单个固件单播升级：握手->{请求固件下发->下发固件（每包即时校验/补发）->总校验}->启动升级->查询进度；
        private void SgpFirmwareUpgrade(byte[] slaves, string[] firmwaresPath, SgpUpgradeConfig config,
            CancellationToken cancellationToken, IProgress<SgpUpgradeProgressInfo> pgReporter)
        {
            #region 固件导入

            bool isSofar = false;
            List<byte[]> firmwaresDataList = new();
            SofarPackageInfoV2? sofarPackInfo = null;
            List<BinFirmwareInfoV3>? binInfoList = null;
            if (firmwaresPath.Length == 1 && firmwaresPath[0].EndsWith(".sofar"))
            {
                isSofar = true;
                firmwaresDataList.Add(File.ReadAllBytes(firmwaresPath[0]));
                sofarPackInfo = new SofarPackageInfoV2(firmwaresDataList[0].TakeLast(SofarPackageInfoV2.SIGNATURE_SIZE).ToArray());
            }
            else if (firmwaresPath.Length > 0 && firmwaresPath.All(path => path.EndsWith(".bin")))
            {
                isSofar = false;
                binInfoList = new();
                for (int i = 0; i < firmwaresPath.Length; i++)
                {
                    byte[] binBytes = File.ReadAllBytes(firmwaresPath[i]);
                    firmwaresDataList.Add(binBytes);
                    binInfoList.Add(new BinFirmwareInfoV3(binBytes.TakeLast(BinFirmwareInfoV3.SIGNATURE_SIZE).ToArray()));
                }
            }
            else
            {
                throw new ArgumentException("The list of the firmware path is invalid.");
            }

            #endregion 固件导入

            #region 升级流程上下文

            var slavesList = slaves.ToArray();
            var devPgInfoDict = new Dictionary<byte, SgpUpgradeProgressInfo>();
            foreach (var slave in slavesList)
            {
                SgpUpgradeProgressInfo devPgInfo = new()
                {
                    Slave = slave,
                    FileType = FirmwareFileType.None,
                    ChipRole = FirmwareChipRole.None,
                    ProtocolVersion = 0x21,
                    Progress = 0,
                    Stage = SgpUpgradeStage.None,
                    Message = null,
                };
                devPgInfoDict[slave] = devPgInfo;
            }

            SgpUpgradeContext context = new SgpUpgradeContext()
            {
                SlavesList = slavesList,
                Config = config,
                FirmwaresDataList = firmwaresDataList,
                BinInfoList = binInfoList,
                PgReporter = pgReporter,
                SofarPackInfo = sofarPackInfo,
                DevPgInfoDict = devPgInfoDict,
                IsSofarMode = isSofar,
            };

            #endregion 升级流程上下文

            // 升级
            try
            {
                #region <1> 握手

                SgpFwStep_ShakeHands(context, cancellationToken);

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1000);

                #endregion <1> 握手

                #region <1.5> 协议版本兼容性检查

                if (isSofar == false)
                {
                    for (int n = 0; n < binInfoList!.Count; n++)
                    {
                        foreach (var slave in slavesList)
                        {
                            if (!devPgInfoDict.ContainsKey(slave))
                                continue;
                            // 协议版本0x01不支持ARM, DSPM, DSPS以外的升级
                            if (devPgInfoDict[slave].ProtocolVersion == 0x01
                                && binInfoList[n].FirmwareChipRole != FirmwareChipRole.ARM_OLD
                                && binInfoList[n].FirmwareChipRole != FirmwareChipRole.DSPM_OLD
                                && binInfoList[n].FirmwareChipRole != FirmwareChipRole.DSPS_OLD)
                            {
                                devPgInfoDict[slave].ChipRole = ChipRoleHelper.ConvertToNewChipRole(binInfoList[n].FirmwareChipRole);
                                devPgInfoDict[slave].Stage = SgpUpgradeStage.CheckCompatibility;
                                devPgInfoDict[slave].Message =
                                    $"The protocol version '0x01' does not support for {devPgInfoDict[slave].ChipRole} upgrade.";
                                devPgInfoDict[slave].Failed = true;
                                pgReporter.Report(devPgInfoDict[slave]);
                                devPgInfoDict.Remove(slave);
                            }
                        }
                    }
                }

                if (devPgInfoDict.Count == 0)
                    return;

                #endregion <1.5> 协议版本兼容性检查

                #region <2~5> loop{请求固件下发->下发固件->loop{查询位图->补包}->总校验}

                for (int n = 0; n < firmwaresDataList.Count; n++)
                {
                    byte[] firmwareBytes = firmwaresDataList[n];

                    uint crcId = 0;
                    uint programStart = 0;
                    if (isSofar)
                    {
                        crcId = CRCHelper.ComputeCrc32(firmwareBytes, CRCHelper.Crc32Method.CRC32);
                        programStart = 0;
                    }
                    else
                    {
                        crcId = config.UseBinCrc32 ? binInfoList[n].Crc32 : CRCHelper.ComputeCrc32(firmwareBytes, CRCHelper.Crc32Method.CRC32);
                        programStart = config.UseBinProgramOffset ? binInfoList[n].ProgramOffset : 0;
                    }

                    #region <2> 请求下发固件

                    int breakpoint = 0;
                    SgpFwStep_RequestToSendFirmware(context, cancellationToken, firmwareBytes, n, crcId, out breakpoint);

                    if (devPgInfoDict.Count == 0)
                        return;
                    Thread.Sleep(1000);

                    #endregion <2> 请求下发固件

                    #region <3> 下发固件

                    SgpFwStep_SendFirmwareData(context, cancellationToken, firmwareBytes, crcId, programStart, breakpoint);

                    if (devPgInfoDict.Count == 0)
                        return;
                    Thread.Sleep(1000);

                    #endregion <3> 下发固件

                    if (!config.IsBroadcast)
                        goto SkipResending;

                    #region <4> loop{查询位图->补包}

                    var loopFinishedList = new SortedSet<byte>();
                    int resendRetry = 0;
                    do
                    {
                        #region <4.1> 查询位图

                        SgpFwStep_ReadFirmwareBitmap(context, cancellationToken, firmwareBytes.Length, crcId, ref loopFinishedList, out SortedSet<int> lostPacks);

                        if (loopFinishedList.Count == devPgInfoDict.Count)
                            break;
                        Thread.Sleep(500);

                        #endregion <4.1> 查询位图

                        #region <4.2> 广播补包

                        SgpFwStep_BroadcastLostPacks(context, cancellationToken, firmwareBytes, crcId, programStart, loopFinishedList, lostPacks);
                        Thread.Sleep(500);

                        #endregion <4.2> 广播补包

                        resendRetry++;
                    } while (resendRetry < config.ResendLostsMaxRetries + 1);

                    // 报告补包失败的设备
                    foreach (var slave in slavesList)
                    {
                        if (!devPgInfoDict.ContainsKey(slave) || loopFinishedList.Contains(slave))
                            continue;
                        devPgInfoDict[slave].Failed = true;
                        devPgInfoDict[slave].Stage = SgpUpgradeStage.ResendingLostPacks;
                        devPgInfoDict[slave].Message = $"Failed to patch the bitmap after {config.ResendLostsMaxRetries} tries.";
                        pgReporter.Report(devPgInfoDict[slave]);
                        devPgInfoDict.Remove(slave);
                    }

                    if (devPgInfoDict.Count == 0)
                        return;
                    Thread.Sleep(1000);

#endregion <4> loop{查询位图->补包}

SkipResending:

#region <5> 总校验

                    SgpFwStep_VerifyFirmware(context, cancellationToken, crcId);
                    Thread.Sleep(1000);

                    #endregion <5> 总校验
                }

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(500);

                #endregion <2~5> loop{请求固件下发->下发固件->loop{查询位图->补包}->总校验}

                /*此后步骤，有的设备或已启动升级，因此不再允许中途取消*/

                #region <6> 启动升级

                SgpFwStep_StartUpgrade(context);

                if (devPgInfoDict.Count == 0 || config.UpgradeTime != null)
                    return;
                Thread.Sleep(1000);

                #endregion <6> 启动升级

                #region <7> 轮询进度

                if (!config.EnabledCheckProgress)
                    return;

                if (isSofar)
                {
                    SgpFwStep_CheckSofarProgress(context, FirmwareFileType.None);
                }
                else
                {
                    SgpFwStep_CheckBinProgress(context);
                }

                #endregion <7> 轮询进度
            }
            catch (OperationCanceledException oce)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    foreach (var slave in slavesList)
                    {
                        if (!devPgInfoDict.ContainsKey(slave))
                            continue;

                        devPgInfoDict[slave].Stage = SgpUpgradeStage.Cancelled;
                        devPgInfoDict[slave].Message = "The upgrade task was cancelled.";
                        devPgInfoDict[slave].Failed = true;
                        pgReporter.Report(devPgInfoDict[slave]);
                        devPgInfoDict.Remove(slave);
                    }
                }
            }
        }

        private void SgpFwStep_ShakeHands(in SgpUpgradeContext ctx, CancellationToken cancellationToken)
        {
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 握手");
                    var response = _modbusClient.SgpTryShakeHandsByVersions(slave);
                    if (response != null)
                    {
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ShakeHands;
                        ctx.DevPgInfoDict[slave].Progress = 100;
                        ctx.DevPgInfoDict[slave].ProtocolVersion = response.VersionCode;
                        ctx.DevPgInfoDict[slave].Message = $"Successfully shake hands and confirm the protocol version: " +
                                                           $"0x{response.VersionCode:X2}";
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ShakeHands;
                        ctx.DevPgInfoDict[slave].Message = "Failed to shake hands";
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

        private void SgpFwStep_RequestToSendFirmware(in SgpUpgradeContext ctx, CancellationToken cancellationToken,
                                                    byte[] firmwareBytes, int binIdx, uint crcId, out int breakpoint)
        {
            breakpoint = 0;
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                string fwName = "";
                string fwVersion = "";
                uint fwSize = 0;
                byte reqChipRole = 0;
                byte reqFirmwareType = 0;

                if (ctx.IsSofarMode)
                {
                    fwName = ctx.SofarPackInfo!.PackageName;
                    if (fwName.Length > 20)
                        fwName = fwName.Substring(0, 20);
                    fwVersion = "";
                    fwSize = (uint)firmwareBytes.Length;
                    reqChipRole = 0xFF;
                    reqFirmwareType = 0x80;

                    ctx.DevPgInfoDict[slave].ChipRole = FirmwareChipRole.None;
                }
                else
                {
                    fwName = ctx.BinInfoList[binIdx].ProjectName;
                    fwVersion = ctx.BinInfoList[binIdx].SoftwareVersion;

                    if (ctx.BinInfoList[binIdx].SignatureVersion == 0x01)
                    {
                        byte[] fwSizeBytes = firmwareBytes.TakeLast(BinFirmwareInfoV3.SIGNATURE_SIZE + 8).Take(4).ToArray();
                        fwSize = (uint)(fwSizeBytes[0] | fwSizeBytes[1] << 8 | fwSizeBytes[2] << 16 | fwSizeBytes[3] << 24);
                    }
                    else
                    {
                        fwSize = ctx.BinInfoList[binIdx].FirmwareSize;
                    }

                    reqChipRole = ctx.DevPgInfoDict[slave].ProtocolVersion >= 0x21 ?
                        (byte)ChipRoleHelper.ConvertToNewChipRole(ctx.BinInfoList[binIdx].FirmwareChipRole) : (byte)ctx.BinInfoList[binIdx].FirmwareChipRole;

                    reqFirmwareType = (byte)ctx.BinInfoList[binIdx].FirmwareFileType;

                    #region 安规文件

                    if (ctx.BinInfoList[binIdx].SignatureVersion == 0x01
                        && ctx.BinInfoList[binIdx].FirmwareChipRole == 0)
                    {
                        reqFirmwareType = (byte)FirmwareFileType.Safety;
                        reqChipRole = 0;
                    }
                    else if (ctx.BinInfoList[binIdx].SignatureVersion >= 0x21
                             && ctx.BinInfoList[binIdx].FirmwareFileType == FirmwareFileType.Safety)
                    {
                        reqFirmwareType = (byte)FirmwareFileType.Safety;
                        reqChipRole = 0;
                    }

                    #endregion 安规文件

                    ctx.DevPgInfoDict[slave].FileType = ctx.BinInfoList[binIdx].FirmwareFileType;
                    ctx.DevPgInfoDict[slave].ChipRole = ChipRoleHelper.ConvertToNewChipRole(ctx.BinInfoList[binIdx].FirmwareChipRole);
                }

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    SgpFirmwareTransferStartResponse? response;
                    _logger.Information($"设备{slave}, 请求发送固件");
                    if (ctx.DevPgInfoDict[slave].ProtocolVersion == 0x01)
                    {
                        response = _modbusClient.SgpStartFirmwareTransfer_Old(slave, fwName, fwVersion, fwSize, crcId, reqChipRole, 20_000);
                    }
                    else
                    {
                        response = _modbusClient.SgpStartFirmwareTransfer(slave, fwName, fwVersion, fwSize, crcId, reqChipRole, reqFirmwareType, 20_000);
                    }

                    if (response != null && (response.ResultCode == 0 || response.ResultCode == 1))
                    {
                        // 请求成功
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.RequestToSendFile;
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
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.RequestToSendFile;
                        ctx.DevPgInfoDict[slave].Message = "Failed to request sending firmware.";
                        if (response != null)
                            ctx.DevPgInfoDict[slave].Message += $" Result Code: {response.ResultCode}";
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                        break;
                    }

                    Thread.Sleep(500);
                }

                Thread.Sleep(500);
            }
        }

        private void SgpFwStep_SendFirmwareData(in SgpUpgradeContext ctx, CancellationToken cancellationToken,
                                             byte[] firmwareBytes, uint crcId, uint programStart, int breakpoint)
        {
            breakpoint = !ctx.Config.IsBroadcast && ctx.Config.ResumeFromBreakPoint ? breakpoint : 0;
            int firmwareByteCount = firmwareBytes.Length - breakpoint;
            int sendCount = firmwareByteCount / ctx.Config.SendingSegmentSize + (firmwareByteCount % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);

            // long lastSendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

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
                        _logger.Information($"广播发送固件({i + 1}/{sendCount})");
                        result = SgpBroadcastFirmwareData(crcId, programStart + (uint)(i * ctx.Config.SendingSegmentSize),
                            firmwareBytes.Skip(i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());
                        // long t = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastSendTime;
                        // lastSendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        // Debug.WriteLine(t);
                        if (result) break;
                        Thread.Sleep(ctx.Config.SendingInterval);
                    }
                    Thread.Sleep(ctx.Config.SendingInterval);
                }
                else  // 单播发包
                {
                    foreach (var slave in ctx.SlavesList)
                    {
                        if (!ctx.DevPgInfoDict.ContainsKey(slave))
                            continue;
                        for (int retry = 0; retry < ctx.Config.ResendLostsMaxRetries; retry++)
                        {
                            _logger.Information($"设备{slave}, 发送固件({i + 1}/{sendCount})");
                            var response = _modbusClient.SgpTransferFirmwareData(slave, crcId, programStart + (uint)(i * ctx.Config.SendingSegmentSize),
                                firmwareBytes.Skip(breakpoint + i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());
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

                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.SendingFile;

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

        // 广播固件数据块
        private bool SgpBroadcastFirmwareData(uint firmwareID, uint segmentOffset, byte[] fileSegmentBytes)
        {
            var requestInfo = new SgpFirmwareTransferDataRequest(firmwareID, fileSegmentBytes, segmentOffset);
            return _modbusClient.SgpTryBroadcast(requestInfo, 0);
        }

        private void SgpFwStep_ReadFirmwareBitmap(in SgpUpgradeContext ctx, CancellationToken cancellationToken, int firmwareSize, uint crcId,
                                              ref SortedSet<byte> resendFinishedList, out SortedSet<int> lostPacks)
        {
            lostPacks = new SortedSet<int>();
            int totalBits = firmwareSize / ctx.Config.SendingSegmentSize + (firmwareSize % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);
            int totalBitmapBytes = totalBits / 8 + (totalBits % 8 == 0 ? 0 : 1);
            int queryCount = totalBitmapBytes / ctx.Config.BitmapSegmentSize
                             + (totalBitmapBytes % ctx.Config.BitmapSegmentSize == 0 ? 0 : 1);  // 查一位图所需要的查询次数

            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave) || resendFinishedList.Contains(slave))
                    continue;

                bool noLost = true;
                bool querySuccess = true;
                for (int i = 0; i < queryCount; i++)  // 查一个设备的位图
                {
                    int currentBitCount = ctx.Config.BitmapSegmentSize * 8;
                    if (currentBitCount > totalBits - i * ctx.Config.BitmapSegmentSize * 8)
                        currentBitCount = totalBits - i * ctx.Config.BitmapSegmentSize * 8;
                    int currentBitOffset = i * ctx.Config.BitmapSegmentSize * 8;

                    for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                    {
                        _logger.Information($"设备{slave}, 查询位图");
                        var response = _modbusClient.SgpReadFirmwareBitmap(slave, crcId, (ushort)currentBitOffset, (ushort)currentBitCount);
                        if (response != null && response.ResultCode == 0
                                             && response.BitCount == currentBitCount
                                             && response.BitOffset == (ushort)currentBitOffset)
                        {
                            // 合并各个设备的位图
                            for (int p = 0; p < response.BitmapBytes.Length; p++)
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    if (p * 8 + k < currentBitCount &&
                                        (response.BitmapBytes[p] & 1 << k) == 1 << k)
                                    {
                                        int lostPackIdx = currentBitOffset + p * 8 + k;
                                        if (lostPackIdx != totalBits - 1)
                                        {
                                            lostPacks.Add(lostPackIdx);
                                            noLost = false;   // 发生丢包
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        else
                        {
                            // 查询失败
                            if (retry + 1 == ctx.Config.RequestMaxTries)
                            {
                                ctx.DevPgInfoDict[slave].Failed = true;
                                ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ResendingLostPacks;
                                ctx.DevPgInfoDict[slave].Message = "Failed to check the bitmap.";
                                ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                                ctx.DevPgInfoDict.Remove(slave);
                                querySuccess = false;
                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }

                    if (!querySuccess)
                    {
                        break;
                    }

                    Thread.Sleep(250);
                }

                if (querySuccess && noLost)
                {
                    // 对包齐全的设备不再复查
                    ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ResendingLostPacks;
                    ctx.DevPgInfoDict[slave].Progress = 100;
                    ctx.DevPgInfoDict[slave].Message = "All segment pack of the firmware has been received by the device.";
                    resendFinishedList.Add(slave);
                    ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                }

                Thread.Sleep(500);
            }
        }

        private void SgpFwStep_BroadcastLostPacks(in SgpUpgradeContext ctx, CancellationToken cancellationToken, byte[] firmwareBytes, uint crcId,
                                                uint programStart, in SortedSet<byte> resendFinishedList, in SortedSet<int> lostPacks)
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
                    result = SgpBroadcastFirmwareData(crcId, programStart + (uint)(packIdx * ctx.Config.SendingSegmentSize),
                        firmwareBytes.Skip(packIdx * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());

                    if (result) break;
                    Thread.Sleep(ctx.Config.SendingInterval);
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
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ResendingLostPacks;
                            ctx.DevPgInfoDict[slave].Progress = (int)Math.Round(pgValue, 0);
                            ctx.DevPgInfoDict[slave].Message = "Resending the lost packs...";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.ResendingLostPacks;
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

        private void SgpFwStep_VerifyFirmware(in SgpUpgradeContext ctx, CancellationToken cancellationToken, uint crcId)
        {
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 固件校验");
                    var response = _modbusClient.SgpVerifyFirmware(slave, crcId, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 3000);
                    if (response != null && response.ResultCode == 0)
                    {
                        // 校验成功
                        ctx.DevPgInfoDict[slave].Message = "Successfully verified the package.";
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Verification;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Message = "Failed to verify the firmware.";
                        if (response != null)
                            ctx.DevPgInfoDict[slave].Message += $" Result Code: {response.ResultCode}";
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Verification;
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

        private void SgpFwStep_StartUpgrade(in SgpUpgradeContext ctx)
        {
            byte[] timeBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            if (ctx.Config.UpgradeTime != null)
            {
                timeBytes[0] = (byte)(ctx.Config.UpgradeTime.Value.Year - 2000);
                timeBytes[1] = (byte)ctx.Config.UpgradeTime.Value.Month;
                timeBytes[2] = (byte)ctx.Config.UpgradeTime.Value.Day;
                timeBytes[3] = (byte)ctx.Config.UpgradeTime.Value.Hour;
                timeBytes[4] = (byte)ctx.Config.UpgradeTime.Value.Minute;
                timeBytes[5] = (byte)ctx.Config.UpgradeTime.Value.Second;
            }

            byte[] chipRoles;
            uint[] firmwareIDs;
            if (ctx.IsSofarMode)
            {
                chipRoles = new byte[] { 0xFF };
                firmwareIDs = new uint[] { CRCHelper.ComputeCrc32(ctx.FirmwaresDataList[0], CRCHelper.Crc32Method.CRC32) };
            }
            else
            {
                chipRoles = new byte[ctx.BinInfoList.Count];
                firmwareIDs = new uint[ctx.BinInfoList.Count];
                for (int n = 0; n < ctx.BinInfoList.Count; n++)
                {
                    firmwareIDs[n] = ctx.BinInfoList[n].Crc32;
                }
            }

            foreach (var slave in ctx.SlavesList)
            {
                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                ctx.DevPgInfoDict[slave].ChipRole = FirmwareChipRole.None;
                ctx.DevPgInfoDict[slave].FileType = FirmwareFileType.None;
                if (!ctx.IsSofarMode)
                {
                    for (int n = 0; n < ctx.BinInfoList.Count; n++)
                    {
                        chipRoles[n] = ctx.DevPgInfoDict[slave].ProtocolVersion >= 0x21
                            ? (byte)ChipRoleHelper.ConvertToNewChipRole(ctx.BinInfoList[n].FirmwareChipRole)
                            : (byte)ctx.BinInfoList[n].FirmwareChipRole;

                        #region 安规文件

                        switch (ctx.BinInfoList[n].SignatureVersion)
                        {
                            case 0x01
                            when ctx.BinInfoList[n].FirmwareChipRole == 0:
                            case >= 0x21
                                 when ctx.BinInfoList[n].FirmwareFileType == FirmwareFileType.Safety:
                                chipRoles[n] = 0;
                                break;
                        }

                        #endregion 安规文件
                    }
                }

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 启动升级");
                    var response = _modbusClient.SgpStartUpgrade(slave, timeBytes, chipRoles, firmwareIDs);
                    if (response != null && response.ResultCodes.Length > 0 && response.ResultCodes[0] == 0)
                    {
                        if (ctx.Config.UpgradeTime != null) // 定时升级
                        {
                            ctx.DevPgInfoDict[slave].Message = "Set upgrade time successfully.";
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.SetAlarm;
                            ctx.DevPgInfoDict[slave].Progress = 100;
                        }
                        else // 即时升级
                        {
                            ctx.DevPgInfoDict[slave].Message = "Start upgrade successfully.";
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.StartUpgrade;
                            ctx.DevPgInfoDict[slave].Progress = 100;
                        }
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        if (ctx.Config.UpgradeTime != null)
                        {
                            ctx.DevPgInfoDict[slave].Message = "Failed to set upgrade time.";
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.SetAlarm;
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Message = "Failed to start upgrade.";
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.StartUpgrade;
                        }

                        if (response != null && response.ResultCodes.Length > 0)
                            ctx.DevPgInfoDict[slave].Message += $" Result Code: {response.ResultCodes[0]}";
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                    }
                    Thread.Sleep(500);
                }
                Thread.Sleep(500);
            }
        }

        private void SgpFwStep_CheckBinProgress(in SgpUpgradeContext ctx)
        {
            var upgradeFinishedList = new SortedSet<byte>();
            var ugProgressDict = new Dictionary<byte, (long PreviousRefreshTime, FirmwareChipRole PreviousChipRole)>();   // 记录每个设备的进度刷新时间
            int refreshTimeoutSeconds = 40;

            foreach (var slave in ctx.SlavesList)
            {
                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;
                ugProgressDict[slave] = new ValueTuple<long, FirmwareChipRole>(DateTimeOffset.Now.ToUnixTimeMilliseconds(), FirmwareChipRole.None);
                ctx.DevPgInfoDict[slave].Progress = 0;
            }

            do
            {
                foreach (var slave in ctx.SlavesList)
                {
                    if (!ctx.DevPgInfoDict.ContainsKey(slave) || upgradeFinishedList.Contains(slave))
                        continue;

                    // if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    //     continue;

                    int prevProgress = ctx.DevPgInfoDict[slave].Progress;
                    bool isFrozen = true;
                    _logger.Information($"设备{slave}, 查询进度");
                    bool result = ReadUpgradeProgress_203F(slave, out FirmwareChipRole chipRole, out byte progress);
                    if (result && ctx.DevPgInfoDict[slave].ProtocolVersion >= 0x21)
                        chipRole = ChipRoleHelper.ConvertToNewChipRole(chipRole);

                    if (result && chipRole == FirmwareChipRole.None)
                    {
                        if (ugProgressDict[slave].PreviousChipRole != FirmwareChipRole.None)
                        {
                            ctx.DevPgInfoDict[slave].ChipRole = FirmwareChipRole.None;
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Finished;
                            ctx.DevPgInfoDict[slave].Progress = 100;
                            ctx.DevPgInfoDict[slave].Failed = false;
                            ctx.DevPgInfoDict[slave].Message = $"The device {slave} has finished upgrade.";
                            ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                            upgradeFinishedList.Add(slave);
                            continue;
                        }
                    }
                    else if (result && chipRole != FirmwareChipRole.None)
                    {
                        int pgValue = progress;
                        ctx.DevPgInfoDict[slave].ChipRole = chipRole;

                        if (chipRole != ugProgressDict[slave].PreviousChipRole ||
                            chipRole == ugProgressDict[slave].PreviousChipRole && pgValue > prevProgress)
                        {
                            isFrozen = false;
                            var newProgressInfo = new ValueTuple<long, FirmwareChipRole>(DateTimeOffset.Now.ToUnixTimeMilliseconds(), chipRole);
                            ugProgressDict[slave] = newProgressInfo;
                        }

                        if (pgValue == 100)
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Finished;
                            ctx.DevPgInfoDict[slave].Progress = pgValue;
                            ctx.DevPgInfoDict[slave].Message = $"Device{slave}_{chipRole} upgrade finished.";
                            ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Upgrading;
                            ctx.DevPgInfoDict[slave].Progress = pgValue;
                            ctx.DevPgInfoDict[slave].Message = $"Device{slave}_{chipRole} is upgrading...({pgValue}%)";
                            ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                        }
                    }

                    if (isFrozen && ugProgressDict[slave].PreviousRefreshTime > 0
                                 && DateTimeOffset.Now.ToUnixTimeMilliseconds() - ugProgressDict[slave].PreviousRefreshTime > refreshTimeoutSeconds * 1000)
                    {
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Upgrading;
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.DevPgInfoDict[slave].Message =
                            $"The upgrading progress of [Slave{slave}] has been frozen for over {refreshTimeoutSeconds} seconds.";
                        ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                    }

                    Thread.Sleep(1000);
                }

                if (ctx.DevPgInfoDict.Count == 0 || upgradeFinishedList.Count == ctx.DevPgInfoDict.Count)
                {
                    break;
                }

                Thread.Sleep(3000);
            } while (true);
        }

        private void SgpFwStep_CheckSofarProgress(in SgpUpgradeContext ctx, FirmwareFileType fileType)
        {
            var upgradeFinishedList = new SortedSet<byte>();
            var refreshTimeDict = new Dictionary<byte, long>();
            int refreshTimeoutSeconds = 40;

            foreach (var slave in ctx.SlavesList)
            {
                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;
                refreshTimeDict[slave] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                ctx.DevPgInfoDict[slave].Progress = 0;
            }

            do
            {
                foreach (var slave in ctx.SlavesList)
                {
                    if (!ctx.DevPgInfoDict.ContainsKey(slave) || upgradeFinishedList.Contains(slave))
                        continue;

                    _logger.Information($"设备{slave}, 查询升级进度");
                    int prevProgress = ctx.DevPgInfoDict[slave].Progress;
                    bool isFrozen = true;
                    bool result = ReadUpgradeProgress_2038(slave, out FirmwareFileType fileTypeRead, out byte progress);

                    if (result
                        && fileTypeRead == fileType
                        )
                    {
                        int pgValue = progress;
                        ctx.DevPgInfoDict[slave].FileType = fileType;

                        if (pgValue > prevProgress)
                        {
                            isFrozen = false;
                            refreshTimeDict[slave] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        }

                        if (pgValue == 100)
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Finished;
                            ctx.DevPgInfoDict[slave].Progress = pgValue;
                            ctx.DevPgInfoDict[slave].Message = $"{slave}_{fileType} upgrade finished.";
                            ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                            upgradeFinishedList.Add(slave);
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Upgrading;
                            ctx.DevPgInfoDict[slave].Progress = pgValue;
                            ctx.DevPgInfoDict[slave].Message = $"{slave}_{fileType} is upgrading...({pgValue}%)";
                            ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                        }
                    }

                    if (isFrozen && refreshTimeDict[slave] > 0
                                 && DateTimeOffset.Now.ToUnixTimeMilliseconds() - refreshTimeDict[slave] > refreshTimeoutSeconds * 1000)
                    {
                        ctx.DevPgInfoDict[slave].Stage = SgpUpgradeStage.Upgrading;
                        ctx.DevPgInfoDict[slave].Failed = true;
                        ctx.DevPgInfoDict[slave].Message =
                            $"The upgrading progress of [Slave{slave}] has been frozen for over {refreshTimeoutSeconds} seconds.";
                        ctx.PgReporter?.Report(ctx.DevPgInfoDict[slave]);
                        ctx.DevPgInfoDict.Remove(slave);
                    }

                    Thread.Sleep(500);
                }

                if (ctx.DevPgInfoDict.Count == 0 || upgradeFinishedList.Count == ctx.DevPgInfoDict.Count)
                {
                    break;
                }

                Thread.Sleep(500);
            } while (true);
        }

        #endregion 升级任务

        #region G3 APIs

        private bool ReadUpgradeProgress_203F(byte slave, out FirmwareChipRole chipRole, out byte progress)
        {
            chipRole = FirmwareChipRole.None;
            progress = 0;

            try
            {
                var response = _modbusClient.ReadHoldingRegisters(slave, 0x203F, 1);
                if (response != null)
                {
                    chipRole = response.RegistersBytes[0] switch
                    {
                        // 点表和芯片编码文档序号是不一样的。。
                        0x00 => FirmwareChipRole.None,
                        0x01 => FirmwareChipRole.DSPM_OLD,
                        0x02 => FirmwareChipRole.DSPS_OLD,
                        0x03 => FirmwareChipRole.ARM_OLD,
                        _ => (FirmwareChipRole)response.RegistersBytes[0]
                    };
                    progress = response.RegistersBytes[1];
                    if (progress <= 100)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return false;
        }

        private bool ReadUpgradeProgress_2038(byte slave, out FirmwareFileType fileType, out byte progress)
        {
            fileType = FirmwareFileType.None;
            progress = 0;

            try
            {
                var response = _modbusClient.ReadHoldingRegisters(slave, 0x2038, 1);
                if (response != null)
                {
                    // if (response.RegistersBytes[0] > 0x03)
                    //     return false;

                    fileType = response.RegistersBytes[0] switch
                    {
                        0x00 => FirmwareFileType.App,
                        0x01 => FirmwareFileType.Core,
                        0x02 => FirmwareFileType.Safety,
                        0x03 => FirmwareFileType.Pack,
                        _ => (FirmwareFileType)response.RegistersBytes[0]
                    };

                    if (response.RegistersBytes[1] > 100)
                        return false;

                    progress = response.RegistersBytes[1];
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            return false;
        }

        #endregion G3 APIs
    }
}