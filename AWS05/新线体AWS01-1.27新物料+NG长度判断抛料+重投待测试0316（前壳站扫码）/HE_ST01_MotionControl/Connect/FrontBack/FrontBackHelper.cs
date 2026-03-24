using Handler.Motion;
using Handler.Process.Station;
using Platform;

namespace Handler.Connect.FrontBack
{
    public static class FrontBackHelper
    {

        //public static BackConnectTCP TCP_Upline = new BackConnectTCP(
        //    ConnectFactory.Connect_UplineNextSite,
        //    () => ((TCPHelper3.ConnectServerHelper)ConnectFactory.Connect_UplineNextSite).remoteClientList.Count > 0);

        // TCP_Downline = new FrontConnectTCP(
        //  ConnectFactory.Connect_DownlineNextSite,
        //  ()=> ConnectFactory.Connect_DownlineNextSite.IsConnected);

       // public static Platform.ElevatingPlatform Upline1 = new Platform.ElngPlatform Downline1 = new Platform.ElevatingPlatform("下流水线", TCP_Downline, null);


        static FrontBackHelper()
        {
            Initial();
        }

        public static void Stop()
        {
            //Upline1.Init();
            //Downline1.Init();
        }

        public static void Initial()
        {

            //TCP_Upline.GetMachineStopState += () =>
            //{
            //    return StaticInitial.Motion.CurOrder.IsAutoRunning == false;
            //};
            //Upline1.WriteErrToUserEventHandler += WriteErrToUser;
            //Upline1.GetMachineIsRunningEventHandler += GetMachineIsRunning;
            //Upline1.GetStationIsAutoRunningFunc += GetStationIsAutoRunning;
            //Upline1.Initial();

            //Downline1.WriteErrToUserEventHandler += WriteErrToUser;
            //Downline1.GetMachineIsRunningEventHandler += GetMachineIsRunning;
            //Downline1.GetStationIsAutoRunningFunc += GetStationIsAutoRunning;
            //Downline1.IsLastStation = true;
            //Downline1.Initial();

        }

        public static bool GetStationIsAutoRunning()
        {
            return StaticInitial.Motion.CurOrder.IsAutoRunning;
        }

        public static Platform.SampleMessage GetMachineIsRunning()
        {
            Platform.SampleMessage message = new SampleMessage();
            bool result = true;
            string info = string.Empty;
            if (StaticInitial.Motion.CurOrder.IsAutoRunning == false)
            {
                info = "设备不在自动运行中";
                result = false;
            }
            if (StaticInitial.Motion.CurOrder.IsPause == true)
            {
                info = "设备暂停中";
                result = false;
            }
            if (StaticInitial.Motion.CurOrder.IsErrPause == true)
            {
                info = "设备错误暂停中";
                result = false;
            }
            //if (StationDispenser.Cur.ProductFlowOnPosSignalEventHandler?.Invoke() == true)
            //{
            //    info = "下一工位治具到位传感器可以检测到治具";
            //    result = false;
            //}
            message.Result = result;
            message.Message = info;
            return message;
        }

        
        public static void WriteErrToUser(string msg)
        {
            if (!StaticInitial.Motion.CurOrder.IsErr)
            {
                StaticInitial.Motion.WriteErrToUser(msg);
            }
        }
    }
}
