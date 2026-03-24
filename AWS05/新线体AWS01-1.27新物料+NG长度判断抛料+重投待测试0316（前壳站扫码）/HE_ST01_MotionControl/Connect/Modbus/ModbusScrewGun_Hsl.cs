using Handler.Funs;
using Handler.Motion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using HslCommunication;
using HslCommunication.ModBus;
using System.Net;
using SunWinGroupMES_dll;
using System.IO.Ports;

namespace Handler.Connect
{
    public class ModbusScrewGun_Hsl
    {
        /// <summary>
        /// 私有ModbusTCP从站字段
        /// </summary>
        public ModbusTcpNet slave { get; private set; }

        public string IP { get; private set; }  //从站TCP通信IP地址
        public int Port { get; private set; }   //从站TCP通信端口号

        public bool Connect = false;            //连接状态

        public ModbusScrewGun_Hsl(string ip = "192.168.0.1", int port = 503)
        {
            this.IP = ip;
            this.Port = port;
            this.slave = new ModbusTcpNet(this.IP, this.Port);
            this.ConnectServer();
        }

        ///<summary>
        ///连接Screw
        ///</summary>
        public void ConnectServer()
        {
            if (this.slave == null) this.slave = new ModbusTcpNet(this.IP, this.Port);
            if (this.slave.ConnectionId == string.Empty || this.slave.ConnectionId == null)
            {
                this.slave.ConnectServer();
            }
            if (this.slave.ConnectionId != string.Empty && this.slave.ConnectionId != null) this.Connect = true; else this.Connect = false;
        }

        public void DisConnect()
        {
            this.slave.ConnectClose();
        }

        public Action<string> WriteToUserEventHandler;

        public void WriteToUser(string msg)
        {
            WriteToUserEventHandler?.Invoke(msg);
        }

        /// <summary>
        /// 最终扭矩
        /// </summary>
        public double Final_torque = 0;

        /// <summary>
        /// 扭矩上限值
        /// </summary>
        public double Final_Up_torque = 0;
        /// <summary>
        /// 扭矩下限值
        /// </summary>
        public double Final_Down_torque = 0;

        /// <summary>
        /// 最终旋转角度
        /// </summary>
        public double Final_angle = 0;

        /// <summary>
        /// 角度上限值
        /// </summary>
        public double Final_Up_angle = 0;

        /// <summary>
        /// 角度下限值
        /// </summary>
        public double Final_Down_angle = 0;

        /// <summary>
        /// 当前程序号
        /// </summary>
        public string ProjectNums = "1";


        public bool ReadScrewData()
        {
            WriteToUser("读取螺丝枪信息");

            string str_Final_angle = string.Empty;
            string str_Final_torque = string.Empty;
            string str_Project_num = string.Empty;

            try
            {
                //读取最终扭矩
                str_Final_torque = (this.ReadHoldingRegisters("25") / 1000).ToString();
                Final_torque = double.Parse(str_Final_torque);

                //读取最终旋转角度
                str_Final_angle = this.ReadHoldingRegisters("27").ToString();
                Final_angle = double.Parse(str_Final_angle);

                //读取程序号
                str_Project_num = this.ReadHoldingRegisters("48").ToString();
                ProjectNums = str_Project_num;
                WriteToUser("当前程序号：" + str_Project_num);


                WriteToUser("扭矩下限：" + Final_Down_torque + " mN.m");
                FunScrewGun.Cur.ForqueWorkDownLimit.GetValue = Final_Down_torque;
                WriteToUser("扭矩上限：" + Final_Up_torque + " mN.m");
                FunScrewGun.Cur.ForqueDownDownLimit.GetValue = Final_Up_torque;
                WriteToUser("最终转矩：" + Final_torque + " mN.m");


                WriteToUser("角度上限：" + Final_Up_angle + " °");
                FunScrewGun.Cur.AngelDownDownLimit.GetValue = Final_Up_angle;
                WriteToUser("角度下限：" + Final_Down_angle + " °");
                FunScrewGun.Cur.AngelWorkDownLimit.GetValue = Final_Down_angle;
                WriteToUser("最终角度：" + Final_angle + " °");

                return true;
            }
            catch (Exception ex)
            {
                WriteToUser("读取螺丝枪信息出错:" + ex.Message);
                return false;
            }
        }

        public bool ReadScrewDataTest(out string data, out string error)
        {
            WriteToUser("读取螺丝枪信息");

            string str_Final_angle = string.Empty;
            string str_Final_torque = string.Empty;
            string str_Project_num = string.Empty;

            try
            {
                //读取最终扭矩
                str_Final_torque = (this.ReadHoldingRegisters("25") / 1000).ToString();
                str_Final_torque = "最终扭矩（" + str_Final_torque + "）";

                //读取最终旋转角度
                str_Final_angle = this.ReadHoldingRegisters("27").ToString();
                str_Final_angle = "最终角度（" + str_Final_angle + "）";

                //读取程序号
                str_Project_num = this.ReadHoldingRegisters("48").ToString();
                str_Project_num = "程序号（" + str_Project_num + "）";

                data = str_Final_torque + str_Final_angle + str_Project_num;
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                data = "";
                error = ex.Message;
                WriteToUser("读取螺丝枪信息出错:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设定螺丝枪程序
        /// </summary>
        /// 
        public bool SetScrewTaskNo(int TaskNo)
        {
            bool result = false;
            int intVal = 0;
            for (int i = 0; i < 5; i++)
            {
                result = this.WriteSingleRegister("48", (short)TaskNo);
                if (result == true)
                {
                    intVal = this.ReadHoldingRegisters("48");
                    if (intVal == TaskNo) break;
                }
                if (i == 4) break;
            }
            return true;
        }

        ///<summary>
        ///读取Screw保持寄存器值(FC3功能码，Screw读取 D 地址区域)
        ///</summary>
        public short ReadHoldingRegisters(string startingAddress)
        {
            OperateResult<short> result = null;
            short numArray = 0;
            try
            {
                this.ConnectServer();
                result = this.slave.ReadInt16(startingAddress);
                if (result != null & result.IsSuccess == true) numArray = result.Content;
                return numArray;
            }
            catch (Exception ex)
            {
                return numArray;
            }
        }

        ///<summary>
        ///Screw写单个寄存器Int值(FC6功能码，Screw写入单个 D 地址区域)
        ///</summary>
        public bool WriteSingleRegister(string startAddress, short value)
        {
            OperateResult result = null;
            this.ConnectServer();
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    result = slave.WriteOneRegister(startAddress, value);
                    if (result.IsSuccess == true)
                    {
                        break;
                    }
                }
                if (result.IsSuccess == true) return true; else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// 字符串转换为16进制字符数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] HexStringToBytes(string str)
        {
            string[] str_temp = str.Trim().Split(' ');
            byte[] bytes = new byte[str_temp.Length];
            for (int i = 0; i < str_temp.Length; i++)
            {
                bytes[i] = Convert.ToByte(str_temp[i], 16);
            }
            return bytes;
        }
    }
}