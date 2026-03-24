using Handler.View;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Handler.Connect.RFID
{
    public class RFIDModel
    {
        public string ProductID { get; set; } = "";
        public string CarrierID { get; set; } = "";
        public string Barcode { get; set; } = "";

        public string Result { get; set; } = "1";//1OK;-1NG;2空载具，清料时候处理
        public bool isFront { get; set; }//true前段，false，后段
        public string StationState { get; set; }//自定义工站状态从右到左依次为st01-1，st01-2，st02-1,st02-2  1：ok，2.ng，0，未做 比如：0021指st01-1ok，st01-2ng，st02-1未做，st02-2未做
        public Dictionary<string, int> FormateStationState { get; set; } = new Dictionary<string, int>();

        public static string PCBST = "ST01-2";
        public static string SHELLST = "ST01-1";
        public static string TEARST = "ST02-1";
        public static string SCREWST = "ST02-2";
    }
    public class RFIDDataHelper
    {
        public static bool SetRfid(RFIDHelper_AYNETTEK rfid, RFIDModel model, out string error)
        {
            error = "";
            string ret = "";
            string isFront = model.isFront ? "1" : "2";
            string stationState = "";
            foreach (var one in model.FormateStationState)
            {
                stationState += one.Value;
            }
            ret += $"{model.ProductID}:{model.CarrierID}:{model.Barcode}:{model.Result}:{isFront}:{stationState}\n";
            var rs = rfid.WriteData(ret, out error);

            error += "rfid写入数据:" + ret;

            return rs;
        }

        public static RFIDModel GetRfid(RFIDHelper_AYNETTEK rfid, out string error)
        {
            string rfidInfo;
            var ret = rfid.ReadData(out rfidInfo, out error);
            if (!ret)
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

        public static bool WriteNewCarrier(RFIDHelper_AYNETTEK rfid, string carrierID, out string error)
        {
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

            return SetRfid(rfid, ret, out error);
        }
        public static bool WriteAllRFID(RFIDHelper_AYNETTEK rfid, string AllRFID, out string error)
        {

            var ret = new RFIDModel();

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
            return SetRfid(rfid, ret, out error);
        }
        public static bool WriteProduct(RFIDHelper_AYNETTEK rfid, string productID, out string error)
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

            return SetRfid(rfid, ret, out error);
        }

        public static bool UnbindingRfid(RFIDHelper_AYNETTEK rfid, out string error)
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
            return SetRfid(rfid, ret, out error);
        }
    }
}
