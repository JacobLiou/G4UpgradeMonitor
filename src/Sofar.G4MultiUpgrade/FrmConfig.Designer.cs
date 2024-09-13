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
            this.txtIpPort = new System.Windows.Forms.TextBox();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.rtbPrintinfo = new System.Windows.Forms.RichTextBox();
            this.uiButton_OK = new Sunny.UI.UIButton();
            this.uiButton_Cancel = new Sunny.UI.UIButton();
            this.nudNumber = new Sunny.UI.UIDoubleUpDown();
            this.SuspendLayout();
            // 
            // txtIpPort
            // 
            this.txtIpPort.Location = new System.Drawing.Point(282, 60);
            this.txtIpPort.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtIpPort.Name = "txtIpPort";
            this.txtIpPort.Size = new System.Drawing.Size(75, 21);
            this.txtIpPort.TabIndex = 11;
            this.txtIpPort.Text = "502";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(133, 60);
            this.txtIpAddress.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(139, 21);
            this.txtIpAddress.TabIndex = 10;
            this.txtIpAddress.Text = "127.0.0.1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(81, 64);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "IP地址";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(394, 64);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "连续数";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(582, 57);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 27);
            this.btnConnect.TabIndex = 16;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // rtbPrintinfo
            // 
            this.rtbPrintinfo.Location = new System.Drawing.Point(29, 109);
            this.rtbPrintinfo.Margin = new System.Windows.Forms.Padding(4);
            this.rtbPrintinfo.Name = "rtbPrintinfo";
            this.rtbPrintinfo.Size = new System.Drawing.Size(703, 268);
            this.rtbPrintinfo.TabIndex = 17;
            this.rtbPrintinfo.Text = "";
            // 
            // uiButton_OK
            // 
            this.uiButton_OK.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.uiButton_OK.Location = new System.Drawing.Point(162, 411);
            this.uiButton_OK.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_OK.Name = "uiButton_OK";
            this.uiButton_OK.Size = new System.Drawing.Size(125, 34);
            this.uiButton_OK.TabIndex = 18;
            this.uiButton_OK.Text = "确认";
            this.uiButton_OK.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.uiButton_OK.Click += new System.EventHandler(this.uiButton_OK_Click);
            // 
            // uiButton_Cancel
            // 
            this.uiButton_Cancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.uiButton_Cancel.Location = new System.Drawing.Point(475, 411);
            this.uiButton_Cancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_Cancel.Name = "uiButton_Cancel";
            this.uiButton_Cancel.Size = new System.Drawing.Size(125, 34);
            this.uiButton_Cancel.TabIndex = 18;
            this.uiButton_Cancel.Text = "取消";
            this.uiButton_Cancel.Click += new System.EventHandler(this.uiButton_Cancel_Click);
            // 
            // nudNumber
            // 
            this.nudNumber.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudNumber.Location = new System.Drawing.Point(444, 60);
            this.nudNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudNumber.MinimumSize = new System.Drawing.Size(100, 0);
            this.nudNumber.Name = "nudNumber";
            this.nudNumber.ShowText = false;
            this.nudNumber.Size = new System.Drawing.Size(100, 24);
            this.nudNumber.Step = 1D;
            this.nudNumber.TabIndex = 19;
            this.nudNumber.Text = "uiDoubleUpDown1";
            this.nudNumber.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(796, 479);
            this.Controls.Add(this.nudNumber);
            this.Controls.Add(this.uiButton_Cancel);
            this.Controls.Add(this.uiButton_OK);
            this.Controls.Add(this.rtbPrintinfo);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIpPort);
            this.Controls.Add(this.txtIpAddress);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfig";
            this.Text = "通讯配置";
            this.TitleFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 888, 499);
            this.Load += new System.EventHandler(this.FrmConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}