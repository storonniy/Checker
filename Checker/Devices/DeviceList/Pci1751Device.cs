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