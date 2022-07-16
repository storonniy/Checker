using System;
using System.IO;
using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using System.Threading;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Steps;
using static Checker.Auxiliary.UnitValuePair;
using Checker.Device;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    class PSP405_device : IDeviceInterface
    {
        readonly int delay = 500;
        readonly Psp405 psp405;

        public PSP405_device(SerialPort serialPort)
        {
            psp405 = new Psp405(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, psp405.SetVoltage);
                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(step);
                    return GetResult($"{step.DeviceName}: Установлен ток", step, UnitValuePair.UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, psp405.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, psp405.PowerOff);
                case DeviceCommands.SetCurrentLimit:
                    return SetCurrentLimit(step, psp405.SetCurrentLimit);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }

        public double SetCurrent(Step step)
        {
            psp405.SetVoltageLimit(40);
            psp405.SetCurrentLimit(step.UpperLimit);
            double current = Math.Abs(double.Parse(step.Argument));
            Thread.Sleep(delay);
            double resistance = 480000.0;
            double voltage = current * resistance;
            Thread.Sleep(delay);
            psp405.SetVoltage(voltage);
            Thread.Sleep(delay);
            var volt = psp405.GetOutputVoltage();
            var actualCurrent = volt / resistance;
            return actualCurrent;
        }
    }
}