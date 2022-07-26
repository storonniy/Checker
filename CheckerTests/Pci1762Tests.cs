using System.Collections.Generic;
using Advantech;
using NUnit.Framework;

namespace CheckerTests
{
    public class Pci1762Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetRelaysAsByteMoreTests()
        {
            GetRelaysAsByteTest(new [] {0, 1, 2, 3, 4, 5, 6, 7}, 255);
            GetRelaysAsByteTest(new [] {0, 7}, 129);
        }

        private void GetRelaysAsByteTest(IEnumerable<int> relayNumbers, byte expected)
        {
            var actual = Pci1762.ConvertRelayNumbersToByte(relayNumbers);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /*[Test]
        public void GetPortNumDictionaryTests()
        {
            GetPortNumDictionaryTest(new [] {0, 1, 2, 8, 9, 10, 15},
                new Dictionary<int, byte>() {{0, 7}, {1, 135}});
        }

        [Test]
        public void OneRelayInOnePort()
        {
            GetPortNumDictionaryTest(new [] {0},
                new Dictionary<int, byte>() {{0, 1}});
            GetPortNumDictionaryTest(new [] {5},
                new Dictionary<int, byte>() {{0, 32}});
            GetPortNumDictionaryTest(new [] {8},
                new Dictionary<int, byte>() {{1, 1}});
            GetPortNumDictionaryTest(new [] {13},
                new Dictionary<int, byte>() {{1, 32}});
        }*/

        /*private static void GetPortNumDictionaryTest(int[] relayNumbers, Dictionary<int, byte> expected)
        {
            var actual = Pci1762.GetPortBytesDictionary(relayNumbers);
            CollectionAssert.AreEquivalent(expected, actual);
        }*/
    }
}