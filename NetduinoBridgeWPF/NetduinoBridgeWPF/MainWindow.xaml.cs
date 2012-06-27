using System.Windows;
using Netduino.Communication;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Timers;

namespace NetduinoBridgeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Action<System.Action> executor = action => action();
        private CommunicateWithNetduino _ethernetCommunication = CommunicateWithNetduino.GetInstance();
        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer aTemperatureTimer;


        public MainWindow()
        {
            InitializeComponent();

            // Subscribing to events
            _ethernetCommunication.EventHandlerMessageReceived += new EventHandler<MessageEventArgs>(OnMessageReceived);
            _ethernetCommunication.EventHandlerStatusUpdate += new EventHandler<MessageEventArgs>(OnStatusUpdate);
            _ethernetCommunication.StartListening(null);

            // Connect must be called after the blocking accept
            aTimer = new System.Timers.Timer(500);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.AutoReset = false;
            aTimer.Enabled = true;

            aTemperatureTimer = new System.Timers.Timer(5000);
            aTemperatureTimer.Elapsed += new ElapsedEventHandler(OnTimedTemperatureEvent);
            aTemperatureTimer.Enabled = true;
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
    }
}
