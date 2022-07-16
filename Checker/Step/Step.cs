using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO.Ports;
using System.Globalization;
using Checker.Devices;

namespace Checker.Steps
{
    public struct StepsInfo
    {
        public Dictionary<string, Dictionary<string, List<Step>>> VoltageSupplyModesDictionary { get; set; }
        public Dictionary<string, Dictionary<string, List<Step>>> ModesDictionary { get; set; }
        public List<Step> EmergencyStepList { get; set; }
        //public DeviceInit DeviceHandler;
        public Dictionary<string, List<Step>> StepsDictionary { get; set; }
        public int StepNumber { get; }
        public List<Devices.Device> DeviceList { get; set; }
        public string ProgramName { get; set; }
    }

    public class Step
    {
        public string AdditionalArg { get; }
        public int Channel { get;}
        public DeviceNames DeviceName { get; }
        public DeviceCommands Command { get; }
        public string Argument { get; }
        public string Description { get; }
        public double LowerLimit { get; }
        public double UpperLimit { get; }
        public bool ShowStep { get; }
        
        public Step(DeviceNames deviceName, DeviceCommands deviceCommand, string argument, string additionalArg,
            string description, double lowerLimit, double upperLimit, int channel, bool showStep)
        {
            DeviceName = deviceName;
            Command = deviceCommand;
            Argument = argument;
            AdditionalArg = additionalArg;
            Description = description;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            Channel = channel;
            ShowStep = showStep;
        }
        
        public Step(DeviceNames deviceName, DeviceCommands command) : this(deviceName, command, "", "", "", 0, 0, -1, false) {}

        private Step (DataRow row)
        {
            var command = row["command"].ToString();
            if (!Enum.TryParse(command, out DeviceCommands parsedDeviceCommand))
                throw new FormatException($"Команда не найдена: {command}");
            var deviceName = row["device"].ToString();
            if (!Enum.TryParse(deviceName, out DeviceNames parsedDeviceName))
                throw new FormatException($"Тип устройства не найден: {deviceName}");
            DeviceName = parsedDeviceName;
            Command = parsedDeviceCommand;
            Argument = row["argument"].ToString();
            Description = row["description"].ToString();
            Channel = int.Parse(row["channel"].ToString());
            LowerLimit = double.Parse(row["lowerLimit"].ToString(), NumberStyles.Float);
            UpperLimit = double.Parse(row["upperLimit"].ToString(), NumberStyles.Float);
            ShowStep = Convert.ToBoolean(row["showStep"]);
            if (Channel > 0)
                Description = $"Канал {Channel}: {Description}";
            AdditionalArg = row["additionalArg"].ToString();
        }

        private static SerialPort GetSerialPort(string portName, int baudRate)
        {
            var serialPort = new SerialPort()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DtrEnable = true,
                RtsEnable = true,
                Handshake = Handshake.None,
                ReadTimeout = 2000,
                WriteTimeout = 2500
            };
            return serialPort;
        }

        private static string GetProgramName(DataSet dataSet)
        {
            var table = dataSet.Tables["ProgramName"];
            var programName = table.Rows[0]["ProgramName"].ToString();
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return programName;
        }

        private static Dictionary<string, Dictionary<string, List<Step>>> GetModeStepDictionary(Dictionary<string, List<string>> modesDictionary, IReadOnlyDictionary<string, List<Step>> allStepsDictionary)
        {
            var modeStepDictionary = new Dictionary<string, Dictionary<string, List<Step>>>();
            foreach (var modeName in modesDictionary.Keys)
            {
                var stepsDictionary = new Dictionary<string, List<Step>>();
                var tableNames = modesDictionary[modeName];
                foreach (var tableName in tableNames)
                {
                    if (tableName.Split(' ').Length > 1)
                        stepsDictionary.Add(tableName, allStepsDictionary[$"'{tableName}'"]);
                    else
                        stepsDictionary.Add(tableName, allStepsDictionary[tableName]);
                }
                modeStepDictionary.Add(modeName, stepsDictionary);
            }
            return modeStepDictionary;
        }

        private static List<Devices.Device> GetDeviceList(DataSet dataSet)
        {
            var deviceList = new List<Devices.Device>();
            foreach (DataRow row in dataSet.Tables["DeviceInformation"].Rows)
            {
                var deviceName = row["device"].ToString();
                var portName = row["portName"].ToString();
                var baudRate = int.Parse(row["baudRate"].ToString());
                var description = row["description"].ToString();
                var device = new Devices.Device();
                device.SerialPort = GetSerialPort(portName, baudRate);
                device.Description = description;
                if (!Enum.TryParse(deviceName, out DeviceNames dev))
                    throw new ArgumentException($"Устройство {deviceName} не найдено в списке доступных устройств {description}");
                device.Name = dev;
                deviceList.Add(device);
            }
            dataSet.Tables.Remove(dataSet.Tables["DeviceInformation"]);
            return deviceList;
        }

        private static Dictionary<string, List<string>> GetVoltageSupplyModesDictionary(DataSet dataSet)
        {
            var modesDictionary = new Dictionary<string, List<string>>();
            var table = dataSet.Tables["VoltageSupply"];
            foreach (DataRow row in table.Rows)
            {
                var checkingMode = row["VoltageSupplyMode"].ToString();
                var names = row["TableNames"].ToString();
                List<string> tableNames = names.Split(';').ToList();
                for (int i = 0; i < tableNames.Count; i++)
                {
                    while (tableNames[i].StartsWith(" "))
                        tableNames[i] = tableNames[i].Remove(0, 1);
                }
                modesDictionary.Add(checkingMode, tableNames);
            }
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return modesDictionary;
        }

        private static Dictionary<string, List<string>> GetModesDictionary (DataSet dataSet)
        {
            var modesDictionary = new Dictionary<string, List<string>>();
            var table = dataSet.Tables["Settings"];
            foreach (DataRow row in table.Rows)
            {
                var checkingMode = row["CheckingMode"].ToString();
                List<string> tableNames = row["TableNames"].ToString().Split(';').ToList();
                for (int i = 0; i < tableNames.Count; i++)
                {
                    while (tableNames[i].StartsWith(" "))
                        tableNames[i] = tableNames[i].Remove(0, 1);
                }
                modesDictionary.Add(checkingMode, tableNames);
            }
            dataSet.Tables.Remove(dataSet.Tables[table.TableName]);
            return modesDictionary;
        }

        public static StepsInfo GetStepsInfo(DataSet dataSet)
        {
            var programName = GetProgramName(dataSet);
            var voltageModesDictionary = GetVoltageSupplyModesDictionary(dataSet);
            var modesDictionary = GetModesDictionary(dataSet);
            var deviceList = GetDeviceList(dataSet);
            var emergencyStepsDictionary = GetEmergencyStepsDictionary(dataSet);
            var stepsDictionary = GetStepsDictionary(dataSet);
            var voltageSupplyDictionary = GetModeStepDictionary(voltageModesDictionary, stepsDictionary);
            var modeStepDictionary = GetModeStepDictionary(modesDictionary, stepsDictionary);
            var info = new StepsInfo()
            {
                VoltageSupplyModesDictionary = voltageSupplyDictionary,
                ModesDictionary = modeStepDictionary,
                StepsDictionary = stepsDictionary,
                EmergencyStepList = emergencyStepsDictionary["EmergencyBreaking"],
                //DeviceHandler = new DeviceInit(deviceList),
                DeviceList = deviceList,
                ProgramName = programName
            };
            return info;
        }

        private static Dictionary<string, List<Step>> GetStepsDictionary(DataSet dataSet)
        {
            var dictionary = new Dictionary<string, List<Step>>();           
            foreach (DataTable table in dataSet.Tables)
            {
                var stepList = new List<Step>();
                var tableName = table.ToString();
                foreach (DataRow row in table.Rows)
                {
                    var step = new Step(row);
                    stepList.Add(step);
                }
                dictionary.Add(tableName, stepList);
            }
            return dictionary;
        }

        private static Dictionary<string, List<Step>> GetEmergencyStepsDictionary(DataSet dataSet)
        {
            var tableEmergencyBreaking = dataSet.Tables["EmergencyBreaking"];
            dataSet.Tables.Remove(dataSet.Tables[tableEmergencyBreaking.TableName]);
            var dataSetEmergency = new DataSet();
            dataSetEmergency.Tables.Add(tableEmergencyBreaking);
            var dictionary = GetStepsDictionary(dataSetEmergency);
            return dictionary;
        }
    }
}
