using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Modbus.Device;

namespace Checker.DeviceDrivers.Aktakom
{
    public abstract class Modbus_device
    {//[01 10 0A 00 00 01 02 00 2B 4C 4F] >> [01 10 0A 00 00 01 02 11]
        SerialPort serialPort;
        IModbusSerialMaster master;
        byte slaveId;
        public Modbus_device(string portName)
        {
            serialPort = new SerialPort(portName);

            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            this.master = ModbusSerialMaster.CreateRtu(serialPort);
            this.slaveId = 1;
            //serialPort.Open();
        }

        public bool WriteHoldingRegInt(ushort address, ushort[] value)
        {
            master.WriteMultipleRegistersAsync(slaveId, address, value);     
            return true;
        }

        public bool WriteHoldingRegFloat(ushort address, float value)
        {
            var fData = BitConverter.GetBytes(value);
            ushort[] uData = new ushort[4];
            ushort firstByte = BitConverter.ToUInt16(fData, 2);
            ushort[] writtenValue = new ushort[] { firstByte, 0 };
            var ans = master.WriteMultipleRegistersAsync(slaveId, address, writtenValue);
            Thread.Sleep(1000);
            var current = ReadHoldingReg(address, 1);
            return ans.IsCompleted;
        }

        public float ReadHoldingReg(ushort address, ushort count)
        {
            ushort[] uShortAnswer = master.ReadHoldingRegisters(slaveId, address, count);
            byte[] byteAnswer = BitConverter.GetBytes(uShortAnswer[0]);
            var meow = byteAnswer;
            byteAnswer.Reverse();
            byte[] writtenData = new byte[4];
            meow.CopyTo(writtenData, 2);
            return BitConverter.ToSingle(writtenData, 0);
        }
    }
}
