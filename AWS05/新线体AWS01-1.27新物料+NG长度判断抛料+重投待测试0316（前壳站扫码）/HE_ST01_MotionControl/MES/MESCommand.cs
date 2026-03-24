using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalysis;
using Newtonsoft.Json;

namespace Handler.MES
{
    public class MESCommand
    {
        public static Dictionary<string, MESCommand> MESCommandDict = new Dictionary<string, MESCommand>();
        public static DataAnalysis.MotionSendMsg dataSend;
        public static DataAnalysis.MotionReciveMsg dataRecieve;
        public MESCommand(string name, string header, string command)
        {
            Name = name;
            Header = header;
            Command = command;
            if (MESCommandDict.ContainsKey(header) == false)
            {
                MESCommandDict.Add(header, this);
            }
            dataSend = new DataAnalysis.MotionSendMsg(Station);
            dataRecieve = new DataAnalysis.MotionReciveMsg(Station);

        }
        public static string Station => GetStationNameEventHandler?.Invoke();
        public string Name { get; set; }
        public string Header { get; set; }
        public string Command { get; }
        public string Data { get; set; }

        public MESResult Result { get; set; } = new MESResult();

        public static string End { get; set; }

        public static Func<string> GetStationNameEventHandler;

        public string AllJson;//为新mes的json格式
        public void SetData(string data)
        {
            Data = data;
        }

        public void SetDataWithCommand( string data)
        {
            if (Name == "登录")
            {
                AllJson = dataSend.SendLoginMsgToMes(JsonConvert.DeserializeObject<JsonClass.LoginJsonDataToMes>(data));
            }
            else if (Name == "CheckSn")
            {
                AllJson = dataSend.SendCheckSnMsgToMes(JsonConvert.DeserializeObject<JsonClass.CheckSnJsonDataToMes>(data));
            }
            else if (Name == "数据上传")
            {
                AllJson = dataSend.SendDataUpMsgToMes(JsonConvert.DeserializeObject<JsonClass.DataUpJsonDataToMes>(data));
            }
            else if (Name == "条码绑定")
            {
                AllJson = dataSend.SendBindMsgToMes(JsonConvert.DeserializeObject<JsonClass.BindJsonDataToMes>(data));
            }
            else if (Name == "故障收集")
            {
                AllJson = dataSend.SendAlarmUpMsgToMes(JsonConvert.DeserializeObject<JsonClass.AlarmUpJsonDataToMes>(data));
            }
            else if (Name == "设备点检")
            {
                AllJson = dataSend.SendPointInspectionMsgToMes(JsonConvert.DeserializeObject<JsonClass.PointInspectionJsonDataToMes>(data));
            }
            else if (Name == "获取设备状态")
            {
                AllJson = dataSend.SendUpStatusMsgToMes(JsonConvert.DeserializeObject<JsonClass.UpStatusJsonDataToMes>(data));
            }
        }

        //public override string ToString()
        //{
        //    return $"{Header};Command:{Command};Station:{Station};{Data}{End}";

        //}

        public override string ToString()
        {
            return AllJson;

        }
    }

}
