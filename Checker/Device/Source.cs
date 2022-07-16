using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCA.Devices;
using static UCA.Auxiliary.UnitValuePair;

namespace UPD.Device
{
    public abstract class Source : IDeviceInterface
    {
        public static string message = "Установлено";
        public abstract double SetVoltage(DeviceData deviceData);
        public abstract double SetCurrent(DeviceData deviceData);
        public abstract double SetCurrentLimit(DeviceData deviceData);
        public abstract void PowerOn();
        public abstract void PowerOff();

        public DeviceResult GetResult(DeviceData deviceData, UnitType unitType, double value)
        {
            return GetResult(message, deviceData, unitType, value);
        }
    }
}
