using Sofar.CommunicationLib.Connection;
using Sofar.ProtocolLibs.Modbus;
using Sofar.ProtocolLibs.Modbus.Message.Sofar;
using Sofar.ProtocolLibs.SofarSGP;
using Sofar.ProtocolLibs.SofarSGP.Info;
using Sofar.ProtocolLibs.Utils.CRC;
using System.Diagnostics;

namespace Sofar.CommunicationLib.Modbus
{
    public partial class SofarModbusClient : ModbusClient
    {
        private SGPMode _sgpMode = SGPMode.Advanced;

        private readonly AutoResetEvent _longRunningEvent = new(true);

        public SofarModbusClient(TinyBytesStream commStream, ModbusFrameType frameType) : base(commStream, frameType)
        {
        }

        #region G4片段化文件传输功能码

        // 请求开始写入文件片段
        public WriteFileSegmentStartResponse? StartWriteFileSegment(byte slave, string fileName, int rxTimeoutMs)
        {
            var requestEntity = new WriteFileSegmentStartRequest(fileName);
            return Execute<WriteFileSegmentStartRequest, WriteFileSegmentStartResponse>(slave, requestEntity, rxTimeoutMs);
        }

        // 写入文件片段数据
        public WriteFileSegmentDataResponse? WriteFileSegmentData(byte slave, uint fileCrcId, ushort offset, byte[] segmentBytes, int rxTimeoutMs)
        {
            var requestEntity = new WriteFileSegmentDataRequest(fileCrcId, offset, segmentBytes);
            return Execute<WriteFileSegmentDataRequest, WriteFileSegmentDataResponse>(slave, requestEntity, rxTimeoutMs);
        }

        // 请求读取文件片段
        public ReadFileSegmentStartResponse? StartReadFileSegment(byte slave, string fileName, int rxTimeoutMs)
        {
            var requestEntity = new ReadFileSegmentStartRequest(fileName);
            return Execute<ReadFileSegmentStartRequest, ReadFileSegmentStartResponse>(slave, requestEntity, rxTimeoutMs);
        }

        // 读取文件片段数据
        public ReadFileSegmentDataResponse? ReadFileSegmentData(byte slave, uint fileCrcId, ushort unitOffset,
                            ushort unitCount, byte unitSize, int rxTimeoutMs)
        {
            var requestEntity = new ReadFileSegmentDataRequest(fileCrcId, unitOffset, unitCount, unitSize);
            return Execute<ReadFileSegmentDataRequest, ReadFileSegmentDataResponse>(slave, requestEntity, rxTimeoutMs);
        }

        #endregion G4片段化文件传输功能码

        #region G4固件升级功能码

        // 启动固件传输(旧版，文件名称固定为30字节)
        public WriteFileStartResponse? G4StartWritingFile_Old(byte slave, string fileName, uint fileSize, uint crc32, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestEntity = new WriteFileStartRequest_Old(fileName, fileSize, crc32);
                return Execute<WriteFileStartRequest_Old, WriteFileStartResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 启动固件传输(文件名称最多200字节)
        public WriteFileStartResponse? G4StartWritingFile(byte slave, string fileName, uint fileSize, uint crc32, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new WriteFileStartRequest(fileName, fileSize, crc32);
                return Execute<WriteFileStartRequest, WriteFileStartResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 下发固件数据段
        public WriteFileDataResponse? G4WriteFileData(byte slave, byte[] fileSegmentBytes, uint offset, uint fileCrc32, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new WriteFileDataRequest(fileSegmentBytes, offset, fileCrc32);
                return Execute<WriteFileDataRequest, WriteFileDataResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 对已下发的固件的总校验
        public WriteFileVerifyResponse? G4VerifyTransferredFile(byte slave, uint fileCrc32, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new WriteFileVerifyRequest(fileCrc32);
                return Execute<WriteFileVerifyRequest, WriteFileVerifyResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 查询位图（旧版，不发送当前查询的位图段长度，需要上下位机约定）
        public FirmwareVerifyBitmapResponse_Old? G4ReadFileBitmap_Old(byte slave, uint segmentIndex,
                                                                            ushort segmentSize, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new FirmwareVerifyBitmapRequest_Old(segmentIndex, segmentSize);
                return Execute<FirmwareVerifyBitmapRequest_Old, FirmwareVerifyBitmapResponse_Old>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 查询位图
        public FirmwareVerifyBitmapResponse? G4ReadFileBitmap(byte slave, uint segmentIndex, ushort segmentSize, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new FirmwareVerifyBitmapRequest(segmentIndex, segmentSize);
                return Execute<FirmwareVerifyBitmapRequest, FirmwareVerifyBitmapResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 启动升级
        public FirmwareUpgradeStartResponse? G4StartUpgrade(byte slave, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new FirmwareUpgradeStartRequest();
                return Execute<FirmwareUpgradeStartRequest, FirmwareUpgradeStartResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 设置升级时间
        public FirmwareUpgradeSetTimeResponse? G4SetUpgradeTime(byte slave, DateTime upgradeTime, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new FirmwareUpgradeSetTimeRequest(upgradeTime);
                return Execute<FirmwareUpgradeSetTimeRequest, FirmwareUpgradeSetTimeResponse>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 查询升级进度（查询类型 = 1）
        public FirmwareUpgradeStatusResponse_Progress? G4CheckUpgradeStatus_Progress(byte slave, byte fileType, byte chipRole, int rxTimeoutMs = 1500)
        {
            try
            {
                var requestEntity = new FirmwareUpgradeStatusRequest(1, fileType, chipRole);
                return Execute<FirmwareUpgradeStatusRequest, FirmwareUpgradeStatusResponse_Progress>(slave, requestEntity, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        #endregion G4固件升级功能码

        #region SGP 协议交互

        #region SGP握手

        public SgpShakeHandsResponse? SgpShakeHands(byte slave, byte versionCode, ushort bufferSize = 1054,
            byte deviceType = 0, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpShakeHandsRequest(versionCode, bufferSize, deviceType);
                return DoSgpAdTransaction<SgpShakeHandsRequest,
                    SgpShakeHandsResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public SgpShakeHandsResponse? SgpTryShakeHandsByVersions(byte slave, ushort bufferSize = 1054, byte deviceType = 0, int rxTimeoutMs = 3000)
        {
            // 先尝试用0x21~0x24握手
            for (byte protocolVer = 0x21; protocolVer <= 0x24; protocolVer++)
            {
                var response = SgpShakeHands(slave, protocolVer, bufferSize, deviceType, rxTimeoutMs);
                if (response != null && response.VersionCode == protocolVer)
                    return response;
            }

            // 再尝试用0x01握手
            var response01 = SgpShakeHands(slave, 0x01, bufferSize, deviceType, rxTimeoutMs);
            if (response01 != null && response01.VersionCode == 0x01)
                return response01;

            return null;
        }

        #endregion SGP握手

        #region SGP固件升级功能码

        // 读版本号
        public SgpFirmwareVersionResponse? SgpReadFirmwareVersion(byte slave, byte chipRole, int rxTimeoutMs = 1000)
        {
            var requestInfo = new SgpFirmwareVersionRequest(chipRole);

            try
            {
                return DoSgpAdTransaction<SgpFirmwareVersionRequest,
                    SgpFirmwareVersionResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 请求启动固件下发
        public SgpFirmwareTransferStartResponse? SgpStartFirmwareTransfer(byte slave, string firmwareName,
            string firmwareVersion, uint firmwareSize, uint firmwareCrc32, byte chipRole, byte fileType, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareTransferStartRequest(firmwareName, firmwareVersion, firmwareSize, firmwareCrc32, chipRole, fileType);
                return DoSgpAdTransaction<SgpFirmwareTransferStartRequest,
                    SgpFirmwareTransferStartResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 请求启动固件下发(旧版)
        public SgpFirmwareTransferStartResponse? SgpStartFirmwareTransfer_Old(byte slave, string firmwareName,
            string firmwareVersion, uint firmwareSize, uint firmwareCrc32, byte chipRole, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareTransferStartRequest_Old(firmwareName, firmwareVersion, firmwareSize, firmwareCrc32, chipRole);
                return DoSgpAdTransaction<SgpFirmwareTransferStartRequest_Old,
                    SgpFirmwareTransferStartResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 发送固件数据段
        public SgpFirmwareTransferDataResponse? SgpTransferFirmwareData(byte slave, uint firmwareID, uint segmentOffset,
                                    byte[] fileSegmentBytes, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareTransferDataRequest(firmwareID, fileSegmentBytes, segmentOffset);
                return DoSgpAdTransaction<SgpFirmwareTransferDataRequest,
                    SgpFirmwareTransferDataResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 广播固件数据块
        private bool SgpBroadcastFirmwareData(uint firmwareID, uint segmentOffset, byte[] fileSegmentBytes)
        {
            var requestInfo = new SgpFirmwareTransferDataRequest(firmwareID, fileSegmentBytes, segmentOffset);
            return SgpTryBroadcast(requestInfo, 0);
        }

        // 查询位图
        public SgpFirmwareVerifyBitmapResponse? SgpReadFirmwareBitmap(byte slave, uint firmwareID, ushort bitOffset,
                                    ushort bitCount, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareVerifyBitmapRequest(firmwareID, bitOffset, bitCount);
                return DoSgpAdTransaction<SgpFirmwareVerifyBitmapRequest,
                    SgpFirmwareVerifyBitmapResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 总校验
        public SgpFirmwareVerifyResponse? SgpVerifyFirmware(byte slave, uint firmwareID, byte[] upgradeTimeBytes, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareVerifyRequest(firmwareID, upgradeTimeBytes);
                return DoSgpAdTransaction<SgpFirmwareVerifyRequest,
                    SgpFirmwareVerifyResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 启动升级
        public SgpFirmwareUpgradeStartResponse? SgpStartUpgrade(byte slave, byte[] upgradeTimeBytes, byte[] chipRoles,
                                        uint[] firmwareIDs, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFirmwareUpgradeStartRequest(upgradeTimeBytes, chipRoles, firmwareIDs);
                return DoSgpAdTransaction<SgpFirmwareUpgradeStartRequest,
                    SgpFirmwareUpgradeStartResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        #endregion SGP固件升级功能码

        #region SGP文件传输功能码

        // 启动文件传输
        public SgpFileTransferStartResponse? SgpStartFileTransfer(byte slave, string fileName, string filePath,
            uint fileSize, uint fileCrc32, byte chipRole, byte fileType, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFileTransferStartRequest(fileName, filePath, fileSize, fileCrc32, chipRole, fileType);
                return DoSgpAdTransaction<SgpFileTransferStartRequest,
                    SgpFileTransferStartResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 传输文件数据段
        public SgpFileTransferDataResponse? SgpTransferFileData(byte slave, uint fileID, uint segmentOffset,
            byte[] fileSegmentBytes, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFileTransferDataRequest(fileID, fileSegmentBytes, segmentOffset);
                return DoSgpAdTransaction<SgpFileTransferDataRequest,
                    SgpFileTransferDataResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 查询文件校验位图
        public SgpFileTransferVerifyBitmapResponse? SgpReadFileBitmap(byte slave, uint fileID, ushort bitOffset,
            ushort bitCount, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFileTransferVerifyBitmapRequest(fileID, bitOffset, bitCount);
                return DoSgpAdTransaction<SgpFileTransferVerifyBitmapRequest,
                    SgpFileTransferVerifyBitmapResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        // 校验文件
        public SgpFileTransferVerifyResponse? SgpVerifyFile(byte slave, uint fileID, byte[] upgradeTimeBytes, int rxTimeoutMs = 1000)
        {
            try
            {
                var requestInfo = new SgpFileTransferVerifyRequest(fileID, upgradeTimeBytes);
                return DoSgpAdTransaction<SgpFileTransferVerifyRequest,
                    SgpFileTransferVerifyResponse>(slave, requestInfo, 0, rxTimeoutMs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        #endregion SGP文件传输功能码

        public TResponse? DoSgpAdTransaction<TRequest, TResponse>(byte slave, TRequest requestInfo, byte seqNo, int rxTimeoutMs)
            where TRequest : SGPInfoBase
            where TResponse : SGPInfoBase
        {
            SgpADHeader txHeader = new SgpADHeader(slave, requestInfo.CommandCode, requestInfo.Size, seqNo);
            byte[] frameBytes = SgpFramer.GetFrameBytesWithCrc(txHeader, requestInfo);

            Send(frameBytes);
            if (slave == 0)
                return null;

            int expectedRxLen = SgpFramer.GetResponseSize(txHeader, requestInfo)!.Value;
            long txTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            ReceiveOneFrame(txTimestamp, rxTimeoutMs, out byte[] rxBytes);
            if (rxBytes.Length == 0)
                return null;

            bool parseResult = false;
            string? errorMsg = "";
            int msgOffset = 0, msgSize = 0;
            for (int p = 0; p < rxBytes.Length; p++)
            {
                if (rxBytes.Length - p < 11)
                {
                    errorMsg = "The incoming message is invalid.";
                    break;
                }

                if (rxBytes[0] == txHeader.StartOfFrame[0] && rxBytes[1] == txHeader.StartOfFrame[1]
                    && rxBytes.Length - p >= expectedRxLen)
                {
                    if (CRCHelper.ComputeCrc16(rxBytes, p + 4, expectedRxLen - 4, CRCHelper.Crc16Method.CRC16_MODBUS)
                        == (rxBytes[2] | rxBytes[3] << 8))
                    {
                        parseResult = true;
                        msgOffset = p;
                        msgSize = expectedRxLen;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMsg) || !parseResult)
                return null;

            var responseFrame = rxBytes.Skip(msgOffset).Take(msgSize).ToArray();
            int headerSize = SgpADHeader.HEADER_SIZE;

            TResponse? responseEntity = Activator.CreateInstance(typeof(TResponse),
                responseFrame, headerSize, responseFrame.Length - headerSize) as TResponse;
            return responseEntity;
        }

        public bool SgpTryBroadcast<TRequest>(TRequest requestInfo, byte seqNo) where TRequest : SGPInfoBase
        {
            SgpADHeader txHeader = new SgpADHeader(0, requestInfo.CommandCode, requestInfo.Size, seqNo);
            byte[] frameBytes = SgpFramer.GetFrameBytesWithCrc(txHeader, requestInfo);
            try
            {
                Send(frameBytes);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        #endregion SGP 协议交互
    }
}