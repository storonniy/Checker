using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Checker.Auxiliary;

namespace Checker.DeviceDrivers
{
    public class Akip3407
    {
        private readonly SerialPort serialPort;
        private const int delay = 2000;

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

        public double SetFrequency(string frequency)
        {
            SendCommand($"SOUR1:FREQ {frequency}");
            SendCommand("SOUR1:FREQ?");
            return serialPort.ReadDouble();
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

        public bool SetSignalShape(string shape, string frequency, double highLevel, double lowLevel)
        {
            if (Enum.TryParse<Shape>(shape, out _)) 
                throw new Exception($"Неизвестная форма сигнала {shape}");
            serialPort.SendCommand($"SOUR1:APPL:{shape} {frequency}, {highLevel}, {lowLevel}");
            serialPort.SendCommand($"SOUR1:FUNC?");
            return serialPort.ReadExisting().Contains(shape);
        }
        public bool SetSignalShape(string shape, string parameters)
        {
            if (Enum.TryParse<Shape>(shape, out _)) 
                throw new Exception($"Неизвестная форма сигнала {shape}");
            serialPort.SendCommand($"SOUR1:APPL:{shape} {parameters}");
            serialPort.SendCommand($"SOUR1:FUNC?");
            return serialPort.ReadExisting().Contains(shape);
        }

        public double SetLowLevel(double lowLevel)
        {
            serialPort.SendCommand($"SOUR1:VOLT:LOW {lowLevel}");
            serialPort.SendCommand("SOUR1:VOLT:LOW?");
            return serialPort.ReadDouble();
        }
        
        public double SetHighLevel(double highLevel)
        {
            serialPort.SendCommand($"SOUR1:VOLT:HIGH {highLevel}");
            serialPort.SendCommand("SOUR1:VOLT:HIGH?");
            return serialPort.ReadDouble();
        }

        public double SetDutyCycle(double dutyCycle)
        {
            serialPort.SendCommand("SOUR1:FUNC?");
            var shape = serialPort.ReadExisting().Replace("\r", "");
            serialPort.SendCommand($"FUNC:{shape}:DCYC {dutyCycle}");
            serialPort.SendCommand($"FUNC:{shape}:DCYC?");
            return serialPort.ReadDouble();
        }
        
        public enum Shape
        {
            SIN,
            SQU,
            TANG,
            PULS,
            NOIS,
            PRBS,
            REXP,
            FEXP,
            SINC,
            CIRC,
            GAUS,
            CARD,
            QUAK
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
