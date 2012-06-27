using System.Threading;
using System;


namespace Netduino.Controller
{
    public class Program
    {
        private static bool _exitProgram = false;

        public static void Main()
        {
            EthernetCommunication.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);
            //EthernetCommunication.SendMessage("Communications are initialized.");
            EthernetCommunication.GetInstance();
            
            while (!_exitProgram)
            {
                Thread.Sleep(500);
            }
        }

        private static void OnMessageReceived(string message)
        {
            string[] parts = message.Split(' ');
            switch(parts[0].ToUpper()) 
            {
                case "TIME":
                {
                    DateTime presentTime = new DateTime(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]), int.Parse(parts[7]));
                    Microsoft.SPOT.Hardware.Utility.SetLocalTime(presentTime);

                    EthernetCommunication.SendMessage("Communications are initialized.");
                    break;
                }

                case "EXIT":
                {
                    _exitProgram = true;
                    break;
                }

                default:
                {
                    EthernetCommunication.SendMessage("Unknown command: " + message);
                    break;
                }
            }
        }
    }
}
