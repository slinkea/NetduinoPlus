using System;
using System.Threading;

namespace Netduino.Home.Controller
{
    public class Program
    {
        private static bool _exitProgram = false;
        private static WindowShade _windowShade = new WindowShade();
        private static double _temperature = double.MaxValue;
        private static double _humidity = double.MaxValue;

        public static void Main()
        {
            EthernetCommunication.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);
            EthernetCommunication.SendMessage("Communications are initialized.");

            SHT15.Init();
            Time.RunRepetitively(OnGetTemperature, 3000);

            while (!_exitProgram)
            {
                Thread.Sleep(500);
            }
        }

        private static void OnGetTemperature()
        {
          double temperature = SHT15.GetTemperatureCelcius();

          if (temperature != _temperature)
          {
            _temperature = temperature;
              EthernetCommunication.SendMessage("Temperature: " + _temperature.ToString() + "°C @ " + DateTime.Now.ToString());
          }

          double humidity = SHT15.GetHumidity();

          if (humidity != _humidity)
          {
            _humidity = humidity;
            EthernetCommunication.SendMessage("Humidity: " + _humidity.ToString() + "% @ " + DateTime.Now.ToString());
          }
        }

        private static void OnMessageReceived(string message)
        {
            string[] parts = message.Split(' ');
            switch(parts[0].ToUpper()) 
            {
                case "TIME":
                   Time.SetTime(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]), int.Parse(parts[7]));
                   break;

                case "SHOWTIME":
                   EthernetCommunication.SendMessage("Time: " + DateTime.Now.ToString());
                   break;

                case "WINDOWSHADE":
                   {
                       if (parts[1] == "STOP")
                       {
                           _windowShade.Execute(WindowShade.ShadeCommand.Stop);
                       }
                       else if (parts[1] == "OPEN")
                       {
                           _windowShade.Execute(WindowShade.ShadeCommand.Open);
                       }
                       else if (parts[1] == "CLOSE")
                       {
                           _windowShade.Execute(WindowShade.ShadeCommand.Close);
                       }
                       else
                       {
                           EthernetCommunication.SendMessage("Unknown command: " + message);
                       }
                       
                       break;
                   }

                case "EXIT":
                    _exitProgram = true;
                    break;

                default:
                    EthernetCommunication.SendMessage("Unknown command: " + message);
                    break;
            }
        }
    }
}
