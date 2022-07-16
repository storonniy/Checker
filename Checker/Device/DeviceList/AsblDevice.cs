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
    public class AsblDevice : IDeviceInterface
    {
        readonly Asbl asbl;
        public AsblDevice()
        {
            asbl = new Asbl();
        }

        public static uint[] GetLineNumbers(string argument)
        {
            var lineNumbers = argument.Trim().Split(';');
            var uintLineNumbers = new List<uint>();
            for (int i = 0; i < lineNumbers.Length; i++)
            {
                if (lineNumbers[i] != "")
                    uintLineNumbers.Add(uint.Parse(lineNumbers[i]));
            }
            return uintLineNumbers.ToArray();
        }

        public override DeviceResult DoCommand(Step step)
        {
            try
            {
                switch (step.Command)
                {
                    case DeviceCommands.SetLineDirToOutput:
                        var lineNumbers = GetLineNumbers(step.Argument);
                        asbl.SetLineDirection(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены на выход");
                    case DeviceCommands.SetLineDirToInput:
                        lineNumbers = GetLineNumbers(step.Argument); 
                        asbl.ClearLineDirection(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены на вход");
                    case DeviceCommands.SetLineData:
                        lineNumbers = GetLineNumbers(step.Argument);
                        asbl.SetLineData(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены в 1");
                    case DeviceCommands.ClearLineData:
                        try
                        {
                            lineNumbers = GetLineNumbers(step.Argument);
                            asbl.ClearLineData(lineNumbers);
                            return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены в 0");
                        }
                        catch (FailedToSetLineException ex)
                        {
                            return ResultError(ex.Message);
                        }
                    case DeviceCommands.GetLineState:
                        var lineNumber = (uint)int.Parse(step.Argument);
                        var lineState = bool.Parse(step.AdditionalArg);
                        var actualLineState = asbl.GetLineData(lineNumber);
                        var msg =
                            $"Линия {lineNumber} находится в состоянии лог. {(actualLineState ? 1 : 0)}, ожидалось состояние лог. {(lineState ? 1 : 0)}";
                        return lineState == actualLineState ? ResultOk(msg) : ResultError(msg);
                    case DeviceCommands.ClearAll:
                        asbl.ClearAll();
                        return ResultOk("");
                    default:
                        return ResultError($"Неизвестная команда {step.Command}");
                }
            }
            catch (FailedToSetLineException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (LineIsSetToReceiveException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (AsblException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
        }
    }
}
