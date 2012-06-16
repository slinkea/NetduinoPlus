namespace Netduino.DesktopMessenger
{
    partial class NetduinoCommunicationCenter
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
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxSendMessage = new System.Windows.Forms.TextBox();
            this.textBoxReceiveMessage = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelCommunicationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(12, 38);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxSendMessage
            // 
            this.textBoxSendMessage.Location = new System.Drawing.Point(12, 12);
            this.textBoxSendMessage.Name = "textBoxSendMessage";
            this.textBoxSendMessage.Size = new System.Drawing.Size(431, 20);
            this.textBoxSendMessage.TabIndex = 0;
            this.textBoxSendMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSendMessage_KeyPress);
            // 
            // textBoxReceiveMessage
            // 
            this.textBoxReceiveMessage.Location = new System.Drawing.Point(12, 77);
            this.textBoxReceiveMessage.Multiline = true;
            this.textBoxReceiveMessage.Name = "textBoxReceiveMessage";
            this.textBoxReceiveMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxReceiveMessage.Size = new System.Drawing.Size(431, 282);
            this.textBoxReceiveMessage.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelCommunicationStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 382);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(455, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelCommunicationStatus
            // 
            this.toolStripStatusLabelCommunicationStatus.Name = "toolStripStatusLabelCommunicationStatus";
            this.toolStripStatusLabelCommunicationStatus.Size = new System.Drawing.Size(206, 17);
            this.toolStripStatusLabelCommunicationStatus.Text = "toolStripStatusLabelCommunicationStatus";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(93, 38);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(368, 38);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(75, 23);
            this.buttonDown.TabIndex = 7;
            this.buttonDown.Text = "Down";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(287, 38);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 8;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Location = new System.Drawing.Point(206, 38);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(75, 23);
            this.buttonUp.TabIndex = 9;
            this.buttonUp.Text = "Up";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // NetduinoCommunicationCenter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 404);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxReceiveMessage);
            this.Controls.Add(this.textBoxSendMessage);
            this.Controls.Add(this.buttonSend);
            this.Name = "NetduinoCommunicationCenter";
            this.Text = "Talk To Netduino";
            this.Load += new System.EventHandler(this.TalkToNetduino_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSendMessage;
        private System.Windows.Forms.TextBox textBoxReceiveMessage;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCommunicationStatus;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonUp;
    }
}

