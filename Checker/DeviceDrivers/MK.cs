using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VciCAN;
using Ixxat.Vci4.Bal.Can;
using System.Collections;
using System.Threading;
using Checker.Auxiliary;

namespace Checker.DeviceDrivers
{
    public class MK
    {
        private readonly CanConNet vciDevice;
        private readonly List<BlockData> blockDataList;

        public MK()
        {
            vciDevice = new CanConNet();
            blockDataList = WakeUp(); 
        }

        /// <summary>
        /// Убивает ReceiveThreadFunc, которая мешает вызвать деструктор внутри CanConNet
        /// </summary>
        public void Die()
        {
            vciDevice.Die();
        }

        ~MK()
        {
            vciDevice.FinalizeApp();        
        }

        private ICanMessage GetAnswer(byte validFirstByte)
        {
            ICanMessage answer;
            do
            {
                Thread.Sleep(100);
                answer = vciDevice.GetData();
                if (answer == null)
                    throw new MkException("Устройство не отвечает");
            } while (answer[0] != validFirstByte);
            return answer;
        }

        private List<ICanMessage> GetICanMessagesList(byte validFirstByte)
        {
            var messagesList = new List<ICanMessage>();
            while (true)
            {
                Thread.Sleep(100);
                var answer = vciDevice.GetData();
                if (answer == null) break;
                if (answer[0] == validFirstByte)
                {
                    messagesList.Add(answer);
                }
            }
            return messagesList;
        }

        #region 1 Assign Block ID

        /// <summary>
        /// Присвоение ID блоку МК, подключённому к ПК.
        /// </summary>
        /// <param name="blockType"> Тип блока </param>
        /// <param name="moduleNumber"> Номер модуля </param>
        /// <param name="placeNumber"> Номер платоместа </param>
        /// <param name="factoryNumber"> Заводской номер </param>
        /// <returns> Возвращает ID блока МК, подключённого к ПК. </returns>

        private uint AssignBlockID(int blockType, int moduleNumber, int placeNumber, int factoryNumber)
        {
            uint msgID = 0x00;
            byte[] canMessage = new byte[8];
            canMessage[0] = 0x01;
            canMessage[1] = 0xFF;
            canMessage[2] = BitConverter.GetBytes(blockType)[0];
            canMessage[2] = BitConverter.GetBytes(moduleNumber)[0];
            canMessage[4] = BitConverter.GetBytes(placeNumber)[0];
            canMessage[5] = BitConverter.GetBytes(factoryNumber)[0];
            canMessage[6] = BitConverter.GetBytes(factoryNumber)[1];
            canMessage[7] = 0xFF;
            vciDevice.TransmitData(canMessage, msgID);
            var answer = GetAnswer(0xFE);
            return answer[2];
        }

        #endregion

        #region 2 Emergency Breaking
        /// <summary>
        /// Разомкнуть все реле МК
        /// </summary>
        /// <returns> </returns>
        public bool EmergencyBreak()
        {
            const uint id = 0x00;
            byte[] canMessage = { 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            Thread.Sleep(30);
            for (var blockNumber = 0; blockNumber < blockDataList.Count; blockNumber++)
            {
                var answer = GetAnswer(0xFD);
            }
            return true;
        }

        #endregion

        #region 3 Connect array of relays
        /// <summary>
        /// Замкнуть массив реле. В случае внутренней ошибки блока МК размыкает все реле МК.
        /// </summary>
        /// <returns> Возвращает статус операции (реле МК успешно разомкнуты/произошла ошибка) </returns>
        public byte CloseRelaysArray(int blockNumber, byte[] state)
        {
            var id = blockDataList[blockNumber].Id;
            byte[] message1 = { 0x03, 0x01, 0x00, state[0], state[1], state[2], state[3], state[4] };
            vciDevice.TransmitData(message1, id);
            byte[] message2 = { 0x03, 0x02, 0x00, state[5], state[6], state[7], state[8], state[9] };
            vciDevice.TransmitData(message2, id);
            var answer = GetAnswer(0xFC);
            var status = answer[2];
            return status;
        }

        public bool CloseRelays(int blockNumber, params int[] relayNumbers)
        {
            relayNumbers = relayNumbers.Select(r => r - 1).ToArray();
            var newStates = GetChangedRelaysBytes(blockNumber, relayNumbers, GetCloseRelayData);
            var status = CloseRelaysArray(blockNumber, newStates);
            return status == 0x00;
        }
        
        public bool OpenRelays(int blockNumber, params int[] relayNumbers)
        {
            relayNumbers = relayNumbers.Select(r => r - 1).ToArray();
            var newStates = GetChangedRelaysBytes(blockNumber, relayNumbers, GetOpenRelayData);
            var status = CloseRelaysArray(blockNumber, newStates);
            return status == 0x00;
        }
        
        private static readonly Func<byte, byte, byte> GetOpenRelayData =
            (currentStates, newStates) => (byte) (currentStates - (byte) (currentStates & newStates));
        
        private static readonly Func<byte, byte, byte> GetCloseRelayData =
            (currentStates, newStates) => (byte) (currentStates | newStates);
        
        public static byte[] GetRelayStatesBytes(int[] relayNumbers)
        {
            if (relayNumbers.Any(relayNumber => relayNumber < 0 || relayNumber > 79))
                throw new ArgumentOutOfRangeException($"Номер реле должен быть от 0 до {79}");
            var relayStatesBytes = new byte[10];
            var a = relayNumbers
                .Select(r => Tuple.Create(r / 8, r % 8))
                .GroupBy(r => r.Item1)
                .Select(g => Tuple.Create(g.Key, ConvertRelayNumbersToByte(g.Select(x => x.Item2).ToArray())))
                .Select(tuple => relayStatesBytes[tuple.Item1] = tuple.Item2)
                .ToArray();
            return relayStatesBytes;
        }
        
        public byte[] GetChangedRelaysBytes(int blockNumber, int[] relayNumbers, Func<byte, byte, byte> changeByte)
        {
            var newStates = GetRelayStatesBytes(relayNumbers);
            var currentStates = RequestAllRelayStatus(blockNumber);
            return Enumerable.Range(0, 10)
                .Select(i => changeByte(currentStates[i], newStates[i]))
                .ToArray();
        }
        
        public static byte ConvertRelayNumbersToByte(IEnumerable<int> relayNumbers)
        {
            return (byte) relayNumbers
                .Distinct()
                .Where(relayNumber => relayNumber >= 0 && relayNumber <= 7)
                .Select(relayNumber => (byte) (1 << relayNumber))
                .Sum(x => x);
        }
        
        #endregion

        #region 4 Request status of all relays

        /// <summary>
        /// Запрос состояния всех реле.
        /// </summary>
        /// <param name="blockNumber"> Номер блока </param>
        /// <returns> Returns array of 10 bytes which contains relay states (each bit represents relay state) </returns>
        public byte[] RequestAllRelayStatus(int blockNumber)
        {
            var id = blockDataList[blockNumber].Id;
            byte[] canMessage = { 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer1 = GetAnswer(0xFB);
            var answer2 = GetAnswer(0xFB);
            var a1 = new byte[8];
            var a2 = new byte[8];
            for (var i = 0; i < 8; i++)
            {
                a1[i] = answer1[i];
                a2[i] = answer2[i];
            }
            return GetRelayStatesBytes(new List<byte[]> { a1, a2 });
        }

        public static byte[] GetRelayStatesBytes(IEnumerable<byte[]> canMessages)
        {
            return canMessages
                .Select(b => Tuple.Create(b[1], b))
                .OrderBy(tuple => tuple.Item1)
                .Select(tuple => tuple.Item2)
                .SelectMany(b => new [] { b[3], b[4], b[5], b[6], b[7] })
                .ToArray();
        }
        
        /// <summary>
        /// Возвращает номера замкнутых реле, нумеруя реле с 1
        /// </summary>
        /// <param name="relayStatusBytes"></param>
        /// <returns></returns>
        public static int[] GetRelayNumbers(byte[] relayStatusBytes)
        {
            return Enumerable.Range(0, relayStatusBytes.Length)
                .SelectMany(i => Enumerable.Range(0, 8)
                    .Where(bitNumber => relayStatusBytes[i].BitState(bitNumber))
                    .Select(bitNumber => 8 * i + bitNumber + 1))
                .ToArray();
        }


        /// <returns>Возвращает номера всех замкнутых реле всех блоков МК, находящихся на данной шине CAN</returns>
        public string[] GetClosedRelayNames()
        {
            return Enumerable.Range(0, blockDataList.Count)
                .Select(blockNumber => GetClosedRelayNames(blockNumber))
                .ToArray();
        }

        /// <returns>Возвращает номера всех замкнутых реле конкретного блока МК</returns>

        public string GetClosedRelayNames(int blockNumber)
        {
            var relayNumbers = GetRelayNumbers(RequestAllRelayStatus(blockNumber));
            return $"MK{blockNumber + 1}: {string.Join(", ", relayNumbers)}";
        }

        #endregion

        #region 5 Change relay state

        /// <summary>
        /// Изменяет состояние одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер реле (от 0 до 79) </param>
        /// <param name="relayState"> Состояние реле. True - замкнуть, false - разомкнуть </param>
        private bool ChangeRelayState(int blockNumber, int relayNumber, bool relayState)
        {
            var id = blockDataList[blockNumber].Id;
            if (relayNumber < 0 || relayNumber > 79)
            {
                throw new Exception("Номер реле должен быть в диапазоне от 0 до 79");
            }
            var stateByte = (byte)(relayState ? 0x01 : 0x00);
            byte[] canMessage = { 0x05, (byte)relayNumber, stateByte, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            Thread.Sleep(150);
            var answer = GetAnswer(0xFA);
            var returnedRelayNumber = answer[1];
            if (returnedRelayNumber != (byte)relayNumber)
            {
                EmergencyBreak();
                throw new Exception($"МК не изменил состояние нужных реле: {string.Join(" ", answer)}");
            }
            var requestedRelayStatus = (byte)(relayState ? 0x01 : 0x00);
            var actualStatus = RequestSingleRelayStatus(blockNumber, relayNumber);
            return requestedRelayStatus == actualStatus;
        }

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"> Номер блока </param>
        /// <param name="relayNumbers">Принимает номера реле, нумерующиеся с 1</param>
        /// <returns></returns>
        public bool CloseRelays(int blockNumber, int[] relayNumbers) => relayNumbers
            .Select(r => ChangeRelayState(blockNumber, r - 1, true))
            .All(status => status);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <param name="relayNumbers">Принимает номера реле, нумерующиеся с 1</param>
        /// <returns></returns>
        public bool OpenRelays(int blockNumber, int[] relayNumbers) => relayNumbers
            .Select(r => ChangeRelayState(blockNumber, r - 1, false))
            .All(status => status);*/

        
        #endregion

        #region 6 Request status of one of the relays

        /// <summary>
        /// Запрос состояния одного реле.
        /// </summary>
        /// <param name="relayNumber"> Номер запрашиваемого реле. </param>
        /// <returns> Возвращает объект типа CanConNet.DataBuf, содержащий данные об ответе МК. </returns>
        public byte RequestSingleRelayStatus(int blockNumber, int relayNumber)
        {
            var id = blockDataList[blockNumber].Id;
            byte[] canMessage = { 0x06, (byte)relayNumber, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            Thread.Sleep(100);
            var answer = GetAnswer(0xF9);
            var returnedRelayNumber = answer[1];
            var status = answer[2];
            if (returnedRelayNumber == (byte)relayNumber)
            {
                return status;
            }
            throw new Exception($"МК вернул статус реле {returnedRelayNumber}, требовался статус реле {relayNumber}");
        }

        #endregion

        #region 7 Request REC relay state

        /// <summary>
        /// Запрос состояния реле РЭК по факту
        /// </summary>
        public ICanMessage RequestRecRelayState(int blockNumber)
        {
            var id = blockDataList[blockNumber].Id;
            const byte byte1 = 0x07;
            byte[] canMessage = { byte1, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            vciDevice.TransmitData(canMessage, id);
            var answer = GetAnswer(0xF9);
            return answer;
        }

        #endregion 

        #region 8 Wake Up

        /// <summary>
        /// Проверка наличия оборудования.
        /// </summary>
        private List<BlockData> WakeUp()
        {
            const uint id = 0x00;
            byte[] canMessage = { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            vciDevice.TransmitData(canMessage, id);
            return GetICanMessagesList(0xF7)
                .Select(msg => new BlockData(msg.Identifier, 256 * msg[3] + msg[2]))
                .OrderBy(blockData => blockData.Id)
                .ToList();
        }

        #endregion
    }

    public class BlockData
    {
        public BlockData(uint id, int factoryNumber)
        {
            Id = id;
            FactoryNumber = factoryNumber;
        }

        public uint Id { get; }
        public int FactoryNumber { get; }

        public override bool Equals(object obj)
        {
            if (obj is not BlockData blockData)
                return false;
            return Id.Equals(blockData.Id) && FactoryNumber.Equals(blockData.FactoryNumber);
        }
    }

    public class MkException : Exception
    {
        public MkException(string message) : base(message) { }
    }
}
