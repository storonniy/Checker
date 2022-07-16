using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
namespace Checker.DeviceDrivers
{
    public class Value
    {
        public string Flag { get; set; }
        public int Length { get; set; }

        public static readonly Value voltage = new Value() { Flag = "V", Length = 5 };
        public static readonly Value voltageLimit = new Value() { Flag = "U", Length = 2 };
        public static readonly Value current = new Value() { Flag = "A", Length = 5 };
        public static readonly Value currentLimit = new Value() { Flag = "I", Length = 4 };
        public static readonly Value power = new Value() { Flag = "W", Length = 5 };
        public static readonly Value powerLimit = new Value() { Flag = "P", Length = 3 };
    };

    public class Psp405 
    {
        readonly SerialPort serialPort;
        public Psp405 (SerialPort serialPort)
        {

            this.serialPort = serialPort;
           // this.serialPort.Open();
        }

        ~Psp405()
        {
            serialPort.Close();
        }


        public class Status
        {
            public bool RelayStatus;
            public bool TemperatureStatus;
            public bool StepMode;
            public bool ScrollWheelStatus;
            public bool RemoteStatus;
            public bool LockStatus;
        }

        /// <summary>
        /// Returns the status of the power supply PSP-405.
        /// </summary>
        /// <returns> RelayStatus (false: On, true: on)\</returns>
        /// <returns> TemperatureStatus (false: normal, true: overtemp) </returns>
        /// <returns> StepMode (false: normal, true: fine) </returns>
        /// <returns> ScrollWheelStatus (false: locked, true: unlocked) </returns>
        /// <returns> RemoteStatus (false: normal, true: remote) </returns>
        /// <returns> LockStatus (false: unlocked, true: locked) </returns>
        public Status GetStatus()
        {
            serialPort.SendCommand("F\r");
            string data = serialPort.ReadExisting();
            return GeneratePowerSupplyStatusData(data);
        }

        /// <summary>
        /// Sets the step resolution to fine mode.
        /// </summary>
        public void StepResolutionToFineMode()
        {
            serialPort.SendCommand("KF \r");
        }

        /// <summary>
        /// Sets the step resolution to normal (coarse) mode.
        /// </summary>
        public void StepResolutionToNormalMode()
        {
            serialPort.SendCommand("KN \r");
        }

        public static Status GeneratePowerSupplyStatusData(string data)
        {
            var status = new Status();
            string flag = "F";
            if (!data.StartsWith(flag))
                throw new Exception("PSP-405 returned an incorrect response to the request.");
            data = data.Replace(flag, "").Replace("\n", "").Replace("\r", "");
            var arrayData = data.ToCharArray();
            for (var i = 0; i < arrayData.Length; i++)
            {
                var state = (int)arrayData[i] - 48 == 1;
                switch (i)
                {
                    case 0:
                        status.RelayStatus = state;
                        break;
                    case 1:
                        status.TemperatureStatus = state;
                        break;
                    case 2:
                        status.StepMode = state;
                        break;
                    case 3:
                        status.ScrollWheelStatus = state;
                        break;
                    case 4:
                        status.RemoteStatus = state;
                        break;
                    case 5:
                        status.LockStatus = state;
                        break;
                }
            }
            return status;
        }

        /// <summary>
        /// Set the voltage output level of the PSP-405.
        /// </summary>
        /// <param name="voltage"> Voltage value (xx.xx). </param>
        public double SetVoltage(double voltage)
        {
            serialPort.SendCommand($"SV {SetTheNumberOfDecimalPlaces(voltage, 2, 2)}\r");
            return GetOutputVoltage();
        }

        /// <summary>
        /// Sets the voltage output limit of PSP-405;
        /// </summary>
        /// <param name="voltageLimit"> Voltage limit value (2 characters, no decimal) </param>
        public void SetVoltageLimit(int voltageLimit)
        {
            if (voltageLimit >= 100)
                throw new ArgumentOutOfRangeException("Voltage limit cannot be greater than or equal to 100.");
            serialPort.SendCommand($"SU {SetTheNumberOfDecimalPlaces(voltageLimit, 2, 0)}\r");
        }

        /// <summary>
        /// Returns the voltage output of the PSP-405
        /// </summary>
        /// <returns></returns>
        public double GetOutputVoltage()
        {
            serialPort.SendCommand("V\r");
            return GetDoubleDataFromPowerSupply(Value.voltage);
        }

        /// <summary>
        /// Returns the current voltage limit of the PSP-405
        /// </summary>
        /// <returns></returns>
        public int GetVoltageLimit()
        {
            serialPort.SendCommand("U\r");
            return (int)GetDoubleDataFromPowerSupply(Value.voltageLimit);
        }

        /// <summary>
        /// Increases the voltage output by 1V(coarse mode) or 1mV(fine mode).
        /// </summary>
        public void IncreaseOutputVoltage()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SV+ \r");
        }

        /// <summary>
        /// Decreases the voltage output by 1V(coarse mode) or 1mV(fine mode).
        /// </summary>
        public void DecreaseOutputVoltage()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SV- \r");
        }

        /// <summary>
        /// Increases the voltage limit by 1V(coarse mode) or 1mV(fine mode).
        /// </summary>
        public void IncreaseVoltageLimit()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SU+ \r");
        }

        /// <summary>
        /// Decreases the voltage limit by 1V (coarse mode) or 1mV (fine mode).
        /// </summary>
        public void DecreaseVoltageLimit()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SU- \r");
        }

        /// <summary>
        /// Sets the voltage limit to the maximum rating.
        /// </summary>
        public void SetVoltageLimitToMax()
        {
            serialPort.SendCommand("SUM \r");
        }


        /// <summary>
        /// Sets the current limit of the PSP-405.
        /// </summary>
        /// <param name="currentLimit"> Current limit value (x.xx) </param>
        public double SetCurrentLimit(double currentLimit)
        {
            if (currentLimit - 5 >= 0.00001)
                throw new ArgumentOutOfRangeException("Current limit cannot be greater than or equal to 5 A.");
            serialPort.SendCommand($"SI {SetTheNumberOfDecimalPlaces(currentLimit, 1, 2)}\r");
            return GetCurrentLimit();
        }

        /// <summary>
        /// Sets the current limit to the maximum rating.
        /// </summary>
        public void SetCurrentLimitToMax()
        {
            serialPort.SendCommand("SIM \r");
        }

        /// <summary>
        /// Returns the current output of the PSP-405
        /// </summary>
        public double GetOutputCurrent()
        {
            serialPort.SendCommand("A\r");
            return GetDoubleDataFromPowerSupply(Value.current);
        }

        /// <summary>
        /// Returns the current current limit of the PSP-405
        /// </summary>
        /// <returns></returns>
        public double GetCurrentLimit()
        {
            serialPort.SendCommand("I\r");
            return GetDoubleDataFromPowerSupply(Value.currentLimit);
        }

        /// <summary>
        /// Increases the current limit by 10 mA (coarse mode) or 1 mA (fine mode).
        /// </summary>
        public void IncreaseCurrentLimit()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SI+ \r");
        }

        /// <summary>
        /// Decreases the current limit by 10 mA (coarse mode) or 1 mA (fine mode).
        /// </summary>
        public void DecreaseCurrentLimit()
        {
            StepResolutionToNormalMode();
            serialPort.SendCommand("SI- \r");
        }

        /// <summary>
        /// Sets the power limit of the PSP-405.
        /// </summary>
        /// <param name="powerLimit"> Power limit value (3 characters, no decimal) </param>
        public void SetPowerLimit(int powerLimit)
        {
            if (powerLimit > 200)
                throw new ArgumentOutOfRangeException("Power limit cannot be greater than 200.");
            serialPort.SendCommand($"SP {SetTheNumberOfDecimalPlaces(powerLimit, 3, 0)}\r");
        }

        /// <summary>
        /// Sets the power limit to the maximum rating.
        /// </summary>
        public void SetPowerLimitToMax()
        {
            serialPort.SendCommand("SPM \r");
        }

        /// <summary>
        /// Returns the load output of the PSP-405
        /// </summary>
        /// <returns></returns>
        public double GetOutputPower()
        {
            serialPort.SendCommand("W\r");
            return GetDoubleDataFromPowerSupply(Value.power);
        }

        /// <summary>
        /// Returns the current load limit of the PSP-405
        /// </summary>
        /// <returns></returns>
        public double GetPowerLimit()
        {
            serialPort.SendCommand("P\r");
            return GetDoubleDataFromPowerSupply(Value.powerLimit);
        }

        /// <summary>
        /// Increases the power limit by 1 W.
        /// </summary>
        public void IncreasePowerLimit()
        {
            serialPort.SendCommand("SP+ \r");
        }

        /// <summary>
        /// Decreases the power limit by 1 W.
        /// </summary>
        public void DecreasePowerLimit()
        {
            serialPort.SendCommand("SP- \r");
        }

        /// <summary>
        /// Turn the output of the PSP-405 OFF.
        /// </summary>

        public void PowerOff()
        {
            serialPort.SendCommand("KOD\r");
        }

        /// <summary>
        /// Turn the output of the PSP-405 ON.
        /// </summary>
        public void PowerOn()
        {
            serialPort.SendCommand("KOE\r");
        }


        /// <summary>
        /// Toggle output of the PSP-405 ON or OFF.
        /// </summary>
        public void ToggleOnOrOff()
        {
            serialPort.SendCommand("KO\r");
        }

        /// <summary>
        /// Retuns the relay status of the power supply PSP-405 (true - ON, false - OFF)
        /// </summary>
        /// <returns></returns>
        public bool GetRelayStatus()
        {
            return GetStatus().RelayStatus;
        }

        #region Get data from PSP-405

        public double GetDoubleDataFromPowerSupply(Value value)
        {
            var data = serialPort.ReadExisting();
            if (data.Substring(0, 1) != value.Flag)
            {
                throw new Exception("PSP-405 returned an incorrect response.");
            }
            var stringValue = data.Substring(value.Flag.Length, value.Length);
            return double.Parse(stringValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        public class ValuesStatus
        {
            public double voltage;
            public double current;
            public double power;
            public bool relayStatus;

            public double voltageLimit;
            public double currentLimit;
            public double powerLimit;
        };
        /// <summary>
        /// Returns all the status values of the power supply PSP-405 (voltage, current, load output, voltage limit, current limit, load output limit...).
        /// </summary>
        public ValuesStatus GetAllTheStatusValues()
        {
            serialPort.SendCommand("L\r");
            var data = serialPort.ReadExisting();
            var statusPSP405 = new ValuesStatus();
            var theFirstSymbol = data.Substring(0, 1);
            if (theFirstSymbol == "V")
            {
                var voltage = data.Substring(1, 5);
                statusPSP405.voltage = Double.Parse(voltage, CultureInfo.InvariantCulture);
                var current = data.Substring(7, 5);
                statusPSP405.current = Double.Parse(current, CultureInfo.InvariantCulture);
                var power = data.Substring(13, 5);
                statusPSP405.power = Double.Parse(power, CultureInfo.InvariantCulture);
                var powerStatus = data.Substring(data.Length - 1 - 3 - 5, 1);
                if (powerStatus == "1")
                    statusPSP405.relayStatus = true;
                if (powerStatus == "0")
                    statusPSP405.relayStatus = false;
                var voltageLimit = data.Substring(19, 2);
                statusPSP405.voltageLimit = Double.Parse(voltageLimit, CultureInfo.InvariantCulture);
                var currentLimit = data.Substring(22, 4);
                statusPSP405.currentLimit = Double.Parse(currentLimit, CultureInfo.InvariantCulture);
                var powerLimit = data.Substring(27, 3);
                statusPSP405.powerLimit = Double.Parse(powerLimit, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception("PSP-405 returned an incorrect response to the request.");
            }
            return statusPSP405;
        }


        #endregion

        #region Send data to the PSP-405

        private static string SetTheNumberOfDecimalPlaces(double number, int significantPlaces, int decimalPlaces)
        {
            var significantFormat = "";
            var decimalFormat = "";
            for (var i = 0; i < significantPlaces; i++)
                significantFormat += '0';
            for (var i = 0; i < decimalPlaces; i++)
                decimalFormat += '0';
            var stringFormat = "{0:" + significantFormat + '.' + decimalFormat + '}';
            return string.Format(CultureInfo.InvariantCulture, stringFormat, number);
        }

        #endregion
    }
}
