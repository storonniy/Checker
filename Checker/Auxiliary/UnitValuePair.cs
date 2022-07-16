using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker.Auxiliary
{
    public class UnitValuePair
    {
        private readonly UnitType unitType;
        private readonly double value;
        private string prefix;

        public UnitValuePair(double value, UnitType unitType)
        {
            this.value = value;
            this.unitType = unitType;
        }

        public override string ToString()
        {
            if (Math.Abs(value) * Math.Pow(10, 6) < 1000)
            {
                return $"{Math.Round(value * Math.Pow(10, 6), 3)} мк{unitType.GetUnit()}";
            }
            return Math.Abs(value) * Math.Pow(10, 3) < 1000 
                ? $"{Math.Round(value * Math.Pow(10, 3), 3)} м{unitType.GetUnit()}" 
                : $"{Math.Round(value, 3)} {unitType.GetUnit()}";
        }

        public enum UnitType
        {
            Voltage, 
            Current,
            Power,
            Frequency,
            Resistance
        }

        public static UnitType GetUnitType (string unitName)
        {
            switch (unitName)
            {
                case "V":
                    return UnitType.Voltage;
                case "A":
                    return UnitType.Current;
                case "W":
                    return UnitType.Power;
                case "Hz":
                    return UnitType.Frequency;
                case "Ohm":
                    return UnitType.Resistance;
                default:
                    throw new Exception("Неизвестный тип величины");
            }
        }

        public class ValueKeys
        {
            public UnitType UnitType { get;}
            public string[] Keys { get; }
            public ValueKeys(UnitType unitType, string[] keys)
            {
                UnitType = unitType;
                Keys = keys;
            }
        }
    }

    internal static class UnitTypeExtensions
    {
        internal static string GetUnit(this UnitValuePair.UnitType unitType)
        {
            switch (unitType)
            {
                case UnitValuePair.UnitType.Current:
                    return "А";
                case UnitValuePair.UnitType.Voltage:
                    return "В";
                case UnitValuePair.UnitType.Power:
                    return "Вт";
                case UnitValuePair.UnitType.Frequency:
                    return "Гц";
                case UnitValuePair.UnitType.Resistance:
                    return "Ом";
                default:
                    throw new Exception("Неизвестный тип величины");
            }
        }
    }
}
