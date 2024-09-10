using Sofar.CommunicationLib.Service.AppModels;
using Sofar.ProtocolLibs.FirmwareInfo;
using Sofar.ProtocolLibs.SofarSGP.Info;
using Sofar.ProtocolLibs.Utils.CRC;
using System.Diagnostics;

namespace Sofar.CommunicationLib.Service.FileTransfer
{
    public class SgpFileTransferService : ServiceBase
    {
        private class SgpFileTransferContext
        {
            public byte[] SlavesList;
            public SgpFileTransferConfig Config;
            public Dictionary<byte, SgpFileTransferProgressInfo> DevPgInfoDict;
            public IProgress<SgpFileTransferProgressInfo> PgReporter;
        }

        public Task SgpTransferFileAsync(IEnumerable<byte> slaves, string filePath, SgpFileTransferConfig config,
            CancellationToken cancellationToken, IProgress<SgpFileTransferProgressInfo> pgReporter)
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
                    SgpTransferFile(slaves, filePath, config, cancellationToken, pgReporter);
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

        // 单个文件广播传输：握手->(请求文件下发->下发文件->查询位图->补包)->总校验；
        // 单个文件单播传输：握手->请求文件下发-->下发文件（每包即时校验/补发）->总校验；
        private void SgpTransferFile(IEnumerable<byte> slaves, string filePath, SgpFileTransferConfig config,
            CancellationToken cancellationToken, IProgress<SgpFileTransferProgressInfo> pgReporter)
        {
            #region 导入文件

            byte[] fileBytes = File.ReadAllBytes(filePath);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string filePathToRequest = "";
            uint fileStart = 0;
            uint fileSizeToRequest = (uint)fileBytes.Length;
            uint crc32 = CRCHelper.ComputeCrc32(fileBytes, CRCHelper.Crc32Method.CRC32);

            if (filePath.EndsWith(".bin", true, null))
            {
                var binInfo = new BinFirmwareInfoV3(fileBytes.TakeLast(BinFirmwareInfoV3.SIGNATURE_SIZE).ToArray());
                fileName = binInfo.ProjectName;
                if (config.UseBinFileSize)
                {
                    if (binInfo.SignatureVersion == 0x01)
                    {
                        byte[] fwSizeBytes = fileBytes.TakeLast(BinFirmwareInfoV3.SIGNATURE_SIZE + 8).Take(4).ToArray();
                        fileSizeToRequest = (uint)(fwSizeBytes[0] | fwSizeBytes[1] << 8 | fwSizeBytes[2] << 16 | fwSizeBytes[3] << 24);
                    }
                    else
                    {
                        fileSizeToRequest = binInfo.FirmwareSize;
                    }
                }

                if (config.UseBinProgramOffset)
                    fileStart = binInfo.ProgramOffset;
                if (config.UseBinCrc32)
                    crc32 = binInfo.Crc32;
            }

            #endregion 导入文件

            var slavesList = slaves.ToArray();
            var devPgInfoDict = new Dictionary<byte, SgpFileTransferProgressInfo>();
            foreach (var slave in slavesList)
            {
                SgpFileTransferProgressInfo devPgInfo = new()
                {
                    Slave = slave,
                    Progress = 0,
                    Stage = SgpFileTransferStage.None,
                    Message = null,
                };
                devPgInfoDict[slave] = devPgInfo;
            }

            SgpFileTransferContext context = new SgpFileTransferContext()
            {
                Config = config,
                DevPgInfoDict = devPgInfoDict,
                PgReporter = pgReporter,
                SlavesList = slavesList,
            };

            try
            {
                #region <1> 握手

                SgpFileStep_ShakeHands(context, cancellationToken);
                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1000);

                #endregion <1> 握手

                #region <1.1>  协议版本兼容性检查

                foreach (var slave in slavesList)
                {
                    if (!devPgInfoDict.ContainsKey(slave))
                        continue;
                    // 协议版本0x01不支持ARM, DSPM, DSPS以外的升级
                    if (devPgInfoDict[slave].ProtocolVersion == 0x01)
                    {
                        devPgInfoDict[slave].Stage = SgpFileTransferStage.CheckCompatibility;
                        devPgInfoDict[slave].Message =
                            $"The protocol version '0x01' does not support for file transfer.";
                        devPgInfoDict[slave].Failed = true;
                        pgReporter.Report(devPgInfoDict[slave]);
                        devPgInfoDict.Remove(slave);
                    }
                }
                if (devPgInfoDict.Count == 0)
                    return;

                #endregion <1.1>  协议版本兼容性检查

                #region <2> 请求下发文件

                SgpFileStep_RequestToSendFile(context, cancellationToken, fileName, filePathToRequest,
                                                fileSizeToRequest, crc32, out int breakpoint);
                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1000);

                #endregion <2> 请求下发文件

                #region <3> 下发文件数据

                SgpFileStep_SendFileData(context, cancellationToken, fileBytes, crc32, fileStart, breakpoint);

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(1000);

                #endregion <3> 下发文件数据

                if (!config.IsBroadcast)
                    goto SkipResending;

                #region <4> loop{查询位图->补包}

                var loopFinishedList = new SortedSet<byte>();
                int resendRetry = 0;
                do
                {
                    #region <4.1> 查询位图

                    SgpFileStep_ReadFileBitmap(context, cancellationToken, fileBytes.Length, crc32, ref loopFinishedList, out var lostPacks);
                    if (loopFinishedList.Count == devPgInfoDict.Count)
                        break;
                    Thread.Sleep(500);

                    #endregion <4.1> 查询位图

                    #region <4.2> 广播补包

                    SgpFileStep_BroadcastLostPacks(context, cancellationToken, fileBytes, crc32, fileStart, loopFinishedList, lostPacks);
                    Thread.Sleep(500);

                    #endregion <4.2> 广播补包

                    resendRetry++;
                } while (resendRetry < config.ResendLostsMaxRetries);

                // 报告补包失败的设备
                foreach (var slave in slavesList)
                {
                    if (!devPgInfoDict.ContainsKey(slave) || loopFinishedList.Contains(slave))
                        continue;
                    devPgInfoDict[slave].Failed = true;
                    devPgInfoDict[slave].Stage = SgpFileTransferStage.ResendingLostPacks;
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

                SgpFileStep_VerifyFile(context, cancellationToken, crc32);

                if (devPgInfoDict.Count == 0)
                    return;
                Thread.Sleep(500);

                #endregion <5> 总校验
            }
            catch (OperationCanceledException oce)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    foreach (var slave in slavesList)
                    {
                        if (!devPgInfoDict.ContainsKey(slave))
                            continue;

                        devPgInfoDict[slave].Stage = SgpFileTransferStage.Cancelled;
                        devPgInfoDict[slave].Message = "The file transfer task was cancelled.";
                        devPgInfoDict[slave].Failed = true;
                        pgReporter.Report(devPgInfoDict[slave]);
                        devPgInfoDict.Remove(slave);
                    }
                }
            }
        }

        private void SgpFileStep_ShakeHands(in SgpFileTransferContext ctx, CancellationToken cancellationToken)
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
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ShakeHands;
                        ctx.DevPgInfoDict[slave].Progress = 100;
                        ctx.DevPgInfoDict[slave].ProtocolVersion = response.VersionCode;
                        ctx.DevPgInfoDict[slave].Message = $"Successfully shake hands and confirm the protocol version: " +
                                                           $"0x{response.VersionCode:X2}";
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ShakeHands;
                        ctx.DevPgInfoDict[slave].Message = "Failed to shake hands.";
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

        private void SgpFileStep_RequestToSendFile(in SgpFileTransferContext ctx, CancellationToken cancellationToken,
                                                string fileName, string filePath, uint fileSize, uint crcId, out int breakpoint)
        {
            breakpoint = 0;
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                // 协议版本0x01不支持ARM,DSPM,DSPS以外的升级
                if (ctx.DevPgInfoDict[slave].ProtocolVersion == 0x01)
                {
                    ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.RequestToSendFile;
                    ctx.DevPgInfoDict[slave].Message = "The protocol version '0x01' does not support for file transfer.";
                    ctx.DevPgInfoDict[slave].Failed = true;
                    ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                    ctx.DevPgInfoDict.Remove(slave);
                    continue;
                }

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 请求发送固件");
                    var response = _modbusClient.SgpStartFileTransfer(slave, fileName, filePath, fileSize,
                        crcId, (byte)ctx.Config.FirmwareChipRole, (byte)ctx.Config.FirmwareType, 20_000);

                    if (response != null && (response.ResultCode == 0 || response.ResultCode == 1))
                    {
                        // 请求成功
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.RequestToSendFile;
                        ctx.DevPgInfoDict[slave].Message = "Ready to send the file.";
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
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.RequestToSendFile;
                        ctx.DevPgInfoDict[slave].Message = "Failed to request sending file.";
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

        private void SgpFileStep_SendFileData(in SgpFileTransferContext ctx, CancellationToken cancellationToken,
                                                byte[] fileBytes, uint crcId, uint fileStart, int breakpoint)
        {
            breakpoint = !ctx.Config.IsBroadcast && ctx.Config.ResumeFromBreakPoint ? breakpoint : 0;
            int fileByteCount = fileBytes.Length - breakpoint;
            int sendCount = fileByteCount / ctx.Config.SendingSegmentSize + (fileByteCount % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);

            for (int i = 0; i < sendCount; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentSize = ctx.Config.SendingSegmentSize;
                if (currentSize > fileByteCount - i * ctx.Config.SendingSegmentSize)
                    currentSize = fileByteCount - i * ctx.Config.SendingSegmentSize;
                bool result = false;
                int errorCode = -1;

                if (ctx.Config.IsBroadcast) // 广播发包
                {
                    for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                    {
                        _logger.Information($"广播发送文件({i + 1}/{sendCount})");
                        result = SgpBroadcastFileData(crcId, fileStart + (uint)(i * ctx.Config.SendingSegmentSize),
                                                        fileBytes.Skip(i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());
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
                            _logger.Information($"设备{slave}, 发送固件");
                            var response = _modbusClient.SgpTransferFileData(slave, crcId, fileStart + (uint)(i * ctx.Config.SendingSegmentSize),
                                                                                fileBytes.Skip(breakpoint + i * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());
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

                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.SendingFile;

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

        // 广播文件数据段
        private bool SgpBroadcastFileData(uint fileID, uint segmentOffset, byte[] fileSegmentBytes)
        {
            var requestInfo = new SgpFileTransferDataRequest(fileID, fileSegmentBytes, segmentOffset);
            return _modbusClient.SgpTryBroadcast(requestInfo, 0);
        }

        private void SgpFileStep_ReadFileBitmap(in SgpFileTransferContext ctx, CancellationToken cancellationToken, int fileBytesCount, uint crcId,
                                                ref SortedSet<byte> loopFinishedList, out SortedSet<int> lostPacks)
        {
            int totalBits = fileBytesCount / ctx.Config.SendingSegmentSize + (fileBytesCount % ctx.Config.SendingSegmentSize == 0 ? 0 : 1);
            int totalBitmapBytes = totalBits / 8 + (totalBits % 8 == 0 ? 0 : 1);
            int queryCount = totalBitmapBytes / ctx.Config.BitmapSegmentSize
                             + (totalBitmapBytes % ctx.Config.BitmapSegmentSize == 0 ? 0 : 1);  // 查一位图所需要的查询次数

            lostPacks = new SortedSet<int>();
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave) || loopFinishedList.Contains(slave))
                    continue;

                bool noLost = true;
                bool queryResult = true;
                for (int i = 0; i < queryCount; i++)  // 查一个设备的位图
                {
                    int currentBitCount = ctx.Config.BitmapSegmentSize * 8;
                    if (currentBitCount > totalBits - i * ctx.Config.BitmapSegmentSize * 8)
                        currentBitCount = totalBits - i * ctx.Config.BitmapSegmentSize * 8;
                    int currentBitOffset = i * ctx.Config.BitmapSegmentSize * 8;

                    for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                    {
                        _logger.Information($"设备{slave}, 查询位图");
                        var response = _modbusClient.SgpReadFileBitmap(slave, crcId, (ushort)currentBitOffset, (ushort)currentBitCount);
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
                            noLost = false;
                            if (retry + 1 == ctx.Config.RequestMaxTries)
                            {
                                ctx.DevPgInfoDict[slave].Failed = true;
                                ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ResendingLostPacks;
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
                    ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ResendingLostPacks;
                    ctx.DevPgInfoDict[slave].Progress = 100;
                    ctx.DevPgInfoDict[slave].Message = "All segment pack of the file has been received by the device.";
                    loopFinishedList.Add(slave);
                    ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                }
            }
        }

        private void SgpFileStep_BroadcastLostPacks(in SgpFileTransferContext ctx, CancellationToken cancellationToken, byte[] fileBytes,
                                                    uint crcId, uint fileStart, in SortedSet<byte> loopFinishedList, in SortedSet<int> lostPacks)
        {
            int lostPacksCnt = 0;
            foreach (int packIdx in lostPacks)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentSize = ctx.Config.SendingSegmentSize;
                if (currentSize > fileBytes.Length - packIdx * ctx.Config.SendingSegmentSize)
                    currentSize = fileBytes.Length - packIdx * ctx.Config.SendingSegmentSize;

                bool result = false;
                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"广播补包({packIdx + 1}/{lostPacks.Count})");
                    result = SgpBroadcastFileData(crcId, fileStart + (uint)(packIdx * ctx.Config.SendingSegmentSize),
                                                    fileBytes.Skip(packIdx * ctx.Config.SendingSegmentSize).Take(currentSize).ToArray());

                    if (result) break;
                    Thread.Sleep(ctx.Config.SendingInterval);
                }

                if (lostPacksCnt % ctx.Config.SendPackReportIntervals == 0
                    || lostPacksCnt + 1 == lostPacks.Count || result == false) // 报告补包中的设备
                {
                    foreach (var slave in ctx.SlavesList)
                    {
                        if (!ctx.DevPgInfoDict.ContainsKey(slave) || loopFinishedList.Contains(slave))
                            continue;

                        if (result)
                        {
                            var pgValue = (lostPacksCnt + 1.0) / lostPacks.Count * 100;
                            ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ResendingLostPacks;
                            ctx.DevPgInfoDict[slave].Progress = (int)Math.Round(pgValue, 0);
                            ctx.DevPgInfoDict[slave].Message = "Resending the lost packs...";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        }
                        else
                        {
                            ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.ResendingLostPacks;
                            ctx.DevPgInfoDict[slave].Failed = true;
                            ctx.DevPgInfoDict[slave].Message = "Failed to resend package.";
                            ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                            ctx.DevPgInfoDict.Remove(slave);
                        }
                    }
                }

                lostPacksCnt++;
                Thread.Sleep(ctx.Config.SendingInterval);
            }
        }

        private void SgpFileStep_VerifyFile(in SgpFileTransferContext ctx, CancellationToken cancellationToken, uint crcId)
        {
            foreach (var slave in ctx.SlavesList)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (!ctx.DevPgInfoDict.ContainsKey(slave))
                    continue;

                for (int retry = 0; retry < ctx.Config.RequestMaxTries; retry++)
                {
                    _logger.Information($"设备{slave}, 文件校验");
                    var response = _modbusClient.SgpVerifyFile(slave, crcId, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 2000);
                    if (response != null && response.ResultCode == 0)
                    {
                        // 校验成功
                        ctx.DevPgInfoDict[slave].Message = "Successfully verified the package.";
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.Verification;
                        ctx.PgReporter.Report(ctx.DevPgInfoDict[slave]);
                        break;
                    }

                    if (retry == ctx.Config.RequestMaxTries - 1)
                    {
                        ctx.DevPgInfoDict[slave].Message = "Failed to verify the file.";
                        if (response != null)
                            ctx.DevPgInfoDict[slave].Message += $" Result Code: {response.ResultCode}";
                        ctx.DevPgInfoDict[slave].Stage = SgpFileTransferStage.Verification;
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
    }
}