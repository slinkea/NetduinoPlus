using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;



namespace NetLogger
{
    class NetLog
    {
        public string Host = null;
        public int Port = 80;

        //I think this is mostly borrowed from:
        //http://forums.netduino.com/index.php?/topic/389-how-to-use-the-new-functionalities/page__view__findpost__p__2804

        public void Print(String Message)
        {
            if (Host != null && Port > 0)
            {
                using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {

                    IPHostEntry entry = Dns.GetHostEntry(Host);
                    IPAddress address = entry.AddressList[0];
                    IPEndPoint endpoint = new IPEndPoint(address, Port);

                    try
                    {
                        socket.Connect(endpoint);
                        socket.Send(Encoding.UTF8.GetBytes(Message));
                        socket.Close();
                        Debug.Print(Message);
                    }
                    catch (SocketException se)  
                    {
                        Debug.Print("Socket Exception!  Probably no server or bad ip?");
                        Debug.Print(se.StackTrace);
                    }
                }
            }
        }
    }
}
