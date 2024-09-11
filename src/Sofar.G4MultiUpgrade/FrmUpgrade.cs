using Serilog;
using Sofar.CommunicationLib;
using Sofar.CommunicationLib.Model;
using Sofar.CommunicationLib.Service;
using Sofar.ProtocolLibs.FirmwareInfo;
using System.Diagnostics;

namespace Sofar.G4MultiUpgrade
{
    public partial class FrmUpgrade : Form
    {
        private string? _firmwarePath;

        private SofarSubFirmwareInfo[]? _subModuleInfos;

        private List<G4UpgradeService> _upgradeService;

        private readonly ILogger _logger = Serilog.Log.Logger;

        public bool IsUpgrading { get; private set; } = false;

        public FrmUpgrade()
        {
            InitializeComponent();

            _upgradeService = new();
            foreach (var item in CommManager.Instance.ModbusClients)
            {
                _upgradeService.Add(new G4UpgradeService(item));
            }
        }

        private void FrmUpgrade_Load(object sender, EventArgs e)
        {
            // 加载通讯列表...

            // 创建ListView的列
            lvConnectlist.View = View.Details;
            lvConnectlist.Columns.Add("IpAddress", 120);
            lvConnectlist.Columns.Add("State", 50);

            BindDictionaryToListView(FrmConfig.ConnectArray, lvConnectlist);
        }

        private void BindDictionaryToListView(Dictionary<string, bool> dict, ListView listView)
        {
            // 清除ListView中现有的项
            listView.Items.Clear();

            // 遍历字典
            foreach (var kvp in dict)
            {
                // 创建一个新的ListViewItem
                ListViewItem item = new ListViewItem(kvp.Key); // 第一个参数是Key，也是主项

                // 为ListViewItem添加子项，此处是bool值转换为字符串
                item.SubItems.Add(kvp.Value.ToString());

                // 将ListViewItem添加到ListView中
                listView.Items.Add(item);
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
                this.rtbUpgradeLog.Text = $"{System.DateTime.Now} -->{versions}";
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

        private void btnStartUpgrade_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                foreach (var item in _upgradeService)
                {
                    await SingUpgrade(item);
                }
            });
        }

        private async Task SingUpgrade(G4UpgradeService _upgradeService)
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

            //if (!GetDeviceAddresses(out byte[] slaves))
            //{
            //    return;
            //}

            IsUpgrading = true;
            ClearUpgradeMsg();
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
                IsBroadcast = true,
                ResumeFromBreakPoint = false,
                UseNew5001 = false,
                Use5108 = false,
            };

            var readFinishedList = new byte[] { };//await ReadDeviceInfo(slaves);

            var pgReporter = new Progress<G4UpgradeProgressInfo>();
            //pgReporter.ProgressChanged += PgReporter_OnProgressChanged;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _logger.Information("\n---------------G4升级开始--------------");
                var sw = Stopwatch.StartNew();

                // 升级
                _globalStage = G4UpgradeStage.None;

                await _upgradeService.G4FirmwareUpgradeAsync(readFinishedList, _firmwarePath!, upgradeConfig,
                    _cancellationTokenSource.Token, pgReporter);

                /*
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
                }*/

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
                _globalStage = G4UpgradeStage.None;
                //StopBusy();
            }
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
        }

        #endregion 升级启停

        #region 升级信息打印

        private void rtbUpgradeLog_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            rtbUpgradeLog.SelectionStart = rtbUpgradeLog.Text.Length;
            rtbUpgradeLog.ScrollToCaret();
        }

        private void AppendUpgradeMsg(string message)
        {
            this.BeginInvoke(() =>
            {
                rtbUpgradeLog.AppendText($"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n");
            });
        }

        private void ReplaceLastUpgradeMsg(string message)
        {
            this.BeginInvoke(() =>
            {
                string currentMsg = rtbUpgradeLog.Text;
                int idx = currentMsg.Substring(0, currentMsg.Length - 1).LastIndexOf('\n');
                if (idx < 0) return;
                rtbUpgradeLog.Text = currentMsg.Substring(0, idx + 1) + $"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n";
            });
        }

        private void ClearUpgradeMsg()
        {
            this.BeginInvoke(() =>
            {
                rtbUpgradeLog.Text = String.Empty;
            });
        }

        #endregion 升级信息打印
    }
}