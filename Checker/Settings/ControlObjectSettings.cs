using System;
using System.ComponentModel;

namespace Checker.Settings
{
    public class PowerSupplyMode
    {

        public enum SupplyVoltageValue
        {
            Nominal,
            Min,
            Max
        }
    }

    public class ControlObjectSettings
    {
        public struct Settings
        {
            public Regime Regime { get; set; }
            public string Comment { get; set; }
            public string FactoryNumber { get; set; }
            public string OperatorName { get; set; }
            public string TableName { get; set; }
        }

        public static Regime GetRegimeAsEnum(string regimeName)
        {
            switch(regimeName)
            {
                case "Номинальный":
                    return Regime.Nominal;
                case "Минимальный":
                    return Regime.Min;
                case "Максимальный":
                    return Regime.Max;
                default:
                    throw new Exception($"Выбран неизвестный режим - {regimeName}");
            }
        }

        public static string GetRegimeAsString(Regime regime)
        {
            switch (regime)
            {
                case Regime.Max:
                    return "Максимальный";
                case Regime.Min:
                    return "Минимальный";
                case Regime.Nominal:
                    return "Номинальный";
                default:
                    throw new Exception($"Выбран неизвестный режим - {regime}");
            }
        }

        public enum Regime
        {
            [Description("Номинальный")]
            Nominal,
            [Description("Минимальный")]
            Min,
            [Description("Максимальный")]
            Max
        }

        public enum CheckingType
        {
            [Description("Самопроверка")]
            SelfChecking,
            [Description("Алгоритм")]
            Algorithm
        }

        public static string GetCheckingTypeAsString(CheckingType checkingType)
        {
            switch (checkingType)
            {
                case CheckingType.SelfChecking:
                    return "Самопроверка";
                case CheckingType.Algorithm:
                    return "Алгоритм";
                default:
                    throw new Exception($"Выбран неизвестный тип проверки - {checkingType}");
            }
        }
    }
}
