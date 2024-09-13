using Sofar.CommunicationLib;
using Sofar.CommunicationLib.Model;
using Sofar.G4MultiUpgrade.Common;
using Sunny.UI;
using System.Diagnostics;
using System.Net;

namespace Sofar.G4MultiUpgrade
{
    public partial class FrmConfig : UIForm
    {
        private ConnectionParams _connectParams = new();

        private readonly string _connectConfigFile = "Connection.json";

        public static Dictionary<string, bool> ConnectArray { get; private set; } = new Dictionary<string, bool>();

        public FrmConfig()
        {
            InitializeComponent();

            this.Load += FrmConfig_Load;
        }

        private void FrmConfig_Load(object? sender, EventArgs e)
        {
            _connectParams = JsonFileHelper.LoadConfig<ConnectionParams>(_connectConfigFile) ?? new ConnectionParams();

            // IP
            this.txtIpAddress.Text = _connectParams.IPAdressList.Count > 0 ? _connectParams.IPAdressList[0] : "";
            nudNumber.Value = _connectParams.IPAdressList.Count > 0 ? _connectParams.IPAdressList.Count : 1;

            CommManager.Instance.ConnectionParams = _connectParams;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                SaveConfig();

                CommManager.Instance.Connect(_connectParams, out var _array);
                if (_array == null) return;
                rtbPrintinfo.Text = string.Empty;
                ConnectArray = _array;
                foreach (var item in _array)
                {
                    if (item.Value)
                        rtbPrintinfo.Text += $"IP:{item.Key},连接成功 \r\n";
                    else
                        rtbPrintinfo.Text += $"IP:{item.Key},连接失败 \r\n";
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Serilog.Log.Error($"启动连接错误:\n{exception}");
            }
        }

        private void SaveConfig()
        {
            _connectParams.IPAdressList.Clear();
            _connectParams.Port = int.Parse(txtIpPort.Text);

            int continueNum = Convert.ToUInt16(nudNumber.Value);
            string ip = txtIpAddress.Text.Trim();
            //_connectParams.IPAdressList.Add(ip);
            for (int i = 1; i <= continueNum; i++)
            {
                _connectParams.IPAdressList.Add(ip);

                ip = IncrementLastOctet(ip);    //计算下一个IP地址
            }

            JsonFileHelper.SaveConfig(_connectConfigFile, _connectParams);
        }

        private void btnDisConnect_Click(object sender, EventArgs e)
        {
            CommManager.Instance.DisConnect();
            var disConnectArray = new Dictionary<string, bool>();
            foreach (var keyValue in ConnectArray)
            {
                {
                    disConnectArray.Add(keyValue.Key, false);
                }
            }

            ConnectArray = disConnectArray;
            rtbPrintinfo.Text = "全部断开连接";
        }

        private string IncrementLastOctet(string ipAddress)
        {
            // 确保IP地址格式正确
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                throw new ArgumentException("Invalid IP address format.");
            }

            // 分割IP地址
            string[] parts = ipAddress.Split('.');

            // 检查是否有足够的部分
            if (parts.Length != 4)
            {
                throw new ArgumentException("IP address must have exactly 4 octets.");
            }

            // 尝试将最后一部分转换为整数
            if (!int.TryParse(parts[3], out int lastOctet))
            {
                throw new ArgumentException("The last octet of the IP address must be a number.");
            }

            // 将最后一部分加1，并处理溢出（回到0）
            lastOctet = (lastOctet + 1) % 256;

            // 将新的最后一部分转换回字符串，并重新组合IP地址
            parts[3] = lastOctet.ToString();
            return string.Join(".", parts);
        }

        private void AppendConnectMsg(string message)
        {
            this.Invoke(new Action(() =>
            {
                rtbPrintinfo.Text += $"{DateTime.Now.ToString("HH:mm:ss.fff")} {message}\n";
            }));
        }

        private void uiButton_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void uiButton_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}