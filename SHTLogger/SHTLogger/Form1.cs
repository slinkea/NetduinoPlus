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
    private Thread _clientThread;
    private Socket _clientSocket = null;
    public delegate void DataReceivedDelegate(string rawData);

    public delegate void AddDataDelegate();
    public AddDataDelegate addDataDel;

    private DateTime minValue, maxValue;
    private Random rand = new Random();

    public Form1()
    {
      InitializeComponent();

    }

    private void Form1_Load(object sender, System.EventArgs e)
    {
      // create a delegate for adding data
      addDataDel += new AddDataDelegate(AddData);

      ThreadStart addDataFromNetduino = new ThreadStart(AddDataFromNetduino);
      _clientThread = new Thread(addDataFromNetduino);
      //_clientThread.IsBackground = true;

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

      _clientThread.Start();
    }

    private void AddDataFromNetduino()
    {
      string rawData = "";

      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        socket.Bind(new IPEndPoint(IPAddress.Any, 8000));
        socket.Listen(10);

        while (true)
        {
          AddDataPoint(rawData);
          Thread.Sleep(1000);
          continue;

          toolStripStatusLabel1.Text = "Accepting connection...";
          _clientSocket = socket.Accept();  // This call is "blocking" as it will wait for a connection, which also means the thread hangs around
          toolStripStatusLabel1.Text = "Connection Accepted!";

          using (_clientSocket)
          {
            while (!(_clientSocket.Poll(1000, SelectMode.SelectRead) & (_clientSocket.Available == 0)))
            {
              int availablebytes = _clientSocket.Available;
              byte[] buffer = new byte[availablebytes];  // ignored all the buffer overflow prevention stuff in the web server code
              _clientSocket.Receive(buffer);

              if (buffer.Length > 0) // make sure there's something to add
              {
                rawData = Encoding.UTF8.GetString(buffer);
                toolStripStatusLabel1.Text = "Adding: " + rawData;

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

      ////

      chart1.Invoke(addDataDel);
      return;
      ////

      return;

      try
      {
        i = int.Parse(sa[0]);
      }
      catch { }

      if (i != null)
      {
        chart1.Invoke(addDataDel);
      }
    }

    public void AddData()
    {
      DateTime timeStamp = DateTime.Now;

      foreach (Series ptSeries in chart1.Series)
      {
        AddNewPoint(timeStamp, ptSeries);
      }
    }

    /// The AddNewPoint function is called for each series in the chart when
    /// new points need to be added.  The new point will be placed at specified
    /// X axis (Date/Time) position with a Y value in a range +/- 1 from the previous
    /// data point's Y value, and not smaller than zero.
    public void AddNewPoint(DateTime timeStamp, Series ptSeries)
    {
      // Add new data point to its series.
      ptSeries.Points.AddXY(timeStamp.ToOADate(), rand.Next(-30, 30));

      // remove all points from the source series older than 1.5 minutes.
      double removeBefore = timeStamp.AddSeconds((double)(90) * (-1)).ToOADate();

      //remove oldest values to maintain a constant number of data points
      while (ptSeries.Points[0].XValue < removeBefore)
      {
        ptSeries.Points.RemoveAt(0);
      }

      chart1.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
      chart1.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(2).ToOADate();

      chart1.Invalidate();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_clientThread != null && _clientThread.IsAlive)
        {
          if (_clientSocket != null)
          {
            _clientSocket.Shutdown(SocketShutdown.Receive);
          }

          _clientThread.Abort();
        }

        if (components != null)
        {
          components.Dispose();
        }
      }

      base.Dispose(disposing);
    }
  }
}
