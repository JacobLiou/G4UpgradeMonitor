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
            btnConnect = new Button();
            rtbPrintinfo = new RichTextBox();
            uiButton_OK = new Sunny.UI.UIButton();
            uiButton_Cancel = new Sunny.UI.UIButton();
            nudNumber = new Sunny.UI.UIDoubleUpDown();
            btnDisConnect = new Button();
            SuspendLayout();
            // 
            // txtIpPort
            // 
            txtIpPort.Location = new Point(244, 61);
            txtIpPort.Margin = new Padding(5, 4, 5, 4);
            txtIpPort.Name = "txtIpPort";
            txtIpPort.Size = new Size(75, 25);
            txtIpPort.TabIndex = 11;
            txtIpPort.Text = "502";
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(97, 61);
            txtIpAddress.Margin = new Padding(5, 4, 5, 4);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(139, 25);
            txtIpAddress.TabIndex = 10;
            txtIpAddress.Text = "127.0.0.1";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ImeMode = ImeMode.NoControl;
            label6.Location = new Point(37, 66);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(53, 15);
            label6.TabIndex = 8;
            label6.Text = "IP地址";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(355, 65);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 12;
            label1.Text = "连续数";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(582, 57);
            btnConnect.Margin = new Padding(5);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(90, 27);
            btnConnect.TabIndex = 16;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // rtbPrintinfo
            // 
            rtbPrintinfo.Location = new Point(29, 109);
            rtbPrintinfo.Margin = new Padding(4);
            rtbPrintinfo.Name = "rtbPrintinfo";
            rtbPrintinfo.Size = new Size(743, 268);
            rtbPrintinfo.TabIndex = 17;
            rtbPrintinfo.Text = "";
            // 
            // uiButton_OK
            // 
            uiButton_OK.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_OK.Location = new Point(162, 411);
            uiButton_OK.MinimumSize = new Size(1, 1);
            uiButton_OK.Name = "uiButton_OK";
            uiButton_OK.Size = new Size(125, 34);
            uiButton_OK.TabIndex = 18;
            uiButton_OK.Text = "确认";
            uiButton_OK.Click += uiButton_OK_Click;
            // 
            // uiButton_Cancel
            // 
            uiButton_Cancel.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_Cancel.Location = new Point(502, 411);
            uiButton_Cancel.MinimumSize = new Size(1, 1);
            uiButton_Cancel.Name = "uiButton_Cancel";
            uiButton_Cancel.Size = new Size(125, 34);
            uiButton_Cancel.TabIndex = 18;
            uiButton_Cancel.Text = "取消";
            uiButton_Cancel.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton_Cancel.Click += uiButton_Cancel_Click;
            // 
            // nudNumber
            // 
            nudNumber.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            nudNumber.Location = new Point(410, 61);
            nudNumber.Margin = new Padding(4, 5, 4, 5);
            nudNumber.MinimumSize = new Size(100, 0);
            nudNumber.Name = "nudNumber";
            nudNumber.ShowText = false;
            nudNumber.Size = new Size(100, 24);
            nudNumber.Step = 1D;
            nudNumber.TabIndex = 19;
            nudNumber.Text = "uiDoubleUpDown1";
            nudNumber.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // btnDisConnect
            // 
            btnDisConnect.Location = new Point(682, 57);
            btnDisConnect.Margin = new Padding(5);
            btnDisConnect.Name = "btnDisConnect";
            btnDisConnect.Size = new Size(90, 27);
            btnDisConnect.TabIndex = 16;
            btnDisConnect.Text = "断开";
            btnDisConnect.UseVisualStyleBackColor = true;
            btnDisConnect.Click += btnDisConnect_Click;
            // 
            // FrmConfig
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(831, 479);
            Controls.Add(nudNumber);
            Controls.Add(uiButton_Cancel);
            Controls.Add(uiButton_OK);
            Controls.Add(rtbPrintinfo);
            Controls.Add(btnDisConnect);
            Controls.Add(btnConnect);
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
            TitleFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ZoomScaleRect = new Rectangle(19, 19, 888, 499);
            Load += FrmConfig_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtIpPort;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect;
        private RichTextBox rtbPrintinfo;
        private Sunny.UI.UIButton uiButton_OK;
        private Sunny.UI.UIButton uiButton_Cancel;
        private Sunny.UI.UIDoubleUpDown nudNumber;
        private Button btnDisConnect;
    }
}