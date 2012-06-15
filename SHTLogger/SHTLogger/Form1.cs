using System.Windows.Forms;
using System.Threading;
using System;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SHTLogger
{
  public partial class Form1 : Form
  {
    private Thread _netThread;
    private Thread _addDataRunner;
    public delegate void AddDataDelegate();
    public AddDataDelegate _addDataDel;
    private Socket clientSocket = null;
    public delegate void DataReceivedDelegate(string rawData);

    public Form1()
    {
      InitializeComponent();

    }

    private void Form1_Load(object sender, System.EventArgs e)
    {
      // Predefine the viewing area of the chart
      DateTime minValue = DateTime.Now;
      DateTime maxValue = minValue.AddSeconds(120);

      chart1.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
      chart1.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();

      // Reset number of series in the chart.
      chart1.Series.Clear();

      // create a line chart series
      Series newSeries = new Series("Series1");
      newSeries.ChartType = SeriesChartType.Line;
      newSeries.BorderWidth = 2;
      newSeries.Color = Color.OrangeRed;
      newSeries.XValueType = ChartValueType.DateTime;
      chart1.Series.Add(newSeries);

      //////////////////////////////////////////////////////////////////////////

      ThreadStart addDataFromNetduino = new ThreadStart(AddDataFromNetduino);
      _netThread = new Thread(new ThreadStart(addDataFromNetduino));
      _netThread.IsBackground = true;
      _netThread.Start();
    }

    private void AddDataFromNetduino()
    {
      string rawData = "";

      using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Bind(new IPEndPoint(IPAddress.Any, 8000));
        socket.Listen(10);

        while (true)
        {
          //toolStripStatusLabel1.Text = "Accepting connections...";
          clientSocket = socket.Accept();  //This call is "blocking" as it will wait for a connection, which also means the thread hangs around
          //toolStripStatusLabel1.Text = "Connection Accepted!";

          using (clientSocket)
          {
            while (SocketConnected(clientSocket))
            {
              int availablebytes = clientSocket.Available;
              byte[] buffer = new byte[availablebytes];  // i ignored all the buffer overflow prevention stuff in the web server code
              clientSocket.Receive(buffer);

              if (buffer.Length > 0) // make sure there's something to add
              {
                rawData = Encoding.UTF8.GetString(buffer);
                //toolStripStatusLabel1.Text = "Adding:" + s;

                AddDataPoint(rawData);
              }
            }
          }
        }
      }
    }

    public void AddDataPoint(string rawData)
    {
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

    static bool SocketConnected(Socket s)
    {
      bool ret = true;
      bool part1 = s.Poll(1000, SelectMode.SelectRead);
      bool part2 = (s.Available == 0);
      
      if (part1 & part2)
      {
        ret = false;
      }

      return ret;
    }


    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (_netThread != null && _netThread.IsAlive)
      {
        if ((_netThread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
        {
          _netThread.Resume();
        }

        _netThread.Abort();
      }
    }
  }
}
