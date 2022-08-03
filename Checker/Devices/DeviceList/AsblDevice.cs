using System;
using System.Collections.Generic;
using System.Linq;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Devices;
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

        public static int[] GetLineNumbers(string argument)
        {
            return argument.Replace(" ", "").Split(',')
                .Where(x => x != "")
                .Select(int.Parse)
                .ToArray();
        }

        public override DeviceResult DoCommand(Steps.Step step)
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
                        lineNumbers = GetLineNumbers(step.Argument);
                        asbl.ClearLineData(lineNumbers);
                        return ResultOk($"Линии {string.Join(", ", lineNumbers)} установлены в 0");
                    case DeviceCommands.GetLineState:
                        var lineNumber = int.Parse(step.Argument);
                        var lineStateN = int.Parse(step.AdditionalArg);
                        if (lineStateN != 1 && lineStateN != 0)
                            throw new Exception($"Невозможное логическое состояние {lineStateN}");
                        var lineState = lineStateN == 1;
                        var actualLineState = asbl.GetLineData(lineNumber);
                        var msg =
                            $"Линия {lineNumber} находится в состоянии лог. {(actualLineState ? 1 : 0)}, ожидалось состояние лог. {(lineState ? 1 : 0)}";
                        return lineState == actualLineState ? ResultOk(msg) : ResultError(msg);
                    case DeviceCommands.ClearAll:
                        asbl.ClearAll();
                        return ResultOk("Направление всех линий установлено на выход, всех состояния линий выставлены в 0");
                    case DeviceCommands.SetFrequency:
                        var frequency = int.Parse(step.Argument);
                        asbl.SetFrequency(frequency);
                        return ResultOk($"{step.DeviceName}: установлена частота {frequency} Гц");
                    case DeviceCommands.StartGenerator:
                        asbl.StartGenerator();
                        return ResultOk($"{step.DeviceName}: генерация частоты запущена");    
                    case DeviceCommands.StopGenerator:
                        asbl.StopGenerator();
                        return ResultOk($"{step.DeviceName}: генерация частоты остановлена");         
                    default:
                        return ResultError($"Неизвестная команда {step.Command}");
                }
            }
            catch (FailedToSetLineException ex)
            {
                return ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (LineIsSetToReceiveException ex)
            {
                return ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (AsblException ex)
            {
                return ResultError($"{step.DeviceName}: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
            catch (SerialPortDeviceException ex)
            {
                return ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
        }
    }
}
