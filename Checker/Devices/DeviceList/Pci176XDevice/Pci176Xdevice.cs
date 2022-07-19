using Advantech;
using Checker.DeviceDrivers;
using Checker.Devices;
using static Checker.Devices.DeviceResult;

namespace Checker.Device.DeviceList
{
    public class Pci176XDevice<T> : IDeviceInterface where T : Pci176X
    {
        private readonly T pci176X;

        protected Pci176XDevice(T pci176X)
        {
            this.pci176X = pci176X;
        }

        public override DeviceResult DoCommand(Steps.Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.CloseRelays:
                    return CloseRelays(step, pci176X.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(step, pci176X.OpenRelays);
                case DeviceCommands.ReadPCI1762Data:
                    var port = int.Parse(step.Argument);
                    var signal = int.Parse(step.AdditionalArg);
                    var portByte = pci176X.Read(port);
                    return portByte == (byte) signal
                        ? ResultOk($"Сигнал {portByte} присутствует")
                        : ResultError($"Ошибка: сигнал {portByte} отсутствует");
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(step, pci176X.OpenAllRelays);
                case DeviceCommands.GetClosedRelayNames:
                    return ResultOk($"{step.DeviceName}: {string.Join(", ", pci176X.GetClosedRelaysNumbers())}");
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}