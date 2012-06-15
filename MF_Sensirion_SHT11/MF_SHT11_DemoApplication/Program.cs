/**
 * .NET Micro Framework 3.0 Demo Application for the Sensirion SHT11 Class
 *
 * (C)opyright 2008 Elze Kool, http://www.microframework.nl
 * 
 * This sourcecode is provided AS-IS. I take no responsibility for direct or indirect
 * damage coused by this program/class. 
 * 
 * You are free to use this class Non-Commercialy and Commercialy as long as you add the above
 * copyright as reference in your sourcecode.
 * 
 **/

using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using ElzeKool.io;
using ElzeKool.io.sht11_io;

using SecretLabs.NETMF.Hardware.NetduinoPlus;


namespace MF_SHT11_DemoApplication
{
    public class Program
    {
        public static void Main()
        {
            // First create IO Provider
            // Data: GPIO_PIN_D1
            // Clock: GPIO_PIN_D2
            SHT11_GPIO_IOProvider SHT11_IO = new SHT11_GPIO_IOProvider(Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2);


            // Create SHT11 Interface with the IO provider we've just created
            SensirionSHT11 SHT11 = new SensirionSHT11(SHT11_IO);

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

            // Now enter a loop, constantly outputing Temperature and Humidity
            while (true)
            {
                // Read Temperature with SHT11 VDD = +/- 3.5V and in Celcius
                double Temperature = SHT11.ReadTemperature(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V, SensirionSHT11.SHT11TemperatureUnits.Celcius);
                Debug.Print("Temperature Celcius: " + Temperature.ToString());

                // Read Humidity with SHT11 VDD = +/- 3.5V 
                double Humidity = SHT11.ReadRelativeHumidity(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V);
                Debug.Print("Humidity in percent: " + Humidity.ToString());

                // Wait a little
                Thread.Sleep(100);
            }
        }

    }
}
