using System.Windows.Forms;
using System.Threading;
using System;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace SHTLogger
{
  public partial class Form1 : Form
  {
    private Thread _addDataRunner;
    public delegate void AddDataDelegate();
    public AddDataDelegate _addDataDel;

    public Form1()
    {
      InitializeComponent();

    }

    private void Form1_Load(object sender, System.EventArgs e)
    {
      // create the Adding Data Thread but do not start until start button clicked
      ThreadStart addDataThreadStart = new ThreadStart(AddDataThreadLoop);
      _addDataRunner = new Thread(addDataThreadStart);

      // create a delegate for adding data
      _addDataDel += new AddDataDelegate(AddData);

      StartTrending();
    }

    private void StartTrending()
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

      // start worker threads.
      if (_addDataRunner.IsAlive == true)
      {
        _addDataRunner.Resume();
      }
      else
      {
        _addDataRunner.Start();
      }
    }

    private void StopTrending()
    {
      if (_addDataRunner.IsAlive == true)
      {
        _addDataRunner.Suspend();
      }
    }

    /// Main loop for the thread that adds data to the chart.
    /// The main purpose of this function is to Invoke AddData
    /// function every 1000ms (1 second).
    private void AddDataThreadLoop()
    {
      while (true)
      {
        chart1.Invoke(_addDataDel);

        Thread.Sleep(1000);
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
    public void AddNewPoint(DateTime timeStamp, System.Windows.Forms.DataVisualization.Charting.Series ptSeries)
    {
      double newVal = 0;

      if (ptSeries.Points.Count > 0)
      {
        newVal = 20; //ptSeries.Points[ptSeries.Points.Count - 1].YValues[0] + ((rand.NextDouble() * 2) - 1);
      }

      if (newVal < 0)
        newVal = 0;

      // Add new data point to its series.
      ptSeries.Points.AddXY(timeStamp.ToOADate(), 20 /*rand.Next(10, 20)*/);

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
      if ((_addDataRunner.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
      {
        _addDataRunner.Resume();
      }
      _addDataRunner.Abort();

      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }
  }
}
