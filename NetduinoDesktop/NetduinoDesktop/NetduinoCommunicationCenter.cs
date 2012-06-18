using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Netduino.Communication;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;

namespace Netduino.DesktopMessenger
{
    public partial class NetduinoCommunicationCenter : Form
    {
        private DateTime minValue, maxValue;
        private CommunicateWithNetduino _ethernetCommunication = CommunicateWithNetduino.GetInstance();

        public NetduinoCommunicationCenter()
        {
            InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (textBoxSendMessage.Text.Length > 0)
            {
                _ethernetCommunication.SendMessage(textBoxSendMessage.Text);

                if (textBoxSendMessage.Text.ToUpper() == "EXIT")
                {
                    Application.Exit();
                }
                textBoxSendMessage.Text = string.Empty;
            }
        }

        private void TalkToNetduino_Load(object sender, EventArgs e)
        {
          // Predefine the viewing area of the chart
          minValue = DateTime.Now;
          maxValue = minValue.AddSeconds(30);

          chart1.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
          chart1.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();

          // Reset number of series in the chart.
          chart1.Series.Clear();

          Series newSeries = new Series("Series1");
          newSeries.ChartType = SeriesChartType.Line;
          newSeries.BorderWidth = 1;
          //newSeries.Color = Color.FromArgb(224, 64, 10);
          newSeries.ShadowOffset = 1;
          newSeries.XValueType = ChartValueType.DateTime;
          chart1.Series.Add(newSeries);


          _ethernetCommunication.EventHandlerMessageReceived += new EventHandler<MessageEventArgs>(OnMessageReceived);
          _ethernetCommunication.EventHandlerStatusUpdate += new EventHandler<MessageEventArgs>(OnStatusUpdate);
          _ethernetCommunication.StartListening((ISynchronizeInvoke)this);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            textBoxReceiveMessage.Text = textBoxReceiveMessage.Text + e.Message + "\r\n";

            string[] data = e.Message.Split('|');
            double? temperature = null;
            double? humidity = null;

            try
            {
              temperature = Convert.ToDouble(data[0], System.Globalization.CultureInfo.InvariantCulture);
              humidity = Convert.ToDouble(data[1], System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
              Debug.Print(ex.StackTrace);
            }

            if (temperature != null)
            {
              AddTemperaturePoint(chart1.Series[0], temperature);
            }

            //Move the scroll position to the end of the text
            textBoxReceiveMessage.SelectionStart = textBoxReceiveMessage.Text.Length;
            textBoxReceiveMessage.ScrollToCaret();

            //The netduino has sent a message and is now ready to receive
            if (e.Message=="Communications are initialized.")
            {
                buttonSend.Enabled = true;
                SetTimeOnNetdino();
            }
        }

        public void AddTemperaturePoint(Series series, double? temperature)
        {
          DateTime timeStamp = DateTime.Now;

          series.Points.AddXY(timeStamp.ToOADate(), temperature);

          double removeBefore = timeStamp.AddSeconds((double)(90) * (-1)).ToOADate();

          while (series.Points[0].XValue < removeBefore)
          {
            series.Points.RemoveAt(0);
          }

          chart1.ChartAreas[0].AxisX.Minimum = series.Points[0].XValue;
          chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(series.Points[0].XValue).AddMinutes(2).ToOADate();

          chart1.Invalidate();
        }

        private void OnStatusUpdate(object sender, MessageEventArgs e)
        {
            toolStripStatusLabelCommunicationStatus.Text = e.Message;
        }

        private void textBoxSendMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar ==  (char)Keys.Enter && buttonSend.Enabled)
            {
                SendMessage();
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxReceiveMessage.Text = string.Empty;
        }

        private void SetTimeOnNetdino()
        {
            string presentTime = string.Format("TIME {0} {1} {2} {3} {4} {5} {6}",
                                                DateTime.Now.Year,
                                                DateTime.Now.Month,
                                                DateTime.Now.Day,
                                                DateTime.Now.Hour,
                                                DateTime.Now.Minute,
                                                DateTime.Now.Second,
                                                DateTime.Now.Millisecond);
            _ethernetCommunication.SendMessage(presentTime);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            _ethernetCommunication.SendMessage("WINDOWSHADE OPEN");
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _ethernetCommunication.SendMessage("WINDOWSHADE STOP");
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            _ethernetCommunication.SendMessage("WINDOWSHADE CLOSE");
        }
    }
}
