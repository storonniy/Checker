using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UCA.Devices;

namespace USA.Example
{
    public enum DeviceEnum
    {
        Controller,
        PSP_405,
        Commutator,
        GDM_78261,
        Keysight_34410,
        None
    }
    
    public abstract class AbstractDevice
    {
        public static readonly Dictionary<DeviceEnum, Func<SerialPort, AbstractDevice>> DeviceFactory =
            new Dictionary<DeviceEnum, Func<SerialPort, AbstractDevice>>()
            {
                { DeviceEnum.Controller, serialport => new ControllerAbstractDevice(serialport) }
            };
        // Создать экземпляр порта - DeviceFactory[DeviceEnum.Controller](serialPort);

        private static readonly Dictionary<CommandEnum, ICommand> DictionaryFactory = new List<ICommand>()
            {
                new CommandSetCurrent()
            }
            .ToDictionary(c => c.GetCommandEnum(), c => c);

        public SerialPort SerialPort { get; private set; }

        public Dictionary<CommandEnum, ICommand> CommandDict { get; private set; }

        protected AbstractDevice(SerialPort serialPort, params CommandEnum[] commands)
        {
            this.SerialPort = serialPort;
            this.CommandDict = DictionaryFactory
                .Where(p => commands.Contains(p.Key))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        bool IsValidCommand(CommandEnum commandEnum)
        {
            return CommandDict.ContainsKey(commandEnum);
        }

        Result Run(CommandEnum commandEnum, List<string> args)
        {
            if (!IsValidCommand(commandEnum))
            {
                throw new NotImplementedException("wrong command");
            }

            var command = CommandDict[commandEnum];

            return command.Run(this, args);
        }
    }

    public class ControllerAbstractDevice : AbstractDevice
    {
        public long CurrentValue;

        public ControllerAbstractDevice(SerialPort serialPort) : base(serialPort, CommandEnum.SetCurrent)
        {
        }
    }
}