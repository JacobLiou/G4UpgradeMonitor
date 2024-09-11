namespace Sofar.G4MultiUpgrade
{
    partial class FrmConfig
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
            txtIpPort = new TextBox();
            txtIpAddress = new TextBox();
            label6 = new Label();
            label1 = new Label();
            nudNumber = new NumericUpDown();
            btnConnect = new Button();
            rtbPrintinfo = new RichTextBox();
            uiButton_OK = new Sunny.UI.UIButton();
            uiButton_Cancel = new Sunny.UI.UIButton();
            ((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
            SuspendLayout();
            // 
            // txtIpPort
            // 
            txtIpPort.Location = new Point(350, 52);
            txtIpPort.Margin = new Padding(5, 4, 5, 4);
            txtIpPort.Name = "txtIpPort";
            txtIpPort.Size = new Size(95, 25);
            txtIpPort.TabIndex = 11;
            txtIpPort.Text = "502";
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(160, 52);
            txtIpAddress.Margin = new Padding(5, 4, 5, 4);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(179, 25);
            txtIpAddress.TabIndex = 10;
            txtIpAddress.Text = "127.0.0.1";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ImeMode = ImeMode.NoControl;
            label6.Location = new Point(70, 58);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(53, 15);
            label6.TabIndex = 8;
            label6.Text = "IP地址";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(70, 94);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 12;
            label1.Text = "连续个数";
            // 
            // nudNumber
            // 
            nudNumber.Location = new Point(160, 88);
            nudNumber.Margin = new Padding(5);
            nudNumber.Name = "nudNumber";
            nudNumber.Size = new Size(180, 25);
            nudNumber.TabIndex = 14;
            nudNumber.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(463, 52);
            btnConnect.Margin = new Padding(5);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(96, 32);
            btnConnect.TabIndex = 16;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // rtbPrintinfo
            // 
            rtbPrintinfo.Location = new Point(48, 134);
            rtbPrintinfo.Margin = new Padding(4);
            rtbPrintinfo.Name = "rtbPrintinfo";
            rtbPrintinfo.Size = new Size(888, 355);
            rtbPrintinfo.TabIndex = 17;
            rtbPrintinfo.Text = "";
            // 
            // uiButton_OK
            // 
            uiButton_OK.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_OK.Location = new Point(277, 516);
            uiButton_OK.MinimumSize = new Size(1, 1);
            uiButton_OK.Name = "uiButton_OK";
            uiButton_OK.Size = new Size(125, 34);
            uiButton_OK.TabIndex = 18;
            uiButton_OK.Text = "确认";
            uiButton_OK.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_OK.Click += uiButton_OK_Click;
            // 
            // uiButton_Cancel
            // 
            uiButton_Cancel.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_Cancel.Location = new Point(488, 516);
            uiButton_Cancel.MinimumSize = new Size(1, 1);
            uiButton_Cancel.Name = "uiButton_Cancel";
            uiButton_Cancel.Size = new Size(125, 34);
            uiButton_Cancel.TabIndex = 18;
            uiButton_Cancel.Text = "取消";
            uiButton_Cancel.Click += uiButton_Cancel_Click;
            // 
            // FrmConfig
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(975, 562);
            Controls.Add(uiButton_Cancel);
            Controls.Add(uiButton_OK);
            Controls.Add(rtbPrintinfo);
            Controls.Add(btnConnect);
            Controls.Add(nudNumber);
            Controls.Add(label1);
            Controls.Add(txtIpPort);
            Controls.Add(txtIpAddress);
            Controls.Add(label6);
            Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmConfig";
            Text = "通讯配置";
            ZoomScaleRect = new Rectangle(19, 19, 888, 499);
            ((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtIpPort;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudNumber;
        private System.Windows.Forms.Button btnConnect;
        private RichTextBox rtbPrintinfo;
        private Sunny.UI.UIButton uiButton_OK;
        private Sunny.UI.UIButton uiButton_Cancel;
    }
}