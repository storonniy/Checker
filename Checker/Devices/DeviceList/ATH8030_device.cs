using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Device;
using Checker.DeviceDrivers.Aktakom;
using static Checker.Auxiliary.UnitValuePair;


namespace Checker.Devices
{
    class ATH8030_device : IDeviceInterface
    {
        readonly Ath8030 ath8030;  

        public ATH8030_device (string portName)
        {
            ath8030 = new Ath8030(portName);
        }

        public override DeviceResult DoCommand(Steps.Step step)
        {
            var lowerLimit = step.LowerLimit;
            var upperLimit = step.UpperLimit;
            switch (step.Command)
            {
                case DeviceCommands.PowerOff:
                    if (ath8030.PowerOff())
                        return DeviceResult.ResultOk($"Снятие входного сигнала с {step.DeviceName} прошло успешно");
                    else
                        return DeviceResult.ResultError($"ОШИБКА: Снятие входного сигнала с {step.DeviceName} завершилось с ошибкой");
                case DeviceCommands.PowerOn:
                    return IDeviceInterface.PowerOn(step, ath8030.PowerOn);
/*                    if (ath8030.PowerOn())
                        return DeviceResult.ResultOk($"Подан входной сигнал с {deviceData.DeviceName}");
                    else
                        return DeviceResult.ResultError($"Ошибка: попытка подачи входного сигнала с {deviceData.DeviceName} завершилась с ошибкой");*/
                case DeviceCommands.SetCurrent:
                    var current = float.Parse(step.Argument);
                    var fData = BitConverter.GetBytes(current);
                    var resultCurrent = $"Уcтановлено значение тока {new UnitValuePair(current, UnitValuePair.UnitType.Current)} \t Нижний предел: {new UnitValuePair(lowerLimit, UnitValuePair.UnitType.Current)}\t Верхний предел {new UnitValuePair(upperLimit, UnitType.Current)}";
                    if (ath8030.SetCurrent(current))
                        return DeviceResult.ResultOk(resultCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: не удалось установить сигнал {new UnitValuePair(current, UnitValuePair.UnitType.Current)}");
                case DeviceCommands.SetMaxCurrent:
                    var maxCurrent = float.Parse(step.Argument);
                    var resultMaxCurrent = $"Уcтановлено максимальное значение тока {new UnitValuePair(maxCurrent, UnitValuePair.UnitType.Current)} \t Нижний предел: {new UnitValuePair(lowerLimit, UnitValuePair.UnitType.Current)}\t Верхний предел {new UnitValuePair(upperLimit, UnitValuePair.UnitType.Current)}";
                    if (ath8030.SetMaxCurrent(maxCurrent))
                        return DeviceResult.ResultOk(resultMaxCurrent);
                    else
                        return DeviceResult.ResultError($"Ошибка: {resultMaxCurrent}");
                case DeviceCommands.SetCurrentControlMode:
                    if (ath8030.SetCurrentControlMode())
                    {
                        return DeviceResult.ResultOk($"Устройство {step.DeviceName} установлено в режим стабилизации тока");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"Ошибка: устройство {step.DeviceName} не установлено в режим стабилизации тока");
                    }

                case DeviceCommands.GetCurrentDC:
                    var actualCurrent = ath8030.GetConstantCurrent();
                    return GetResult("Измерено значение тока", step, UnitValuePair.UnitType.Current, actualCurrent);

                case DeviceCommands.GetLoadCurrent:                  
                    var actualLoadCurrent = ath8030.GetLoadCurrent();
                    return GetResult("Измерено значение тока", step, UnitValuePair.UnitType.Current, actualLoadCurrent);
                case DeviceCommands.GetMaxCurrent:
                    var maxCurrent1 = ath8030.GetMaxCurrent();
                    return GetResult("Измерен ток", step, UnitValuePair.UnitType.Current, maxCurrent1);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}
