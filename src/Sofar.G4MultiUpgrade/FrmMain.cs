using Sofar.CommunicationLib;
using Sofar.CommunicationLib.Model;
using Sofar.CommunicationLib.Service;
using Sofar.ProtocolLibs.FirmwareInfo;
using Sunny.UI;
using System.Diagnostics;
using System.Text;
using ILogger = Serilog.ILogger;

namespace Sofar.G4MultiUpgrade
{
    public partial class FrmMain : UIForm
    {
        private System.Timers.Timer? timer;

        private FrmConfig? frmConfig;

        public FrmMain()
        {
            InitializeComponent();

            LoadModbusClients();
        }

        private void LoadModbusClients()
        {
            _upgradeService = new();
            foreach (var item in CommManager.Instance.ModbusClients)
            {
                _upgradeService.Add(new G4UpgradeService(item));
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer(1000);
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            // 创建ListView的列
            lvConnectlist.View = View.Details;
            lvConnectlist.Columns.Add("IpAddress", 120);
            lvConnectlist.Columns.Add("State", 50);

            frmConfig = new FrmConfig();
            if (frmConfig.ShowDialog() == DialogResult.OK)
            {
                LoadModbusClients();

                // 加载通讯列表...

                BindDictionaryToListView(FrmConfig.ConnectArray, lvConnectlist);
            }

            RefreshInvertersGrid();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("++++++");

            bool? connectState = null;

            foreach (var pair in FrmConfig.ConnectArray)
            {
                if (pair.Value)
                {
                    connectState = true;
                    break;
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke(() =>
                {
                    if (connectState == true)
                    {
                        ttxtCommState.Text = "已连接";
                    }
                    else
                    {
                        ttxtCommState.Text = "未连接";
                    }
                });
            }
            else
            {
                if (connectState == true)
                {
                    ttxtCommState.Text = "已连接";
                }
                else
                {
                    ttxtCommState.Text = "未连接";
                }
            }
        }

        private void tsmi_Comm_Click(object sender, EventArgs e)
        {
            if (frmConfig.ShowDialog() == DialogResult.OK)
            {
                LoadModbusClients();

                // 加载通讯列表...
                BindDictionaryToListView(FrmConfig.ConnectArray, lvConnectlist);
            }
        }

        private string? _firmwarePath;

        private SofarSubFirmwareInfo[]? _subModuleInfos;

        private List<G4UpgradeService> _upgradeService;

        private readonly ILogger _logger = Serilog.Log.Logger;

        //private readonly List<G4InverterUpgradeInfo> _invertersInfos = new() { new G4InverterUpgradeInfo(0x01) { } };

        private List<G4InverterUpgradeInfo> _invertersInfos = new();

        public bool IsUpgrading { get; private set; } = false;

        private void BindDictionaryToListView(Dictionary<string, bool> dict, ListView listView)
        {
            // 清除ListView中现有的项
            listView.Items.Clear();
            _invertersInfos = new();

            // 遍历字典
            foreach (var kvp in dict)
            {

                // 创建一个新的ListViewItem
                ListViewItem item = new ListViewItem(kvp.Key); // 第一个参数是Key，也是主项

                // 为ListViewItem添加子项，此处是bool值转换为字符串
                item.SubItems.Add(kvp.Value.ToString());

                // 将ListViewItem添加到ListView中
                listView.Items.Add(item);

                if (kvp.Value)
                {
                    _invertersInfos.Add(new G4InverterUpgradeInfo(0x01, kvp.Key));
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "sofar package (*.sofar)|*.sofar";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var filename = openFileDialog.FileName;

            byte[] firmwareBytes = File.ReadAllBytes(filename);

            try
            {
                SofarPackageInfoV2 packageInfo = new(firmwareBytes.TakeLast(SofarPackageInfoV2.SIGNATURE_SIZE).ToArray());
                _subModuleInfos = packageInfo.SubFirmwareInfos;

                string versions = "内含固件版本：";
                for (int i = 0; i < _subModuleInfos.Length; i++)
                {
                    string name = _subModuleInfos[i].FirmwareName.Replace('\0', ' ');
                    string version = _subModuleInfos[i].FirmwareVersion.Replace('\0', ' ');
                    versions += $"【{name}" + $"-{version}】";
                    if (i < _subModuleInfos.Length - 1)
                    {
                        if ((i + 1) % 3 == 0) versions += ";\n          ";
                        else versions += "; ";
                    }
                }
                this.lblUpgradeLog.Text = versions;
                this.txtPath.Text = filename;
                _firmwarePath = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "导入错误");
                return;
            }
        }

        #region 升级启停

        private G4UpgradeStage _globalStage = G4UpgradeStage.None;
        private CancellationTokenSource _cancellationTokenSource;
        private List<CancellationTokenSource> _cancellationTokenSourceList;


        private void btnStartUpgrade_Click(object sender, EventArgs e)
        {
            if (IsUpgrading)
            {
                MessageBox.Show("已有升级任务进行中！", "错误");
                return;
            }

            if (_firmwarePath == null || _subModuleInfos == null)
            {
                MessageBox.Show("没有导入固件包！", "错误");
                return;
            }

            _dict.Clear();

            _cancellationTokenSourceList =  new List<CancellationTokenSource>();
            foreach (var item in _upgradeService)
            {
                var task = SingUpgrade(item);

            }
        }

        private Dictionary<int, string> _dict = new Dictionary<int, string>();

        private async Task SingUpgrade(G4UpgradeService _upgradeService)
        {
            byte[] slaves = { 0x01 };
            //if (!GetDeviceAddresses(out byte[] slaves))
            //    return;

            IsUpgrading = true;
            //ClearUpgradeMsg();
            //StartBusy("正在升级G4设备");

            G4UpgradeConfig upgradeConfig = new G4UpgradeConfig
            {
                RequestMaxTries = 3,
                SendingSegmentSize = (int)PackageSize_NumericUpDown.Value,
                SendingInterval = (int)SendingPackageInterval_NumericUpDown.Value,
                ResendLostsMaxRetries = (int)ResendLostsRetries_NumericUpDown.Value,
                BitmapSegmentSize = 100,
                IsSofarOrBin = true,
                UpgradeTime = null,
                IsBroadcast = false,
                ResumeFromBreakPoint = false,
                UseNew5001 = true,
                Use5108 = false,
                //IPAddress = ((Sofar.CommunicationLib.Connection.TcpStream)_upgradeService._modbusClient.CommStream).RemoteEndPoint.Address.ToString()
            };

            //var readFinishedList = await ReadDeviceInfo(slaves);

            var pgReporter = new Progress<G4UpgradeProgressInfo>();
            pgReporter.ProgressChanged += func;

            void func(object? sender, G4UpgradeProgressInfo pgInfo)
            {
                this.PgReporter_OnProgressChanged(pgInfo, ((Sofar.CommunicationLib.Connection.TcpStream)_upgradeService.ModbusClient.CommStream).RemoteEndPoint.Address.ToString());
            };

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSourceList.Add(_cancellationTokenSource);
            try
            {
                _logger.Information("\n---------------G4升级开始--------------");
                var sw = Stopwatch.StartNew();

                // 升级
                _globalStage = G4UpgradeStage.None;

                var task = _upgradeService.G4FirmwareUpgradeAsync(new byte[] { 0x01 }, _firmwarePath!, upgradeConfig,
                    _cancellationTokenSource.Token, pgReporter);
                await task;

                //var timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString("%m");
                //AppendUpgradeMsg($"操作总耗时：{timeSpan}Min");

                _logger.Information("---------------G4升级结束--------------\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"升级失败，内部错误：\n{ex.Message}", "错误");
            }
            finally
            {
                IsUpgrading = false;
                _globalStage = G4UpgradeStage.None;
                //StopBusy();
            }
        }


        private void PgReporter_OnProgressChanged(G4UpgradeProgressInfo pgInfo, string ip)
        {
            string stepName = pgInfo.Stage switch
            {
                G4UpgradeStage.None => "",
                G4UpgradeStage.RequestToSendFile => "请求发送固件",
                G4UpgradeStage.SendingFile => "发包",
                G4UpgradeStage.ResendingLostPacks => "补包",
                G4UpgradeStage.Verification => "固件校验",
                G4UpgradeStage.StartUpgrade => "启动升级",
                G4UpgradeStage.CheckProgress => "查询进度",
                G4UpgradeStage.Finished => "升级完成",
                G4UpgradeStage.SetAlarm => "设置升级时间",
                G4UpgradeStage.Cancelled => "取消升级",
                _ => throw new ArgumentOutOfRangeException()
            };

            byte slave = pgInfo.Slave;
            //var ip = pgInfo.IP;
            int idx = _invertersInfos.FindIndex(x => x.SlaveNo == slave && x.IP == ip);//增加IP地址的条件
            if (idx < 0)
                return;

            if (pgInfo.Failed)
            {
                if (pgInfo.Stage == G4UpgradeStage.Cancelled)
                {
                    _invertersInfos[idx].StatusMsg = "升级已取消";
                    //AppendUpgradeMsg($"升级已取消。");

                }
                else
                {
                    _invertersInfos[idx].StatusMsg = $"升级失败。步骤：{stepName}";
                    _logger.Error($"[设备{pgInfo.Slave}][升级失败] {pgInfo.Message}\n");
                }
                RefreshInvertersGrid();
                return;
            }

            int pgValue = pgInfo.Progress;
            switch (pgInfo.Stage)
            {
                case G4UpgradeStage.None:
                    break;
                case G4UpgradeStage.RequestToSendFile:
                    _invertersInfos[idx].StatusMsg = $"准备传输固件";
                    break;
                case G4UpgradeStage.SendingFile:
                    _invertersInfos[idx].StatusMsg = $"传输固件({pgValue}%)";
                    break;
                case G4UpgradeStage.ResendingLostPacks:
                    _invertersInfos[idx].StatusMsg = $"补包({pgValue}%)";
                    break;
                case G4UpgradeStage.Verification:
                    _invertersInfos[idx].StatusMsg = $"固件校验成功";
                    break;
                case G4UpgradeStage.StartUpgrade:
                    _invertersInfos[idx].StatusMsg = $"启动升级成功";
                    break;
                case G4UpgradeStage.CheckProgress:
                    UpdateProgressString(idx, pgInfo.FileType, pgInfo.ChipRole, $"{pgValue}%");
                    break;
                case G4UpgradeStage.Finished:
                    UpdateProgressString(idx, pgInfo.FileType, pgInfo.ChipRole, $"完成");
                    break;
                case G4UpgradeStage.SetAlarm:
                    break;
                case G4UpgradeStage.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _invertersInfos[idx].RefreshTime = DateTime.Now.ToString("MM/dd-HH:mm:ss");
            _globalStage = pgInfo.Stage;
            _logger.Information($"[设备{pgInfo.Slave}] {pgInfo.Message}");

            RefreshInvertersGrid();
        }

        private void UpdateProgressString(int slaveGridIdx, FirmwareFileType fileType, FirmwareChipRole chipRole, string pgString)
        {
            if (fileType == FirmwareFileType.Safety)
            {
                _invertersInfos[slaveGridIdx].Compliance_Progress = pgString;
            }

            switch (chipRole)
            {
                case FirmwareChipRole.ARM_OLD:
                case FirmwareChipRole.ARM:
                    _invertersInfos[slaveGridIdx].ARM_Progress = pgString;
                    break;

                case FirmwareChipRole.DSPM_OLD:
                case FirmwareChipRole.DSPM:
                    _invertersInfos[slaveGridIdx].DSPM_Progress = pgString;
                    break;

                case FirmwareChipRole.DSPS_OLD:
                case FirmwareChipRole.DSPS:
                    _invertersInfos[slaveGridIdx].DSPS_Progress = pgString;
                    break;

                case FirmwareChipRole.PLC_STA:
                    _invertersInfos[slaveGridIdx].PLCSTA_Progress = pgString;
                    break;
            }

            string statusMsg = $"进度:";
            if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].ARM_Progress))
            {
                statusMsg += $"[ARM:{_invertersInfos[slaveGridIdx].ARM_Progress}] ";
            }
            else if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].DSPM_Progress))
            {
                statusMsg += $"[DSPM:{_invertersInfos[slaveGridIdx].DSPM_Progress}] ";
            }
            else if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].DSPS_Progress))
            {
                statusMsg += $"[DSPS:{_invertersInfos[slaveGridIdx].DSPS_Progress}] ";
            }
            else if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].PLCSTA_Progress))
            {
                statusMsg += $"[STA:{_invertersInfos[slaveGridIdx].PLCSTA_Progress}] ";
            }
            else if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].Compliance_Progress))
            {
                statusMsg += $"[安规:{_invertersInfos[slaveGridIdx].Compliance_Progress}] ";
            }
            else
            {
                statusMsg += $"[升级:{pgString}] ";
            }

            if (statusMsg.Length > 0)
                statusMsg = statusMsg.Remove(statusMsg.Length - 1, 1);
            _invertersInfos[slaveGridIdx].StatusMsg = statusMsg;
        }

        private string? CompareVersions(int slaveGridIdx)
        {
            if (_subModuleInfos == null || _subModuleInfos.Length == 0)
            {
                return null;
            }

            var sb = new StringBuilder();

            string? importedVersion = _subModuleInfos
                .FirstOrDefault(m => (m.FirmwareChipRole == FirmwareChipRole.ARM
                                      || m.FirmwareChipRole == FirmwareChipRole.ARM_OLD))?
                .FirmwareVersion;
            string deviceVersion = _invertersInfos[slaveGridIdx].ARM_Version;
            if (!string.IsNullOrEmpty(importedVersion)
                && !string.IsNullOrEmpty(deviceVersion)
                && importedVersion != deviceVersion)
            {
                // sb.AppendLine($"ARM: [{deviceVersion}]->[{importedVersion}]");
                sb.Append("[ARM] ");
            }

            importedVersion = _subModuleInfos
                .FirstOrDefault(m => (m.FirmwareChipRole == FirmwareChipRole.DSPM
                                      || m.FirmwareChipRole == FirmwareChipRole.DSPM_OLD))?
                .FirmwareVersion;
            deviceVersion = _invertersInfos[slaveGridIdx].DSPM_Version;
            if (!string.IsNullOrEmpty(importedVersion)
                && !string.IsNullOrEmpty(deviceVersion)
                && importedVersion != deviceVersion)
            {
                // sb.AppendLine($"DSPM: [{deviceVersion}]->[{importedVersion}]");
                sb.Append("[DSPM] ");
            }

            importedVersion = _subModuleInfos
                .FirstOrDefault(m => (m.FirmwareChipRole == FirmwareChipRole.DSPS
                                      || m.FirmwareChipRole == FirmwareChipRole.DSPS_OLD))?
                .FirmwareVersion;
            deviceVersion = _invertersInfos[slaveGridIdx].DSPS_Version;
            if (!string.IsNullOrEmpty(importedVersion)
                && !string.IsNullOrEmpty(deviceVersion)
                && importedVersion != deviceVersion)
            {
                // sb.AppendLine($"DSPS: [{deviceVersion}]->[{importedVersion}]");
                sb.Append("[DSPS] ");
            }

            importedVersion = _subModuleInfos
                .FirstOrDefault(m => (m.FirmwareChipRole == FirmwareChipRole.PLC_STA))?
                .FirmwareVersion;
            deviceVersion = _invertersInfos[slaveGridIdx].PLCSTA_Version;
            if (!string.IsNullOrEmpty(importedVersion)
                && !string.IsNullOrEmpty(deviceVersion)
                && importedVersion != deviceVersion)
            {
                // sb.AppendLine($"PLC-STA: [{deviceVersion}]->[{importedVersion}]");
                sb.Append("[STA] ");
            }

            return sb.ToString();
        }



        private void RefreshInvertersGrid()
        {

            this.BeginInvoke(() =>
            {
                this.InvertersInfo_DataGridView.AutoGenerateColumns = false;

                if (_invertersInfos.Count == 0)
                {
                    this.InvertersInfo_DataGridView.DataSource = null;
                    return;
                }

                if (this.InvertersInfo_DataGridView.DataSource == null)
                {
                    this.InvertersInfo_DataGridView.DataSource = _invertersInfos;
                    return;
                }

                if (this.InvertersInfo_DataGridView.Rows.Count != _invertersInfos.Count)
                {
                    this.InvertersInfo_DataGridView.DataSource = null;
                    this.InvertersInfo_DataGridView.DataSource = _invertersInfos;
                    return;
                }
                else
                {
                    this.InvertersInfo_DataGridView.Invalidate();
                    return;
                }
            });
        }


        private void btnStopUpgrade_Click(object sender, EventArgs e)
        {
            if (!IsUpgrading)
            {
                return;
            }

            if (_globalStage >= G4UpgradeStage.Verification)
            {
                MessageBox.Show("目标设备已启动升级，无法取消。", "错误");
                return;
            }

            if (_cancellationTokenSourceList.Count>=1)
            {
                foreach (var _cts in _cancellationTokenSourceList)
                {
                    _cts.Cancel(false);
                }
            }
            else
            _cancellationTokenSource.Cancel(false);
            IsUpgrading = false;
        }

        #endregion 升级启停

        #region 升级信息打印

        //private void rtbUpgradeLog_ContentsResized(object sender, ContentsResizedEventArgs e)
        //{
        //    rtbUpgradeLog.SelectionStart = rtbUpgradeLog.Text.Length;
        //    rtbUpgradeLog.ScrollToCaret();
        //}

        //private void AppendUpgradeMsg(string message)
        //{
        //    this.BeginInvoke(() =>
        //    {
        //        rtbUpgradeLog.AppendText($"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n");
        //    });
        //}

        //private void ReplaceLastUpgradeMsg(string message)
        //{
        //    this.BeginInvoke(() =>
        //    {
        //        string currentMsg = rtbUpgradeLog.Text;
        //        int idx = currentMsg.Substring(0, currentMsg.Length - 1).LastIndexOf('\n');
        //        if (idx < 0) return;
        //        rtbUpgradeLog.Text = currentMsg.Substring(0, idx + 1) + $"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n";
        //    });
        //}

        //private void ClearUpgradeMsg()
        //{
        //    this.BeginInvoke(() =>
        //    {
        //        rtbUpgradeLog.Text = String.Empty;
        //    });
        //}

        #endregion 升级信息打印
    }


    public class G4InverterUpgradeInfo
    {
        public G4InverterUpgradeInfo(byte slaveNo)
        {
            SlaveNo = slaveNo;
        }

        public G4InverterUpgradeInfo(byte slaveNo, string ip)
        {
            SlaveNo = slaveNo;
            IP = ip;
        }

        public string IP { get; set; } = "";
        public byte SlaveNo { get; set; } = 0x01;
        public string SerialNo { get; set; } = "";
        public string ARM_Version { get; set; } = "";
        public string DSPM_Version { get; set; } = "";
        public string DSPS_Version { get; set; } = "";
        // public string AFCI_Version { get; set; } = "";
        public string PLCSTA_Version { get; set; } = "";
        public string Compliance_Version { get; set; } = "";


        public string ARM_Progress { get; set; } = "";

        public string DSPM_Progress { get; set; } = "";

        public string DSPS_Progress { get; set; } = "";

        public string PLCSTA_Progress { get; set; } = "";

        public string Compliance_Progress { get; set; } = "";

        public string StatusMsg { get; set; } = "";

        public string RefreshTime { get; set; } = "";
    }
}