using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using static UCA.Auxiliary.UnitValuePair;

namespace UPD.Device
{
    public abstract class Multimeter : IDeviceInterface
    {
        public abstract double GetVoltage();
        public abstract double GetCurrent();
        public static string message = "Измерено";

        public DeviceResult GetResult(DeviceData deviceData, UnitType unitType, double value)
        {
            return GetResult(message, deviceData, unitType, value);
        }
    }
}
