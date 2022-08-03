using System.Linq;
using Advantech;
using Checker.Steps;
using static Checker.Devices.DeviceResult;

namespace Checker.Devices.DeviceList
{
    public class Pci1751Device : IDeviceInterface
    {
        private readonly Pci1751 pci1751;

        public Pci1751Device(string description)
        {
            pci1751 = new Pci1751(description);
        }
        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetSignal:
                    var status = pci1751.SetSignal(GetSignalNames(step.Argument));
                    if (status)
                        return ResultOk($"{step.DeviceName}: установлены сигналы {step.Argument} в 1");
                    return ResultError($"{step.DeviceName}: произошла ошибка при установке сигналов {step.Argument} в 1");
                case DeviceCommands.ClearSignal:
                    var stat = pci1751.ClearSignal(GetSignalNames(step.Argument));
                    if (stat)
                        return ResultOk($"{step.DeviceName}: установлены сигналы {step.Argument} в 0");
                    return ResultError($"{step.DeviceName}: произошла ошибка при установке сигналов {step.Argument} в 0");
                case DeviceCommands.GetSignals:
                    var expectedSignals = GetSignalNames(step.Argument);
                    var signals = pci1751.GetSignals();
                    var result = expectedSignals.All(s => signals.Contains(s));
                    return ResultOk(string.Join(", ", signals));
                case DeviceCommands.ClearAllSignals:
                    var st = pci1751.ClearAllSignals();
                    return st ? ResultOk($"{step.DeviceName}: все сигналы установлены в 0") : ResultError($"{step.DeviceName}: при установке всех сигналов в 0 произошла ошибка");
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }

        private static string[] GetSignalNames(string signalsLine)
        {
            return signalsLine.Replace(" ", "").Split(',');
        }
    }
}