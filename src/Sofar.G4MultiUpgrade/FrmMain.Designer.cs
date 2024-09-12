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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmi_Comm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ttxtCommState = new System.Windows.Forms.ToolStripTextBox();
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
            this.InvertersInfo_DataGridView = new System.Windows.Forms.DataGridView();
            this.Slave = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ARM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DSPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DSPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLCSTA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefreshTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rtbUpgradeLog = new System.Windows.Forms.RichTextBox();
            this.lvConnectlist = new System.Windows.Forms.ListView();
            this.btnStopUpgrade = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResendLostsRetries_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendingPackageInterval_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageSize_NumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InvertersInfo_DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Comm});
            this.menuStrip1.Location = new System.Drawing.Point(0, 35);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1406, 29);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmi_Comm
            // 
            this.tsmi_Comm.Name = "tsmi_Comm";
            this.tsmi_Comm.Size = new System.Drawing.Size(68, 21);
            this.tsmi_Comm.Text = "通讯管理";
            this.tsmi_Comm.Click += new System.EventHandler(this.tsmi_Comm_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.ttxtCommState});
            this.toolStrip1.Location = new System.Drawing.Point(0, 744);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1406, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "通讯状态";
            // 
            // ttxtCommState
            // 
            this.ttxtCommState.Name = "ttxtCommState";
            this.ttxtCommState.Size = new System.Drawing.Size(127, 25);
            this.ttxtCommState.Text = "None";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnStopUpgrade);
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
            this.groupBox4.Location = new System.Drawing.Point(0, 64);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.groupBox4.Size = new System.Drawing.Size(1406, 139);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "下载升级固件";
            // 
            // ResendLostsRetries_NumericUpDown
            // 
            this.ResendLostsRetries_NumericUpDown.Location = new System.Drawing.Point(723, 89);
            this.ResendLostsRetries_NumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.ResendLostsRetries_NumericUpDown.Name = "ResendLostsRetries_NumericUpDown";
            this.ResendLostsRetries_NumericUpDown.Size = new System.Drawing.Size(129, 21);
            this.ResendLostsRetries_NumericUpDown.TabIndex = 29;
            this.ResendLostsRetries_NumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // SendingPackageInterval_NumericUpDown
            // 
            this.SendingPackageInterval_NumericUpDown.Location = new System.Drawing.Point(440, 89);
            this.SendingPackageInterval_NumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.SendingPackageInterval_NumericUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.SendingPackageInterval_NumericUpDown.Name = "SendingPackageInterval_NumericUpDown";
            this.SendingPackageInterval_NumericUpDown.Size = new System.Drawing.Size(129, 21);
            this.SendingPackageInterval_NumericUpDown.TabIndex = 28;
            this.SendingPackageInterval_NumericUpDown.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // PackageSize_NumericUpDown
            // 
            this.PackageSize_NumericUpDown.Location = new System.Drawing.Point(131, 89);
            this.PackageSize_NumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.PackageSize_NumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PackageSize_NumericUpDown.Name = "PackageSize_NumericUpDown";
            this.PackageSize_NumericUpDown.Size = new System.Drawing.Size(129, 21);
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
            this.label3.Location = new System.Drawing.Point(612, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "补包上限次数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(313, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "传输包间隔(ms)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(30, 97);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "数据包大小";
            // 
            // btnStartUpgrade
            // 
            this.btnStartUpgrade.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStartUpgrade.Location = new System.Drawing.Point(864, 84);
            this.btnStartUpgrade.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnStartUpgrade.Name = "btnStartUpgrade";
            this.btnStartUpgrade.Size = new System.Drawing.Size(129, 38);
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
            this.ck40001.Location = new System.Drawing.Point(1008, 39);
            this.ck40001.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.ck40001.Name = "ck40001";
            this.ck40001.Size = new System.Drawing.Size(138, 16);
            this.ck40001.TabIndex = 19;
            this.ck40001.Text = "新协议版本（>=1.5）";
            this.ck40001.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(30, 40);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "文件路径";
            // 
            // btnImport
            // 
            this.btnImport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnImport.Location = new System.Drawing.Point(864, 27);
            this.btnImport.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(129, 38);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "选择文件";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(131, 33);
            this.txtPath.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(719, 21);
            this.txtPath.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.InvertersInfo_DataGridView);
            this.groupBox1.Controls.Add(this.rtbUpgradeLog);
            this.groupBox1.Controls.Add(this.lvConnectlist);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 203);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1406, 541);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "升级信息";
            // 
            // InvertersInfo_DataGridView
            // 
            this.InvertersInfo_DataGridView.AllowUserToAddRows = false;
            this.InvertersInfo_DataGridView.AllowUserToDeleteRows = false;
            this.InvertersInfo_DataGridView.AllowUserToResizeColumns = false;
            this.InvertersInfo_DataGridView.AllowUserToResizeRows = false;
            this.InvertersInfo_DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.InvertersInfo_DataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.InvertersInfo_DataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.InvertersInfo_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InvertersInfo_DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Slave,
            this.SerialNo,
            this.ARM,
            this.DSPM,
            this.DSPS,
            this.PLCSTA,
            this.Status,
            this.RefreshTime});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(3);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InvertersInfo_DataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.InvertersInfo_DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InvertersInfo_DataGridView.GridColor = System.Drawing.SystemColors.Control;
            this.InvertersInfo_DataGridView.Location = new System.Drawing.Point(266, 146);
            this.InvertersInfo_DataGridView.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.InvertersInfo_DataGridView.MultiSelect = false;
            this.InvertersInfo_DataGridView.Name = "InvertersInfo_DataGridView";
            this.InvertersInfo_DataGridView.ReadOnly = true;
            this.InvertersInfo_DataGridView.RowHeadersVisible = false;
            this.InvertersInfo_DataGridView.RowHeadersWidth = 51;
            this.InvertersInfo_DataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InvertersInfo_DataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.InvertersInfo_DataGridView.RowTemplate.Height = 29;
            this.InvertersInfo_DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.InvertersInfo_DataGridView.ShowCellErrors = false;
            this.InvertersInfo_DataGridView.ShowRowErrors = false;
            this.InvertersInfo_DataGridView.Size = new System.Drawing.Size(1136, 391);
            this.InvertersInfo_DataGridView.TabIndex = 39;
            // 
            // Slave
            // 
            this.Slave.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Slave.DataPropertyName = "SlaveNo";
            this.Slave.FillWeight = 60F;
            this.Slave.HeaderText = "设备号";
            this.Slave.MinimumWidth = 60;
            this.Slave.Name = "Slave";
            this.Slave.ReadOnly = true;
            this.Slave.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Slave.Width = 60;
            // 
            // SerialNo
            // 
            this.SerialNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SerialNo.DataPropertyName = "SerialNo";
            this.SerialNo.FillWeight = 50F;
            this.SerialNo.HeaderText = "序列号";
            this.SerialNo.MinimumWidth = 150;
            this.SerialNo.Name = "SerialNo";
            this.SerialNo.ReadOnly = true;
            this.SerialNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SerialNo.Width = 150;
            // 
            // ARM
            // 
            this.ARM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ARM.DataPropertyName = "ARM_Version";
            this.ARM.HeaderText = "ARM";
            this.ARM.MinimumWidth = 80;
            this.ARM.Name = "ARM";
            this.ARM.ReadOnly = true;
            this.ARM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ARM.Width = 80;
            // 
            // DSPM
            // 
            this.DSPM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DSPM.DataPropertyName = "DSPM_Version";
            this.DSPM.FillWeight = 50F;
            this.DSPM.HeaderText = "DSPM";
            this.DSPM.MinimumWidth = 90;
            this.DSPM.Name = "DSPM";
            this.DSPM.ReadOnly = true;
            this.DSPM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DSPM.Width = 90;
            // 
            // DSPS
            // 
            this.DSPS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DSPS.DataPropertyName = "DSPS_Version";
            this.DSPS.HeaderText = "DSPS";
            this.DSPS.MinimumWidth = 76;
            this.DSPS.Name = "DSPS";
            this.DSPS.ReadOnly = true;
            this.DSPS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.DSPS.Width = 76;
            // 
            // PLCSTA
            // 
            this.PLCSTA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.PLCSTA.DataPropertyName = "PLCSTA_Version";
            this.PLCSTA.HeaderText = "PLC-STA";
            this.PLCSTA.MinimumWidth = 70;
            this.PLCSTA.Name = "PLCSTA";
            this.PLCSTA.ReadOnly = true;
            this.PLCSTA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PLCSTA.Width = 70;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.DataPropertyName = "StatusMsg";
            this.Status.FillWeight = 130F;
            this.Status.HeaderText = "状态信息";
            this.Status.MinimumWidth = 160;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RefreshTime
            // 
            this.RefreshTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.RefreshTime.DataPropertyName = "RefreshTime";
            this.RefreshTime.HeaderText = "刷新时间";
            this.RefreshTime.MinimumWidth = 100;
            this.RefreshTime.Name = "RefreshTime";
            this.RefreshTime.ReadOnly = true;
            this.RefreshTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // rtbUpgradeLog
            // 
            this.rtbUpgradeLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.rtbUpgradeLog.Location = new System.Drawing.Point(266, 18);
            this.rtbUpgradeLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbUpgradeLog.Name = "rtbUpgradeLog";
            this.rtbUpgradeLog.Size = new System.Drawing.Size(1136, 128);
            this.rtbUpgradeLog.TabIndex = 30;
            this.rtbUpgradeLog.Text = "";
            // 
            // lvConnectlist
            // 
            this.lvConnectlist.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvConnectlist.Location = new System.Drawing.Point(4, 18);
            this.lvConnectlist.Margin = new System.Windows.Forms.Padding(4);
            this.lvConnectlist.Name = "lvConnectlist";
            this.lvConnectlist.Size = new System.Drawing.Size(262, 519);
            this.lvConnectlist.TabIndex = 0;
            this.lvConnectlist.UseCompatibleStateImageBehavior = false;
            this.lvConnectlist.View = System.Windows.Forms.View.Details;
            // 
            // btnStopUpgrade
            // 
            this.btnStopUpgrade.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStopUpgrade.Location = new System.Drawing.Point(1005, 83);
            this.btnStopUpgrade.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnStopUpgrade.Name = "btnStopUpgrade";
            this.btnStopUpgrade.Size = new System.Drawing.Size(129, 38);
            this.btnStopUpgrade.TabIndex = 30;
            this.btnStopUpgrade.Text = "停止升级";
            this.btnStopUpgrade.UseVisualStyleBackColor = true;
            this.btnStopUpgrade.Click += new System.EventHandler(this.btnStopUpgrade_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1406, 769);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmMain";
            this.Text = "升级平台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 1200, 751);
            this.Click += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResendLostsRetries_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendingPackageInterval_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageSize_NumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.InvertersInfo_DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private DataGridView InvertersInfo_DataGridView;
        private DataGridViewTextBoxColumn Slave;
        private DataGridViewTextBoxColumn SerialNo;
        private DataGridViewTextBoxColumn ARM;
        private DataGridViewTextBoxColumn DSPM;
        private DataGridViewTextBoxColumn DSPS;
        private DataGridViewTextBoxColumn PLCSTA;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewTextBoxColumn RefreshTime;
        private Button btnStopUpgrade;
    }
}

