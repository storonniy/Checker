using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using Checker.Steps;
using Checker.DeviceInterface;
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

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, psh73610.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(step, psh73610.SetCurrentLimit);
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
