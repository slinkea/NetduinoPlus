using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;

using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Net.NetworkInformation;

using System.Threading;

namespace Netduino.Home.Controller
{
    public delegate void MessageEventHandler(string Message);

    class EthernetCommunication
    {
        #region Private Variables
        private string _hostAddress = null;
        private int _port = 80;
        private string _netduinoStaticIPAddress = null;
        private string _subnetMask = null;
        private string _gatewayAddress = null;
        private Thread _listeningThread;
        private Socket _clientSocket = null;
        private static EthernetCommunication _ethernetCommunication;
        #endregion

        #region Constructors
        //This keeps other classes from creating an instance
        private EthernetCommunication()
        {
        }
        #endregion

        #region Public Properties
        public string HostAddress
        {
            set { _hostAddress = value; }
            get { return _hostAddress; }
        }
        public int Port
        {
            set { _port = value; }
            get { return _port; }
        }
        public string NetduinoStaticIPAddress
        {
            set 
            { 
                _netduinoStaticIPAddress = value;
                SetNetduinoStaticIPConfiguration();
            }
            get { return _netduinoStaticIPAddress; }
        }
        public string SubnetMask
        {
            set
            {
                _subnetMask = value;
                SetNetduinoStaticIPConfiguration();
            }
            get { return _subnetMask; }
        }
        public string GatewayAddress
        {
            set 
            {
                _gatewayAddress = value;
                SetNetduinoStaticIPConfiguration();
            }
            get { return _gatewayAddress; }
        }
        #endregion

        #region Events
        public static event MessageEventHandler EventHandlerMessageReceived;
        #endregion

        #region Public Methods
        
        private void StartListening()
        {
            _listeningThread = new Thread(new ThreadStart(ReceiveSocketsInListeningThread));
            _listeningThread.Start();
        }

        private void InitializeConfiguration()
        {
            if (_netduinoStaticIPAddress == null)
                throw new Exception("The netduino Static IP Address nust be set!");

            if (_subnetMask == null)
                throw new Exception("The Subnet Mask must be set!");

            if (_gatewayAddress == null)
                throw new Exception("The Gateway address must be set.");

            SetNetduinoStaticIPConfiguration();
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];

            if (_netduinoStaticIPAddress != networkInterface.IPAddress)
                throw new Exception("Problem setting the static IP.");

            if (_subnetMask != networkInterface.SubnetMask)
                throw new Exception("Problem setting the subnet mask.");

            if (_gatewayAddress != networkInterface.GatewayAddress)
                throw new Exception("Problem setting the gateway address.");
        }
        #endregion

        #region Public Static Methods
        public static EthernetCommunication GetInstance()
        {
            if (_ethernetCommunication == null)
            {
                _ethernetCommunication = new EthernetCommunication();
                _ethernetCommunication.HostAddress = Config.HostAddress;
                _ethernetCommunication.Port = Config.Port;
                _ethernetCommunication.NetduinoStaticIPAddress = Config.NetduinoStaticIPAddress;
                _ethernetCommunication.SubnetMask = Config.SubnetMask;
                _ethernetCommunication.GatewayAddress = Config.GatewayAddress;
                _ethernetCommunication.InitializeConfiguration();
                _ethernetCommunication.StartListening();
            }
            return _ethernetCommunication;
        }

        public static void SendMessage(string message)
        {
            GetInstance().SendEthernetMessage(message);
        }
        #endregion

        #region Private Methods
        private bool IsSocketConnected(Socket socket)
        {
            bool connectionNotClosedResetOrTerminated = !socket.Poll(1000, SelectMode.SelectRead);
            bool socketHasDataAvailableToRead = (socket.Available != 0);
            return (connectionNotClosedResetOrTerminated || socketHasDataAvailableToRead);
        }

        private void ReceiveSocketsInListeningThread()
        {
            string receiveMessage = "";
            bool exitProgram = false;

            using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, _port));
                socket.Listen(10);

                while (!exitProgram)
                {
                    Debug.Print("Waiting for connection...");

                    try
                    {
                      _clientSocket = socket.Accept();  //This call is "blocking" and will will wait for a connection, which also means the thread hangs around
                    }
                    catch (SocketException se)
                    {
                      Debug.Print("Socket Exception!  Ethernet cable disconnected?");
                      Debug.Print(se.StackTrace);
                    }

                    Debug.Print( "Connection Accepted!");

                    using (_clientSocket)
                    {
                        while (IsSocketConnected(_clientSocket))
                        {
                            int availablebytes = _clientSocket.Available;
                            byte[] buffer = new byte[availablebytes];
                            _clientSocket.Receive(buffer);
                            if (buffer.Length > 0)
                            {
                                receiveMessage = new string(Encoding.UTF8.GetChars(buffer));
                                RaiseMessageReceivedEvent(receiveMessage);
                                if (receiveMessage.ToUpper() == "EXIT")
                                {
                                    exitProgram = true;
                                }
                            }
                            
                        }
                    }
                }
            }
        }

        private void RaiseMessageReceivedEvent(string message)
        {
            // Event will be null if there are no subscribers
            if (EventHandlerMessageReceived != null)
            {
                EventHandlerMessageReceived(message);
            }
        }

        private void SetNetduinoStaticIPConfiguration()
        {
            //Exit if not all of the configuration properties are set
            if (_netduinoStaticIPAddress == null || _subnetMask == null || _gatewayAddress == null)
                return;

            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];

            bool _ipAddressAlreadySet = _netduinoStaticIPAddress == networkInterface.IPAddress;
            bool _subnetMaskAlreadySet = _subnetMask == networkInterface.SubnetMask;
            bool _gatewayAlreadySet = _gatewayAddress == networkInterface.GatewayAddress;

            if (_ipAddressAlreadySet && _subnetMaskAlreadySet && _gatewayAlreadySet)
                return;

            // Set our IP address to a new value
            // This will be saved in the config sector of the netduino and will survive reboots 
            networkInterface.EnableStaticIP(_netduinoStaticIPAddress, _subnetMask, _gatewayAddress);
        }

        private void SendEthernetMessage(string message)
        {
            if (_hostAddress != null && _port > 0)
            {
                using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {

                    IPHostEntry entry = Dns.GetHostEntry(_hostAddress);
                    IPAddress address = entry.AddressList[0];
                    IPEndPoint endpoint = new IPEndPoint(address, _port);

                    try
                    {
                        socket.Connect(endpoint);
                        socket.Send(Encoding.UTF8.GetBytes(message));
                        socket.Close();
                        Debug.Print(message);
                    }
                    catch (SocketException se)
                    {
                        Debug.Print("Socket Exception!  Probably no server or bad ip?");
                        Debug.Print(se.StackTrace);
                    }
                }
            }
        }
        #endregion
    }
}
