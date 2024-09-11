namespace Sofar.G4MultiUpgrade
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            tsmi_Comm = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            ttxtCommState = new ToolStripTextBox();
            groupBox4 = new GroupBox();
            ResendLostsRetries_NumericUpDown = new NumericUpDown();
            SendingPackageInterval_NumericUpDown = new NumericUpDown();
            PackageSize_NumericUpDown = new NumericUpDown();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            btnStartUpgrade = new Button();
            ck40001 = new CheckBox();
            label6 = new Label();
            btnImport = new Button();
            txtPath = new TextBox();
            groupBox1 = new GroupBox();
            rtbUpgradeLog = new RichTextBox();
            lvConnectlist = new ListView();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ResendLostsRetries_NumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SendingPackageInterval_NumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PackageSize_NumericUpDown).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsmi_Comm });
            menuStrip1.Location = new Point(0, 35);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(9, 4, 0, 4);
            menuStrip1.Size = new Size(1406, 32);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsmi_Comm
            // 
            tsmi_Comm.Name = "tsmi_Comm";
            tsmi_Comm.Size = new Size(83, 24);
            tsmi_Comm.Text = "通讯管理";
            tsmi_Comm.Click += tsmi_Comm_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, ttxtCommState });
            toolStrip1.Location = new Point(0, 742);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1406, 27);
            toolStrip1.TabIndex = 6;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(69, 24);
            toolStripLabel1.Text = "通讯状态";
            // 
            // ttxtCommState
            // 
            ttxtCommState.Name = "ttxtCommState";
            ttxtCommState.Size = new Size(127, 27);
            ttxtCommState.Text = "None";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(ResendLostsRetries_NumericUpDown);
            groupBox4.Controls.Add(SendingPackageInterval_NumericUpDown);
            groupBox4.Controls.Add(PackageSize_NumericUpDown);
            groupBox4.Controls.Add(label3);
            groupBox4.Controls.Add(label2);
            groupBox4.Controls.Add(label1);
            groupBox4.Controls.Add(btnStartUpgrade);
            groupBox4.Controls.Add(ck40001);
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(btnImport);
            groupBox4.Controls.Add(txtPath);
            groupBox4.Dock = DockStyle.Top;
            groupBox4.Location = new Point(0, 67);
            groupBox4.Margin = new Padding(6, 7, 6, 7);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(6, 7, 6, 7);
            groupBox4.Size = new Size(1406, 139);
            groupBox4.TabIndex = 21;
            groupBox4.TabStop = false;
            groupBox4.Text = "下载升级固件";
            // 
            // ResendLostsRetries_NumericUpDown
            // 
            ResendLostsRetries_NumericUpDown.Location = new Point(723, 89);
            ResendLostsRetries_NumericUpDown.Margin = new Padding(4);
            ResendLostsRetries_NumericUpDown.Name = "ResendLostsRetries_NumericUpDown";
            ResendLostsRetries_NumericUpDown.Size = new Size(129, 25);
            ResendLostsRetries_NumericUpDown.TabIndex = 29;
            ResendLostsRetries_NumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // SendingPackageInterval_NumericUpDown
            // 
            SendingPackageInterval_NumericUpDown.Location = new Point(440, 89);
            SendingPackageInterval_NumericUpDown.Margin = new Padding(4);
            SendingPackageInterval_NumericUpDown.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            SendingPackageInterval_NumericUpDown.Name = "SendingPackageInterval_NumericUpDown";
            SendingPackageInterval_NumericUpDown.Size = new Size(129, 25);
            SendingPackageInterval_NumericUpDown.TabIndex = 28;
            SendingPackageInterval_NumericUpDown.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // PackageSize_NumericUpDown
            // 
            PackageSize_NumericUpDown.Location = new Point(131, 89);
            PackageSize_NumericUpDown.Margin = new Padding(4);
            PackageSize_NumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            PackageSize_NumericUpDown.Name = "PackageSize_NumericUpDown";
            PackageSize_NumericUpDown.Size = new Size(129, 25);
            PackageSize_NumericUpDown.TabIndex = 27;
            PackageSize_NumericUpDown.Value = new decimal(new int[] { 220, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ImeMode = ImeMode.NoControl;
            label3.Location = new Point(612, 97);
            label3.Margin = new Padding(6, 0, 6, 0);
            label3.Name = "label3";
            label3.Size = new Size(97, 15);
            label3.TabIndex = 25;
            label3.Text = "补包上限次数";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ImeMode = ImeMode.NoControl;
            label2.Location = new Point(313, 96);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(114, 15);
            label2.TabIndex = 23;
            label2.Text = "传输包间隔(ms)";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ImeMode = ImeMode.NoControl;
            label1.Location = new Point(30, 97);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 21;
            label1.Text = "数据包大小";
            // 
            // btnStartUpgrade
            // 
            btnStartUpgrade.ImeMode = ImeMode.NoControl;
            btnStartUpgrade.Location = new Point(864, 84);
            btnStartUpgrade.Margin = new Padding(6, 7, 6, 7);
            btnStartUpgrade.Name = "btnStartUpgrade";
            btnStartUpgrade.Size = new Size(129, 38);
            btnStartUpgrade.TabIndex = 15;
            btnStartUpgrade.Text = "启动升级";
            btnStartUpgrade.UseVisualStyleBackColor = true;
            // 
            // ck40001
            // 
            ck40001.AutoSize = true;
            ck40001.Checked = true;
            ck40001.CheckState = CheckState.Checked;
            ck40001.ImeMode = ImeMode.NoControl;
            ck40001.Location = new Point(1008, 39);
            ck40001.Margin = new Padding(6, 7, 6, 7);
            ck40001.Name = "ck40001";
            ck40001.Size = new Size(174, 19);
            ck40001.TabIndex = 19;
            ck40001.Text = "新协议版本（>=1.5）";
            ck40001.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ImeMode = ImeMode.NoControl;
            label6.Location = new Point(30, 40);
            label6.Margin = new Padding(6, 0, 6, 0);
            label6.Name = "label6";
            label6.Size = new Size(67, 15);
            label6.TabIndex = 16;
            label6.Text = "文件路径";
            // 
            // btnImport
            // 
            btnImport.ImeMode = ImeMode.NoControl;
            btnImport.Location = new Point(864, 27);
            btnImport.Margin = new Padding(6, 7, 6, 7);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(129, 38);
            btnImport.TabIndex = 1;
            btnImport.Text = "选择文件";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // txtPath
            // 
            txtPath.Location = new Point(131, 33);
            txtPath.Margin = new Padding(6, 7, 6, 7);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(719, 25);
            txtPath.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rtbUpgradeLog);
            groupBox1.Controls.Add(lvConnectlist);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 67);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(1406, 675);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "升级信息";
            // 
            // rtbUpgradeLog
            // 
            rtbUpgradeLog.Dock = DockStyle.Fill;
            rtbUpgradeLog.Location = new Point(266, 22);
            rtbUpgradeLog.Margin = new Padding(4, 5, 4, 5);
            rtbUpgradeLog.Name = "rtbUpgradeLog";
            rtbUpgradeLog.Size = new Size(1136, 649);
            rtbUpgradeLog.TabIndex = 30;
            rtbUpgradeLog.Text = "";
            // 
            // lvConnectlist
            // 
            lvConnectlist.Dock = DockStyle.Left;
            lvConnectlist.Location = new Point(4, 22);
            lvConnectlist.Margin = new Padding(4);
            lvConnectlist.Name = "lvConnectlist";
            lvConnectlist.Size = new Size(262, 649);
            lvConnectlist.TabIndex = 0;
            lvConnectlist.UseCompatibleStateImageBehavior = false;
            lvConnectlist.View = View.Details;
            // 
            // FrmMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1406, 769);
            Controls.Add(groupBox4);
            Controls.Add(groupBox1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(5);
            Name = "FrmMain";
            Text = "升级平台";
            WindowState = FormWindowState.Maximized;
            ZoomScaleRect = new Rectangle(19, 19, 1200, 751);
            Load += FrmMain_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ResendLostsRetries_NumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SendingPackageInterval_NumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)PackageSize_NumericUpDown).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Comm;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripTextBox ttxtCommState;
        private GroupBox groupBox4;
        private NumericUpDown ResendLostsRetries_NumericUpDown;
        private NumericUpDown SendingPackageInterval_NumericUpDown;
        private NumericUpDown PackageSize_NumericUpDown;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button btnStartUpgrade;
        private CheckBox ck40001;
        private Label label6;
        private Button btnImport;
        private TextBox txtPath;
        private GroupBox groupBox1;
        private RichTextBox rtbUpgradeLog;
        private ListView lvConnectlist;
    }
}

