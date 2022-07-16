using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.DeviceDrivers;
using Checker.DeviceInterface;
using Checker.Devices;
using Checker.Steps;
using static Checker.Devices.DeviceResult;

namespace Checker.Device.DeviceList
{
    public class MK_device : IDeviceInterface
    {
        private readonly MK mk;
        public MK_device()
        {
            mk = new MK();
        }

        public override void Die()
        {
            mk.Die();
        }

        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.GetClosedRelayNames:
                    var closedRelays = mk.GetClosedRelayNames();
                    return ResultOk(string.Join("\n", closedRelays));
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(step, mk.EmergencyBreak);
                case DeviceCommands.CloseRelays:
                    return CloseRelays(step, mk.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(step, mk.OpenRelays);
                default:
                    return ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
