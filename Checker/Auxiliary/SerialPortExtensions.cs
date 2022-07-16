using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checker.Auxiliary
{
    public static class SerialPortExtensions
    {
        private const int delay = 1000;

        public static double ReadDouble(this SerialPort serialPort)
        {
            var value = serialPort.ReadLine().Replace("\r", "").Replace("\n", "");
            value = value.Trim();
            return ParseDouble(value);
        }

        public static double ParseDouble(string data)
        {
            if (!double.TryParse(data, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out var result))
            {
                if (!double.TryParse(data, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result))
                {
                    throw new FormatException();
                }
            }
            return result;
        }

        public static void SendCommand(this SerialPort serialPort, string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
        }
    }
}
