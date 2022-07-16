using System.IO.Ports;
using Checker.DeviceDrivers;
using Checker.DeviceInterface;
using Checker.Devices;
using Checker.Steps;

namespace Checker.Device.DeviceList
{
    internal class Keithley2401Device : IDeviceInterface
    {
        private readonly Keithley2401 keithley2401;

        public Keithley2401Device(SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            keithley2401 = new Keithley2401(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, keithley2401.SetVoltage);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(step, keithley2401.SetCurrentLimit);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, keithley2401.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, keithley2401.PowerOff);
                case DeviceCommands.SetVoltageSourceMode:
                    var status = keithley2401.SetVoltageSourceMode();
                    if (status)
                        return DeviceResult.ResultOk($"{step.DeviceName} переведен в режим стабилизации напряжения");
                    return DeviceResult.ResultError($"{step.DeviceName} не удалось перевести в режим стабилизации напряжения");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}