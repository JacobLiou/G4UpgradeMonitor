using System.Diagnostics;

namespace Sofar.G4MultiUpgrade
{
    public partial class FrmMain : Form
    {
        private System.Timers.Timer? timer;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer(1000);
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
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
            FrmConfig frmConfig = new FrmConfig();
            frmConfig.MdiParent = this;
            frmConfig.Show();
        }

        private void tsmi_Upgrade_Click(object sender, EventArgs e)
        {
            //check common status...?

            FrmUpgrade frmUpgrade = new FrmUpgrade();
            frmUpgrade.MdiParent = this;
            frmUpgrade.Show();
        }
    }
}