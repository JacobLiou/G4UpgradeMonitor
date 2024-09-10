using Sofar.CommunicationLib;
using Sofar.G4MultiUpgrade.Common;
using System.Diagnostics;
using System.Net;

namespace Sofar.G4MultiUpgrade
{
    public partial class FrmConfig : Form
    {
        private ConnectionParams _connectParams;

        private readonly string _connectConfigFile = "Connection.json";

        public static Dictionary<string, bool> ConnectArray { get; private set; } = new Dictionary<string, bool>();

        public FrmConfig()
        {
            InitializeComponent();

            this.Load += FrmConfig_Load;
        }

        private void FrmConfig_Load(object? sender, EventArgs e)
        {
            Init();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int continueNum = Convert.ToUInt16(nudNumber.Value);

            string ip = txtIpAddress.Text.Trim();

            for (int i = 1; i <= continueNum; i++)
            {
                bool status = Connect(ip);
                ConnectArray.TryAdd(ip, status);

                this.AppendConnectMsg($"{ip} 连接状态：{status}");

                ip = IncrementLastOctet(ip);
            }
        }

        private void Init()
        {
            _connectParams = JsonFileHelper.LoadConfig<ConnectionParams>(_connectConfigFile) ?? new ConnectionParams();

            // IP
            this.txtIpAddress.Text = _connectParams.IP;
            CommManager.Instance.ConnectionParams = _connectParams;

            // Connect Button
            if (CommManager.Instance.ConnectionStatus != ConnectionType.None)
            {
                this.btnConnect.Text = "断开";
            }
            else
            {
                this.btnConnect.Text = "连接";
            }
        }

        private bool Connect(string ipAddress)
        {
            try
            {
                if (this.btnConnect.Text == "断开连接" &&
                    CommManager.Instance.ConnectionStatus != ConnectionType.None)
                {
                    CommManager.Instance.SwitchConnection(ConnectionType.None, _connectParams);
                    this.btnConnect.Text = "连接";
                    return true;
                }
                else if (this.btnConnect.Text == "连接")
                {
                    _connectParams.IP = ipAddress;
                    CommManager.Instance.SwitchConnection(ConnectionType.Gcp7428, _connectParams);
                    //JsonFileHelper.SaveConfig(_connectConfigFile, _connectParams);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Serilog.Log.Error($"启动连接错误:\n{exception}");
                //MessageBox.Show($"Connection Failed.", "错误");
            }
            return false;
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
    }
}
