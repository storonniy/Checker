using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Checker.Auxiliary;

namespace Instek
{
    public class PshPstPss
    {
        private readonly SerialPort serialPort;
        private readonly int channelCount;
        private readonly string deviceName;

        public PshPstPss(SerialPort serialPort, int channelCount, string deviceName)
        {
            this.channelCount = channelCount;
            this.deviceName = deviceName;
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }
        
        private int ReadDataNr1()
        {
            return int.Parse(serialPort.ReadLine());
        }
        
        private float ReadDataNr3(string command)
        {
            return float.Parse(serialPort.ReadLine());
        }
        
        private bool ReadBoolean()
        {
            return serialPort.ReadLine() == "1";
        }

        /// <summary>
        /// Sets the output voltage of tyhe specific channel
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="channel"></param>
        /// <returns> Returns actual output voltage </returns>
        public double SetVoltage(double voltage, int channel = 1)
        {
            if (1 > channel || channel > channelCount)
                throw new ArgumentException($"Номер канала {deviceName} может быть равен {string.Join(", ", Enumerable.Range(1, channelCount))}");
            var str = voltage.ToString().Replace(",", ".");
            serialPort.SendCommand($":CHAN{channel}:VOLT {str};VOLT?\n");
            return serialPort.ReadDouble();
        }

        private double ParseValue()
        {
            Thread.Sleep(1000);
            return double.Parse(serialPort.ReadLine().Replace(".", ","));
        }

        public double GetOutputVoltage(int channel = 1)
        {
            if (1 > channel || channel > channelCount)
                throw new ArgumentException($"Номер канала {deviceName} может быть равен {string.Join(", ", Enumerable.Range(1, channelCount))}");
            serialPort.SendCommand($":CHAN{channel}:MEAS:VOLT?\n");
            return serialPort.ReadDouble();
        }

        public double SetCurrentLimit(double current, int channel = 1)
        {
            if (1 > channel || channel > channelCount)
                throw new ArgumentException($"Номер канала {deviceName} может быть равен {string.Join(", ", Enumerable.Range(1, channelCount))}");
            var str = current.ToString().Replace(",", ".");
            serialPort.SendCommand($":CHAN{channel}:CURR {str};CURR?\n");
            return serialPort.ReadDouble();
        }

        private bool ChangeOutputState(string outputState)
        {
            if (outputState != "0" && outputState != "1")
                throw new Exception("Состояние может принимать значение 0 (выключено) и 1 (включено)");
            serialPort.SendCommand($":outp:stat {outputState}\n:outp:stat?");
            return ReadBoolean();
        }
        /// <summary>
        /// Sets the Over Current Protection. Range: false (Off), true (On)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>

        public void SetCurrentProtection(bool state, int channel = 1)
        {
            if (1 > channel || channel > channelCount)
                throw new ArgumentException($"Номер канала {deviceName} может быть равен {string.Join(", ", Enumerable.Range(1, channelCount))}");
            var currProtection = state ? "1" : "0";
            var command = $":chan{channel}:prot:curr {currProtection};:chan1:prot:curr?\n";
            serialPort.SendCommand(command);
        }

        public void PowerOn()
        {
            ChangeOutputState("1");
        }
        public void PowerOff()
        {
            ChangeOutputState("0");
        }

        ~PshPstPss()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }
    }
}