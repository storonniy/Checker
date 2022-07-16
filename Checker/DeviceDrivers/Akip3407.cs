using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;

namespace Checker.DeviceDrivers
{
    public class Akip3407
    {
        private SerialPort serialPort;
        private readonly int delay = 2000;

        public Akip3407(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.Open();
        }

        private void SendCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            Thread.Sleep(delay);
        }

        private static byte[] GetBytes(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command);
            var result = new byte[bytes.Length + 2];
            Array.Copy(bytes, result, bytes.Length);
            result[bytes.Length] = 0x0D;
            result[bytes.Length + 1] = 0x0A;
            return result;
        }

        public double SetVoltage (double voltage)
        {
            var str = voltage.ToString().Replace(",", ".");
            SendCommand($"SOUR1:VOLT {str}");
            SendCommand("SOUR1:VOLT?");
            return serialPort.ReadDouble();
        }

        public double SetFrequency (string frequency)
        {
            SendCommand($"SOUR1:FREQ {frequency}");
            SendCommand("SOUR1:FREQ?");
            return serialPort.ReadDouble();
        }

        private double ParseValue()
        {
            return double.Parse(serialPort.ReadExisting().Replace("\n", ""), CultureInfo.InvariantCulture);
        }

        #region Power Status
        public bool PowerOn()
        {
            return ChangePowerStatus("1");
        }

        public bool PowerOff()
        {
            return ChangePowerStatus("0");
        }

        private bool ChangePowerStatus(string status)
        {
            SendCommand($"OUTP1 {status}");
            return true;
            SendCommand($"OUTP1?");
            var answer = serialPort.ReadLine().Replace("\n", "");
            return answer == "1";
        }

        #endregion

        ~Akip3407()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }
    }
}
