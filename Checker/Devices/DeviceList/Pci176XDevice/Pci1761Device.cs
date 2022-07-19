using Advantech;

namespace Checker.Device.DeviceList.Pci176XDevice
{
    public class Pci1761Device : Pci176XDevice<Pci176X>
    {
        public Pci1761Device(string description) : base(new Pci1761(description))
        {
        }
    }
}