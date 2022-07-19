using Checker.Devices;

namespace Checker.Steps
{
    class StepParser
    {
        private readonly DeviceInit deviceHandler;
        private readonly Steps.Step step;

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