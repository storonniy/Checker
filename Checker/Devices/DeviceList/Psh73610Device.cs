using System.IO.Ports;
using Checker.Auxiliary;
using static Checker.Devices.DeviceResult;
using static Checker.Auxiliary.UnitValuePair;
using Instek;

namespace Checker.Devices
{
    internal class Psh73610Device : IDeviceInterface
    {
        private readonly Psh73610 psh73610;

        public Psh73610Device(SerialPort serialPort)
        {
            psh73610 = new Psh73610(serialPort);
        }

        public override DeviceResult DoCommand(Steps.Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetValue(step, psh73610.SetVoltage, UnitType.Voltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetValue(step, psh73610.SetCurrentLimit, UnitType.Current);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, psh73610.PowerOff);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, psh73610.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
