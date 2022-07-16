using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Instek;

namespace Instek
{
    public class Psh73610 : PshPstPss
    {
        public Psh73610(SerialPort serialPort) : base(serialPort, 1, "Psh73610")
        {
            
        }
    }

}
