using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Advantech;
using Checker.Device.DeviceList.Pci176XDevice;
using Checker.DeviceDrivers;
using Checker.Devices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Advantech
{
    [TestClass]
    public class Pci1762Tests
    {
        [TestMethod]
        public void GetSignals()
        {
            var actual = Pci1751.Signal.ParseAll(new[] { "A00, A01, A15" });
            CollectionAssert.AreEquivalent(new List<Pci1751.Signal> {new Pci1751.Signal("A", 0)}, actual);
        }
        
        [TestMethod]
        public void GetRelaysAsByteMoreTests()
        {
            GetRelaysAsByteTest(new int[] {0, 1, 2, 3, 4, 5, 6, 7}, 255);
        }

        private void GetRelaysAsByteTest(IEnumerable<int> relayNumbers, byte expected)
        {
            var actual = Pci1762.ConvertRelayNumbersToByte(relayNumbers);
            Assert.AreEqual(expected, actual);
        }
        
        //PCI_1762 pci1762 = new PCI_1762("PCI-1762,BID#1");
        [TestMethod]
        public void GetRelayNumbersArray_Test()
        {
            var actual = Pci1762Device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6, 7, 8");
            var expected = new int[] {1, 2, 3, 4, 5, 6, 7, 8 };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetRelayNumbersArray_Test_lessThan8Relays()
        {
            var actual = Pci1762Device.GetRelayNumbersArray("1, 2, 3, 4, 5, 6");
            var expected = new int[] { 1, 2, 3, 4, 5, 6 };
            CollectionAssert.AreEqual(expected, actual);
        }

        /*[TestMethod]
        public void CheckOnePort ()
        {
            var expectedDict = new Dictionary<int, byte>();
            expectedDict.Add(0, 170);
            var actualDict = Pci1762.GetPortBytesDictionary(new [] { 1, 3, 5, 7 });
            CheckDictionary(expectedDict, actualDict);
        }*/

        /*[TestMethod]
        public void CheckTwoPorts()
        {
            var expectedDict = new Dictionary<int, byte>();
            expectedDict.Add(0, 138);
            expectedDict.Add(1, 133);
            var actualDict = Pci1762.GetPortBytesDictionary(new int[] { 8, 1, 3, 7, 15, 10});
            CheckDictionary(expectedDict, actualDict);
        }*/

        private void CheckDictionary(Dictionary<int, byte> expectedDict, Dictionary<int, byte> actualDict)
        {
            CollectionAssert.AreEquivalent(expectedDict.Keys, actualDict.Keys);
            foreach (var key in expectedDict.Keys)
            {
                Assert.AreEqual(expectedDict[key], actualDict[key]);
            }
        }

        
        [TestMethod]
        public void GetRelaysAsByte()
        {
            var expected = 135;
            var actual = Pci1762.ConvertRelayNumbersToByte(new List<int>() { 0, 1, 2, 7 });
            Assert.AreEqual(expected, actual);
            expected = 172;
            actual = Pci1762.ConvertRelayNumbersToByte(new List<int>() { 2, 5, 3, 7 });
            Assert.AreEqual(expected, actual);
            expected = 30;
            actual = Pci1762.ConvertRelayNumbersToByte(new List<int>() { 1, 2, 3, 4 });
            Assert.AreEqual(expected, actual);
            expected = 62;
            actual = Pci1762.ConvertRelayNumbersToByte(new List<int>() { 1, 2, 3, 4, 5 });
            Assert.AreEqual(expected, actual);
        }  
        
        [TestMethod]
        public void CheckDataWithRepeatingNumbers()
        {
            int expected = 44;
            int actual = Pci1762.ConvertRelayNumbersToByte(new List<int>() { 2, 3, 5, 5 });
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetDataAsRelayNumbers()
        {
            var expected = new List<int>() { 1, 3, 7 };
            var actual = Pci1762.ConvertDataToRelayNumbers(138, 0);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void OpenAllRelays()
        {
            Pci1762 pci1762 = new Pci1762("PCI-1762,BID#1");
            pci1762.CloseRelays(new int[] { 0, 7, 8, 15 });
            Thread.Sleep(1000);
            pci1762.OpenAllRelays();
            Thread.Sleep(1000);
            var expected = new int[] { };
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort0()
        {
            Pci1762 pci1762 = new Pci1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(1000);
            var expected = new int[] { 0, 3, 7 };
            pci1762.CloseRelays(expected);
            Thread.Sleep(1000);
            var actual = pci1762.GetClosedRelaysNumbers();
            Thread.Sleep(1000);
            pci1762.CloseRelays(new int[] { 4, 11});
            Thread.Sleep(1000);
            expected = new int[] { 0, 3, 7, 4, 11 };
            actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort1()
        {
            Pci1762 pci1762 = new Pci1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 8, 11, 15 };
            pci1762.CloseRelays(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void CloseRelaysPort1Port2()
        {
            Pci1762 pci1762 = new Pci1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 2, 4, 6, 9, 12, 15 };
            pci1762.CloseRelays(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void OpenRelaysPort1Port2()
        {
            var pci1762 = new Pci1762("PCI-1762,BID#1");
            pci1762.OpenAllRelays();
            Thread.Sleep(100);
            var expected = new int[] { 2, 4, 6, 9, 12, 15 };
            pci1762.CloseRelays(expected);
            Thread.Sleep(100);
            var actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
            pci1762.OpenRelays(new int[] { 2, 9, 15});
            expected = new int[] { 4, 6, 12 };
            actual = pci1762.GetClosedRelaysNumbers();
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}