using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USA
{
    public class UCACheckingFunc
    {
        /// <summary>
        /// Контроль появлений сигналов DCx при замыкании реле Kx.
        /// </summary>
        /// <param name="actualClosedRelayNames"> Массив имён замкнутых реле </param>
        /// <param name="expectedClosedRelayNames"> Массив имён реле, которые должны быть замкнуты </param>
        /// <returns> Возвращает True, если массив всех замкнутых реле содержит все реле, которые должны быть замкнуты </returns>
        public static string DCSignalsControl(string[] actualClosedRelayNames, params string[] expectedClosedRelayNames)
        {
            string messageOfChecking = null;
            if (actualClosedRelayNames.Length != expectedClosedRelayNames.Length)
            {
                var actualClosedRelaysString = GetStringFromArray(actualClosedRelayNames);
                var expectedClosedRelaysString = GetStringFromArray(expectedClosedRelayNames);
                messageOfChecking = "Ошибка в контроле стыковки. Замкнулись реле " + actualClosedRelaysString + ". Ожидалось замыкание реле " + expectedClosedRelaysString + '\n';
            }
            else
            {
                foreach (var relayName in expectedClosedRelayNames)
                {
                    if (!actualClosedRelayNames.Contains(relayName))
                    {
                        messageOfChecking = "Ошибка в контроле стыковки. Не замкнуто реле " + relayName + '\n';
                        if (relayName.StartsWith("K"))
                        {
                            // Отключение питания
                            PSP405Library.PSP405.Power.TurnOff();
                        }
                    }
                    else
                        messageOfChecking = "Done";
                    break;
                }
            }
            return messageOfChecking;
        }
        private static string GetStringFromArray(params string[] array)
        {
            string stringArray = null;
            for (int i = 0; i < array.Length; i++)
            {
                stringArray += array[i];
                if (i != array.Length)
                    stringArray += ", ";
            }
            return stringArray;
        }
    }
}


