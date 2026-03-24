using Handler.Connect.RFID;
using Handler.Process.Station.TrayLoad;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Effects;
using ZMotionHelper.IO;

namespace Handler.Connect.RFID
{
    public class RFIDDataHelperTCP
    {
        public static bool SetRfidS(USR_RfidAynettek.ConnectClientHelper rfid, string ret)
        {
            try
            {
                if (!WriteData(rfid, ret, out string error))
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool SetRfid(USR_RfidAynettek.ConnectClientHelper rfid, RFIDModel model, out string error)
        {
            error = "";
            string ret = "";
            try
            {
                string isFront = model.isFront ? "1" : "2";
                string stationState = "";
                foreach (var one in model.FormateStationState)
                {
                    stationState += one.Value;
                }
                ret += $"{model.ProductID}:{model.CarrierID}:{model.Barcode}:{model.Result}:{isFront}:{stationState}\n";
                if (!WriteData(rfid, ret, out error))
                {
                    error += "rfid写入数据:" + ret;
                }


            }
            catch
            {
                error = "设置rfid参数失败！";
                return false;
            }
            return true;
        }

        public static void SetConnect(USR_RfidAynettek.ConnectClientHelper rfid, string ip, string port)
        {
            rfid.iP = IPAddress.Parse(rfid.ToString());
            rfid.Port = Convert.ToInt32(port);
            rfid.Save();
        }


        public static string GetRfidS(USR_RfidAynettek.ConnectClientHelper rfid)
        {
            string rfidInfo;
            string error = "";
            var dd2 = rfid.IsConnected;
            var ret = ReadData(rfid, out rfidInfo, out error);
            if (!ret || string.IsNullOrEmpty(rfidInfo))
            {
                return null;
            }
            error = "";
            if (string.IsNullOrEmpty(rfidInfo)) return null;
            var model = new RFIDModel();
            var splite = rfidInfo.Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(':');
            if (splite.Length < 5)
            {
                error = $"rfid:{rfidInfo}:当前格式不正确！无法识别！";
                return null;
            }
            error = $"rfid读出数据：{rfidInfo}";
            return rfidInfo;
        }
        public static RFIDModel GetRfid(USR_RfidAynettek.ConnectClientHelper rfid, out string error)
        {
            string rfidInfo;
            error = "";
            var dd2 = rfid.IsConnected;
            var ret = ReadData(rfid, out rfidInfo, out error);
            if (!ret || string.IsNullOrEmpty(rfidInfo))
            {
                return null;
            }
            error = "";
            if (string.IsNullOrEmpty(rfidInfo)) return null;
            var model = new RFIDModel();
            var splite = rfidInfo.Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(':');
            if (splite.Length < 5)
            {
                error = $"rfid:{rfidInfo}:当前格式不正确！无法识别！";
                return null;
            }
            error = $"rfid读出数据：{rfidInfo}";
            model.ProductID = splite[0];
            model.CarrierID = splite[1];
            model.Barcode = splite[2];
            model.Result = splite[3];
            model.isFront = splite[4] == "1";
            if (splite.Length == 6)
            {
                model.StationState = splite[5];

            }
            else
            {
                model.StationState = "0000";
            }

            string[] state = new string[4] { RFIDModel.SHELLST, RFIDModel.PCBST, RFIDModel.TEARST, RFIDModel.SCREWST };
            int index = 0;
            for (int i = 0; i < state.Length; i++)
            {
                model.FormateStationState[state[i]] = 0;
            }
            foreach (var one in model.StationState)
            {
                if (!Int32.TryParse(one.ToString(), out int value))
                {
                    break;
                }
                model.FormateStationState[state[index]] = value;
                index++;
            }
            return model;
        }

        public static bool WriteNewCarrier(USR_RfidAynettek.ConnectClientHelper rfid, string carrierID, out string error)
        {
            error = "";
            var ret = GetRfid(rfid, out error);
            if (ret == null)
            {
                ret = new RFIDModel();
            }
            ret.CarrierID = carrierID;
            ret.Result = "1";
            ret.FormateStationState[RFIDModel.SHELLST] = 0;
            ret.FormateStationState[RFIDModel.PCBST] = 0;
            ret.FormateStationState[RFIDModel.TEARST] = 0;
            ret.FormateStationState[RFIDModel.SCREWST] = 0;

            return SetRfid(rfid, ret,out error);
        }
        public static bool WriteAllRFID(USR_RfidAynettek.ConnectClientHelper rfid, string AllRFID,out string error)
        {

            var ret = new RFIDModel();
            error = "";
            var splite = AllRFID.Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(':');
            if (splite.Length < 5)
            {
                 error = $"rfid:{AllRFID}:当前格式不正确！无法识别！";
                return false;
            }

            ret.ProductID = splite[0];
            ret.CarrierID = splite[1];
            ret.Barcode = splite[2];
            ret.Result = splite[3];
            ret.isFront = splite[4] == "1";
            if (splite.Length == 6)
            {
                ret.StationState = splite[5];
                var splitChar = ret.StationState.ToArray();
                ret.FormateStationState[RFIDModel.SHELLST] = splitChar[0] - '0';
                ret.FormateStationState[RFIDModel.PCBST] = splitChar[1] - '0';
                ret.FormateStationState[RFIDModel.TEARST] = splitChar[2] - '0';
                ret.FormateStationState[RFIDModel.SCREWST] = splitChar[3] - '0';
            }
            else
            {
                ret.StationState = "0000";
            }
            return SetRfid(rfid, ret,out error);
        }
        public static bool WriteProduct(USR_RfidAynettek.ConnectClientHelper rfid, string productID, out string error)
        {
            var ret = GetRfid(rfid, out error);
            if (ret == null)
            {
                ret = new RFIDModel();
            }
            ret.ProductID = productID;
            ret.Result = "1";
            ret.FormateStationState[RFIDModel.SHELLST] = 0;
            ret.FormateStationState[RFIDModel.PCBST] = 0;
            ret.FormateStationState[RFIDModel.TEARST] = 0;
            ret.FormateStationState[RFIDModel.SCREWST] = 0;

            return SetRfid(rfid, ret,out error);
        }

        public static bool UnbindingRfid(USR_RfidAynettek.ConnectClientHelper rfid, out string error)
        {
            var ret = GetRfid(rfid, out error);
            if (ret == null)
            {
                return false;
            }
            //ret.ProductID = WorkItemManagerHelper.LoadedName;
            ret.Barcode = "";
            ret.Result = "1";
            ret.FormateStationState[RFIDModel.SHELLST] = 1;
            ret.FormateStationState[RFIDModel.PCBST] = 0;
            ret.FormateStationState[RFIDModel.TEARST] = 0;
            ret.FormateStationState[RFIDModel.SCREWST] = 0;
            return SetRfid(rfid, ret,out error);
        }

        private static bool ReadData(USR_RfidAynettek.ConnectClientHelper rfid, out string data, out string error)
        {
            error = string.Empty;
            data = string.Empty;
            if (!rfid.IsConnected)
            {
                rfid.Open();
            }
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    data = rfid.ReadRegistersString(1, 0, 50);
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

        private static bool WriteData(USR_RfidAynettek.ConnectClientHelper rfid, string data, out string error)
        {
            error = string.Empty;

            if (!rfid.IsConnected)
            {
                rfid.Open();
            }
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    rfid.WriteSN(data, 1, 0, 50);
                    Thread.Sleep(20);
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
            error = "";
            return true;
        }
    }
}
