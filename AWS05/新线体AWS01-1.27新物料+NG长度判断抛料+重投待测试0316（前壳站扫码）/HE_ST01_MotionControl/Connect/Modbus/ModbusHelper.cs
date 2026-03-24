using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;

namespace Handler.Connect.Modbus
{
    /// <summary>
    /// ModbusHelper帮助类，地址都为十进制
    /// </summary>
    public class ModbusHelper
    {
        /// <summary>
        /// 私有串口实例
        /// </summary>
        public SerialPort CurrentSerialPort { get; private set; } = new SerialPort();

        /// <summary>
        /// 私有ModbusRTU主站字段
        /// </summary>
        public IModbusMaster master { get; private set; }

        public byte SlaveID { get; private set; }    //从站ID

        public ModbusHelper(byte slaveId,SerialPort serialPort)
        {
            SlaveID = slaveId;
            this.CurrentSerialPort = serialPort;
            master = ModbusSerialMaster.CreateRtu(serialPort);
            master.Transport.ReadTimeout = 3000;
            master.Transport.WriteTimeout = 3000;
        }

        /// <summary>
        /// 写入单个线圈 写入值到DO 功能码05
        /// </summary>
        public void WriteSingleCoil(ushort coilAddress, bool value)
        {
            master.WriteSingleCoil(SlaveID, coilAddress, value);
        }

        /// <summary>
        /// 批量写入线圈 写多线圈寄存器 功能码15
        /// </summary>
        public void WriteArrayCoil(ushort startAddress,params bool[] values)
        {
            master.WriteMultipleCoils(SlaveID, startAddress, values);
        }

        /// <summary>
        /// 写入单个寄存器 写入值到AO 功能码06,起始地址为十进制
        /// </summary>
        public void WriteSingleRegister(ushort startAddress,ushort value)
        {
            master.WriteSingleRegister(SlaveID, startAddress, value);
        }

        /// <summary>
        /// 批量写入寄存器 写多个保持寄存器 功能码16
        /// </summary>
        public void WriteArrayRegister(ushort startAddress,params ushort[] values)
        {
            master.WriteMultipleRegisters(SlaveID, startAddress, values);
        }

        /// <summary>
        /// 读取输出线圈 读取DO 功能码01
        /// </summary>
        /// <returns></returns>
        public bool[] ReadCoils(ushort startAddress,ushort length)
        {
            return master.ReadCoils(SlaveID, startAddress, length);
        }

        /// <summary>
        /// 读取输入线圈 读取DI 功能码02
        /// </summary>
        /// <returns></returns>
        public bool[] ReadInputs(ushort startAddress, ushort length)
        {
            return master.ReadInputs(SlaveID, startAddress, length);
        }

        /// <summary>
        /// 读取保持型寄存器  读取AO值  功能吗03
        /// </summary>
        /// <returns></returns>
        public ushort[] ReadHoldingRegisters(ushort startAddress, ushort length)
        {
            var values= master.ReadHoldingRegisters(SlaveID, startAddress, length);
            return values;
        }

        /// <summary>
        /// 读取输入寄存器 读取AI 功能码04
        /// </summary>
        /// <returns></returns>
        public ushort[] ReadInputRegisters(ushort startAddress, ushort length)
        {
            return master.ReadInputRegisters(SlaveID, startAddress, length);
        }

        public virtual double ConvertToDouble(ushort[] values)
        {
            //前面是2，3，后面是0，1
            //var values = new ushort[] { value2, value1 };

            var testValue = values[0];
            var testValue2 = values[1];

            var hex1 = Convert.ToString(testValue, 16).PadLeft(4, '0');
            var hex2 = Convert.ToString(testValue2, 16).PadLeft(4, '0');

            var hex = $"{hex2}{hex1}";
            var dec = long.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier);

            var bin = Convert.ToString(dec, 2).PadLeft(32, '0');
            var valueRet = 0.00;
            if (bin[0] == '1')
            {
                valueRet = (dec - 0xffffffff - 1);
            }
            else
            {
                valueRet = dec;
            }
            //valueRet = valueRet * NumberOfPoint;
            return valueRet;
        }

    }
}
