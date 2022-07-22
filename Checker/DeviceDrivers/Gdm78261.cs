using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;

namespace Checker.DeviceDrivers
{
    public class Gdm78261
    {
        readonly SerialPort serialPort;
        public Gdm78261 (SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        ~Gdm78261()
        {
            serialPort.Close();
        }

        public double MeasureVoltageAC()
        {
            Thread.Sleep(500);
            serialPort.SendCommand("MEAS:VOLT:AC?\r");
            Thread.Sleep(5000);
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Returns the DC voltage measurement as N mV.
        /// </summary>
        /// <returns></returns>
        public double MeasureVoltageDC()
        {
            Thread.Sleep(500);
            serialPort.SendCommand("MEAS:VOLT:DC?\r");
            Thread.Sleep(5000);
            return serialPort.ReadDouble();
        }
        /// <summary>
        /// Returns the DC current measurement, mA.
        /// </summary>
        /// <returns></returns>
        public double MeasureCurrentDC()
        {
            Thread.Sleep(500);
            serialPort.SendCommand("MEAS:CURR:DC?\r");
            Thread.Sleep(5000);
            return serialPort.ReadDouble();
        }

        /// <summary>
        /// Sets measurement to DC Current on the first display and specifies range
        /// </summary>
        /// <param name="range"></param>
        public void SetMeasurementToCurrentDC(double range)
        {
            serialPort.SendCommand($"CONF:CURR:DC {range}\r");
        }
        /// <summary>
        /// Sets measurement to DC Voltage on the first display
        /// </summary>
        public void SetMeasurementToVoltageDC()
        {
            serialPort.SendCommand("CONF:VOLT:DC\r");
        }

        /// <summary>
        /// Sets measurement to AC Voltage on the first display
        /// </summary>
        public void SetMeasurementToVoltageAC()
        {
            serialPort.SendCommand($"CONF:VOLT:AC\r");
        }

        public void SetMeasurementToResistance()
        {
            serialPort.SendCommand($"CONF:RES\r");
        }

        public double MeasureResistance()
        {
            Thread.Sleep(500);
            serialPort.SendCommand("MEAS:RES?\r");
            Thread.Sleep(5000);
            return serialPort.ReadDouble();
        }
    }
}
