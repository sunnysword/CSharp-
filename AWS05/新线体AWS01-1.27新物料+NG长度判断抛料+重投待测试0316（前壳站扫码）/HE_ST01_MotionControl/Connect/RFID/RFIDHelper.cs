using APIDemo;
using INIAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.RFID
{
    public class RFIDHelper
    {
        public RFIDHelper(AYNETTEK.HFModbus.Reader rfid)
        {
            RFIDClient = rfid;
        }


        AYNETTEK.HFModbus.Reader RFIDClient;


        /// <summary>
        /// 写入RFID数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteData(string data, out string error)
        {
            error = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    byte[] sendbyte = utils.HexStringToByte(data, 0);
                    if (RFIDClient.WriteMemByWord(0, 1, sendbyte) == AYNETTEK.HFModbus.Execute_Status.SUCCESS)
                    {
                        return true;
                    }
                    //RFIDClient.WriteSN(data);
                    //return true;
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
            error = "RFID写入超时";
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
            byte[] read = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (RFIDClient.ReadMemByWord(0, 1, 1, ref read) == AYNETTEK.HFModbus.Execute_Status.SUCCESS)
                    {
                        if (read != null)
                        {
                            data = utils.ByteToHexString(read, 0, read.Length, " ");
                            continue;
                        }
                        return true;
                    }
                    //data = RFIDClient.ReadSN();
                    //if (string.IsNullOrEmpty(data))
                    //{
                    //    continue;
                    //}
                    //return true;
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
            error = "RFID读取数据内容超时，未读到rfid或rfid未烧录信息，请手动测试rfid";
            return false;
        }

        /// <summary>
        /// 写入RFID编号，治具编号
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool WriteRFIDNumber(string data, out string error)
        {
            error = string.Empty;
            try
            {
                //RFIDClient.WriteRFID(data);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 读取RFID编号，治具编号
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ReadRFIDNumber(out string data, out string error)
        {
            error = string.Empty;
            data = string.Empty;
            byte[] uid = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (RFIDClient.ReadUID(0, ref uid) == AYNETTEK.HFModbus.Execute_Status.SUCCESS)
                    {
                        if (uid != null)
                        {
                            data = utils.ByteToHexString(uid, 0, uid.Length, " ");
                            continue;
                        }
                        return true;
                    }
                    //data = RFIDClient.ReadRFID();
                    //if (string.IsNullOrEmpty(data))
                    //{
                    //    continue;
                    //}
                    //return true;
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

    }
}
