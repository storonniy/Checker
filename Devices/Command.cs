using System.Collections.Generic;

namespace USA.Example
{
    public enum CommandEnum
    {
        SetCurrent,
        GetValue
    }

    public interface ICommand
    {
        CommandEnum GetCommandEnum();
        Result Run(AbstractDevice device, List<string> args);
    }

    public class Result
    {
        public string Status;
        public string Description;
    }
    
    public class CommandSetCurrent : ICommand {
        public CommandEnum GetCommandEnum()
        {
            return CommandEnum.SetCurrent;
        }

        public Result Run(AbstractDevice device, List<string> args)
        {
            device.SerialPort.WriteLine("SC " + args[0] + "\r");
            return new Result()
            {
                Status = "OK",
                Description = "Maybe"
            };
        }
    }
}