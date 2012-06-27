using System.Threading;
using System;
using ElzeKool.io.sht11_io;
using SecretLabs.NETMF.Hardware.Netduino;
using ElzeKool.io;
using Microsoft.SPOT;


namespace Netduino.Controller
{
    public class Program
    {
        private static bool _exitProgram = false;

        private static SHT11_GPIO_IOProvider SHT11_IO = new SHT11_GPIO_IOProvider(Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2);
        private static SensirionSHT11 SHT11 = new SensirionSHT11(SHT11_IO);

        public static void Main()
        {
            InitSHT();

            EthernetCommunication.EventHandlerMessageReceived += new MessageEventHandler(OnMessageReceived);
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
                case "TEMPERATURE":
                {
                    double temperature = SHT11.ReadTemperature(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V, SensirionSHT11.SHT11TemperatureUnits.Celcius);
                    EthernetCommunication.SendMessage("TEMPERATURE=" + temperature.ToString("F2"));
                    break;
                }
                case "HUMIDITY":
                {
                    double humidity = SHT11.ReadRelativeHumidity(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V);
                    EthernetCommunication.SendMessage("HUMIDITY=" + humidity.ToString("F2"));
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

        private static void InitSHT()
        {
            // Soft-Reset the SHT11
            if (SHT11.SoftReset())
            {
                // Softreset returns True on error
                throw new Exception("Error while resetting SHT11");
            }

            // Set Temperature and Humidity to less acurate 12/8 bit
            if (SHT11.WriteStatusRegister(SensirionSHT11.SHT11Settings.LessAcurate))
            {
                // WriteRegister returns True on error
                throw new Exception("Error while writing status register SHT11");
            }

            // Do readout
            Debug.Print("RAW Temperature 12-Bit: " + SHT11.ReadTemperatureRaw());
            Debug.Print("RAW Humidity 8-Bit: " + SHT11.ReadHumidityRaw());

            // Set Temperature and Humidity to more acurate 14/12 bit
            if (SHT11.WriteStatusRegister((SensirionSHT11.SHT11Settings.NullFlag)))
            {
                // WriteRegister returns True on error
                throw new Exception("Error while writing status register SHT11");
            }

            // Do readout
            Debug.Print("RAW Temperature 14-Bit: " + SHT11.ReadTemperatureRaw());
            Debug.Print("RAW Humidity 12-Bit: " + SHT11.ReadHumidityRaw());
        }
    }
}
