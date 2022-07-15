using System;
using System.Collections.Generic;
using System.Linq;
using Automation.BDaq;
using Checker.Auxiliary;

namespace Advantech
{
    public class Pci176X
    {
        private readonly InstantDoCtrl instantDoCtrl;
        private BDaqDevice device;
        private readonly int portsCount;
        private readonly int maxRelayNumber;

        protected Pci176X(string description, int portsCount)
        {
            this.portsCount = portsCount;
            maxRelayNumber = portsCount * 8 - 1;
            instantDoCtrl = new InstantDoCtrl();
            instantDoCtrl.SelectedDevice = new DeviceInformation(description);
            if (!instantDoCtrl.Initialized)
            {
                throw new InvalidOperationException($"Pci176X {description} не инициализирован");
            }
        }
        
        public bool CloseRelays(int[] relayNumbers)
        {
            return ChangeRelayState(relayNumbers, GetCloseRelayData);
        }

        public bool OpenRelays(int[] relayNumbers)
        {
            return ChangeRelayState(relayNumbers, GetOpenRelayData);
        }

        private static readonly Func<byte, byte, byte> GetOpenRelayData =
            (currentData, newData) => (byte) (currentData - (byte) (currentData & newData));

        private static readonly Func<byte, byte, byte> GetCloseRelayData =
            (currentData, newData) => (byte) (currentData | newData);

        private bool ChangeRelayState(int[] relayNumbers, Func<byte, byte, byte> getNewPortData)
        {
            var dict = GetPortBytesDictionary(relayNumbers);
            return dict.All(portData =>
            {
                var newData = getNewPortData(Read(portData.Key), portData.Value);
                return instantDoCtrl.Write(portData.Key, newData) == ErrorCode.Success;
            });
            /*foreach (var portNum in dict.Keys)
            {
                var currentData = Read(portNum);
                var newData = dict[portNum];
                var status = instantDoCtrl.Write(portNum, getNewPortData(currentData, newData));
                if (status != ErrorCode.Success)
                {
                    return false;
                }
            }
            return true;*/
        }

        public bool OpenAllRelays()
        {
            return Enumerable.Range(0, portsCount)
                .All(portNum => instantDoCtrl.Write(portNum, 0x00) == ErrorCode.Success);
        }

        public static byte ConvertRelayNumbersToByte(IEnumerable<int> relayNumbers)
        {
            return (byte) relayNumbers
                .Distinct()
                .Where(relayNumber => relayNumber >= 0 && relayNumber <= 7)
                .Select(relayNumber => (byte) (1 << relayNumber))
                .Sum(x => x);
        }


        public Dictionary<int, byte> GetPortBytesDictionary(int[] relayNumbers)
        {
            if (relayNumbers.Any(relayNumber => relayNumber < 0 || relayNumber > maxRelayNumber))
                throw new ArgumentOutOfRangeException($"Номер реле должен быть от 0 до {maxRelayNumber}");
            return relayNumbers
                .Select(r => Tuple.Create(r / 8, r % 8))
                .GroupBy(r => r.Item1)
                .ToDictionary(group => group.Key,
                    group => ConvertRelayNumbersToByte(group.Select(x => x.Item2).ToList()));
        }


        public byte Read(int port)
        {
            instantDoCtrl.Read(port, out var data);
            return data;
        }

        public List<int> GetClosedRelaysNumbers()
        {
            return Enumerable.Range(0, portsCount)
                .SelectMany(portNum => ConvertDataToRelayNumbers(Read(portNum), portNum))
                .ToList();
        }

        public static List<int> ConvertDataToRelayNumbers(byte data, int portNum)
        {
            return Enumerable.Range(0, 8)
                .Where(bitNumber => data.BitState(bitNumber))
                .Select(bitNumber => 8 * portNum + bitNumber)
                .ToList();
        }
    }
}