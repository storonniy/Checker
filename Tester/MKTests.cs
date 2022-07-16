using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using Checker.DeviceDrivers;
using System.Collections.Generic;
using System.Linq;

namespace Tester
{
    [TestClass]
    public class MKTests
    {
        [TestMethod]
        public void GetRelayStatesBytesTest()
        {
            for (int j = 0; j < 100; j++)
            {
                byte[] canMsg1 = new byte[8];
                byte[] canMsg2 = new byte[8];
                var rnd = new Random();
                canMsg1[0] = 0xFB;
                canMsg1[1] = 0x01;
                canMsg2[0] = 0xFB;
                canMsg2[1] = 0x02;
                for (var i = 2; i < 8; i++)
                {
                    canMsg1[i] = (byte)rnd.Next(0, 255);
                    canMsg2[i] = (byte)rnd.Next(0, 255);
                }
                var expected = new byte[10];
                Array.Copy(canMsg1, 3, expected, 0, 5);
                Array.Copy(canMsg2, 3, expected, 5, 5);
                var actual = MK.GetRelayStatesBytes(new List<byte[]> { canMsg1, canMsg2 });
                CollectionAssert.AreEqual(expected, actual);
                var reversedActual = MK.GetRelayStatesBytes(new List<byte[]> { canMsg2, canMsg1 });
                CollectionAssert.AreEqual(expected, reversedActual);
            }
        }

        [TestMethod]
        public void GetRelayNumbersTest()
        {
            byte[] canMsg1 = new byte[]{ 0xFB, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            byte[] canMsg2 = new byte[]{ 0xFB, 0x02, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            var relayNumbers = new int[] { 1, 9, 17, 25, 33, 41, 49, 57, 65, 73 };
            var actual = MK.GetRelayNumbers(MK.GetRelayStatesBytes(new List<byte[]> { canMsg1, canMsg2 }));
            CollectionAssert.AreEquivalent(relayNumbers, actual);
            actual = MK.GetRelayNumbers(MK.GetRelayStatesBytes(new List<byte[]> { canMsg2, canMsg1 }));
            CollectionAssert.AreEquivalent(relayNumbers, actual);
            var byteStates = new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 };
            actual = MK.GetRelayNumbers(byteStates);
            CollectionAssert.AreEquivalent(relayNumbers, actual);
        }

        [TestMethod]
        public void BlockDataLambdaSorter()
        {
            var blockDataList = new List<BlockData>()
            {
                new BlockData(711, 7),
                new BlockData(111, 1),
                new BlockData(511, 5),
                new BlockData(311, 3),
                new BlockData(211, 2),
                new BlockData(411, 4),
                new BlockData(611, 6)
            };
            blockDataList = MK.SortByID(blockDataList);
            var expected = new List<BlockData>()
            {
                new BlockData(111, 1),
                new BlockData(211, 2),
                new BlockData(311, 3),
                new BlockData(411, 4),
                new BlockData(511, 5),
                new BlockData(611, 6),
                new BlockData(711, 7)
            };
            CollectionAssert.AreEqual(expected, blockDataList);
        }

        public string[] Meow ()
        {
            var blockDataList = new List<string>() { "", "", "", "", "", "", "" };
            return Enumerable.Range(0, blockDataList.Count)
    .Select(blockNumber => blockNumber.ToString())
    .ToArray();
        }

        [TestMethod]
        public void GodSaveTheLinq()
        {
            CollectionAssert.AreEqual(new string[] { "0", "1", "2", "3", "4", "5", "6" }, Meow());
        }
    }
}
