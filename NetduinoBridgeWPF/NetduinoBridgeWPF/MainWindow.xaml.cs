using System.Windows;
using Netduino.Communication;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Timers;
using MM.Monitor.Plugins;

namespace NetduinoBridgeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Action<System.Action> executor = action => action();
        private CommunicateWithNetduino _ethernetCommunication = CommunicateWithNetduino.GetInstance();
        private static System.Timers.Timer _socketTimer;
        private static System.Timers.Timer _temperatureTimer;
        private Cloud _cloud = null;
        private ControlThink.ZWave.ZWaveController m_ZWaveController = new ControlThink.ZWave.ZWaveController();


        public MainWindow()
        {
            InitializeComponent();

            // Subscribing to events
            _ethernetCommunication.EventHandlerMessageReceived += new EventHandler<MessageEventArgs>(OnMessageReceived);
            _ethernetCommunication.EventHandlerStatusUpdate += new EventHandler<MessageEventArgs>(OnStatusUpdate);
            _ethernetCommunication.StartListening(null);

            // Connect must be called after the blocking accept
            _socketTimer = new System.Timers.Timer(500);
            _socketTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _socketTimer.AutoReset = false;
            _socketTimer.Enabled = true;

            _temperatureTimer = new System.Timers.Timer(5000);
            _temperatureTimer.Elapsed += new ElapsedEventHandler(OnTimedTemperatureEvent);
            _temperatureTimer.Enabled = true;

            //ConnectToZWave();

            _cloud = new Cloud(_ethernetCommunication);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SendTimeToNetdino();
        }

        private void OnTimedTemperatureEvent(object source, ElapsedEventArgs e)
        {
            _ethernetCommunication.SendMessage("TEMPERATURE");
            _ethernetCommunication.SendMessage("HUMIDITY");
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            Debug.Print(e.Message);

            string[] data = e.Message.Split('=');

            if (data[0] == "TEMPERATURE")
            {
                _cloud.Temperature = data[1] + " °C";
            }
            else if (e.Message.Contains("HUMIDITY"))
            {
                _cloud.Humidity = data[1] + " %";
            }
            else if (e.Message.Contains("SHADE"))
            {
              _cloud.Shade = data[1];
            }
        }

        private void OnStatusUpdate(object sender, MessageEventArgs e)
        {
            Debug.Print(e.Message);
        }

        private void SendTimeToNetdino()
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

        private void SendExitToNetdino()
        {
            _ethernetCommunication.SendMessage("EXIT");
        }

        private void ConnectToZWave()
        {
            if (m_ZWaveController.IsConnected == false)
            {
                try
                {
                    m_ZWaveController.Connect();

                    //also, wire up our ControllerNotResponding event so that we can capture
                    //unintentional disconnects or general bad USB behavior...
                    m_ZWaveController.ControllerNotResponding += new System.EventHandler(m_ZWaveController_ControllerNotResponding);
                    //and set the object's SynchronizingObject property to this form so that
                    //any event handlers are called here and we don't have to worry about thread
                    //safety with Windows Forms.
                    m_ZWaveController.SynchronizingObject = this;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("I'm sorry, but I could not connect to the USB dongle." + System.Environment.NewLine + System.Environment.NewLine + "ex: " + ex.Message);
                }
            }
        }

        private void DisconnectFromZWave()
        {
            try
            {
                m_ZWaveController.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void m_ZWaveController_ControllerNotResponding(object sender, EventArgs e)
        {
            //if our controller stops responding, disconnect.  This is probably due to the USB
            //stick being disconnected from the computer.

            DisconnectFromZWave();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DisconnectFromZWave();
        }
    }
}
