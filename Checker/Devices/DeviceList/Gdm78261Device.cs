using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Devices;
using Checker.Steps;

namespace Checker.Device.DeviceList
{
    internal class Gdm78261Device : IDeviceInterface
    {
        private readonly Gdm78261 gdm78261;

        public Gdm78261Device(SerialPort serialPort)
        {
            gdm78261 = new Gdm78261(serialPort);
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.GetResistance:
                    return Measure(step, gdm78261.MeasureResistance, UnitValuePair.UnitType.Resistance);
                case DeviceCommands.GetVoltageDC:
                    return Measure(step, gdm78261.MeasureVoltageDC, UnitValuePair.UnitType.Voltage);
                case DeviceCommands.GetVoltageAC:
                    return Measure(step, gdm78261.MeasureVoltageAC, UnitValuePair.UnitType.Voltage);
                case DeviceCommands.GetCurrentDC:
                    return Measure(step, gdm78261.MeasureCurrentDC, UnitValuePair.UnitType.Current);
                case DeviceCommands.SetMeasurementToCurrent:
                    var currentRange = double.Parse(step.Argument, CultureInfo.InvariantCulture);
                    gdm78261.SetMeasurementToCurrentDC(currentRange);
                    Thread.Sleep(1000);
                    return DeviceResult.ResultOk($"{step.DeviceName} переведен в режим измерения тока");
                case DeviceCommands.SetMeasurementToVoltageAC:
                    gdm78261.SetMeasurementToVoltageAC();
                    Thread.Sleep(1000);
                    return DeviceResult.ResultOk($"{step.DeviceName} переключён в режим измерения переменного напряжения");
                case DeviceCommands.SetMeasurementToVoltageDC:
                    gdm78261.SetMeasurementToVoltageDC();
                    Thread.Sleep(1000);
                    return DeviceResult.ResultOk($"{step.DeviceName} переключён в режим измерения постоянного напряжения");
                case DeviceCommands.SetMeasurementToResistance:
                    gdm78261.SetMeasurementToResistance();
                    Thread.Sleep(1000);
                    return DeviceResult.ResultOk($"{step.DeviceName} переключён в режим измерения сопротивления");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}