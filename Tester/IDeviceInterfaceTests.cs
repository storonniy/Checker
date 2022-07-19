using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Devices;
using Checker.Device;
using static Checker.Auxiliary.UnitValuePair;

namespace Tester
{
    [TestClass]
    public class IDeviceInterfaceTests
    {
        [TestMethod]
        public void Test()
        {
            for (double voltage = -1000.0; voltage < 1000.0; voltage += 0.01)
            {
                TestGetResultOfSetting("", UnitType.Voltage, voltage * 0.99, voltage);
            }
            TestGetResultOfSetting("", UnitType.Voltage, 1.2, 1.2);
            TestGetResultOfSetting("", UnitType.Voltage, 1.21, 1.2);
            TestGetResultOfSetting("", UnitType.Voltage, -1.21, -1.2);
            TestGetResultOfSetting("", UnitType.Voltage, 1.19, 1.2);
            TestGetResultOfSetting("", UnitType.Voltage, -1.19, -1.2);
        }

        public void TestGetResultOfSetting(string message, UnitType unitType, double value, double expectedValue)
        {
            var deviceResult = IDeviceInterface.GetResultOfSetting("", unitType, value, expectedValue);
            Assert.AreEqual(DeviceResult.ResultOk($"{message}: {new UnitValuePair(value, unitType)}"), deviceResult);
        }
    }
}
