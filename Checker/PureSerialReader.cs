using System.IO.Ports;
using System.Threading;

namespace Checker
{
    public static class PureSerialReader
    {
        public static string ReadResponse(this SerialPort serialPort)
        {
            string result = "";
            while (true)
            {
                var read = serialPort.ReadExisting();
                if (read == "" && result.Length > 0)
                {
                    return result;
                }
                result += read;
                Thread.Sleep(50);
            }
        }
    }
}