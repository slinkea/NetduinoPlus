using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;

namespace Netduino.Communication
{
   public class MessageEventArgs : EventArgs 
   {
        private string _message;
        public string Message
        {
            get { return _message; }
        }
        public MessageEventArgs(string Message)
        {
            _message = Message;
        }
    }

    public class CommunicateWithNetduino
    {
        #region Private Variables
        private static CommunicateWithNetduino _communicateWithNetduino;
        private Socket _clientSocket = null;
        private Thread _listeningThread;
        private int _port = 80;
        private string _netduinoAddress = null;
        private ISynchronizeInvoke _callingThreadContextToInvoke;
        #endregion

        #region Constructors
        //Enforce that this class is a singleton that other classes can’t instantiate 
        private CommunicateWithNetduino()
        {
        }
        #endregion

        #region Public Properties
        public int Port
        {
            set { _port = value; }
            get { return _port; }
        }
        public string NetduinoAddress
        {
            set { _netduinoAddress = value; }
            get { return _netduinoAddress; }
        }
        #endregion

        #region Events
        public event EventHandler<MessageEventArgs> EventHandlerMessageReceived;
        public event EventHandler<MessageEventArgs> EventHandlerStatusUpdate;
        #endregion

        #region Public Methods
        //Singleton factory method to load and get the single instance
        public static CommunicateWithNetduino GetInstance()
        {
            if (_communicateWithNetduino == null)
            {
                _communicateWithNetduino = new CommunicateWithNetduino();
                _communicateWithNetduino.Port = 8000;
                _communicateWithNetduino.NetduinoAddress = "192.168.137.2";
            }

            return _communicateWithNetduino;
        }

        public void StartListening(ISynchronizeInvoke CallingThreadContextToInvoke)
        {
            _listeningThread = new Thread(new ThreadStart(ReceiveSocketsInBackgroundThread));
            _listeningThread.IsBackground = true;
            _listeningThread.Start();
            _callingThreadContextToInvoke = CallingThreadContextToInvoke;
        }

        public void SendMessage(string message)
        {
            if (_netduinoAddress != null && _port > 0)
            {
                using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {

                    //IPHostEntry entry = Dns.GetHostEntry(_netduinoAddress);
                    //IPAddress address = entry.AddressList[0];
                    IPAddress address = IPAddress.Parse(_netduinoAddress);
                    IPEndPoint endpoint = new IPEndPoint(address, _port);

                    try
                    {
                        socket.Connect(endpoint);
                        socket.Send(Encoding.UTF8.GetBytes(message));
                        socket.Close();
                    }
                    catch (SocketException se)
                    {
                        RaiseEvent(EventHandlerStatusUpdate, "Socket Exception! Probably bad ip or netduino not ready?");
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private bool IsSocketConnected(Socket socket)
        {
            bool connectionNotClosedResetOrTerminated = !socket.Poll(1000, SelectMode.SelectRead);
            bool socketHasDataAvailableToRead = (socket.Available != 0);
            return (connectionNotClosedResetOrTerminated || socketHasDataAvailableToRead);
        }

        private void ReceiveSocketsInBackgroundThread()
        {
            string receiveMessage = "";

            using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, _port));
                socket.Listen(10);

                while (true)
                {
                    RaiseEvent(EventHandlerStatusUpdate, "Waiting for connection...");
                    _clientSocket = socket.Accept();  //This call is "blocking" and will will wait for a connection, which also means the thread hangs around
                    RaiseEvent(EventHandlerStatusUpdate, "Connection Accepted!");

                    using (_clientSocket)
                    {
                        while (IsSocketConnected(_clientSocket))
                        {
                            int availablebytes = _clientSocket.Available;
                            byte[] buffer = new byte[availablebytes];
                            _clientSocket.Receive(buffer);
                            if (buffer.Length > 0)
                            {
                                receiveMessage = Encoding.UTF8.GetString(buffer);
                                RaiseEvent(EventHandlerMessageReceived, receiveMessage);
                            }
                        }
                    }
                }
            }
        }

        private void RaiseEvent(EventHandler<MessageEventArgs> handler, string message)
        {
            // Event will be null if there are no subscribers
            if (handler != null)
            {
                MessageEventArgs e = new MessageEventArgs(message);
                if (_callingThreadContextToInvoke == null)
                {
                    handler(this, e);
                }
                else
                {
                    //handle cross-thread communication back to the main thread
                    _callingThreadContextToInvoke.Invoke(handler, new object[] { this, e });
                }
            }
        }
        #endregion
    }
}
