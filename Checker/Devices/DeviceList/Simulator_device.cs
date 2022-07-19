using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Checker.DeviceDrivers;
using Checker.Devices;

namespace Checker.Device.DeviceList
{
    class Simulator_device : IDeviceInterface
    {
        readonly Simulator Simulator;

        public Simulator_device(SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            Simulator = new Simulator(serialPort);
        }

        /// дописать OpenAllRelays
        public override DeviceResult DoCommand(Steps.Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.GetClosedRelayNames:
                    var relays = Simulator.GetClosedRelayNames();
                    return DeviceResult.ResultOk($"\nSimulator: {string.Join(", ", relays)}");
                case DeviceCommands.CloseRelays:
                    return CloseRelays(step, Simulator.CloseRelays);
                case DeviceCommands.OpenRelays:
                    return OpenRelays(step, Simulator.OpenRelays);
                case DeviceCommands.OpenAllRelays:
                    return OpenAllRelays(step, Simulator.OpenAllRelays);
                default:
                    return DeviceResult.ResultError($"{step.DeviceName} Неизвестная команда {step.Command}");
            }
        }
    }
}
