using Sofar.ProtocolLibs.FirmwareInfo;

namespace Sofar.CommunicationLib.Service.AppModels
{
    public class SgpFileTransferConfig
    {
        /// <summary>
        /// 普通请求重试次数
        /// </summary>
        public int RequestMaxTries { get; set; } = 3;

        /// <summary>
        /// 每次发送的固件包段落的长度(byte)
        /// </summary>
        public int SendingSegmentSize { get; set; } = 3 * 128;

        /// <summary>
        /// 每次发送的固件包段落的间隔(ms)
        /// </summary>
        public int SendingInterval { get; set; } = 260;

        /// <summary>
        /// 每次查询位图段落的长度(byte)
        /// </summary>
        public int BitmapSegmentSize { get; set; } = 100;

        /// <summary>
        /// 补包重试次数
        /// </summary>
        public int ResendLostsMaxRetries { get; set; } = 10;

        /// <summary>
        /// 是否采用广播传输流程
        /// </summary>
        public bool IsBroadcast { get; set; } = false;

        /// <summary>
        /// 是否启用断点续传（对单播升级有效，对广播升级无效）
        /// </summary>
        public bool ResumeFromBreakPoint { get; set; } = false;

        /// <summary>
        /// 每发送n包报告一次
        /// </summary>
        public int SendPackReportIntervals { get; set; } = 10;

        /// <summary>
        /// 是否使用bin文件签名提供的ProgramOffset
        /// </summary>
        public bool UseBinProgramOffset { get; set; } = true;

        /// <summary>
        /// 是否使用bin文件签名提供的CRC32
        /// </summary>
        public bool UseBinCrc32 { get; set; } = true;

        /// <summary>
        /// 是否使用bin文件签名提供的文件大小
        /// </summary>
        public bool UseBinFileSize { get; set; } = true;

        // /// <summary>
        // /// 文件名称
        // /// </summary>
        // public string FileName { get; set; } = "";
        //
        // /// <summary>
        // /// 文件路径
        // /// </summary>
        // public string FilePath { get; set; } = "";
        //
        //
        // public uint FileSize { get; set; } = 0;
        //
        // public uint FileStartAddress { get; set; } = 0;
        //
        // public uint FileCrc32 { get; set; }

        public FirmwareFileType FirmwareType { get; set; }

        public FirmwareChipRole FirmwareChipRole { get; set; } = 0;
    }

    public class SgpFileTransferProgressInfo
    {
        public byte Slave { get; set; } = 0;

        public byte ProtocolVersion { get; set; }

        public SgpFileTransferStage Stage { get; set; }

        public int Progress { get; set; }

        public string? Message { get; set; }

        public bool Failed { get; set; } = false;
    }

    public enum SgpFileTransferStage
    {
        None = 0,
        ShakeHands,
        CheckCompatibility,
        RequestToSendFile,
        SendingFile,
        ResendingLostPacks,
        Verification,

        // Finished,
        Cancelled,
    }
}