using System;
using System.Collections.Generic;
using System.IO;
using Checker.Device.DeviceList;
using Checker.Device.DeviceList.Pci176XDevice;
using Checker.Steps;

namespace Checker.Devices
{
    public class DeviceInit
    {
        public readonly Dictionary<DeviceNames, IDeviceInterface> Devices;
        public DeviceInit(List<Device> deviceList)
        {
            Devices = new Dictionary<DeviceNames, IDeviceInterface>();
            foreach (var device in deviceList)
            {
                IDeviceInterface newDevice = null;
                switch (device.Name)
                {
                    case DeviceNames.PSP_405:
                    case DeviceNames.PSP_405_power:
                        newDevice = new Psp405Device(device.SerialPort);
                        break;
                    case DeviceNames.GDM_78261:
                        newDevice = new Gdm78261Device(device.SerialPort);
                        break;
                    case DeviceNames.Keithley2401_2:
                    case DeviceNames.Keithley2401_1:
                        newDevice = new Keithley2401Device(device.SerialPort);
                        break;
                    case DeviceNames.Commutator:
                        newDevice = new Commutator_device(device.SerialPort);
                        break;
                    case DeviceNames.Simulator:
                        newDevice = new Simulator_device(device.SerialPort);
                        break;
                    case DeviceNames.None:
                        newDevice = new None();
                        break;
                    // УСА_Т
                    case DeviceNames.PSH_73610:
                    case DeviceNames.PSH_73630:
                    case DeviceNames.PSH_73610_power:
                        newDevice = new Psh73610Device(device.SerialPort);
                        break;
                    case DeviceNames.ATH_8030:
                        newDevice = new ATH8030_device(device.SerialPort.PortName);
                        break;
                    // УПД
                    case DeviceNames.GDM_78341:
                        newDevice = new Gdm78261Device(device.SerialPort);
                        break;
                    case DeviceNames.PCI_1761_1:
                    case DeviceNames.PCI_1761_2:
                        newDevice = new Pci1761Device(device.Description);
                        break;
                    case DeviceNames.PCI_1762:
                    case DeviceNames.PCI_1762_1:
                    case DeviceNames.PCI_1762_2:
                    case DeviceNames.PCI_1762_3:
                    case DeviceNames.PCI_1762_4:
                    case DeviceNames.PCI_1762_5:
                        newDevice = new Pci1762Device(device.Description);
                        break;
                    case DeviceNames.PST_3201:
                        newDevice = new Pst3201Device(device.SerialPort);
                        break;
                    case DeviceNames.AKIP_3407:
                        newDevice = new AKIP3407_device(device.SerialPort);
                        break;
                    case DeviceNames.MK:
                        newDevice = new MK_device();
                        break;
                    case DeviceNames.ASBL:
                        newDevice = new AsblDevice();
                        break;
                }
                Devices.Add(device.Name, newDevice);
            }
        }

        public DeviceResult ProcessDevice(Step step)
        {
            try
            {
                var device = Devices[step.DeviceName];
                return device == null 
                    ? DeviceResult.ResultNotConnected($"NOT_CONNECTED: Устройство {step.DeviceName} не инициализировано") 
                    : device.DoCommand(step);
            }
            catch (IOException)
            {
               return DeviceResult.ResultNotConnected($"NOT_CONNECTED: Порт {step.DeviceName} закрыт");
            }
            catch (InvalidOperationException)
            {
                return DeviceResult.ResultNotConnected($"NOT_CONNECTED: Порт {step.DeviceName} закрыт");
            }
            catch (KeyNotFoundException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
            catch (FormatException ex)
            {
                return DeviceResult.ResultError($"{step.DeviceName} : {step.Command} : {ex.Message}");
            }
        }
    }
}