/*
 ThingSpeak Client
 This program allows you to update a ThingSpeak Channel via the ThingSpeak API using a Netduino Plus
 
 Getting Started with ThingSpeak:
 
    * Sign Up for New User Account - https://www.thingspeak.com/users/new
    * Create a New Channel by selecting Channels and then Create New Channel
    * Enter the Write API Key in this program under "ThingSpeak Settings"
  
 @created: January 26, 2011
 @updated: 
 
 @tutorial: http://community.thingspeak.com/tutorials/netduino/create-your-own-web-of-things-using-the-netduino-plus-and-thingspeak/
  
 @copyright (c) 2011 Hans Scharler (http://www.iamshadowlord.com)
 @licence: Creative Commons, Attribution-Share Alike 3.0 United States, http://creativecommons.org/licenses/by-sa/3.0/us/deed.en
  
 */

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Socket = System.Net.Sockets.Socket;

using ElzeKool.io;
using ElzeKool.io.sht11_io;

namespace ThingSpeakClient
{
    public class Program
    {

        //ThingSpeak Settings
        const string writeAPIKey = "0DTQ70IVTGAGWQ0W "; // Write API Key for a ThingSpeak Channel
        static string tsIP = "184.106.153.149";     // IP Address for the ThingSpeak API
        static Int32 tsPort = 80;                   // Port Number for ThingSpeak
        const int updateInterval = 30000;           // Time interval in milliseconds to update ThingSpeak 

        static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        static InterruptPort button = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptNone);

        static SensirionSHT11 SHT11 = null;

        public static void Main()
        {
          InitSHT();

          SecretLabs.NETMF.Hardware.AnalogInput A0 = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);

            while (true)
            {               
                // Check analog input on Pin A0
                //int analogReading = A0.Read();

                double temperature = SHT11.ReadTemperature(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V, SensirionSHT11.SHT11TemperatureUnits.Celcius); 
                Debug.Print("Temperature Celcius: " + temperature.ToString("F2"));

                double humidity = SHT11.ReadRelativeHumidity(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V);
                Debug.Print("Humidity in percent: " + humidity.ToString("F2"));

                updateThingSpeak("field1=" + temperature.ToString() + "&field2=" + humidity.ToString("F2") + "&status=Temperature: " + temperature.ToString("F2") + "  Humidity: " + humidity.ToString("F2"));

                delayLoop(updateInterval);
            }
        }

        static void updateThingSpeak(string tsData)
        {
            Debug.Print("Connected to ThingSpeak...\n");
            led.Write(true);

            String request = "POST /update HTTP/1.1\n";
            request += "Host: api.thingspeak.com\n";
            request += "Connection: close\n";
            request += "X-THINGSPEAKAPIKEY: " + writeAPIKey + "\n";
            request += "Content-Type: application/x-www-form-urlencoded\n";
            request += "Content-Length: " + tsData.Length + "\n\n";

            request += tsData;

            try
            {
                String tsReply = sendPOST(tsIP, tsPort, request);
                Debug.Print(tsReply);
                Debug.Print("...disconnected.\n");
                led.Write(false);
            }
            catch (SocketException se)
            {
                Debug.Print("Connection Failed.\n");
                Debug.Print("Socket Error Code: " + se.ErrorCode.ToString());
                Debug.Print(se.ToString());
                Debug.Print("\n");
                led.Write(false);
            }
        }

        // Issues a http POST request to the specified server. (From the .NET Micro Framework SDK example)
        private static String sendPOST(String server, Int32 port, String request)
        {
            const Int32 c_microsecondsPerSecond = 1000000;

            // Create a socket connection to the specified server and port.
            using (Socket serverSocket = ConnectSocket(server, port))
            {
                // Send request to the server.
                Byte[] bytesToSend = Encoding.UTF8.GetBytes(request);
                serverSocket.Send(bytesToSend, bytesToSend.Length, 0);

                // Reusable buffer for receiving chunks of the document.
                Byte[] buffer = new Byte[1024];

                // Accumulates the received page as it is built from the buffer.
                String page = String.Empty;

                // Wait up to 30 seconds for initial data to be available.  Throws an exception if the connection is closed with no data sent.
                DateTime timeoutAt = DateTime.Now.AddSeconds(30);
                while (serverSocket.Available == 0 && DateTime.Now < timeoutAt)
                {
                    System.Threading.Thread.Sleep(100);
                }

                // Poll for data until 30-second timeout.  Returns true for data and connection closed.
                while (serverSocket.Poll(30 * c_microsecondsPerSecond, SelectMode.SelectRead))
                {
                    // If there are 0 bytes in the buffer, then the connection is closed, or we have timed out.
                    if (serverSocket.Available == 0) break;

                    // Zero all bytes in the re-usable buffer.
                    Array.Clear(buffer, 0, buffer.Length);

                    // Read a buffer-sized HTML chunk.
                    Int32 bytesRead = serverSocket.Receive(buffer);

                    // Append the chunk to the string.
                    page = page + new String(Encoding.UTF8.GetChars(buffer));
                }

                // Return the complete string.
                return page;
            }
        }

        // Creates a socket and uses the socket to connect to the server's IP address and port. (From the .NET Micro Framework SDK example)
        private static Socket ConnectSocket(String server, Int32 port)
        {
            // Get server's IP address.
            IPHostEntry hostEntry = Dns.GetHostEntry(server);

            // Create socket and connect to the server's IP address and port
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(hostEntry.AddressList[0], port));
            return socket;
        }

        static void delayLoop(int interval)
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            int offset = (int)(now % interval);
            int delay = interval - offset;
            Thread.Sleep(delay);
        }

      private static void InitSHT()
      {
        SHT11_GPIO_IOProvider SHT11_IO = new SHT11_GPIO_IOProvider(Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2);
        SHT11 = new SensirionSHT11(SHT11_IO);

        if (SHT11.SoftReset())
        {
            throw new Exception("Error while resetting SHT11");
        }

        // Set Temperature and Humidity to less acurate 12/8 bit
        if (SHT11.WriteStatusRegister(SensirionSHT11.SHT11Settings.LessAcurate))
        {
            throw new Exception("Error while writing status register SHT11");
        }

        Debug.Print("RAW Temperature 12-Bit: " + SHT11.ReadTemperatureRaw());
        Debug.Print("RAW Humidity 8-Bit: " + SHT11.ReadHumidityRaw());

        // Set Temperature and Humidity to more acurate 14/12 bit
        if (SHT11.WriteStatusRegister((SensirionSHT11.SHT11Settings.NullFlag)))
        {
            throw new Exception("Error while writing status register SHT11");
        }

        Debug.Print("RAW Temperature 14-Bit: " + SHT11.ReadTemperatureRaw());
        Debug.Print("RAW Humidity 12-Bit: " + SHT11.ReadHumidityRaw());
      }
    }
}