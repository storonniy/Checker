using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using Checker.Auxiliary;
using Checker.DeviceDrivers;

namespace Checker.DeviceDrivers
{
    public class Keithley2401
    {
        readonly SerialPort serialPort;
        public Keithley2401(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                serialPort.Open();
        }

        private static readonly int delay = 500;

        public void SelectVoltageSource()
        {
            SendCommand($":SOUR:FUNC VOLT");
        }

        /// <summary>
        /// Select fixed sourcing mode for V source
        /// </summary>
        public void SelectFixedSourcingModeVoltage()
        {
            SendCommand(":SOUR:VOLT:MODE FIX");
        }

        /// <summary>
        /// Select fixed sourcing mode for I source
        /// </summary>
        public void SelectFixedSourcingModeCurrent()
        {
            SendCommand(":SOUR:CURR:MODE FIX");
        }

        private byte[] GetBytes(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command);
            byte[] result = new byte[bytes.Length + 2];
            Array.Copy(bytes, result, bytes.Length);
            //result[bytes.Length] = 0x0D;
            result[bytes.Length] = 0x0A;
            return result;
        }


        /// <summary>
        /// Set I-source amplitude
        /// </summary>
        /// <param name="current"> Amplitude in amps </param>
        public void SetCurrent(double current)
        {
            var str = current.ToString().Replace(",", ".");
            SendCommand($":SOUR:CURR:LEV:AMPL {str}");
        }

        /// <summary>
        /// Set V-source amplitude
        /// </summary>
        /// <param name="voltage"> Amplitude in volts </param>
        public double SetVoltage(double voltage)
        {
            SelectFixedSourcingModeVoltage();
            Thread.Sleep(100);
            var str = voltage.ToString().Replace(",", ".");
            SendCommand($":SOUR:VOLT:LEV:AMPL {str}");
            return GetVoltage();
        }

        /// <summary>
        /// Select source function as voltage.
        /// </summary>
        /// <returns> Status </returns>
        public bool SetVoltageSourceMode()
        {
            SendCommand($":SOUR:FUNC:MODE VOLT");
            SendCommand($":SOUR:FUNC:MODE?");
            Thread.Sleep(1000);
            return serialPort.ReadExisting().Contains("VOLT");
        }

        private double ParseValue()
        {
            var value = serialPort.ReadExisting();
            return (double)decimal.Parse(value.Replace("\r", ""), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Select source function as current
        /// </summary>
        /// <returns> Status </returns>
        public bool SetCurrentSourceMode()
        {
            SendCommand($":SOUR:FUNC:MODE CURR");
            SendCommand($":SOUR:FUNC:MODE?");
            Thread.Sleep(1000);
            return serialPort.ReadExisting().Contains("CURR");
        }


        /// <summary>
        /// Query voltage level
        /// </summary>
        /// <returns></returns>
        public double GetVoltage()
        {
            SendCommand(":SOUR:VOLT:LEV:AMPL?");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Set current limit
        /// </summary>
        /// <returns></returns>
        public double SetCurrentLimit(double limit)
        {
            var str = limit.ToString().Replace(",", ".");
            //:SENS:CURR:RANGE
            SendCommand($":SENS:CURR:PROT {str}");
            SendCommand($":SENS:CURR:RANGE {str}");
            SendCommand($":SENS:CURR:PROT?");
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Turn the output of the Keithley 2401 OFF.
        /// </summary>
        public void PowerOff()
        {
            ChangePowerStatus(PowerState.OFF);
        }

        /// <summary>
        /// Turn the output of the Keithley 2401 ON.
        /// </summary>
        public void PowerOn()
        {
            ChangePowerStatus(PowerState.ON);
        }

        private bool ChangePowerStatus(PowerState state)
        {
            SendCommand($":OUTP {state}");
            SendCommand(":OUTP:STAT?");
            return serialPort.ReadLine() == "1"; // return serialPort.ReadLine() == Convert.ToInt32(state);
        }

        private void SendCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            Thread.Sleep(delay);
        }

        private enum PowerState
        {
            OFF,
            ON       
        }
    }
}
