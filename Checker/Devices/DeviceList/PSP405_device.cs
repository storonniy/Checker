using System;
using System.IO;
using System.IO.Ports;
using static Checker.Devices.DeviceResult;
using System.Threading;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using static Checker.Auxiliary.UnitValuePair;
using Checker.Device;

namespace Checker.Devices
{
    class Psp405Device : IDeviceInterface
    {
        private const int Delay = 500;
        private readonly Psp405 psp405;

        public Psp405Device(SerialPort serialPort)
        {
            psp405 = new Psp405(serialPort);
        }

        public override DeviceResult DoCommand(Steps.Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetValue(step, psp405.SetVoltage, UnitType.Voltage);
                case DeviceCommands.SetCurrent:
                    var actualCurrent = SetCurrent(step);
                    return GetResult($"{step.DeviceName}: Установлен ток", step, UnitValuePair.UnitType.Current, actualCurrent);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, psp405.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, psp405.PowerOff);
                case DeviceCommands.SetCurrentLimit:
                    return SetValue(step, psp405.SetCurrentLimit, UnitType.Current);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }

        public double SetCurrent(Steps.Step step)
        {
            psp405.SetVoltageLimit(40);
            psp405.SetCurrentLimit(step.UpperLimit);
            double current = Math.Abs(double.Parse(step.Argument));
            Thread.Sleep(Delay);
            double resistance = 480000.0;
            double voltage = current * resistance;
            Thread.Sleep(Delay);
            psp405.SetVoltage(voltage);
            Thread.Sleep(Delay);
            var volt = psp405.GetOutputVoltage();
            var actualCurrent = volt / resistance;
            return actualCurrent;
        }
    }
}