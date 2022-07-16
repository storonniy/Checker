using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Checker.Devices;
using System.Threading;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Instek;

namespace Instek
{
    class Pst3201 : PshPstPss
    {
        public Pst3201(SerialPort serialPort) : base(serialPort, 3, "Pst3201")
        {

        }
    }
}