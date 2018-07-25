namespace ESForm
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelDebug = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tbSendMsg = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioSerial = new System.Windows.Forms.RadioButton();
            this.radioTCP = new System.Windows.Forms.RadioButton();
            this.timer_MessageClear = new System.Windows.Forms.Timer(this.components);
            this.timer_reconnect_try = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(12, 95);
            this.labelDebug.Name = "labelDebug";
            this.labelDebug.Size = new System.Drawing.Size(65, 12);
            this.labelDebug.TabIndex = 0;
            this.labelDebug.Text = "상태메시지";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(14, 110);
            this.tbLog.MaxLength = 2147483647;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(553, 430);
            this.tbLog.TabIndex = 1;
            // 
            // tbSendMsg
            // 
            this.tbSendMsg.Location = new System.Drawing.Point(6, 42);
            this.tbSendMsg.Name = "tbSendMsg";
            this.tbSendMsg.Size = new System.Drawing.Size(462, 21);
            this.tbSendMsg.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(477, 42);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "보내기";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSerial);
            this.groupBox1.Controls.Add(this.radioTCP);
            this.groupBox1.Controls.Add(this.tbSendMsg);
            this.groupBox1.Controls.Add(this.btnSend);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 81);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "보낼메시지";
            // 
            // radioSerial
            // 
            this.radioSerial.AutoSize = true;
            this.radioSerial.Location = new System.Drawing.Point(60, 20);
            this.radioSerial.Name = "radioSerial";
            this.radioSerial.Size = new System.Drawing.Size(55, 16);
            this.radioSerial.TabIndex = 6;
            this.radioSerial.TabStop = true;
            this.radioSerial.Text = "Serial";
            this.radioSerial.UseVisualStyleBackColor = true;
            // 
            // radioTCP
            // 
            this.radioTCP.AutoSize = true;
            this.radioTCP.Location = new System.Drawing.Point(6, 20);
            this.radioTCP.Name = "radioTCP";
            this.radioTCP.Size = new System.Drawing.Size(48, 16);
            this.radioTCP.TabIndex = 5;
            this.radioTCP.TabStop = true;
            this.radioTCP.Text = "TCP";
            this.radioTCP.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 559);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.labelDebug);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDebug;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.TextBox tbSendMsg;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioSerial;
        private System.Windows.Forms.RadioButton radioTCP;
        private System.Windows.Forms.Timer timer_MessageClear;
        private System.Windows.Forms.Timer timer_reconnect_try;
    }
}

