using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using FTD2XX_NET;
using Checker.DeviceDrivers;
using System.IO.Ports;
using Checker.Device.DeviceList;

namespace Tester
{
    [TestClass]
    public class ASBLTests
    {

        public ASBLTests()
        {

        }

        [TestMethod]
        public void Meow ()
        {
            uint writtenData = 12;
            uint bitNumber = 3;
            var actualBitState = (writtenData & (1 << (int)bitNumber)) >> (int)bitNumber;
            Assert.AreEqual(1, actualBitState);
        }

        public void TestParseLineNumbers(uint[] expected, string argument)
        {
            var actual = AsblDevice.GetLineNumbers(argument);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseLineNumbersTest_OneLineWithoutSplitter()
        {
            TestParseLineNumbers(new uint[] { 1 }, "1");
            TestParseLineNumbers(new uint[] { 1 }, "1 ");
            TestParseLineNumbers(new uint[] { 1 }, "1;");
            TestParseLineNumbers(new uint[] { 1 }, "1; ");
            TestParseLineNumbers(new uint[] { 1, 2, 3, 4 }, "1; 2; 3; 4");
            TestParseLineNumbers(new uint[] { 1, 2, 3, 4 }, "1; 2; 3; 4 ");
            TestParseLineNumbers(new uint[] { 1, 2, 3, 4 }, "1; 2; 3; 4;");
            TestParseLineNumbers(new uint[] { 1, 2, 3, 4 }, "1;2;3;4");
            TestParseLineNumbers(new uint[] { 1, 2, 3, 4 }, "1;2;3;4;");
        }


        /// <summary>
        /// Показывает, что массив - это ссылочный тип
        /// </summary>
        [TestMethod]
        public void ChangeArrayTest()
        {
            var bytes = new byte[] { 5, 1, 2, 4, 3};
            var extpectedBytes = new byte[] { 0, 1, 2, 3, 4 };
            ChangeArray(bytes);
            CollectionAssert.AreEqual(extpectedBytes, bytes);
        }

        public void ChangeArray(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)i;
        }

        [TestMethod]
        public void ExponentiateTest()
        {
            Assert.AreEqual((uint)4, Line.GetPowerOfTwo(2));
            Assert.AreEqual((uint)8, Line.GetPowerOfTwo(3));
            Assert.AreEqual((uint)512, Line.GetPowerOfTwo(9));
            Assert.AreEqual((uint)2048, Line.GetPowerOfTwo(11));
        }

        [TestMethod]
        public void ClearLineTest()
        {
            uint currentData = 24965;
            uint newData = currentData - (currentData & Line.GetPowerOfTwo(2));
            Assert.AreEqual("110000110000001", Convert.ToString(newData, 2));
            newData = currentData - (currentData & Line.GetPowerOfTwo(3));
            Assert.AreEqual("110000110000101", Convert.ToString(newData, 2));
        }

        [TestMethod]
        public void SetLineTest()
        {
            uint currentData = 24965;
            var newData = currentData | Line.GetPowerOfTwo(3);
            Assert.AreEqual("110000110001101", Convert.ToString(newData, 2));
            newData = currentData | Line.GetPowerOfTwo(2);
            Assert.AreEqual("110000110000101", Convert.ToString(newData, 2));
        }

        [TestMethod]
        public void TestWriteRead()
        {
            TestASBL(0, 10);
            TestASBL(1048565, 1048575);
        }

        public void TestASBL(uint start, uint end)
        {
            Asbl asbl = new Asbl();
            for (uint writeData = start; writeData < end + 1; writeData++)
            {
                asbl.WriteData(Line.ADR_DIR_REG1, writeData);
                uint readData = asbl.ReadData(Line.ADR_DIR_REG1);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG2, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG2);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG3, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG3);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG4, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG4);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG5, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG5);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DIR_REG6, writeData);
                readData = asbl.ReadData(Line.ADR_DIR_REG6);
                Assert.AreEqual(writeData, readData);
            }
        }

        public void TestDataRegisters(uint start, uint end)
        {
            Asbl asbl = new Asbl();
            for (uint writeData = start; writeData < end + 1; writeData++)
            {
                asbl.WriteData(Line.ADR_DATA_REG1, writeData);
                uint readData = asbl.ReadData(Line.ADR_DATA_REG1);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG2, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG2);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG3, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG3);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG4, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG4);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG5, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG5);
                Assert.AreEqual(writeData, readData);
                asbl.WriteData(Line.ADR_DATA_REG6, writeData);
                readData = asbl.ReadData(Line.ADR_DATA_REG6);
                Assert.AreEqual(writeData, readData);
            }
        }

        [TestMethod]
        public void TestClear()
        {
            Asbl asbl = new Asbl();
            asbl.ClearAll();
        }

        [TestMethod]
        public void TestLines()
        {
            Asbl asbl = new Asbl();
            asbl.ClearAll();
            asbl.SetLineData(81);
        }

        [TestMethod]
        public void TestAllLines()
        {
            Asbl asbl = new Asbl();
            for (uint i = 1; i < 121; i++)
            {
                asbl.SetLineDirection(i);
                asbl.SetLineData(i);
            }
        }

        [TestMethod]
        public void TestWithDiffDirections()
        {
            Asbl asbl = new Asbl();
            asbl.WriteData(Line.ADR_DIR_REG1, 0xFFFF5);
            Assert.AreEqual((uint)0xA, asbl.ReadData(Line.ADR_DATA_REG1));
            asbl.ClearAll();
            asbl.ClearLineDirection(2);
            asbl.ClearLineDirection(4);
            Assert.AreEqual((uint)0xA, asbl.ReadData(Line.ADR_DATA_REG1));
        }

        [TestMethod]
        public void TestLines_1()
        {
            Asbl asbl = new Asbl();
            asbl.WriteData(Line.ADR_DIR_REG1, 4);
            asbl.SetLineData(3);
        }

        [TestMethod]
        public void TestLines_2()
        {
            Asbl asbl = new Asbl();
            asbl.SetLineDirection(3);
            asbl.SetLineData(3);
        }

        [TestMethod]
        [ExpectedException(typeof(LineIsSetToReceiveException))]
        public void TestLinesWith0Direction_1()
        {
            Asbl asbl = new Asbl();
            asbl.ClearLineDirection(3);
            asbl.SetLineData(3);
        }

        [TestMethod]
        [ExpectedException(typeof(LineIsSetToReceiveException))]
        public void TestLinesWith0Direction()
        {
            Asbl asbl = new Asbl();
            asbl.WriteData(Line.ADR_DIR_REG1, 0);
            asbl.SetLineData(3);
            asbl.WriteData(Line.ADR_DIR_REG1, 0x1F5);
            asbl.SetLineData(3);
        }

        [TestMethod]
        public void TestLinesParameters()
        {
            TestNewLineParameters((uint)1, Line.ADR_DIR_REG1, Line.ADR_DATA_REG1);
            TestNewLineParameters((uint)21, Line.ADR_DIR_REG2, Line.ADR_DATA_REG2);
            TestNewLineParameters((uint)41, Line.ADR_DIR_REG3, Line.ADR_DATA_REG3);
            TestNewLineParameters((uint)61, Line.ADR_DIR_REG4, Line.ADR_DATA_REG4);
            TestNewLineParameters((uint)81, Line.ADR_DIR_REG5, Line.ADR_DATA_REG5);
            TestNewLineParameters((uint)101, Line.ADR_DIR_REG6, Line.ADR_DATA_REG6);
        }

        public void TestNewLineParameters(uint firstLineNumber, uint expectedDirectionRegister, uint expectedDataRegister)
        {
            uint linePosition = 0;
            for (uint lineNumber = firstLineNumber; lineNumber < firstLineNumber + 19; lineNumber++)
            {
                var line = new Line(lineNumber, new Asbl());
                Assert.AreEqual(line.DirectionRegister, expectedDirectionRegister);
                Assert.AreEqual(line.DataRegister, expectedDataRegister);
                Assert.AreEqual(linePosition, line.bitNumber);
                linePosition++;
            }
        }

    }
}
