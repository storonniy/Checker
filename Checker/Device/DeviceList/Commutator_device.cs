using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using Checker.DeviceDrivers;
using Checker.Steps;
using Checker.Device;
using Checker.DeviceInterface;

namespace Checker.Devices
{
    class Commutator_device : IDeviceInterface
    {
        readonly Commutator Commutator;

        public Commutator_device(SerialPort serialPort)
        {
            Commutator = new Commutator(serialPort);
        }

        /// дописать OpenAllRelays
        public override DeviceResult DoCommand(Step step)
        {
            switch (step.Command)
            {
                case DeviceCommands.CloseRelays:
                    string[] closableRelays = step.Argument.Replace(" ", "").Split(',');
                    Commutator.CloseRelays(closableRelays);
                    // Проверим, всё ли замкнулось:
                    var actualClosedRelayNames = Commutator.GetClosedRelayNames();
                    bool isAllTheseRelaysClosed = false;
                    string relayName = "relay";
                    foreach (var relay in closableRelays)
                    {
                        isAllTheseRelaysClosed = actualClosedRelayNames.Contains(relay);
                        if (!isAllTheseRelaysClosed)
                        {
                            relayName = relay;
                            break;
                        }
                    }
                    if (isAllTheseRelaysClosed)
                    {
                        return DeviceResult.ResultOk("Замыкание реле прошло успешно");
                    }
                    else
                    {
                        return DeviceResult.ResultError($"ОШИБКА: реле {relayName} не замкнуто");
                    }
                case DeviceCommands.OpenRelays:
                    string[] breakableRelays = step.Argument.Replace(" ", "").Split(',');
                    Commutator.OpenRelays(breakableRelays);
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames1 = Commutator.GetClosedRelayNames();
                    bool anyOfTheseRelaysClosed = false;
                    var closedRelayName = "relay";
                    foreach (var relay in breakableRelays)
                    {
                        anyOfTheseRelaysClosed = actualClosedRelayNames1.Contains(relay);
                        if (anyOfTheseRelaysClosed)
                        {
                            closedRelayName = relay;
                            break;
                        }
                    }
                    if (!anyOfTheseRelaysClosed)
                    {
                        return DeviceResult.ResultOk("Размыкание реле прошло успешно");
                    }
                    return DeviceResult.ResultError($"ОШИБКА: реле {closedRelayName} замкнуто");
                case DeviceCommands.OpenAllRelays:
                    string[] breakableRelays1 = new string[] { "all" };
                    Commutator.OpenRelays(breakableRelays1);
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames2 = Commutator.GetClosedRelayNames();
                    bool anyOfTheseRelaysClosed2 = actualClosedRelayNames2[0] == "none";
                    if (anyOfTheseRelaysClosed2)
                    {
                        return DeviceResult.ResultOk("Размыкание реле прошло успешно");
                    }
                    return DeviceResult.ResultError("ОШИБКА: реле остались замкнутыми");
                case DeviceCommands.CheckClosedRelays:
                    string[] signalNames = step.AdditionalArg.Replace(" ", "").Split(',');
                    // Проверим, всё ли разомкнулось:
                    var actualClosedRelayNames3 = Commutator.GetClosedRelayNames();
                    bool isSignalExist = false;
                    foreach (var signal in signalNames)
                    {
                        isSignalExist = actualClosedRelayNames3.Contains(signal);
                        if (!isSignalExist)
                        {
                            break;
                        }
                    }
                    if (isSignalExist)
                    {
                        return DeviceResult.ResultOk($"Сигнал {signalNames[0]} присутствует");
                    }
                    return DeviceResult.ResultError($"Сигнал {signalNames[0]} отсутствует");
                case DeviceCommands.GetSignals:
                    string[] signalNames1 = step.AdditionalArg.Replace(" ", "").Split(',');
                    // Проверим, всё ли разомкнулось:
                    var actualSignals = Commutator.GetSignals();
                    bool isSignalExist1 = false;
                    foreach (var signal in signalNames1)
                    {
                        isSignalExist1 = actualSignals.Contains(signal);
                        if (!isSignalExist1)
                        {
                            break;
                        }
                    }
                    if (isSignalExist1)
                    {
                        return DeviceResult.ResultOk($"Сигнал {signalNames1[0]} присутствует");
                    }
                    return DeviceResult.ResultError($"Сигнал {signalNames1[0]} отсутствует");
                default:
                    return DeviceResult.ResultError($"Неизвестная команда {step.Command}");
            }
        }
    }
}