using Sofar.ProtocolLibs.FirmwareInfo;

namespace Sofar.CommunicationLib.Service.AppModels
{
    public class G4UpgradeConfig
    {
        /// <summary>
        /// 普通请求重试次数
        /// </summary>
        public int RequestMaxTries { get; set; } = 3;

        /// <summary>
        /// 每次发送的固件包段落的长度(byte)
        /// </summary>
        public int SendingSegmentSize { get; set; } = 360;

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
        /// 固件是sofar包(true)或者是bin包(false)
        /// </summary>
        public bool IsSofarOrBin { get; set; } = true;

        /// <summary>
        /// 定时升级时间
        /// </summary>
        public DateTime? UpgradeTime { get; set; } = null;

        /// <summary>
        /// 是否采用广播升级流程
        /// </summary>
        public bool IsBroadcast { get; set; } = false;

        /// <summary>
        /// 是否启用断点续传（对单播升级有效，对广播升级无效）
        /// </summary>
        public bool ResumeFromBreakPoint { get; set; } = false;

        /// <summary>
        /// 是否使用新版的0x50-01功能码进行请求升级
        /// </summary>
        public bool UseNew5001 { get; set; } = true;

        /// <summary>
        /// 是否使用较新的0x51-08功能码进行位图查询，否则将使用旧的0x51-03
        /// </summary>
        public bool Use5108 { get; set; } = true;

        public int SendPackReportIntervals { get; set; } = 10;
    }

    public enum G4UpgradeStage
    {
        None = 0,
        RequestToSendFile,
        SendingFile,
        ResendingLostPacks,
        Verification,
        StartUpgrade,
        CheckProgress,
        Finished,
        SetAlarm,
        Cancelled,
    }

    public class G4UpgradeProgressInfo
    {
        public byte Slave { get; set; } = 0;

        public FirmwareFileType FileType { get; set; }

        public FirmwareChipRole ChipRole { get; set; }

        public G4UpgradeStage Stage { get; set; }

        public int Progress { get; set; }

        public string? Message { get; set; }

        public bool Failed { get; set; } = false;

        // public DateTime? LastUpdateTime { get; set; }
    }

    public class G4DeviceDescription
    {
        public G4DeviceDescription(uint roleCode)
        {
            RoleCode = roleCode;
        }

        /// <summary>
        /// 属性1: 设备型号
        /// </summary>
        public string? DeviceModel { get; set; } = "";

        /// <summary>
        /// 属性2：软件版本
        /// </summary>
        public string? SoftwareVersion { get; set; } = "";

        /// <summary>
        /// 属性3：硬件版本
        /// </summary>
        public string? HardwareVersion { get; set; } = "";

        /// <summary>
        /// 属性4：接口协议版本
        /// </summary>
        public string? ProtocolVersion { get; set; } = "";

        /// <summary>
        /// 属性5：序列号
        /// </summary>
        public string? SN { get; set; } = "";

        /// <summary>
        /// 属性6：设备角色
        /// </summary>
        public uint RoleCode { get; set; } = 0;

        /// <summary>
        /// 属性7：接口协议版本
        /// </summary>
        public string? SpecialVersion { get; set; } = "";
    }
}