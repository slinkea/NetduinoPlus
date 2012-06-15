using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DataLogger
{
    public partial class Form1 : Form
    {
        //The delegate is needed to handle cross-thread communication between netStuff() (which runs in its own thread) and chart1
        //See adddatapoint below for more info!

        public delegate void DataReceivedDelegate(string rawData);

        //This checks to see if the socket is dead
        //I borrowed it from Plater's answer at:
        //http://bytes.com/topic/net/answers/828133-c-net-detecting-socket-closed

        static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
            {//connection is closed
                return false;
            }
            return true;
        }
        private Socket clientSocket = null;
        public void netStuff()
        {
            //This is mostly borrowed from the Netduino Plus Web Server code at:
            //http://forums.netduino.com/index.php?/topic/575-updated-web-server/

            string s = "";


            using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                socket.Bind(new IPEndPoint(IPAddress.Any, 8000));
                socket.Listen(10);

                while (true)
                {
                    toolStripStatusLabel1.Text = "Accepting connections...";
                    clientSocket = socket.Accept();  //This call is "blocking" as it will wait for a connection, which also means the thread hangs around
                    toolStripStatusLabel1.Text = "Connection Accepted!";

                    using (clientSocket)
                    {
                        while (SocketConnected(clientSocket))
                        {
                            int availablebytes = clientSocket.Available;
                            byte[] buffer = new byte[availablebytes];  // i ignored all the buffer overflow prevention stuff in the web server code
                            clientSocket.Receive(buffer);
                            if (buffer.Length > 0) // make sure there's something to add
                            {
                                s = Encoding.UTF8.GetString(buffer);
                                toolStripStatusLabel1.Text = "Adding:" + s;

                                AddDataPoint(s);
                            }
                        }
                    }
                }
            }
        }

        private Thread t;
        public Form1()
        {
            InitializeComponent();

            //some threading stuff borrowed from george1106 at:
            //http://bytes.com/topic/net/answers/642815-c-application-does-not-terminate

            t = new Thread(new ThreadStart(netStuff));

            t.IsBackground = true;

            t.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public void AddDataPoint(string rawData)
        {
            // The thread safety stuff is from:
            // http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k%28EHINVALIDOPERATION.WINFORMS.ILLEGALCROSSTHREADCALL%29;k%28TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22%29;k%28DevLang-CSHARP%29&rd=true
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.

            string[] sa = rawData.Split('|');
            int? i = null;
            try
            {
                i = int.Parse(sa[0]);
            }
            catch { }

            if (i != null)
            {
                if (this.chart1.InvokeRequired)
                {
                    DataReceivedDelegate d = new DataReceivedDelegate(AddDataPoint);

                    this.Invoke(d, new object[] { rawData });
                }
                else
                {
                    chart1.Series["Series1"].Points.AddY(i);
                }
            }
        }


        private void AddDataPoint(int rawDataInt)
        {
            chart1.Series["Series1"].Points.AddY(rawDataInt);
        }

        // there's a button that adds a random data point just for testing purposes...
        public Random rnd = new Random();
        private void button1_Click(object sender, EventArgs e)
        {
            int i = rnd.Next(0, 3300);
            AddDataPoint(i);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this kills the listening thread when the form is closed
            //I have no idea what I'm supposed to do to handle this correctly, but this seems to work, so...
            if (t != null && t.IsAlive)
            {
                t.Abort();
            }
        }

    }
}
