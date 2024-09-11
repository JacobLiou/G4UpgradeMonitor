using Serilog;
using Sofar.CommunicationLib.Service.AppModels;
using Sofar.CommunicationLib.Service.FileTransfer;
using Sofar.ProtocolLibs.FirmwareInfo;
using System.Diagnostics;
using System.Text;

namespace Sofar.G4MultiUpgrade.Control
{
    public partial class UC_G4Upgrade : UC_Base
    {
        private G4UpgradeService _upgradeService = new();


        private bool _isBusy = false;
        public bool IsUpgrading { get; private set; } = false;

        private readonly List<G4InverterUpgradeInfo> _invertersInfos = new();

        private string? _firmwarePath;

        private SofarSubFirmwareInfo[]? _subModuleInfos;

        private readonly ILogger _logger = Serilog.Log.Logger;

        public UC_G4Upgrade()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            this.ToolTip.SetToolTip(this.DevAddrRange_TextBox, "示例:\"1-30\"; \n\"1, 4-7, 10, 15-20\"");


        }



        #region 读取版本号

        private async void ReadDevInfo_Button_Click(object sender, EventArgs e)
        {
            if (IsUpgrading)
            {
                ShowMsgBoxError("正在执行任务，无法读取逆变器信息。");
                return;
            }

            if (!GetDeviceAddresses(out byte[] slaves))
            {
                return;
            }


            IsUpgrading = true;
            UpdateMsgLabel(this.ReadInfo_Msg_Label, "读取中...", -1);
            StartBusy("读取逆变器信息");

            var finishedList = await ReadDeviceInfo(slaves);

            if (finishedList.Length == 0)
            {
                UpdateMsgLabel(this.ReadInfo_Msg_Label, "读取失败", 3000);
            }
            else
            {
                UpdateMsgLabel(this.ReadInfo_Msg_Label, "读取完成，请查看列表", 3000);
            }

            IsUpgrading = false;
            StopBusy();
        }


        private async Task<byte[]> ReadDeviceInfo(byte[] slaves)
        {
            _invertersInfos.Clear();
            foreach (var slave in slaves)
            {
                _invertersInfos.Add(new G4InverterUpgradeInfo(slave));
            }

            var failedList = new SortedSet<byte>();
            var finishedList = new SortedSet<byte>();

            int maxRetries = 3;
            for (int i = 0; i < _invertersInfos.Count; ++i)
            {
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    InverterInfo? infos = await Task.Run(() => ReadInverterInfoService.TryReadDeviceInfoG4(_invertersInfos[i].SlaveNo));

                    if (infos != null)
                    {
                        _invertersInfos[i].SerialNo = infos.SerialNumber!;
                        _invertersInfos[i].ARM_Version = infos.ARM_Version!;
                        _invertersInfos[i].DSPM_Version = infos.DSPM_Version!;
                        _invertersInfos[i].DSPS_Version = infos.DSPS_Version!;
                        _invertersInfos[i].PLCSTA_Version = infos.PLCSTA_Version!;
                        _invertersInfos[i].StatusMsg = "读取逆变器信息完成。";
                        _invertersInfos[i].RefreshTime = DateTime.Now.ToString("MM/dd-HH:mm:ss");
                        finishedList.Add(_invertersInfos[i].SlaveNo);
                        break;
                    }
                    else
                    {
                        if (retry == maxRetries - 1)
                        {
                            _invertersInfos[i].StatusMsg = "读取逆变器信息失败";
                            _invertersInfos[i].RefreshTime = DateTime.Now.ToString("MM/dd-HH:mm:ss");
                            failedList.Add(_invertersInfos[i].SlaveNo);
                        }
                    }

                }

                RefreshInvertersGrid();
            }


            return finishedList.ToArray();
        }


        #endregion


        #region 地址范围

        private bool GetDeviceAddresses(out byte[] slaves)
        {
            slaves = Array.Empty<byte>();
            if (string.IsNullOrEmpty(this.DevAddrRange_TextBox.Text))
            {
                MessageBox.Show("未输入设备号！", "错误");
                return false;
            }

            if (TryParseRangeExpression(this.DevAddrRange_TextBox.Text, out var range, 1, 247))
            {
                slaves = new byte[range.Length];
                for (int i = 0; i < range.Length; i++)
                {
                    slaves[i] = (byte)range[i];
                }
                return true;
            }
            else
            {
                MessageBox.Show("子设备地址表达式内容错误或存在超出支持范围（1~247）的地址号！", "错误");
                return false;
            }
        }


        #endregion


        #region 固件导入

        private void SofarPackage_ImportBtn_Click(object sender, EventArgs e)
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
                this.SofarVersions_Label.Text = versions;
                this.SofarPackagePath_TextBox.Text = filename;
                _firmwarePath = filename;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "导入错误");
                return;
            }

        }

        private void SofarPackage_Cancel_ImportBtn_Click(object sender, EventArgs e)
        {
            this.SofarVersions_Label.Text = string.Empty;
            this.SofarPackagePath_TextBox.Text = string.Empty;
            _subModuleInfos = null;
            _firmwarePath = null;
        }

        #endregion


        #region 升级启停

        private G4UpgradeStage _globalStage = G4UpgradeStage.None;
        private CancellationTokenSource _cancellationTokenSource;
        private async void StartUpgrade_Button_Click(object sender, EventArgs e)
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

            if (!GetDeviceAddresses(out byte[] slaves))
            {
                return;
            }

            IsUpgrading = true;
            this.UpgradeSettings_Panel.Enabled = false;
            ClearUpgradeMsg();
            StartBusy("正在升级G4设备");

            G4UpgradeConfig upgradeConfig = new G4UpgradeConfig
            {
                RequestMaxTries = 3,
                SendingSegmentSize = (int)PackageSize_NumericUpDown.Value,
                SendingInterval = (int)SendingPackageInterval_NumericUpDown.Value,
                ResendLostsMaxRetries = (int)ResendLostsRetries_NumericUpDown.Value,
                BitmapSegmentSize = 100,
                IsSofarOrBin = true,
                UpgradeTime = null,
                IsBroadcast = true,
                ResumeFromBreakPoint = false,
                UseNew5001 = false,
                Use5108 = false,
            };

            var readFinishedList = await ReadDeviceInfo(slaves);

            var pgReporter = new Progress<G4UpgradeProgressInfo>();
            pgReporter.ProgressChanged += PgReporter_OnProgressChanged;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _logger.Information("\n---------------G4升级开始--------------");

                var sw = Stopwatch.StartNew();

                // 升级
                _globalStage = G4UpgradeStage.None;


                await _upgradeService.G4FirmwareUpgradeAsync(readFinishedList, _firmwarePath!, upgradeConfig,
                    _cancellationTokenSource.Token, pgReporter);


                // 等待复位并重读版本号
                if (!_cancellationTokenSource.IsCancellationRequested
                    && _globalStage >= G4UpgradeStage.Finished)
                {
                    for (int i = 30; i >= 0; i--)
                    {
                        for (int j = 0; j < _invertersInfos.Count; j++)
                        {
                            _invertersInfos[j].StatusMsg = $"等待设备复位[{i}s]";
                        }
                        RefreshInvertersGrid();
                        await Task.Delay(1000);
                    }

                    readFinishedList = await ReadDeviceInfo(readFinishedList);
                    for (int i = 0; i < _invertersInfos.Count; i++)
                    {
                        if (!readFinishedList.Contains(_invertersInfos[i].SlaveNo))
                            continue;

                        string? versionsHint = CompareVersions(i);
                        if (!string.IsNullOrEmpty(versionsHint))
                        {
                            _invertersInfos[i].StatusMsg = $"升级失败：{versionsHint}";
                        }
                        else
                        {
                            _invertersInfos[i].StatusMsg = "升级成功";
                        }
                    }
                    RefreshInvertersGrid();
                }

                var timeSpan = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString("%m");
                AppendUpgradeMsg($"操作总耗时：{timeSpan}Min");

                _logger.Information("---------------G4升级结束--------------\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"升级失败，内部错误：\n{ex.Message}", "错误");
            }

            finally
            {
                IsUpgrading = false;
                this.UpgradeSettings_Panel.Enabled = true;
                _globalStage = G4UpgradeStage.None;
                StopBusy();
            }

        }

        private void PgReporter_OnProgressChanged(object? sender, G4UpgradeProgressInfo pgInfo)
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
            int idx = _invertersInfos.FindIndex(x => x.SlaveNo == slave);
            if (idx < 0) return;

            if (pgInfo.Failed)
            {
                if (pgInfo.Stage == G4UpgradeStage.Cancelled)
                {
                    _invertersInfos[idx].StatusMsg = "升级已取消";
                    AppendUpgradeMsg($"升级已取消。");

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


        private void StopUpgrade_Button_Click(object sender, EventArgs e)
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


            _cancellationTokenSource.Cancel(false);
            IsUpgrading = false;

            this.UpgradeSettings_Panel.Enabled = true;
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
            if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].DSPM_Progress))
            {
                statusMsg += $"[DSPM:{_invertersInfos[slaveGridIdx].DSPM_Progress}] ";
            }
            if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].DSPS_Progress))
            {
                statusMsg += $"[DSPS:{_invertersInfos[slaveGridIdx].DSPS_Progress}] ";
            }
            if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].PLCSTA_Progress))
            {
                statusMsg += $"[STA:{_invertersInfos[slaveGridIdx].PLCSTA_Progress}] ";
            }
            if (!string.IsNullOrEmpty(_invertersInfos[slaveGridIdx].Compliance_Progress))
            {
                statusMsg += $"[安规:{_invertersInfos[slaveGridIdx].Compliance_Progress}] ";
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


        #endregion


        #region 升级信息打印

        private void UpgradeLog_RichTextBox_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            UpgradeLog_RichTextBox.SelectionStart = UpgradeLog_RichTextBox.Text.Length;
            UpgradeLog_RichTextBox.ScrollToCaret();
        }

        private void AppendUpgradeMsg(string message)
        {
            this.BeginInvoke(() =>
            {
                UpgradeLog_RichTextBox.AppendText($"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n");
            });

        }

        private void ReplaceLastUpgradeMsg(string message)
        {
            this.BeginInvoke(() =>
            {
                string currentMsg = UpgradeLog_RichTextBox.Text;
                int idx = currentMsg.Substring(0, currentMsg.Length - 1).LastIndexOf('\n');
                if (idx < 0) return;
                UpgradeLog_RichTextBox.Text = currentMsg.Substring(0, idx + 1) + $"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n";
            });
        }

        private void ClearUpgradeMsg()
        {
            this.BeginInvoke(() =>
            {
                UpgradeLog_RichTextBox.Text = String.Empty;
            });
        }




        #endregion

    }

    public class G4InverterUpgradeInfo
    {
        public G4InverterUpgradeInfo(byte slaveNo)
        {
            SlaveNo = slaveNo;
        }

        public byte SlaveNo { get; set; } = 0;
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
