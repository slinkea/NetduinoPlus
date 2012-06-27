using System;
using Microsoft.SPOT;

namespace Netduino.Controller
{
    class Config
    {
        ////Ethernet Settings 
        public const string HostAddress = "192.168.137.1";
        public const int Port = 8000;
        public const string NetduinoStaticIPAddress = "192.168.137.2";
        public const string SubnetMask = "255.255.255.0";
        public const string GatewayAddress = "192.168.137.1";
    }
}
