using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker;
using System.IO;
using System.IO.Ports;
using Checker.Devices;

namespace Checker.Steps
{
    class StepParser
    {
        private readonly DeviceInit deviceHandler;
        private readonly Step step;

        public StepParser(DeviceInit deviceHandler, Step step)
        {
            this.deviceHandler = deviceHandler;
            this.step = step;
        }
        
        public DeviceResult DoStep()
        {
            if (deviceHandler == null)
            {
                return DeviceResult.ResultError($"Устройство {step.DeviceName} не подключено");
            }
            return deviceHandler.ProcessDevice(step);
        }
    }
}