using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Automation.BDaq;
using Checker.Auxiliary;

namespace Advantech
{
    public class Pci1751
    {
        public class Signal
        {
            public int Data { get; }
            public PortName Port { get; }

            public Signal(string portName, int data)
            {
                if (!Enum.TryParse(portName, out PortName p))
                    throw new ArgumentException($"Устройство {portName} не найдено в списке доступных устройств");
                Port = p;
                Data = data;
            }

            public Signal(string signalName)
            {
                var signal = Parse(signalName);
                Data = signal.Data;
                Port = signal.Port;
            }

            public enum PortName
            {
                A,
                B,
                C
            }

            private static Signal Parse(string signalName)
            {
                return new Signal(signalName.Substring(0, 1), int.Parse(signalName.Substring(1)));
            }
            
            public static List<Signal> ParseAll(IEnumerable<string> signalNames)
            {
                return signalNames
                    .Select(Parse)
                    .ToList();
            }

            public override bool Equals(object obj)
            {
                if (obj is null or not Signal) return false;
                var signal = (Signal)obj;
                return signal.Data.Equals(Data) && signal.Port.Equals(Port);
            }
        }

        private readonly InstantDoCtrl instantDoCtrl;
        private readonly int portsCount;
        private readonly int maxRelayNumber;

        public Pci1751(string description)
        {
            portsCount = 6;
            maxRelayNumber = 15;
            instantDoCtrl = new InstantDoCtrl();
            instantDoCtrl.SelectedDevice = new DeviceInformation(description);
            if (!instantDoCtrl.Initialized)
            {
                throw new InvalidOperationException($"Pci176X {description} не инициализирован");
            }
        }
        
        public bool SetSignal(string[] signals)
        {
            return ChangeRelayState(signals, GetCloseRelayData);
        }

        public bool ClearSignal(string[] signals)
        {
            return ChangeRelayState(signals, GetOpenRelayData);
        }

        private static readonly Func<byte, byte, byte> GetOpenRelayData =
            (currentData, newData) => (byte) (currentData - (byte) (currentData & newData));

        private static readonly Func<byte, byte, byte> GetCloseRelayData =
            (currentData, newData) => (byte) (currentData | newData);

        private bool ChangeRelayState(string[] signalNames, Func<byte, byte, byte> getNewPortData)
        {
            var signals = Signal.ParseAll(signalNames);
            var dict = GetPortBytesDictionary(signals);
            return dict.All(portData =>
            {
                var newData = getNewPortData(Read(portData.Key), portData.Value);
                Thread.Sleep(100);
                return instantDoCtrl.Write(portData.Key, newData) == ErrorCode.Success;
            });
        }

        public bool ClearAllSignals()
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


        public static Dictionary<int, byte> GetPortBytesDictionary(List<Signal> signals)
        {
            if (signals.Any(relayNumber => relayNumber.Data < 0 || relayNumber.Data > 15))
                throw new ArgumentOutOfRangeException($"Номер реле должен быть от 0 до {15}");
            return signals
                .Select(r => Tuple.Create(2 * (int)r.Port + r.Data / 8, r.Data % 8))
                .GroupBy(r => r.Item1)
                .ToDictionary(group => group.Key,
                    group => ConvertRelayNumbersToByte(group.Select(x => x.Item2).ToList()));
        }


        public byte Read(int port)
        {
            instantDoCtrl.Read(port, out var data);
            return data;
        }

        public List<string> GetSignals()
        {
            return Enumerable.Range(0, portsCount)
                .SelectMany(portNum => ConvertDataToRelayNumbers(Read(portNum), portNum))
                .ToList();
        }

        public static IEnumerable<string> ConvertDataToRelayNumbers(byte data, int portNum)
        {
            var portName = (Signal.PortName)(portNum / 2);
            return Enumerable.Range(0, 8)
                .Where(bitNumber => data.BitState(bitNumber))
                .Select(bitNumber => $"{portName}{8 * (portNum % 2) + bitNumber}")
                .ToList();
        }
    }
}