using System.Collections.Generic;
using System.Linq;
using Advantech;
using NUnit.Framework;

namespace CheckerTests
{
    public class Pci1751Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetPortBytesDictionaryTest()
        {
            var expected = new Dictionary<int, byte>();
            expected.Add(0, 0b00011000);
            expected.Add(1, 0b10000001);
            expected.Add(2, 0b00011000);
            expected.Add(3, 0b10000001);
            expected.Add(4, 0b00011000);
            expected.Add(5, 0b10000001);
            GetPortBytesDictionaryTests(expected, new Pci1751.Signal("A3"), new Pci1751.Signal("A4"),
                new Pci1751.Signal("A8"), new Pci1751.Signal("A15"), new Pci1751.Signal("B3"), new Pci1751.Signal("B4"),
                new Pci1751.Signal("B8"), new Pci1751.Signal("B15"), new Pci1751.Signal("C3"), new Pci1751.Signal("C4"),
                new Pci1751.Signal("C8"), new Pci1751.Signal("C15"));
        }

        private void GetPortBytesDictionaryTests(Dictionary<int, byte> expected, params Pci1751.Signal[] signals)
        {
            var actual = Pci1751.GetPortBytesDictionary(signals.ToList());
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ConvertDataToRelayNumbersTest()
        {
            ConvertDataToRelayNumbersTests(0b10000001, 0, "A0", "A7");
            ConvertDataToRelayNumbersTests(0b10000001, 1, "A8", "A15");
            ConvertDataToRelayNumbersTests(0b10000001, 2, "B0", "B7");
            ConvertDataToRelayNumbersTests(0b10000001, 3, "B8", "B15");
            ConvertDataToRelayNumbersTests(0b10000001, 4, "C0", "C7");
            ConvertDataToRelayNumbersTests(0b10000001, 5, "C8", "C15");
            ConvertDataToRelayNumbersTests(0b10000001, 3, "B8", "B15");
            ConvertDataToRelayNumbersTests(0b10000001, 3, "B8", "B15");
        }

        private void ConvertDataToRelayNumbersTests(byte data, int portNum, params string[] expected)
        {
            var actual = Pci1751.ConvertDataToRelayNumbers(data, portNum);
            CollectionAssert.AreEquivalent(actual, expected.ToList());
        }

        [Test]
        public void GetSignals()
        {
            var actual = Pci1751.Signal.ParseAll(new[] { "A00", "A01", "A15" });
            var list = new List<Pci1751.Signal>();
            list.AddRange(new [] {new Pci1751.Signal("A00"), new Pci1751.Signal("A1"), new Pci1751.Signal("A15")});
            CollectionAssert.AreEquivalent(list, actual);
        }

        private void GetRelaysAsByteTest(IEnumerable<string> signalNames, List<Pci1751.Signal> expected)
        {
            var actual = Pci1751.Signal.ParseAll(signalNames);
            CollectionAssert.AreEquivalent(actual, expected);
        }

        [Test]
        public void GetPortBytesDictionaryTests()
        {
            var signals = Pci1751.Signal.ParseAll(new[] { "A00", "A15", "B01", "B08" });
            var actual = Pci1751.GetPortBytesDictionary(signals);
            var expected = new Dictionary<int, byte>();
            expected.Add(0, 1);
            expected.Add(1, 128);
            expected.Add(2, 2);
            expected.Add(3, 1);
            CollectionAssert.AreEquivalent(actual, expected);
        }
    }
}