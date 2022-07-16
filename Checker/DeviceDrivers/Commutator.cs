using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Checker.DeviceDrivers
{
    public class Commutator
    {
        readonly SerialPort serialPort;
        readonly int delay = 1500;
        public Commutator(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            //serialPort.Open();
        }

        ~Commutator()
        {
            this.serialPort.Close();
        }

        public static string[] GetRelayNamesAsAnArray(string relayNamesString)
        {
            relayNamesString = relayNamesString.Replace(" ", "").Replace("\r", "");
            return relayNamesString.Split(',');
        }

        public static string DeleteIdentifierFromAnswer(string relayNamesString, string identifier)
        {
            if (!relayNamesString.Contains(identifier))
                throw new Exception("Checker Swithing Adapter вернул неверный ответ на запрос.");
            return relayNamesString.Replace(identifier, "");
        }

        /// <summary>
        ///  Запрашивает имена замкнутых реле адаптера стыковки с ААП
        /// </summary>
        /// <returns>Возвращает массив типа string[], содержащий имена замкнутых реле</returns>
        public string[] GetClosedRelayNames()
        {
            serialPort.WriteLine("*GetClosedRelayNames\r");
            Thread.Sleep(delay);
            string closedRelayNamesString = serialPort.ReadExisting();
            string checkedClosedRelayNamesString = DeleteIdentifierFromAnswer(closedRelayNamesString, "*ClosedRelayNames:");
            return GetRelayNamesAsAnArray(checkedClosedRelayNamesString);
        }

        public string PrepareCommandForAdapter(params string[] relays)
        {
            string command = "";
            for (var i = 0; i < relays.Length; i++)
            {
                command += relays[i];
                if (i != relays.Length - 1)
                    command += ',';
                else
                    command += "\r";
            }
            return command;
        }

        public class CommutatorException : Exception
        {
            public CommutatorException(string message) : base(message)
            {

            }
        }

        public void CloseRelays(params string[] relays)
        {
            string command = "*CloseRelays:" + PrepareCommandForAdapter(relays);
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            var answerFromAdapter = serialPort.ReadExisting();
            if (answerFromAdapter != "*CloseRelays:OK\r")
                throw new CommutatorException($"При замыкании реле {command} возникла ошибка");
            try
            {

            }
            catch { TimeoutException e; }
            {
                // Время ожидания ответа от коммутационного адаптера превышено.
            }
            //return answerFromAdapter.Replace("\r", "");
        }

        public void OpenRelays(params string[] relays)
        {
            string command = "*OpenRelays:" + PrepareCommandForAdapter(relays);
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            var answerFromAdapter = serialPort.ReadExisting();
            //if (answerFromAdapter != "*OpenRelays:OK\r" && answerFromAdapter != "*OpenRelays:all\r" && answerFromAdapter != "* OpenRelays:all\r* OpenRelays:all\r")
                //throw new Exception($"При размыкании реле {command} возникла ошибка");
        }

        public string GetIdentifier()
        {
            var command = "*IDN?\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            return serialPort.ReadLine();
        }

        public string[] GetSignals()
        {
            var command = "*GetID\r";
            serialPort.WriteLine(command);
            Thread.Sleep(delay);
            var signalsString = serialPort.ReadExisting();
            string checkedSignals = DeleteIdentifierFromAnswer(signalsString, "*ID:");
            return GetRelayNamesAsAnArray(checkedSignals);
        }
    }  
}
