using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using Checker.Steps;
using Checker.DeviceInterface;
using Instek;

namespace Checker.Devices
{
    public class Pst3201Device : IDeviceInterface
    {
        private readonly Pst3201 pst3201;
        
        public Pst3201Device(SerialPort serialPort)
        {
            pst3201 = new Pst3201(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, pst3201.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrent(step, pst3201.SetCurrentLimit);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, pst3201.PowerOff);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, pst3201.PowerOn);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}