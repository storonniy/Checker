using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Checker.Auxiliary;
using Checker.Steps;
using static Checker.Auxiliary.UnitValuePair;


namespace Checker.Devices
{
    public abstract class IDeviceInterface
    {
        public abstract DeviceResult DoCommand(Step step);
        
        protected static DeviceResult SetValue(Step step, Func<double, double> setValue, UnitType unitType)
        {
            var value = double.Parse(step.Argument, CultureInfo.InvariantCulture);
            var result = setValue(value);
            return GetResultOfSetting($"{step.DeviceName}: установлено", unitType, result, value);
        }
        
        protected static DeviceResult SetValue(Step step, Func<double, int, double> setValue, UnitType unitType)
        {
            var channel = int.Parse(step.AdditionalArg);
            var value = double.Parse(step.Argument, CultureInfo.InvariantCulture);
            var result = setValue(value, channel);
            return GetResultOfSetting($"{step.DeviceName}: установлено", unitType, result, value);
        }
        

        protected static DeviceResult PowerOn(Step step, Action powerOn)
        {
            powerOn();
            return DeviceResult.ResultOk($"{step.DeviceName}: подан входной сигнал");
        }

        protected static DeviceResult PowerOff(Step step, Action powerOff)
        {
            powerOff();
            return DeviceResult.ResultOk($"{step.DeviceName}: снят входной сигнал");
        }

        protected static DeviceResult PowerOn(Step step, Func<bool> powerOn)
        {
            var status = powerOn();
            return status ? DeviceResult.ResultOk($"{step.DeviceName}: подан входной сигнал") : DeviceResult.ResultError($"{step.DeviceName}: ошибка при подаче входного сигнала");
        }

        protected static DeviceResult PowerOff(Step step, Func<bool> powerOff)
        {
            var status = powerOff();
            return status ? DeviceResult.ResultOk($"{step.DeviceName}: снят входной сигнал") : DeviceResult.ResultError($"{step.DeviceName}: ошибка при снятии входного сигнала");
        }

        public static DeviceResult GetResult(string message, Step step, UnitType unitType, double value)
        {
            var result = $"{message}: {new UnitValuePair(value, unitType)} \tНижний предел: {new UnitValuePair(step.LowerLimit, unitType)}\t Верхний предел {new UnitValuePair(step.UpperLimit, unitType)}";
            if (value >= step.LowerLimit && value <= step.UpperLimit)
                return DeviceResult.ResultOk(result);
            return DeviceResult.ResultError(result);
        }

        public static DeviceResult GetResultOfSetting(string message, UnitType unitType, double value, double expectedValue)
        {
            var result = $"{message}: {new UnitValuePair(value, unitType)}";
            return Math.Abs(value - expectedValue) <= 0.1 * Math.Abs(expectedValue) ? DeviceResult.ResultOk(result) : DeviceResult.ResultError(result);
        }

        protected static DeviceResult CloseRelays(Step step, Func<int[], bool> closeRelays)
        {
            var relayNumbers = GetRelayNumbersArray(step.Argument);
            var status = closeRelays(relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{step.DeviceName}: Реле {string.Join(", ", relayNumbers)} замкнуты успешно");
            return DeviceResult.ResultError($"{step.DeviceName}: При замыкании реле {string.Join(", ", relayNumbers)} произошла ошибка");
        }

        protected static DeviceResult CloseRelays(Step step, Func<int, int[], bool> closeRelays)
        {
            var relayNumbers = GetRelayNumbersArray(step.Argument);
            var blockNumber = int.Parse(step.AdditionalArg) - 1;
            var status = closeRelays(blockNumber, relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{step.DeviceName}{step.AdditionalArg} замкнуты реле {String.Join(", ", relayNumbers)}");
            return DeviceResult.ResultError($"ОШИБКА: {step.DeviceName}{step.AdditionalArg} не замкнуты реле {String.Join(", ", relayNumbers)}");
        }
        
        protected static DeviceResult OpenRelays(Step step, Func<int, int[], bool> openRelays)
        {
            var relayNumbers = GetRelayNumbersArray(step.Argument);
            var blockNumber = int.Parse(step.AdditionalArg) - 1;
            var status = openRelays(blockNumber, relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{step.DeviceName}{step.AdditionalArg} разомкнуты реле {String.Join(", ", relayNumbers)}");
            return DeviceResult.ResultError($"ОШИБКА: {step.DeviceName}{step.AdditionalArg} не разомкнуты реле {String.Join(", ", relayNumbers)}");
        }
        
        protected static DeviceResult OpenRelays(Step step, Func<int[], bool> openRelays)
        {
            var relayNumbers = GetRelayNumbersArray(step.Argument);
            var status = openRelays(relayNumbers);
            if (status)
                return DeviceResult.ResultOk($"{step.DeviceName}: Реле {string.Join(", ", relayNumbers)} разомкнуты успешно");
            return DeviceResult.ResultError($"{step.DeviceName}: При размыкании реле {string.Join(", ", relayNumbers)} произошла ошибка");
        }

        protected static DeviceResult OpenAllRelays(Step step, Func<bool> openAllRelays)
        {
            var status = openAllRelays();
            return status ? DeviceResult.ResultOk($"{step.DeviceName}: разомкнуты все реле") : DeviceResult.ResultError($"{step.DeviceName}: не удалось разомкнуть все реле");
        }
        
        public static int[] GetRelayNumbersArray(string relayNames)
        {
            return relayNames.Replace(" ", "").Split(',')
                .Select(int.Parse)
                .ToArray();
        }

        public static DeviceResult Measure(Step step, Func<double> measure, UnitType unitType)
        {
            var value = measure();
            if (step.Argument != "-")
            {
                var key = step.Argument;
                AddValues(key, value);
            }
            return GetResult($"Измерено", step, unitType, value);
        }
        
        public virtual void Die()
        {

        }

        #region Coefficient
        public struct InputData
        {
            public InputData(int channel, double inputValue)
            {
                this.channel = channel;
                this.inputValue = inputValue;
            }

            private int channel;
            private double inputValue;
        }

        private static readonly Dictionary<InputData, List<double>> CoefficientValuesDictionary = new ();

        public static readonly Action ClearCoefficientDictionary = () => CoefficientValuesDictionary.Clear();

        public static void AddCoefficientData(int channel, double expectedValue, double value)
        {
            if (channel <= 0) return;
            var inputData = new InputData(channel, expectedValue);
            if (!CoefficientValuesDictionary.ContainsKey(inputData))
            {
                CoefficientValuesDictionary.Add(inputData, new List<double> { value });
            }
            else
            {
                CoefficientValuesDictionary[inputData].Add(value);
            }
        }

        protected static List<double> GetCoefficientValues(int channel, double value)
        {
            var inputData = new InputData(channel, value);
            return CoefficientValuesDictionary[inputData];
        }

        #endregion

        #region GDM78261 Saving Values

        public static readonly Action<string, double> AddValues = (key, measuredValue) =>
            ValuesDictionary.SafeAdd(key, measuredValue);

        public static readonly Func<string, double> GetValue = (key) => ValuesDictionary[key];

        private static readonly Dictionary<string, double> ValuesDictionary = new Dictionary<string, double>();

        /// <summary>
        /// Возвращает библиотеку с сохраненными парами ключ-значение (измеренное GDM)
        /// </summary>
        /// <returns></returns>
        public static readonly Func<Dictionary<string, double>> GetValuesDictionary = () => ValuesDictionary;

        public static readonly Action ClearValuesDictionary = () => ValuesDictionary.Clear();

        #endregion
    }

    public static class DictionaryExtensions
    {
        public static void SafeAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
