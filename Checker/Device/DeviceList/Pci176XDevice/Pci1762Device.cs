using Advantech;
using Checker.DeviceDrivers;

namespace Checker.Device.DeviceList.Pci176XDevice
{
    public class Pci1762Device : Pci176XDevice<Pci176X>
    {
        public Pci1762Device (string description) : base(new Pci1762(description))
        {
            
        }
    }
}
