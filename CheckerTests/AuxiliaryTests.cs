using NUnit.Framework;
using Checker.Auxiliary;
namespace CheckerTests
{
    public class AuxiliaryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BoolComparing()
        {
            var bitState1 = true;
            var bitState2 = true;
            Assert.AreEqual(true, bitState1 == bitState2);
            bitState1 = false;
            bitState2 = false;
            Assert.AreEqual(true, bitState1 == bitState2);
            bitState1 = true;
            bitState2 = false;
            Assert.AreEqual(false, bitState1 == bitState2);
            bitState1 = false;
            bitState2 = true;
            Assert.AreEqual(false, bitState1 == bitState2);
        }

        [Test]
        public void ParseDoubleTest()
        {
            // Keithley, Akip
            ParseDoubleTest("+4.744283E-1", 0.4744283);
            ParseDoubleTest("3.000000E+06", 3000000);
            ParseDoubleTest("1.000000E+00", 1);
            // GDM
            ParseDoubleTest("+0.488E-4", 0.0000488);
            ParseDoubleTest("+1.181372E+6", 1181372);
            //PSH, PST, PSS
            ParseDoubleTest("1.5", 1.5);
            ParseDoubleTest("3.141", 3.141);
            ParseDoubleTest("8.4", 8.4);
        }

        private void ParseDoubleTest(string data, double expected)
        {
            var actual = SerialPortExtensions.ParseDouble(data);
            Assert.AreEqual(expected, actual);
        }
    }
}