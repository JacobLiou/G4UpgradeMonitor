namespace Sofar.G4MultiUpgrade
{
    partial class FrmUpgrade
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ResendLostsRetries_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SendingPackageInterval_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.PackageSize_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartUpgrade = new System.Windows.Forms.Button();
            this.ck40001 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtbUpgradeLog = new System.Windows.Forms.RichTextBox();
            this.lvConnectlist = new System.Windows.Forms.ListView();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResendLostsRetries_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendingPackageInterval_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageSize_NumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ResendLostsRetries_NumericUpDown);
            this.groupBox4.Controls.Add(this.SendingPackageInterval_NumericUpDown);
            this.groupBox4.Controls.Add(this.PackageSize_NumericUpDown);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.btnStartUpgrade);
            this.groupBox4.Controls.Add(this.ck40001);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.btnImport);
            this.groupBox4.Controls.Add(this.txtPath);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(10, 10);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox4.Size = new System.Drawing.Size(988, 118);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "下载升级固件";
            // 
            // ResendLostsRetries_NumericUpDown
            // 
            this.ResendLostsRetries_NumericUpDown.Location = new System.Drawing.Point(562, 76);
            this.ResendLostsRetries_NumericUpDown.Name = "ResendLostsRetries_NumericUpDown";
            this.ResendLostsRetries_NumericUpDown.Size = new System.Drawing.Size(100, 23);
            this.ResendLostsRetries_NumericUpDown.TabIndex = 29;
            this.ResendLostsRetries_NumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // SendingPackageInterval_NumericUpDown
            // 
            this.SendingPackageInterval_NumericUpDown.Location = new System.Drawing.Point(342, 76);
            this.SendingPackageInterval_NumericUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.SendingPackageInterval_NumericUpDown.Name = "SendingPackageInterval_NumericUpDown";
            this.SendingPackageInterval_NumericUpDown.Size = new System.Drawing.Size(100, 23);
            this.SendingPackageInterval_NumericUpDown.TabIndex = 28;
            this.SendingPackageInterval_NumericUpDown.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // PackageSize_NumericUpDown
            // 
            this.PackageSize_NumericUpDown.Location = new System.Drawing.Point(102, 76);
            this.PackageSize_NumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PackageSize_NumericUpDown.Name = "PackageSize_NumericUpDown";
            this.PackageSize_NumericUpDown.Size = new System.Drawing.Size(100, 23);
            this.PackageSize_NumericUpDown.TabIndex = 27;
            this.PackageSize_NumericUpDown.Value = new decimal(new int[] {
            220,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(474, 79);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 25;
            this.label3.Text = "补包上限次数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(241, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "传输包间隔(ms)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(21, 79);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 21;
            this.label1.Text = "数据包大小";
            // 
            // btnStartUpgrade
            // 
            this.btnStartUpgrade.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStartUpgrade.Location = new System.Drawing.Point(672, 71);
            this.btnStartUpgrade.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnStartUpgrade.Name = "btnStartUpgrade";
            this.btnStartUpgrade.Size = new System.Drawing.Size(100, 32);
            this.btnStartUpgrade.TabIndex = 15;
            this.btnStartUpgrade.Text = "启动升级";
            this.btnStartUpgrade.UseVisualStyleBackColor = true;
            this.btnStartUpgrade.Click += new System.EventHandler(this.btnStartUpgrade_Click);
            // 
            // ck40001
            // 
            this.ck40001.AutoSize = true;
            this.ck40001.Checked = true;
            this.ck40001.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ck40001.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ck40001.Location = new System.Drawing.Point(782, 30);
            this.ck40001.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.ck40001.Name = "ck40001";
            this.ck40001.Size = new System.Drawing.Size(146, 21);
            this.ck40001.TabIndex = 19;
            this.ck40001.Text = "新协议版本（>=1.5）";
            this.ck40001.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(21, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 16;
            this.label6.Text = "文件路径";
            // 
            // btnImport
            // 
            this.btnImport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnImport.Location = new System.Drawing.Point(672, 23);
            this.btnImport.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(100, 32);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "选择文件";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(102, 28);
            this.txtPath.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(560, 23);
            this.txtPath.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtbUpgradeLog);
            this.groupBox1.Controls.Add(this.lvConnectlist);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(10, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(988, 591);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "升级信息";
            // 
            // rtbUpgradeLog
            // 
            this.rtbUpgradeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbUpgradeLog.Location = new System.Drawing.Point(208, 19);
            this.rtbUpgradeLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbUpgradeLog.Name = "rtbUpgradeLog";
            this.rtbUpgradeLog.Size = new System.Drawing.Size(777, 569);
            this.rtbUpgradeLog.TabIndex = 30;
            this.rtbUpgradeLog.Text = "";
            this.rtbUpgradeLog.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.rtbUpgradeLog_ContentsResized);
            // 
            // lvConnectlist
            // 
            this.lvConnectlist.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvConnectlist.Location = new System.Drawing.Point(3, 19);
            this.lvConnectlist.Name = "lvConnectlist";
            this.lvConnectlist.Size = new System.Drawing.Size(205, 569);
            this.lvConnectlist.TabIndex = 0;
            this.lvConnectlist.UseCompatibleStateImageBehavior = false;
            this.lvConnectlist.View = System.Windows.Forms.View.Details;
            // 
            // FrmUpgrade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmUpgrade";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmUpgrade";
            this.Load += new System.EventHandler(this.FrmUpgrade_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResendLostsRetries_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendingPackageInterval_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageSize_NumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartUpgrade;
        private System.Windows.Forms.CheckBox ck40001;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.TextBox txtPath;
        private Label label3;
        private GroupBox groupBox1;
        private ListView lvConnectlist;
        private DataGridView dgvInvertersInfo;
        private DataGridViewTextBoxColumn Slave;
        private DataGridViewTextBoxColumn SerialNo;
        private DataGridViewTextBoxColumn ARM;
        private DataGridViewTextBoxColumn DSPM;
        private DataGridViewTextBoxColumn DSPS;
        private DataGridViewTextBoxColumn PLCSTA;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewTextBoxColumn RefreshTime;
        private RichTextBox rtbUpgradeLog;
        private NumericUpDown PackageSize_NumericUpDown;
        private NumericUpDown SendingPackageInterval_NumericUpDown;
        private NumericUpDown ResendLostsRetries_NumericUpDown;
    }
}