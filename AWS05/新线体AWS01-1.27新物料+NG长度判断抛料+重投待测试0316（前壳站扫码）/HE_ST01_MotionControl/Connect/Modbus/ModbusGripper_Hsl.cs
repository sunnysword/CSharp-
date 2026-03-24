using Handler.View.MoudbusRTU;
using HslCommunication;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.Modbus
{
    public class ModbusGripper_Hsl
    {
        /// <summary>
        /// 私有ModbusRTU从站字段
        /// </summary>
        public ModbusRtu slave { get; private set; }

        public byte SlaveID { get; private set; }    //从站ID
        public string PortName { get; private set; }   //从站串口通信端口号
        public int BaudRate { get; private set; }   //从站串口通信波特率
        public int DataBits { get; private set; }   //从站串口通信数据位
        public StopBits StopBits { get; private set; }   //从站串口通信停止位
        public Parity Parity { get; private set; }     //从站串口通信奇偶校验

        public ModbusGripper_Hsl(byte slaveId, SerialPort serialPort)
        {
            SlaveID = slaveId;

            if (serialPort.IsOpen == true) serialPort.Close();
            this.PortName = serialPort.PortName;
            this.BaudRate = serialPort.BaudRate;
            this.DataBits = serialPort.DataBits;
            this.StopBits = serialPort.StopBits;
            this.Parity = serialPort.Parity;


            if (!HslCommunication.Authorization.SetAuthorizationCode("95057912-579c-42d1-ad31-eb598f73706f"))
            {
                throw new Exception("Authorization failed! The current program can only be used for 8 hours!");

            }
            slave = new ModbusRtu(SlaveID);

            slave.SerialPortInni(sp =>
            {
                sp.PortName = this.PortName;
                sp.BaudRate = this.BaudRate;
                sp.DataBits = this.DataBits;
                sp.Parity = this.Parity;
                sp.StopBits = this.StopBits;
            });

            slave.Open();
        }

        public ModbusGripper_Hsl(byte slaveId, string deviceName)
        {
            SlaveID = slaveId;

            if (deviceName == "前壳夹爪") this.PortName = "COM11";
            if (deviceName == "PCB夹爪") this.PortName = "COM12";

            if (this.PortName == "") return;
            this.BaudRate = 115200;
            this.DataBits = 8;
            this.StopBits = StopBits.One;
            this.Parity = Parity.None;
            //todo先屏蔽
           
            //return;
            slave = new ModbusRtu(SlaveID);

            slave.SerialPortInni(sp =>
            {
                sp.PortName = this.PortName;
                sp.BaudRate = this.BaudRate;
                sp.DataBits = this.DataBits;
                sp.Parity = this.Parity;
                sp.StopBits = this.StopBits;
            });

            slave.Open();
            
            Init();
        }

        public void Init()
        {
            try
            {
                if (this.ReadSingleRegister("512") == 0)
                {
                    this.WriteSingleRegister("256", 1);
                    for (int i = 0; i < 3; i++)
                    {
                        Thread.Sleep(1000);
                        if (this.ReadSingleRegister("512") == 1) break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Init2()
        {
            
            WriteSingleRegister("261", 0);           
        }


        /// <summary>
        /// 写入单个寄存器 写入值到AO 功能码06,起始地址为十进制
        /// </summary>
        public void WriteSingleRegister(string startAddress, ushort value)
        {
            OperateResult result = null;

            for (int i = 0; i < 3; i++)
            {
                if (slave.IsOpen() == false) slave.Open();
                result = slave.WriteOneRegister(startAddress, value);
                if (result.IsSuccess == true) break;
            }
        }

        public void WriteSingleRegister(string startAddress, short value)
        {
            OperateResult result = null;

            for (int i = 0; i < 3; i++)
            {
                if (slave.IsOpen() == false) slave.Open();
                result = slave.WriteOneRegister(startAddress, value);
                if (result.IsSuccess == true) break;
            }
        }

        /// <summary>
        /// 读取输出线圈 读取DO 功能码01
        /// </summary>
        /// <returns></returns>
        public bool[] ReadCoils(string startAddress, ushort length)
        {
            bool[] flagArray = null;
            OperateResult<bool[]> result = null;

            if (slave.IsOpen() == false) slave.Open();
            result = slave.ReadCoil(startAddress, length);
            if (result != null & result.IsSuccess == true) flagArray = result.Content;
            return flagArray;
        }

        /// <summary>
        /// 读取保持型寄存器  读取AO值  功能吗03
        /// </summary>
        /// <returns></returns>
        public ushort[] ReadHoldingRegisters(string startAddress, ushort length)
        {
            ushort[] flagArray = null;
            OperateResult<ushort[]> result = null;

            if (slave.IsOpen() == false) slave.Open();
            result = slave.ReadUInt16(startAddress, length);
            if (result != null & result.IsSuccess == true) flagArray = result.Content;
            return flagArray;
        }

        /// <summary>
        /// 读取单个寄存器
        /// </summary>
        /// <returns></returns>
        public short ReadSingleRegister(string startAddress)
        {
            short flagArray = 0;
            OperateResult<short> result = null;

            if (slave.IsOpen() == false) slave.Open();
            result = slave.ReadInt16(startAddress);
            if (result != null & result.IsSuccess == true) flagArray = result.Content;
            return flagArray;
        }

        /// <summary>
        /// 读取单个寄存器(无符号)
        /// </summary>
        /// <returns></returns>
        public ushort ReadSingleRegisterU(string startAddress)
        {
            ushort flagArray = 0;
            OperateResult<ushort> result = null;
            if (slave == null) return 0;
            if (slave.IsOpen() == false) slave.Open();
            result = slave.ReadUInt16(startAddress);
            if (result != null & result.IsSuccess == true) flagArray = result.Content;
            return flagArray;
        }
    }
}
