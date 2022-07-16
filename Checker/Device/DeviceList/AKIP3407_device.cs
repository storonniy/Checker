﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Globalization;
using Checker.Auxiliary;
using Checker.DeviceDrivers;
using Checker.Steps;
using static Checker.Auxiliary.UnitValuePair;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    class AKIP3407_device : IDeviceInterface
    {
        int delay = 1000;
        private Akip3407 akip3407;
        public AKIP3407_device (SerialPort serialPort)
        {
            serialPort.NewLine = "\r";
            akip3407 = new Akip3407(serialPort);
        }
        public override DeviceResult DoCommand (Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.SetVoltage:
                    return SetVoltage(step, akip3407.SetVoltage);
                case DeviceCommands.SetFrequency:
                    var actualFrequency = akip3407.SetFrequency(step.Argument);
                    return GetResult("Установлена частота", step, UnitValuePair.UnitType.Frequency, actualFrequency);
                case DeviceCommands.PowerOn:
                    return PowerOn(step, akip3407.PowerOn);
                case DeviceCommands.PowerOff:
                    return PowerOff(step, akip3407.PowerOff);
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}