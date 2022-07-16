using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Checker.Auxiliary.UnitValuePair;
using System.Threading;
using Checker.Auxiliary;
using Checker.Steps;
using Checker.Device;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    public class None : IDeviceInterface
    {
        private double CalculateUCACoefficient(int channel, double value)
        {
            var valuesAtZero = GetCoefficientValues(channel, 0);
            var values = GetCoefficientValues(channel, value);
            var coeff = (values[1] - valuesAtZero[1]) / (values[0] - valuesAtZero[0]);
            switch (channel)
            {
                case 1:
                case 2:
                    return coeff;//Math.Abs(coeff * 672000.0 / 1000000.0);
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return Math.Abs(coeff / 1000000.0);
                default:
                    throw new Exception($"Номер канала должен быть от 1 до 10 для УСА, указан номер канала {channel}");
            }
        }

        public override DeviceResult DoCommand(Step step)
        {
            //try
            //{
                switch (step.Command)
                {
                    case DeviceCommands.CalculateCoefficient:
                        {
                            var value = double.Parse(step.Argument, NumberStyles.Float);
                            var lowerLimit = step.LowerLimit;
                            var upperLimit = step.UpperLimit;
                            try
                            {
                                var actualCoefficient = CalculateUCACoefficient(step.Channel, value);
                                var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
                                if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                                    return DeviceResult.ResultOk(result);
                                return DeviceResult.ResultError($"Ошибка: {result}");
                            }
                            catch (KeyNotFoundException)
                            {
                                var unitType = (step.Channel > 2) ? UnitValuePair.UnitType.Current : UnitValuePair.UnitType.Voltage;
                                var data = $"lowerLimit {step.LowerLimit}; upperLimit {step.UpperLimit}";
                                return DeviceResult.ResultError($"{data} \n Для входного воздействия {new UnitValuePair(value, unitType)} и канала {step.Channel} не измерялись входные и выходные воздействия");
                            }
                        }
                    case DeviceCommands.CalculateCoefficient_UCAT:
                        return GetCoefficient_UCAT(step);
                    case DeviceCommands.Sleep:
                        var timeInSeconds = int.Parse(step.Argument);
                        var t = TimeSpan.FromSeconds(timeInSeconds);
                        var startTime = DateTime.Now;
                        while (DateTime.Now - startTime < t)
                        {
                            Thread.Sleep(1000);
                        }
                        return DeviceResult.ResultOk("");
                    case DeviceCommands.Divide:
                        {
                            var keys = GetKeys(step.Argument);
                            var value = Divide(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            return GetResult("Получено значение", step, keys.UnitType, value);
                        }
                    case DeviceCommands.Substract:
                        {
                            var keys = GetKeys(step.Argument);
                            var value = Substract(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            return GetResult("Получено значение", step, keys.UnitType, value);
                        }
                    case DeviceCommands.Save:
                        {
                            var keys = GetKeys(step.Argument);
                            var value = double.Parse(step.AdditionalArg, CultureInfo.InvariantCulture);
                            AddValues(keys.Keys[0], value);
                            var result = $"Сохранено значение {new UnitValuePair(value, keys.UnitType)}";
                            return DeviceResult.ResultOk(result);
                        }
                    case DeviceCommands.MultiplyAndSave:
                        {
                            var keys = GetKeys(step.Argument);
                            var value = Multiply(GetValue(keys.Keys[0]), GetValue(keys.Keys[1]));
                            var keyToSave = step.AdditionalArg;
                            AddValues(keyToSave, value);
                            return GetResult("Получено значение", step, keys.UnitType, value);
                        }
                    default:
                        return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
                }
            //}
/*            catch (KeyNotFoundException)
            {
                return DeviceResult.ResultError($"ОШИБКА: {deviceData.Command}: Ключ не найден");
            }*/
        }

        public static double Divide(double arg1, double arg2)
        {
            return arg1 / arg2;
        }

        public static double Multiply(double arg1, double arg2)
        {
            return arg1 * arg2;
        }

        private static double Substract(double arg1, double arg2)
        {
            return arg1 - arg2;
        }

        public static UnitValuePair.ValueKeys GetKeys(string rawArgument)
        {
            rawArgument = rawArgument.Replace(" ", "").Replace("\r", "");
            int unitIndex = rawArgument.IndexOf(';');
            string unitName = rawArgument.Substring(unitIndex + 1);
            UnitValuePair.UnitType unitType = GetUnitType(unitName);
            var keyString = rawArgument.Remove(unitIndex);
            string[] keys = keyString.Split(',');
            return new UnitValuePair.ValueKeys(unitType, keys);
        }

        private DeviceResult GetCoefficient_UCAT(Step step)
        {
            var value = double.Parse(step.Argument, NumberStyles.Float);
            var lowerLimit = step.LowerLimit;
            var upperLimit = step.UpperLimit;
            var actualCoefficient = CalculateCoefficient_UCAT(step.Channel, value);
            var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
            if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                return DeviceResult.ResultOk(result);
            else
                return DeviceResult.ResultError($"Ошибка: {result}");
/*            try
            {
                var actualCoefficient = CalculateCoefficient_UCAT(deviceData.Channel, value);
                var result = $"Коэффициент равен {string.Format("{0:0.000}", actualCoefficient)} В/мкА \tНижний предел  {lowerLimit} В/мкА \tВерхний предел {upperLimit} В/мкА";
                if (actualCoefficient >= lowerLimit && actualCoefficient <= upperLimit)
                    return DeviceResult.ResultOk(result);
                else
                    return DeviceResult.ResultError($"Ошибка: {result}");
            }
            catch (KeyNotFoundException)
            {
                UnitType unitType = (deviceData.Channel > 2) ? UnitType.Current : UnitType.Voltage;
                var data = $"lowerLimit {deviceData.LowerLimit}; upperLimit {deviceData.UpperLimit}";
                return DeviceResult.ResultError($"{data} \n Для входного воздействия {GetValueUnitPair(value, unitType)} и канала {deviceData.Channel} не измерялись входные и выходные воздействия");
            }*/
        }

        private double CalculateCoefficient_UCAT(int channel, double value)
        {
            var valuesAtZero = GetCoefficientValues(channel, 0);
            var values = GetCoefficientValues(channel, value);
            var meow = (values[0] - Math.Abs(valuesAtZero[0])) / value;
            return meow;    
        }
    }
}
