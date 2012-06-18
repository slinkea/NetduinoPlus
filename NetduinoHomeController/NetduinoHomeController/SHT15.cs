using System;
using Microsoft.SPOT;
using ElzeKool.io.sht11_io;
using SecretLabs.NETMF.Hardware.Netduino;
using ElzeKool.io;

namespace Netduino.Home.Controller
{
  class SHT15
  {
    #region Private Variables
    
    private static SHT11_GPIO_IOProvider SHT11_IO = new SHT11_GPIO_IOProvider(Pins.GPIO_PIN_D1, Pins.GPIO_PIN_D2);
    private static SensirionSHT11 SHT11 = new SensirionSHT11(SHT11_IO);

    #endregion
    

    #region Constructors
    
    //This keeps other classes from creating an instance
    private SHT15()
    {
    }

    #endregion


    #region Public Static Methods

    public static void Init()
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

    public static double GetTemperatureCelcius()
    {
      return SHT11.ReadTemperature(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V, SensirionSHT11.SHT11TemperatureUnits.Celcius);
    }

    public static double GetHumidity()
    {
      return SHT11.ReadRelativeHumidity(SensirionSHT11.SHT11VDD_Voltages.VDD_3_5V);
    }

    #endregion
  }
}
