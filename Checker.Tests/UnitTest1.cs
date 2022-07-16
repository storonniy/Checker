using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCA;
using USA;
using System.IO.Ports;
using UCA.DeviceDrivers;
using System.Threading;


namespace UCA.Tests
{
    [TestClass]
    public class UnitTest1

    {
        private readonly static int delay = 100;
        private static readonly SerialPort serialPort = new SerialPort
        {
            PortName = "COM6",
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DtrEnable = true,
            RtsEnable = true,
            Handshake = Handshake.None,
            ReadTimeout = 10000,
            WriteTimeout = 2500
        };


        [TestMethod]
        public void TestPSP()
        {
            PSP405 psp = new PSP405(serialPort);
            psp.SetVoltage(9);
            var actualVoltage = psp.GetOutputVoltage(); 
            Assert.AreEqual(9, actualVoltage);
        }

        [TestMethod]
        public void TestTestr1r2()
        {
            Commutator comm = new Commutator(serialPort);
            var actualNames = comm.GetClosedRelayNames();
            var expectedNames = new string[] { "none" };
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [TestMethod]
        public void Test()
        {
            serialPort.Open();
            serialPort.WriteLine("*idn?");
            Thread.Sleep(1000);
            var answer = serialPort.ReadLine();
            var request = "meow";
            Assert.AreEqual(request, answer);
            serialPort.Close();
        }
        /*
        [TestMethod]
        public void Test()
        {
            for (int i =0; i < 104; i++)
            {
                string relayName = "r" + (i + 1).ToString();
                CloseRelays__Test(relayName);
                GetClosedRelayNames__Test(relayName);
                OpenRelays__Test(relayName);
            }
        }

        public void GetClosedRelayNames__Test(params string[] expectedClosedRelay)
        {
            serialPort.Open();
            var actualClosedNames = UCASwitchingAdapter.GetClosedRelayNames(serialPort);
            serialPort.Close();
            CollectionAssert.AreEqual(expectedClosedRelay, actualClosedNames);
        }
        */
        /*
        public void OpenRelays__Test(params string[] relayName)
        {
            var expectedState = "*OpenRelays:OK";
            serialPort.Open();
            var actualState = UCASwitchingAdapter.OpenRelays(serialPort, relayName);
            serialPort.Close();
            Assert.AreEqual(expectedState, actualState);
        }
        public void CloseRelays__Test(params string[] expectedClosedRelay)
        {
            serialPort.Open();
            var actualState = UCASwitchingAdapter.CloseRelays(serialPort, expectedClosedRelay);
            var expectedState = "*CloseRelays:OK";
            serialPort.Close();
            Assert.AreEqual(expectedState, actualState);
        }
        
        [TestMethod]
        public void NoneOfRelaysClosed ()
        {
            GetClosedRelayNames__Test("none");
        }

        [TestMethod]
        public void r1Closed()
        {
            GetClosedRelayNames__Test("r1");
        }


        [TestMethod]
        public void CloseRelaysAndCheck()
        {
            string[] allRelayNames = new string[99];
            for (int relayNumber = 1; relayNumber < 100; relayNumber++)
            {
                string relayName = "r" + relayNumber.ToString();
                allRelayNames[relayNumber - 1] = relayName;
                CloseRelays__Test(relayName);
                string[] allRelayNamesActual = new string[relayNumber];
                for (var i = 0; i < relayNumber; i++)
                    if (allRelayNames[i] != null)
                        allRelayNamesActual[i] = allRelayNames[i];
                serialPort.Open();
                var expectedNames = UCASwitchingAdapter.GetClosedRelayNames(serialPort);
                CollectionAssert.AreEqual(allRelayNamesActual, expectedNames);
                serialPort.Close();
                //GetClosedRelayNames__Test(allRelayNames);
            }
        }

        [TestMethod]
        public void OpenAllRelays()
        {
            var expectedState = "*OpenRelays:OK";
            serialPort.Open();
            var namesOfClosedRelays = UCASwitchingAdapter.GetClosedRelayNames(serialPort);
            var actualState = UCASwitchingAdapter.OpenRelays(serialPort, namesOfClosedRelays);
            serialPort.Close();
            Assert.AreEqual(expectedState, actualState);
        }

        public string[] GetAllRelayNames()
        {
            string[] allRelayNames = new string[100];
            for (int relayNumber = 0; relayNumber < 100; relayNumber++)
            {
                allRelayNames[relayNumber] = "r" + (relayNumber + 1).ToString();
            }
            return allRelayNames;
        }

        [TestMethod]
        public void CheckIfAllRelayClosed()
        {
            var expectedNames = GetAllRelayNames();
            serialPort.Open();
            var actualNames = UCASwitchingAdapter.GetClosedRelayNames(serialPort);
            serialPort.Close();
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [TestMethod]
        public void CloseRelaysAndCheck__()
        {
            string[] allRelayNames = new string[99];
            for (int relayNumber = 1; relayNumber < 100; relayNumber++)
            {
                string relayName = "r" + relayNumber.ToString();
                allRelayNames[relayNumber - 1] = relayName;
                CloseRelays__Test(relayName);
                string[] allRelayNamesActual = new string[relayNumber];
                for (var i = 0; i < relayNumber; i++)
                    if (allRelayNames[i] != null)
                        allRelayNamesActual[i] = allRelayNames[i];
                serialPort.Open();
                CollectionAssert.AreEqual(allRelayNamesActual, UCASwitchingAdapter.GetClosedRelayNames(serialPort));
                serialPort.Close();
                //GetClosedRelayNames__Test(allRelayNames);
            }
        }

        [TestMethod]
        public void TestTestr1r2()
        {
            serialPort.Open();
            var actualNames = UCASwitchingAdapter.GetClosedRelayNames(serialPort);
            var expectedNames = new string[] { "r1", "r2" };
            serialPort.Close();
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        */
        /*
        [TestMethod]
        public void Test2()
        {
            serialPort.Open();
            var actualState = UCASwitchingAdapter.OpenRelays(serialPort, "r1", "r2");
            var expectedState = "*OpenRelays:OK";
            serialPort.Close();
            Assert.AreEqual(expectedState, actualState);
        }
        */
        /*
        [TestMethod]
        public void GetStringFromArray_NormalSituation()
        {
            var array = new[] { "K83", "Meow", "DC123" };
            var expectedString = "K83, Meow, DC123";
            var actualString = UCACheckingFunc.GetStringFromArray(array);
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void GetStringFromArray_EmptyArray()
        {
            var array = new string[0];
            var expectedString = "";
            var actualString = UCACheckingFunc.GetStringFromArray(array);
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void GetStringFromArray_ArrayOfLength1()
        {
            var array = new[] { "DC42" };
            var expectedString = "DC42";
            var actualString = UCACheckingFunc.GetStringFromArray(array);
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void DCSignalsControl_NormalSituation()
        {
            // Штатная ситуация
            var expectedToCloseRelayNames = new[] { "K93", "DC3" };
            var actualClosedRelayNames = new[] { "K93", "DC3" };
            var actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            var expectedMessage = "Done";
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public void DCSignalsControl_FewClosedRelays()
        {
            // Замкнулось больше или меньше реле, нежели ожидалось
            var expectedToCloseRelayNames = new[] { "K93", "DC3" };
            var actualClosedRelayNames = new[] { "K93" };
            var actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            var actualRelayNamesString = UCACheckingFunc.GetStringFromArray(actualClosedRelayNames);
            var expectedRelayNamesString = UCACheckingFunc.GetStringFromArray(expectedToCloseRelayNames);
            var expectedMessage = "Ошибка в контроле стыковки. Замкнулись реле " + actualRelayNamesString + ". Ожидалось замыкание реле " + expectedRelayNamesString + ".\n";
            Assert.AreEqual(expectedMessage, actualMessage);

            expectedToCloseRelayNames = new[] { "K93", "DC3" };
            actualClosedRelayNames = new[] { "" };
            actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            actualRelayNamesString = UCACheckingFunc.GetStringFromArray(actualClosedRelayNames);
            expectedRelayNamesString = UCACheckingFunc.GetStringFromArray(expectedToCloseRelayNames);
            expectedMessage = "Ошибка в контроле стыковки. Замкнулись реле " + actualRelayNamesString + ". Ожидалось замыкание реле " + expectedRelayNamesString + ".\n";
            Assert.AreEqual(expectedMessage, actualMessage);

            expectedToCloseRelayNames = new[] { "K93", "DC3" };
            actualClosedRelayNames = new string[0];
            actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            actualRelayNamesString = UCACheckingFunc.GetStringFromArray(actualClosedRelayNames);
            expectedRelayNamesString = UCACheckingFunc.GetStringFromArray(expectedToCloseRelayNames);
            expectedMessage = "Ошибка в контроле стыковки. Замкнулись реле " + actualRelayNamesString + ". Ожидалось замыкание реле " + expectedRelayNamesString + ".\n";
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public void DCSignalsControl_TheWrongRelaysAreClosed()
        {
            // Замкнулись не те реле
            var expectedToCloseRelayNames = new[] { "K93", "DC3" };
            var actualClosedRelayNames = new[] { "K93", "DC5" };
            var relayName = "DC3";
            var actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            var actualClosedRelayNamesString = UCACheckingFunc.GetStringFromArray(actualClosedRelayNames);
            var expectedMessage = "Ошибка в контроле стыковки. Не замкнуто реле " + relayName + ". В данный момент замкнуты реле " + actualClosedRelayNamesString + ".\n";
            Assert.AreEqual(expectedMessage, actualMessage);

            expectedToCloseRelayNames = new[] { "L93", "DC3" };
            actualClosedRelayNames = new[] { "L90", "DC5" };
            relayName = "L93";
            actualMessage = UCACheckingFunc.DCSignalsControl(actualClosedRelayNames, expectedToCloseRelayNames);
            actualClosedRelayNamesString = UCACheckingFunc.GetStringFromArray(actualClosedRelayNames);
            expectedMessage = "Ошибка в контроле стыковки. Не замкнуто реле " + relayName + ". В данный момент замкнуты реле " + actualClosedRelayNamesString + ".\n";
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public void CheckIfTheRelaysAreClosed()
        {
            var closedRelayNames = new[] {"K101", "K98", "K66", "DC1" };
            var relayNames = new[] { "K98", "K66", "K101" };
            var expected = true;
            var actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K101", "K98", "K66" };
            relayNames = new[] { "K98", "K66", "K101" };
            expected = true;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K101", "K98", "K66" };
            relayNames = new[] { "K98", "K66" };
            expected = true;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K101", "K98", "K66" };
            relayNames = new[] { "K98" };
            expected = true;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K101", "K94", "K66" };
            relayNames = new[] { "K98" };
            expected = false;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K101" };
            relayNames = new[] { "K101", "K94", "K66" };
            expected = false;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new[] { "K10" };
            relayNames = new[] { "K101", "K94", "K66" };
            expected = false;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new string[0];
            relayNames = new[] { "K101", "K94", "K66" };
            expected = false;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckIfTheRelaysAreClosed_RelayNamesAreNotSpecified()
        {
            var closedRelayNames = new[] { "K101", "K98", "K66", "DC1" };
            var relayNames = new string[0];
            var expected = false;
            var actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);

            closedRelayNames = new string[0];
            relayNames = new string[0];
            expected = false;
            actual = UCACheckingFunc.CheckIfTheRelaysAreClosed(closedRelayNames, relayNames);
            Assert.AreEqual(expected, actual);
        }
        */
    }
}
