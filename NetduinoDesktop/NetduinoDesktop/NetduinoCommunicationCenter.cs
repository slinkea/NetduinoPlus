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

namespace Netduino.DesktopMessenger
{
    public partial class NetduinoCommunicationCenter : Form
    {
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
              Thread.Sleep(1);
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
