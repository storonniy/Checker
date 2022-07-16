using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.DeviceDrivers;
using System.IO.Ports;
using Checker.Device.DeviceList;

namespace Tester
{
    [TestClass]
    public class PSHTests
    {
        Psh73610 psh3610 = new Psh73610(new SerialPort("COM24", 9600));

        [TestMethod]
        public void SetVoltageTest()
        {
            SetVoltage(0.1);
            SetVoltage(0.2);
            SetVoltage(1);
            SetVoltage(1.1);
            SetVoltage(10.456);
        }

        public void SetVoltage(double voltage)
        {
            var actualVoltage = psh3610.SetVoltage(voltage);
            Assert.AreEqual(voltage, actualVoltage, 0.01);
        }

        [TestMethod]
        public void SetCurrentLimitTest()
        {
            SetCurrentLimit(0.01);
            SetCurrentLimit(0.03);
            SetCurrentLimit(0.04);
            SetCurrentLimit(0.1);
            SetCurrentLimit(0.5);
            SetCurrentLimit(0.9);
            SetCurrentLimit(0.01);
        }

        public void SetCurrentLimit(double current)
        {
            var actualCurrent = psh3610.SetCurrentLimit(current);
            Assert.AreEqual(current, actualCurrent, 0.01);
        }
    }
}
