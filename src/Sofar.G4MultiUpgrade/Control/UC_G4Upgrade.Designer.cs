namespace Sofar.G4MultiUpgrade.Control
{
    partial class UC_G4Upgrade
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            tableLayoutPanel1 = new TableLayoutPanel();
            DeviceInfo_GroupBox = new GroupBox();
            InvertersInfo_DataGridView = new DataGridView();
            Slave = new DataGridViewTextBoxColumn();
            SerialNo = new DataGridViewTextBoxColumn();
            ARM = new DataGridViewTextBoxColumn();
            DSPM = new DataGridViewTextBoxColumn();
            DSPS = new DataGridViewTextBoxColumn();
            PLCSTA = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            RefreshTime = new DataGridViewTextBoxColumn();
            Log_GroupBox = new GroupBox();
            UpgradeLog_RichTextBox = new RichTextBox();
            DevConf_GroupBox = new GroupBox();
            ReadInfo_Msg_Label = new Label();
            ReadDevInfo_Button = new Button();
            DevAddrRange_TextBox = new TextBox();
            SubdevAddr_Label = new Label();
            UpgradeControl_GroupBox = new GroupBox();
            UpgradeSettings_Panel = new Panel();
            UpgradeTime_CheckBox = new CheckBox();
            PackageSize_NumericUpDown = new NumericUpDown();
            label1 = new Label();
            label6 = new Label();
            ResendLostsRetries_NumericUpDown = new NumericUpDown();
            Upgrade_DateTimePicker = new DateTimePicker();
            label7 = new Label();
            SendingPackageInterval_NumericUpDown = new NumericUpDown();
            StopUpgrade_Button = new Button();
            StartUpgrade_Button = new Button();
            Firmware_GroupBox = new GroupBox();
            SofarVersions_Label = new Label();
            SofarPackage_Label = new Label();
            SofarPackage_Cancel_ImportBtn = new Button();
            SofarPackage_ImportBtn = new Button();
            SofarPackagePath_TextBox = new TextBox();
            ToolTip = new ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            DeviceInfo_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)InvertersInfo_DataGridView).BeginInit();
            Log_GroupBox.SuspendLayout();
            DevConf_GroupBox.SuspendLayout();
            UpgradeControl_GroupBox.SuspendLayout();
            UpgradeSettings_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PackageSize_NumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ResendLostsRetries_NumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SendingPackageInterval_NumericUpDown).BeginInit();
            Firmware_GroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(DeviceInfo_GroupBox, 0, 3);
            tableLayoutPanel1.Controls.Add(Log_GroupBox, 1, 0);
            tableLayoutPanel1.Controls.Add(DevConf_GroupBox, 0, 0);
            tableLayoutPanel1.Controls.Add(UpgradeControl_GroupBox, 0, 2);
            tableLayoutPanel1.Controls.Add(Firmware_GroupBox, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(1480, 860);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // DeviceInfo_GroupBox
            // 
            DeviceInfo_GroupBox.Controls.Add(InvertersInfo_DataGridView);
            DeviceInfo_GroupBox.Dock = DockStyle.Fill;
            DeviceInfo_GroupBox.Location = new Point(0, 400);
            DeviceInfo_GroupBox.Margin = new Padding(0);
            DeviceInfo_GroupBox.Name = "DeviceInfo_GroupBox";
            DeviceInfo_GroupBox.Padding = new Padding(3, 4, 3, 4);
            DeviceInfo_GroupBox.Size = new Size(1110, 460);
            DeviceInfo_GroupBox.TabIndex = 51;
            DeviceInfo_GroupBox.TabStop = false;
            DeviceInfo_GroupBox.Text = "逆变器信息";
            // 
            // InvertersInfo_DataGridView
            // 
            InvertersInfo_DataGridView.AllowUserToAddRows = false;
            InvertersInfo_DataGridView.AllowUserToDeleteRows = false;
            InvertersInfo_DataGridView.AllowUserToResizeColumns = false;
            InvertersInfo_DataGridView.AllowUserToResizeRows = false;
            InvertersInfo_DataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            InvertersInfo_DataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            InvertersInfo_DataGridView.BackgroundColor = SystemColors.Control;
            InvertersInfo_DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            InvertersInfo_DataGridView.Columns.AddRange(new DataGridViewColumn[] { Slave, SerialNo, ARM, DSPM, DSPS, PLCSTA, Status, RefreshTime });
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new Padding(3);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            InvertersInfo_DataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            InvertersInfo_DataGridView.Dock = DockStyle.Fill;
            InvertersInfo_DataGridView.GridColor = SystemColors.Control;
            InvertersInfo_DataGridView.Location = new Point(3, 24);
            InvertersInfo_DataGridView.Margin = new Padding(3, 4, 3, 4);
            InvertersInfo_DataGridView.MultiSelect = false;
            InvertersInfo_DataGridView.Name = "InvertersInfo_DataGridView";
            InvertersInfo_DataGridView.ReadOnly = true;
            InvertersInfo_DataGridView.RowHeadersVisible = false;
            InvertersInfo_DataGridView.RowHeadersWidth = 51;
            InvertersInfo_DataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            InvertersInfo_DataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            InvertersInfo_DataGridView.RowTemplate.Height = 29;
            InvertersInfo_DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            InvertersInfo_DataGridView.ShowCellErrors = false;
            InvertersInfo_DataGridView.ShowRowErrors = false;
            InvertersInfo_DataGridView.Size = new Size(1104, 432);
            InvertersInfo_DataGridView.TabIndex = 38;
            // 
            // Slave
            // 
            Slave.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            Slave.DataPropertyName = "SlaveNo";
            Slave.FillWeight = 60F;
            Slave.HeaderText = "设备号";
            Slave.MinimumWidth = 60;
            Slave.Name = "Slave";
            Slave.ReadOnly = true;
            Slave.SortMode = DataGridViewColumnSortMode.NotSortable;
            Slave.Width = 66;
            // 
            // SerialNo
            // 
            SerialNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SerialNo.DataPropertyName = "SerialNo";
            SerialNo.FillWeight = 50F;
            SerialNo.HeaderText = "序列号";
            SerialNo.MinimumWidth = 150;
            SerialNo.Name = "SerialNo";
            SerialNo.ReadOnly = true;
            SerialNo.SortMode = DataGridViewColumnSortMode.NotSortable;
            SerialNo.Width = 150;
            // 
            // ARM
            // 
            ARM.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ARM.DataPropertyName = "ARM_Version";
            ARM.HeaderText = "ARM";
            ARM.MinimumWidth = 80;
            ARM.Name = "ARM";
            ARM.ReadOnly = true;
            ARM.SortMode = DataGridViewColumnSortMode.NotSortable;
            ARM.Width = 80;
            // 
            // DSPM
            // 
            DSPM.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DSPM.DataPropertyName = "DSPM_Version";
            DSPM.FillWeight = 50F;
            DSPM.HeaderText = "DSPM";
            DSPM.MinimumWidth = 90;
            DSPM.Name = "DSPM";
            DSPM.ReadOnly = true;
            DSPM.SortMode = DataGridViewColumnSortMode.NotSortable;
            DSPM.Width = 90;
            // 
            // DSPS
            // 
            DSPS.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DSPS.DataPropertyName = "DSPS_Version";
            DSPS.HeaderText = "DSPS";
            DSPS.MinimumWidth = 76;
            DSPS.Name = "DSPS";
            DSPS.ReadOnly = true;
            DSPS.SortMode = DataGridViewColumnSortMode.NotSortable;
            DSPS.Width = 76;
            // 
            // PLCSTA
            // 
            PLCSTA.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PLCSTA.DataPropertyName = "PLCSTA_Version";
            PLCSTA.HeaderText = "PLC-STA";
            PLCSTA.MinimumWidth = 70;
            PLCSTA.Name = "PLCSTA";
            PLCSTA.ReadOnly = true;
            PLCSTA.SortMode = DataGridViewColumnSortMode.NotSortable;
            PLCSTA.Width = 83;
            // 
            // Status
            // 
            Status.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Status.DataPropertyName = "StatusMsg";
            Status.FillWeight = 130F;
            Status.HeaderText = "状态信息";
            Status.MinimumWidth = 160;
            Status.Name = "Status";
            Status.ReadOnly = true;
            Status.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // RefreshTime
            // 
            RefreshTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            RefreshTime.DataPropertyName = "RefreshTime";
            RefreshTime.HeaderText = "刷新时间";
            RefreshTime.MinimumWidth = 100;
            RefreshTime.Name = "RefreshTime";
            RefreshTime.ReadOnly = true;
            RefreshTime.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Log_GroupBox
            // 
            Log_GroupBox.Controls.Add(UpgradeLog_RichTextBox);
            Log_GroupBox.Dock = DockStyle.Fill;
            Log_GroupBox.Location = new Point(1113, 4);
            Log_GroupBox.Margin = new Padding(3, 4, 3, 4);
            Log_GroupBox.Name = "Log_GroupBox";
            Log_GroupBox.Padding = new Padding(3, 4, 3, 4);
            tableLayoutPanel1.SetRowSpan(Log_GroupBox, 4);
            Log_GroupBox.Size = new Size(364, 852);
            Log_GroupBox.TabIndex = 50;
            Log_GroupBox.TabStop = false;
            Log_GroupBox.Text = "升级信息";
            // 
            // UpgradeLog_RichTextBox
            // 
            UpgradeLog_RichTextBox.Dock = DockStyle.Fill;
            UpgradeLog_RichTextBox.Location = new Point(3, 24);
            UpgradeLog_RichTextBox.Margin = new Padding(4, 5, 4, 5);
            UpgradeLog_RichTextBox.Name = "UpgradeLog_RichTextBox";
            UpgradeLog_RichTextBox.Size = new Size(358, 824);
            UpgradeLog_RichTextBox.TabIndex = 29;
            UpgradeLog_RichTextBox.Text = "";
            // 
            // DevConf_GroupBox
            // 
            DevConf_GroupBox.Controls.Add(ReadInfo_Msg_Label);
            DevConf_GroupBox.Controls.Add(ReadDevInfo_Button);
            DevConf_GroupBox.Controls.Add(DevAddrRange_TextBox);
            DevConf_GroupBox.Controls.Add(SubdevAddr_Label);
            DevConf_GroupBox.Dock = DockStyle.Fill;
            DevConf_GroupBox.Location = new Point(0, 0);
            DevConf_GroupBox.Margin = new Padding(0);
            DevConf_GroupBox.Name = "DevConf_GroupBox";
            DevConf_GroupBox.Padding = new Padding(5);
            DevConf_GroupBox.Size = new Size(1110, 100);
            DevConf_GroupBox.TabIndex = 46;
            DevConf_GroupBox.TabStop = false;
            DevConf_GroupBox.Text = "逆变器（适用于320kW等机型）";
            // 
            // ReadInfo_Msg_Label
            // 
            ReadInfo_Msg_Label.AutoSize = true;
            ReadInfo_Msg_Label.Location = new Point(533, 45);
            ReadInfo_Msg_Label.Name = "ReadInfo_Msg_Label";
            ReadInfo_Msg_Label.Size = new Size(0, 20);
            ReadInfo_Msg_Label.TabIndex = 17;
            // 
            // ReadDevInfo_Button
            // 
            ReadDevInfo_Button.Location = new Point(370, 42);
            ReadDevInfo_Button.Margin = new Padding(4);
            ReadDevInfo_Button.Name = "ReadDevInfo_Button";
            ReadDevInfo_Button.Size = new Size(141, 27);
            ReadDevInfo_Button.TabIndex = 16;
            ReadDevInfo_Button.Text = "读取逆变器信息";
            ReadDevInfo_Button.UseVisualStyleBackColor = true;
            ReadDevInfo_Button.Click += ReadDevInfo_Button_Click;
            // 
            // DevAddrRange_TextBox
            // 
            DevAddrRange_TextBox.Location = new Point(147, 42);
            DevAddrRange_TextBox.Margin = new Padding(3, 4, 3, 4);
            DevAddrRange_TextBox.Name = "DevAddrRange_TextBox";
            DevAddrRange_TextBox.Size = new Size(180, 27);
            DevAddrRange_TextBox.TabIndex = 14;
            // 
            // SubdevAddr_Label
            // 
            SubdevAddr_Label.AutoSize = true;
            SubdevAddr_Label.Location = new Point(48, 47);
            SubdevAddr_Label.Name = "SubdevAddr_Label";
            SubdevAddr_Label.Size = new Size(84, 20);
            SubdevAddr_Label.TabIndex = 15;
            SubdevAddr_Label.Text = "设备号范围";
            // 
            // UpgradeControl_GroupBox
            // 
            UpgradeControl_GroupBox.Controls.Add(UpgradeSettings_Panel);
            UpgradeControl_GroupBox.Controls.Add(StopUpgrade_Button);
            UpgradeControl_GroupBox.Controls.Add(StartUpgrade_Button);
            UpgradeControl_GroupBox.Dock = DockStyle.Fill;
            UpgradeControl_GroupBox.Location = new Point(3, 253);
            UpgradeControl_GroupBox.Name = "UpgradeControl_GroupBox";
            UpgradeControl_GroupBox.Size = new Size(1104, 144);
            UpgradeControl_GroupBox.TabIndex = 49;
            UpgradeControl_GroupBox.TabStop = false;
            UpgradeControl_GroupBox.Text = "升级控制";
            // 
            // UpgradeSettings_Panel
            // 
            UpgradeSettings_Panel.Controls.Add(UpgradeTime_CheckBox);
            UpgradeSettings_Panel.Controls.Add(PackageSize_NumericUpDown);
            UpgradeSettings_Panel.Controls.Add(label1);
            UpgradeSettings_Panel.Controls.Add(label6);
            UpgradeSettings_Panel.Controls.Add(ResendLostsRetries_NumericUpDown);
            UpgradeSettings_Panel.Controls.Add(Upgrade_DateTimePicker);
            UpgradeSettings_Panel.Controls.Add(label7);
            UpgradeSettings_Panel.Controls.Add(SendingPackageInterval_NumericUpDown);
            UpgradeSettings_Panel.Location = new Point(30, 26);
            UpgradeSettings_Panel.Name = "UpgradeSettings_Panel";
            UpgradeSettings_Panel.Size = new Size(759, 112);
            UpgradeSettings_Panel.TabIndex = 36;
            // 
            // UpgradeTime_CheckBox
            // 
            UpgradeTime_CheckBox.AutoSize = true;
            UpgradeTime_CheckBox.Enabled = false;
            UpgradeTime_CheckBox.Location = new Point(27, 11);
            UpgradeTime_CheckBox.Margin = new Padding(4);
            UpgradeTime_CheckBox.Name = "UpgradeTime_CheckBox";
            UpgradeTime_CheckBox.Size = new Size(91, 24);
            UpgradeTime_CheckBox.TabIndex = 26;
            UpgradeTime_CheckBox.Text = "定时升级";
            UpgradeTime_CheckBox.UseVisualStyleBackColor = true;
            // 
            // PackageSize_NumericUpDown
            // 
            PackageSize_NumericUpDown.Location = new Point(395, 73);
            PackageSize_NumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            PackageSize_NumericUpDown.Name = "PackageSize_NumericUpDown";
            PackageSize_NumericUpDown.Size = new Size(86, 27);
            PackageSize_NumericUpDown.TabIndex = 35;
            PackageSize_NumericUpDown.Value = new decimal(new int[] { 220, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 75);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 30;
            label1.Text = "发包间隔(ms): ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(271, 75);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(117, 20);
            label6.TabIndex = 31;
            label6.Text = "单包大小(字节): ";
            // 
            // ResendLostsRetries_NumericUpDown
            // 
            ResendLostsRetries_NumericUpDown.Location = new Point(633, 73);
            ResendLostsRetries_NumericUpDown.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            ResendLostsRetries_NumericUpDown.Name = "ResendLostsRetries_NumericUpDown";
            ResendLostsRetries_NumericUpDown.Size = new Size(72, 27);
            ResendLostsRetries_NumericUpDown.TabIndex = 29;
            ResendLostsRetries_NumericUpDown.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // Upgrade_DateTimePicker
            // 
            Upgrade_DateTimePicker.CustomFormat = "yy-MM-dd HH:mm:ss";
            Upgrade_DateTimePicker.Enabled = false;
            Upgrade_DateTimePicker.Format = DateTimePickerFormat.Custom;
            Upgrade_DateTimePicker.Location = new Point(134, 9);
            Upgrade_DateTimePicker.Margin = new Padding(4);
            Upgrade_DateTimePicker.Name = "Upgrade_DateTimePicker";
            Upgrade_DateTimePicker.Size = new Size(212, 27);
            Upgrade_DateTimePicker.TabIndex = 25;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(519, 75);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(107, 20);
            label7.TabIndex = 28;
            label7.Text = "补包上限次数: ";
            // 
            // SendingPackageInterval_NumericUpDown
            // 
            SendingPackageInterval_NumericUpDown.Location = new Point(142, 73);
            SendingPackageInterval_NumericUpDown.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            SendingPackageInterval_NumericUpDown.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            SendingPackageInterval_NumericUpDown.Name = "SendingPackageInterval_NumericUpDown";
            SendingPackageInterval_NumericUpDown.Size = new Size(86, 27);
            SendingPackageInterval_NumericUpDown.TabIndex = 33;
            SendingPackageInterval_NumericUpDown.Value = new decimal(new int[] { 200, 0, 0, 0 });
            // 
            // StopUpgrade_Button
            // 
            StopUpgrade_Button.Location = new Point(973, 94);
            StopUpgrade_Button.Margin = new Padding(4);
            StopUpgrade_Button.Name = "StopUpgrade_Button";
            StopUpgrade_Button.Size = new Size(94, 35);
            StopUpgrade_Button.TabIndex = 27;
            StopUpgrade_Button.Text = "停止升级";
            StopUpgrade_Button.UseVisualStyleBackColor = true;
            StopUpgrade_Button.Click += StopUpgrade_Button_Click;
            // 
            // StartUpgrade_Button
            // 
            StartUpgrade_Button.Location = new Point(854, 94);
            StartUpgrade_Button.Margin = new Padding(4);
            StartUpgrade_Button.Name = "StartUpgrade_Button";
            StartUpgrade_Button.Size = new Size(94, 35);
            StartUpgrade_Button.TabIndex = 11;
            StartUpgrade_Button.Text = "启动升级";
            StartUpgrade_Button.UseVisualStyleBackColor = true;
            StartUpgrade_Button.Click += StartUpgrade_Button_Click;
            // 
            // Firmware_GroupBox
            // 
            Firmware_GroupBox.Controls.Add(SofarVersions_Label);
            Firmware_GroupBox.Controls.Add(SofarPackage_Label);
            Firmware_GroupBox.Controls.Add(SofarPackage_Cancel_ImportBtn);
            Firmware_GroupBox.Controls.Add(SofarPackage_ImportBtn);
            Firmware_GroupBox.Controls.Add(SofarPackagePath_TextBox);
            Firmware_GroupBox.Dock = DockStyle.Fill;
            Firmware_GroupBox.Location = new Point(3, 103);
            Firmware_GroupBox.Name = "Firmware_GroupBox";
            Firmware_GroupBox.Size = new Size(1104, 144);
            Firmware_GroupBox.TabIndex = 47;
            Firmware_GroupBox.TabStop = false;
            Firmware_GroupBox.Text = "固件包";
            // 
            // SofarVersions_Label
            // 
            SofarVersions_Label.AutoSize = true;
            SofarVersions_Label.Location = new Point(110, 78);
            SofarVersions_Label.Name = "SofarVersions_Label";
            SofarVersions_Label.Size = new Size(0, 20);
            SofarVersions_Label.TabIndex = 78;
            // 
            // SofarPackage_Label
            // 
            SofarPackage_Label.AutoSize = true;
            SofarPackage_Label.Location = new Point(30, 41);
            SofarPackage_Label.Name = "SofarPackage_Label";
            SofarPackage_Label.Size = new Size(92, 20);
            SofarPackage_Label.TabIndex = 77;
            SofarPackage_Label.Text = "Sofar包导入";
            // 
            // SofarPackage_Cancel_ImportBtn
            // 
            SofarPackage_Cancel_ImportBtn.Location = new Point(854, 34);
            SofarPackage_Cancel_ImportBtn.Margin = new Padding(4);
            SofarPackage_Cancel_ImportBtn.Name = "SofarPackage_Cancel_ImportBtn";
            SofarPackage_Cancel_ImportBtn.Size = new Size(94, 29);
            SofarPackage_Cancel_ImportBtn.TabIndex = 73;
            SofarPackage_Cancel_ImportBtn.Text = "Cancel";
            SofarPackage_Cancel_ImportBtn.UseVisualStyleBackColor = true;
            SofarPackage_Cancel_ImportBtn.Click += SofarPackage_Cancel_ImportBtn_Click;
            // 
            // SofarPackage_ImportBtn
            // 
            SofarPackage_ImportBtn.Location = new Point(766, 34);
            SofarPackage_ImportBtn.Margin = new Padding(4);
            SofarPackage_ImportBtn.Name = "SofarPackage_ImportBtn";
            SofarPackage_ImportBtn.Size = new Size(80, 30);
            SofarPackage_ImportBtn.TabIndex = 48;
            SofarPackage_ImportBtn.Text = "...";
            SofarPackage_ImportBtn.UseVisualStyleBackColor = true;
            SofarPackage_ImportBtn.Click += SofarPackage_ImportBtn_Click;
            // 
            // SofarPackagePath_TextBox
            // 
            SofarPackagePath_TextBox.Location = new Point(134, 36);
            SofarPackagePath_TextBox.Margin = new Padding(8);
            SofarPackagePath_TextBox.Name = "SofarPackagePath_TextBox";
            SofarPackagePath_TextBox.ReadOnly = true;
            SofarPackagePath_TextBox.Size = new Size(620, 27);
            SofarPackagePath_TextBox.TabIndex = 47;
            // 
            // ToolTip
            // 
            ToolTip.AutoPopDelay = 10000;
            ToolTip.InitialDelay = 50;
            ToolTip.ReshowDelay = 100;
            // 
            // UC_G4Upgrade
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "UC_G4Upgrade";
            Size = new Size(1480, 860);
            tableLayoutPanel1.ResumeLayout(false);
            DeviceInfo_GroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)InvertersInfo_DataGridView).EndInit();
            Log_GroupBox.ResumeLayout(false);
            DevConf_GroupBox.ResumeLayout(false);
            DevConf_GroupBox.PerformLayout();
            UpgradeControl_GroupBox.ResumeLayout(false);
            UpgradeSettings_Panel.ResumeLayout(false);
            UpgradeSettings_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PackageSize_NumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)ResendLostsRetries_NumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SendingPackageInterval_NumericUpDown).EndInit();
            Firmware_GroupBox.ResumeLayout(false);
            Firmware_GroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox DevConf_GroupBox;
        private Label ReadInfo_Msg_Label;
        private Button ReadDevInfo_Button;
        private TextBox DevAddrRange_TextBox;
        private Label SubdevAddr_Label;
        private GroupBox Firmware_GroupBox;
        private Label SofarPackage_Label;
        private Button SofarPackage_Cancel_ImportBtn;
        private Button SofarPackage_ImportBtn;
        private TextBox SofarPackagePath_TextBox;
        private GroupBox UpgradeControl_GroupBox;
        private NumericUpDown ResendLostsRetries_NumericUpDown;
        private Label label7;
        private Button StopUpgrade_Button;
        private CheckBox UpgradeTime_CheckBox;
        private Button StartUpgrade_Button;
        private DateTimePicker Upgrade_DateTimePicker;
        private Label SofarVersions_Label;
        private Label label1;
        private NumericUpDown PackageSize_NumericUpDown;
        private NumericUpDown SendingPackageInterval_NumericUpDown;
        private Label label6;
        private Panel Settings_Panel;
        private ToolTip ToolTip;
        private GroupBox Log_GroupBox;
        private RichTextBox UpgradeLog_RichTextBox;
        private GroupBox DeviceInfo_GroupBox;
        private DataGridView InvertersInfo_DataGridView;
        private Panel UpgradeSettings_Panel;
        private DataGridViewTextBoxColumn Slave;
        private DataGridViewTextBoxColumn SerialNo;
        private DataGridViewTextBoxColumn ARM;
        private DataGridViewTextBoxColumn DSPM;
        private DataGridViewTextBoxColumn DSPS;
        private DataGridViewTextBoxColumn PLCSTA;
        private DataGridViewTextBoxColumn Status;
        private DataGridViewTextBoxColumn RefreshTime;
    }
}
