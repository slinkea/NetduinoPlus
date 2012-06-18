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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.buttonSend = new System.Windows.Forms.Button();
      this.textBoxSendMessage = new System.Windows.Forms.TextBox();
      this.textBoxReceiveMessage = new System.Windows.Forms.TextBox();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabelCommunicationStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.buttonClear = new System.Windows.Forms.Button();
      this.buttonDown = new System.Windows.Forms.Button();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonUp = new System.Windows.Forms.Button();
      this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
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
      this.textBoxReceiveMessage.Location = new System.Drawing.Point(464, 12);
      this.textBoxReceiveMessage.Multiline = true;
      this.textBoxReceiveMessage.Name = "textBoxReceiveMessage";
      this.textBoxReceiveMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.textBoxReceiveMessage.Size = new System.Drawing.Size(428, 87);
      this.textBoxReceiveMessage.TabIndex = 2;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelCommunicationStatus});
      this.statusStrip1.Location = new System.Drawing.Point(0, 523);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(904, 22);
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
      // chart1
      // 
      this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
      this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
      this.chart1.BackSecondaryColor = System.Drawing.Color.White;
      this.chart1.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
      this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      this.chart1.BorderlineWidth = 2;
      chartArea1.Area3DStyle.Inclination = 15;
      chartArea1.Area3DStyle.IsClustered = true;
      chartArea1.Area3DStyle.IsRightAngleAxes = false;
      chartArea1.Area3DStyle.Perspective = 10;
      chartArea1.Area3DStyle.Rotation = 10;
      chartArea1.Area3DStyle.WallWidth = 0;
      chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
      chartArea1.AxisX.LabelStyle.Format = "hh:mm:ss";
      chartArea1.AxisX.LabelStyle.Interval = 20D;
      chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
      chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      chartArea1.AxisX.MajorGrid.Interval = 20D;
      chartArea1.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
      chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      chartArea1.AxisX.MajorTickMark.Interval = 20D;
      chartArea1.AxisX.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
      chartArea1.AxisY.IsLabelAutoFit = false;
      chartArea1.AxisY.IsStartedFromZero = false;
      chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
      chartArea1.AxisY.LabelStyle.Interval = 10D;
      chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      chartArea1.AxisY.MajorGrid.Interval = 10D;
      chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      chartArea1.AxisY.MajorTickMark.Interval = 10D;
      chartArea1.AxisY.Maximum = 30D;
      chartArea1.AxisY.Minimum = -30D;
      chartArea1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(165)))), ((int)(((byte)(191)))), ((int)(((byte)(228)))));
      chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
      chartArea1.BackSecondaryColor = System.Drawing.Color.White;
      chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      chartArea1.InnerPlotPosition.Auto = false;
      chartArea1.InnerPlotPosition.Height = 85F;
      chartArea1.InnerPlotPosition.Width = 86F;
      chartArea1.InnerPlotPosition.X = 8.3969F;
      chartArea1.InnerPlotPosition.Y = 3.63068F;
      chartArea1.Name = "Default";
      chartArea1.Position.Auto = false;
      chartArea1.Position.Height = 86.76062F;
      chartArea1.Position.Width = 88F;
      chartArea1.Position.X = 5.089137F;
      chartArea1.Position.Y = 5.895753F;
      chartArea1.ShadowColor = System.Drawing.Color.Transparent;
      this.chart1.ChartAreas.Add(chartArea1);
      legend1.Alignment = System.Drawing.StringAlignment.Far;
      legend1.BackColor = System.Drawing.Color.Transparent;
      legend1.DockedToChartArea = "Default";
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
      legend1.IsTextAutoFit = false;
      legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
      legend1.Name = "Default";
      this.chart1.Legends.Add(legend1);
      this.chart1.Location = new System.Drawing.Point(12, 115);
      this.chart1.Name = "chart1";
      series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
      series1.ChartArea = "Default";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
      series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(64)))), ((int)(((byte)(10)))));
      series1.Legend = "Default";
      series1.Name = "Series1";
      series1.ShadowOffset = 1;
      this.chart1.Series.Add(series1);
      this.chart1.Size = new System.Drawing.Size(880, 400);
      this.chart1.TabIndex = 13;
      // 
      // NetduinoCommunicationCenter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(904, 545);
      this.Controls.Add(this.chart1);
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
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
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
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

