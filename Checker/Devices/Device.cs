using System.IO.Ports;

namespace Checker.Devices
{
    public struct Device
    {
        public Device(DeviceNames deviceName, SerialPort serialPort, string description)
        {
            Name = deviceName;
            SerialPort = serialPort;
            Description = description;
        }
        
        public SerialPort SerialPort { get; }
        public DeviceNames Name { get; }
        public string Description { get; }

        public override string ToString()
        {
            return $"{Name} {SerialPort.PortName} {SerialPort.BaudRate} {Description}";
        }
    }

    public enum DeviceStatus
    {
        Error,
        Ok,
        NotConnected
    }
    
    public struct DeviceResult
    {
        public DeviceStatus State;
        public string Description;

        public static DeviceResult ResultOk(string description) => new DeviceResult()
        {
            State = DeviceStatus.Ok,
            Description = description
        };

        public static DeviceResult ResultError(string description) => new DeviceResult()
        {
            State = DeviceStatus.Error,
            Description = $"*: {description}"
        };
        public static DeviceResult ResultNotConnected(string description) => new DeviceResult()
        {
            State = DeviceStatus.NotConnected,
            Description = description
        };
    }

    public enum DeviceCommands
    {
        // GDM
        GetVoltageACAndSave,
        // УСА
        SetCurrentLimit,
        SetPowerLimit,
        SetVoltageLimit,
        SetVoltage,
        SetCurrent,
        PowerOn,
        PowerOff,
        CloseRelays,
        OpenRelays,
        CheckClosedRelays,
        OpenAllRelays,
        GetSignals,
        GetVoltageDC,
        GetCurrentDC,
        CalculateCoefficient,
        GetVoltageRipple,
        // УСА_Т
        SetVoltageProtection,
        SetCurrentProtection,
        // ATH_8030
        SetCurrentControlMode,
        GetMaxCurrent,
        SetMaxCurrent,
        GetLoadCurrent,
        // PCI_1762
        ReadPCI1762Data,
        //
        CalculateCoefficient_UCAT,
        SetMeasurementToCurrent, // GDM
        SetMeasurementToVoltageDC,
        // AKIP_3407
        SetFrequency,
        // None
        Divide,
        Substract,
        Save,
        MultiplyAndSave,
        //
        Sleep,
        // ASBL
        SetLineDirToOutput,
        SetLineDirToInput,
        SetLineData,
        ClearLineData,
        ClearAll,
        GetCurrentDCAndSave,
        /// <summary>
        /// Keithley режим стабилизации напряжения
        /// </summary>
        SetVoltageSourceMode,
        GetVoltageAC,
        SetMeasurementToVoltageAC,
        GetClosedRelayNames,
        GetVoltageDCAndSave,
        GetLineState,
        GetResistance,
        SetMeasurementToResistance,
        CheckResistancesDifference,
        SetSignal,
        ClearSignal,
        SetDutyCycle
    }

    public enum DeviceNames
    {
        PSP_405,
        PSP_405_power,
        Commutator,
        GDM_78261,
        Keysight_34410,
        None,
        PSH_73610,
        PSH_73630,
        ATH_8030,
        PCI_1762,
        GDM_78341, // the same as GDM_78261
        PST_3201,
        PCI_1761_1,
        PCI_1761_2,
        PCI_1762_1,
        PCI_1762_2,
        PCI_1762_3,
        PCI_1762_4,
        PCI_1762_5,
        ASBL,
        AKIP_3407,
        Keithley2401_1,
        Keithley2401_2,
        MK,
        Simulator,
        GetResistance,
        PSH_73610_power
    }
}