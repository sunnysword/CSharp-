using INIAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYNETTEK.HFModbus;
using System.IO.Ports;
using System.Windows.Shapes;
using System.Runtime.Remoting.Messaging;

namespace Handler.Connect.RFID
{
    public class RFIDHelper_AYNETTEK
    {
        public Reader reader { get; private set; }

        public byte DeviceID { get; private set; }      //RFID设备ID
        public string PortName { get; private set; }    //RFID设备串口通信端口号
        public int BaudRate { get; private set; }       //RFID设备串口通信波特率
        public int DataBits { get; private set; }       //RFID设备串口通信数据位
        public StopBits StopBits { get; private set; }  //RFID设备串口通信停止位
        public Parity Parity { get; private set; }      //RFID设备串口通信奇偶校验

        public byte RFID_TYPE = 0;                      //RFID设备软件版本
        public Func<string> deviceCom;

        public RFIDHelper_AYNETTEK(byte deviceId, string deviceName, Func<string> deviceCom)
        {
            //return;
            DeviceID = deviceId;
            this.deviceCom = deviceCom;
            //if (deviceName == "外壳RFID") this.PortName = "COM10";//todo,RFID串口号待配置
            //if (deviceName == "PCBRFID") this.PortName = "COM9";

            this.BaudRate = 115200;
            this.DataBits = 8;
            this.StopBits = StopBits.One;
            this.Parity = Parity.None;


        }

        private void rfid_ctrl(byte flag)
        {
            RFID_TYPE = 0;
            if ((flag & 0x40) == 0x40 && (flag & 0x60) == 0x40)
            {
                RFID_TYPE = 1;
            }
            else if ((flag & 0x40) == 0x40 && (flag & 0x60) == 0x60)
            {
                RFID_TYPE = 2;
            }
        }



        /// <summary>
        /// 写入RFID数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteData(string data, out string error)
        {
            error = string.Empty;

            if (this.GetTagOnline() == false)
            {
                error = "识别范围内无标签";
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (this.WriteMem(data, 0, 50)) return true;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        error = ex.Message;
                        return false;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            error = "RFID写入数据内容超时";
            return false;
        }
        /// <summary>
        /// 读取RFID数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadData(out string data, out string error)
        {
            error = string.Empty;
            data = string.Empty;

            if (this.GetTagOnline() == false)
            {
                error = "识别范围内无标签";
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    data = this.ReadMem(0, 50);
                    if (string.IsNullOrEmpty(data))
                    {
                        continue;
                    }
                    data = data.Replace("\0", "");
                    return true;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        error = ex.Message;
                        return false;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            error = "RFID读取数据内容超时或读到rfid为空";
            return false;
        }

        /// <summary>
        /// 写入RFID治具编号
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteRFIDNumber(string data, out string error)
        {
            error = string.Empty;

            if (this.GetTagOnline() == false)
            {
                error = "识别范围内无标签";
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (this.WriteMem(data, 20, 10)) return true;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        error = ex.Message;
                        return false;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            error = "RFID写入治具编号超时";
            return false;
        }


        /// <summary>
        /// 读取RFID治具编号
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadRFIDNumber(out string data, out string error)
        {
            error = string.Empty;
            data = string.Empty;

            if (this.GetTagOnline() == false)
            {
                error = "识别范围内无标签";
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    data = this.ReadMem(20, 10);
                    if (string.IsNullOrEmpty(data))
                    {
                        continue;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        error = ex.Message;
                        return false;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            error = "RFID读取治具编号超时";
            return false;
        }

        /// <summary>
        /// 读取RFID UID编号
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadRFIDUID(out string data, out string error)
        {
            error = string.Empty;
            data = string.Empty;

            if (this.GetTagOnline() == false)
            {
                error = "识别范围内无标签";
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    data = this.ReadUid();
                    if (string.IsNullOrEmpty(data))
                    {
                        continue;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        error = ex.Message;
                        return false;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            error = "RFID读取UID编号超时";
            return false;
        }

        private void Connect()
        {
            PortName = deviceCom();
            reader = new Reader();

            reader.Connect(this.PortName, this.BaudRate);

            //软件版本号
            byte[] softVersion = null;

            if (reader.GetSoftVer(DeviceID, ref softVersion) != Execute_Status.SUCCESS)
            {
                reader.DisConnect();
                return;
            }
            // 软件版本号区分协议
            rfid_ctrl(softVersion[1]);
        }

        #region 执行标签相关功能
        /// <summary>
        /// 读取UID
        /// </summary>
        private string ReadUid()
        {
            byte[] uid = null;
            string strUID = string.Empty;
            if (reader == null)
            {
                Connect();
            }
            Execute_Status status = Execute_Status.FAILURE;

            if (RFID_TYPE == 0)
            {
                status = reader.ReadUID(DeviceID, ref uid);
            }
            else if (RFID_TYPE == 1)
            {
                status = reader.ReadUID_ISO14443(DeviceID, ref uid);
            }
            else
            {
                status = reader.ReadUID_ISO14443_7_Bytes(DeviceID, ref uid);
            }

            if (status == Execute_Status.SUCCESS)
            {
                strUID = utils.ByteToHexString(uid, 0, uid.Length, "");
            }
            return strUID;
        }

        /// <summary>
        /// 读取标签内存
        /// </summary>
        private string ReadMem(ushort address = 0, byte cout = 30)
        {
            string strRFID = string.Empty;

            if (address > (int)ModbusReg.MODBUS_TAG_END_REG_ADDRESS || cout > 50)
            {
                return strRFID;
            }

            byte[] datas = null;

            if (address > 24575 || cout > 50 || cout == 0)
            {
                return strRFID;
            }

            if (address + cout > 24575)
            {
                return strRFID;
            }
            if (reader == null)
            {
                Connect();
            }
            Execute_Status status = reader.ReadMemByWord(DeviceID, address, cout, ref datas);

            if (status == Execute_Status.SUCCESS)
            {
                if (datas != null)
                {
                    //strRFID = utils.ByteToHexString(datas, 0, datas.Length, "");
                    strRFID = Encoding.ASCII.GetString(datas).Replace("\0", "");
                }
            }
            return strRFID;
        }

        /// <summary>
        /// 写标签内存
        /// </summary>
        private bool WriteMem(string svalues, ushort address = 0, ushort cout = 30)
        {
            if (address > (int)ModbusReg.MODBUS_TAG_END_REG_ADDRESS || cout > 50)
            {
                return false;
            }
            //byte[] wirte = this.HexStringToByte(svalues, 0);
            byte[] wirte = Encoding.ASCII.GetBytes(svalues);

            if (wirte.Length % 2 != 0)
            {
                wirte = Encoding.ASCII.GetBytes(svalues + " ");
            }

            if (address > 24575 || wirte.Length > 120 || wirte.Length % 2 != 0)
            {
                return false;
            }

            if (address + wirte.Length / 2 > 24575)
            {
                return false;
            }
            if (reader == null)
            {
                Connect();
            }
            byte[] finalWrite = new byte[50];//测试超过这个就会error
            Array.Copy(wirte, finalWrite, wirte.Length);
            Execute_Status status = reader.WriteMemByWord(DeviceID, address, finalWrite);

            if (status == Execute_Status.SUCCESS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取标签在线信号
        /// </summary>
        private bool GetTagOnline()
        {
            bool tagSingle = false;
            if (reader == null)
            {
                Connect();
            }
            Execute_Status status = Execute_Status.NO_RESPONSE;
            try
            {
                status = reader.TagOnline(DeviceID, ref tagSingle);
            }
            catch
            {

            }
           
            if (status == Execute_Status.SUCCESS) { tagSingle = true; }

            return tagSingle;
        }

        private byte[] HexStringToByte(string str, int pos)
        {
            string tempStr = GetStringWithoutSpace(str, pos);

            return HexStringToByte(tempStr, 0, (tempStr.Length + 1) >> 1);
        }

        private string GetStringWithoutSpace(string str, int pos)
        {
            string temp = "";
            for (int i = pos; i < str.Length; i++)
            {
                if ((str[i] != ' ') && (str[i] != ':'))
                {
                    temp += str[i];
                }
            }
            return temp;
        }

        //将字符串的pos位置开始，转化为数组，转化后数组的长度为cnt
        private byte[] HexStringToByte(string str, int pos, int cnt)
        {
            if ((!ValidHexString(str)) || ((str.Length - pos + 1) >> 1 < cnt))
            {
                return null;
            }

            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] data = new byte[cnt];

            for (int i = 0; i < cnt; i++)
            {
                data[i] = (byte)(HexStringToHex(str, 2 * i + pos) * 16 + HexStringToHex(str, 2 * i + pos + 1));
            }

            return data;
        }

        //将单个十六进制字符(4 bits)转化为byte
        private byte HexStringToHex(string str, int pos)
        {
            byte value = 0;

            if (str.Length <= pos)
            {
                return value;
            }
            if ((str[pos] >= '0') && (str[pos] <= '9'))
            {
                value = (byte)(str[pos] - '0');
            }
            else if ((str[pos] >= 'a') && (str[pos] <= 'f'))
            {
                value = (byte)(str[pos] - 'a' + 10);
            }
            else if ((str[pos] >= 'A') && (str[pos] <= 'F'))
            {
                value = (byte)(str[pos] - 'A' + 10);
            }
            return value;
        }

        //判断字符串是否是十六进制字符串
        public bool ValidHexString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!
                    (((str[i] >= '0') && (str[i] <= '9')) ||
                    ((str[i] >= 'a') && (str[i] <= 'f')) ||
                    ((str[i] >= 'A') && (str[i] <= 'F')))
                    )
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
