using System;
using Microsoft.SPOT;

namespace Netduino.Home.Controller
{
    class Config
    {
        ////Ethernet Settings 
        public const string HostAddress = "192.168.250.101";
        public const int Port = 8000;
        public const string NetduinoStaticIPAddress = "192.168.250.104";
        public const string SubnetMask = "255.255.255.0";
        public const string GatewayAddress = "192.168.250.1";
    }
}
