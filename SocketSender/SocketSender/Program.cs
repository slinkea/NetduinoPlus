using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;
using NetLogger;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System;

namespace NDP_SocketSender1
{
    public class Program
    {
        // initialize the onboard LED
        private static OutputPort ledPort = new OutputPort(Pins.ONBOARD_LED, false);

        public static void Main()
        { 
            // initialize the onboard switch
            InterruptPort switchPort = new InterruptPort(Pins.ONBOARD_SW1, false,
                                                         Port.ResistorMode.Disabled,
                                                         Port.InterruptMode.InterruptEdgeLow);
            // add an event-handler for the switch
            switchPort.OnInterrupt += new NativeEventHandler(switchPort_OnInterrupt);

            // wait forever...
            Thread.Sleep(Timeout.Infinite);
        }

        ///
        /// Event handler for the onboard switch.
        ///
        ///The port for which the event occurs.
        ///The state of the switch.
        ///Time of the event.
        private static void switchPort_OnInterrupt(uint port, uint data, DateTime time)
        {
          // turn the LED on, if the button is pressed (== 0, because switch is inverted)
          ledPort.Write(data == 0);


          NetLog log1 = new NetLog();

          log1.Host = "192.168.250.1";
          log1.Port = 8000;

          log1.Print("100");
          Thread.Sleep(500);
          log1.Print("400");
          Thread.Sleep(500);
          log1.Print("200");
          Thread.Sleep(500);
          log1.Print("500");
          Thread.Sleep(500);
          log1.Print("300");

          ledPort.Write(data == 1);
        }
    }
}
