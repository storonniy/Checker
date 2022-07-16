using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Text;

namespace Checker.DeviceDrivers
{
    class Simulator
    {
        readonly SerialPort serialPort;
        readonly int delay = 1500;
        public Simulator(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            if (SerialPort.GetPortNames().ToList().Contains(serialPort.PortName))
                this.serialPort.Open();
        }

        ~Simulator()
        {
            serialPort.Close();
        }

        private static List<int> GetRelayNamesAsAnArray(string relayNamesString)
        {
            if (relayNamesString.Contains("none"))
                return new List<int> { -1 };
            return relayNamesString
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }

        private static string DeleteIdentifierFromAnswer(string relayNamesString, string identifier)
        {
            if (!relayNamesString.Contains(identifier))
                throw new Exception("Simulator вернул неверный ответ на запрос.");
            return relayNamesString.Replace(identifier, "");
        }

        /// <summary>
        ///  Запрашивает имена замкнутых реле адаптера стыковки с ААП
        /// </summary>
        /// <returns>Возвращает массив типа string[], содержащий имена замкнутых реле</returns>
        public List<int> GetClosedRelayNames()
        {
            SendCommand("*GetClosedRelayNemes");
            var answer = serialPort.ReadExisting();
            var checkedClosedRelayNamesString = DeleteIdentifierFromAnswer(answer, "*ClosedRelayNames:");
            return GetRelayNamesAsAnArray(checkedClosedRelayNamesString);
        }
        public bool CloseRelays(params int[] relays)
        {
            var command = "*CloseRelays:" + string.Join(",", relays);
            SendCommand(command);
            var answerFromAdapter = serialPort.ReadExisting();
            return answerFromAdapter == "*CloseRelays:Ok\r";
        }
        
        public bool OpenRelays(params int[] relays)
        {
            var command = "*OpenRelays:" +  string.Join(",", relays);
            SendCommand(command);
            var answerFromAdapter = serialPort.ReadExisting();
            return answerFromAdapter == "*OpenRelays:Ok\r";
        }

        private byte[] GetBytes(string command)
        {
            var bytes = Encoding.ASCII.GetBytes(command);
            var result = new byte[bytes.Length + 1];
            Array.Copy(bytes, result, bytes.Length);
            result[bytes.Length] = 0x0A;
            return result;
        }
        

        public bool OpenAllRelays()
        {
            var command = "*OpenRelays:All";
            SendCommand(command);
            var answerFromAdapter = serialPort.ReadExisting();
            return answerFromAdapter == "*OpenRelays:Ok\r";
        }

        public void ConnectRelays(params int[] relays)
        {
            var command = "*ConnectRelays:" + string.Join(",", relays);
            SendCommand(command);
            var answerFromAdapter = serialPort.ReadExisting();
            if (answerFromAdapter.ToLower() != "*DisconnectRelays:Ok\r".ToLower())
                throw new Exception($"При коннекте реле {command} возникла ошибка");
        }

        private void SendCommand(string command)
        {
            var bytes = GetBytes(command);
            serialPort.Write(bytes, 0, bytes.Length);
            Thread.Sleep(delay);
        }

        public string GetIdentifier()
        {
            var command = "*IDN?\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            return serialPort.ReadLine();
        }

        public List<int> GetSignals()
        {
            const string command = "*GetID\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            var signalsString = serialPort.ReadExisting();
            var checkedSignals = DeleteIdentifierFromAnswer(signalsString, "*ID:");
            return GetRelayNamesAsAnArray(checkedSignals);
        }
    }
}
