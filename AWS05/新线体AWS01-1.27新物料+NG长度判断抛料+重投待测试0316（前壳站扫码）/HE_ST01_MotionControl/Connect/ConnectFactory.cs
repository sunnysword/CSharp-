using AM.Communication;
using DH_ClampRotatedAxisDemo;
using Handler.Connect.Modbus;
using Handler.MES;
using Handler.Modbus;
using Handler.View;
using HE_ST01_MotionControl.Connect.Modbus;
using IConnectDLL;
using Iodll;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

namespace Handler.Connect
{
    class ConnectFactory
    {
        static ConnectFactory()
        {
            WorkItemManagerHelper.workIterm.LoadedWorkItemEventHandler +=
                () => AM.Communication.ConnectBuilder.ModeChangedEventHandler?.Invoke();
        }


        public static bool IsInitialed = false;


        public static readonly IConnectDLL.IConnectT<string> 前壳Connect_ScanCode
          = ConnectBuilder.GetConnectInstance<string>("前壳扫码枪", CommunicationType.TCPClient, "扫码枪");

        public static readonly IConnectDLL.IConnectT<string> PCBConnect_ScanCode
        = ConnectBuilder.GetConnectInstance<string>("PCB扫码枪", CommunicationType.TCPClient, "扫码枪");



        public static readonly TCPHelper3.ConnectClientHelper tcpServerMes
            = (TCPHelper3.ConnectClientHelper)ConnectBuilder.GetConnectInstance("MES通讯", CommunicationType.TCPClient);
        //视觉待添加 todo

        /// <summary>
        /// 与相机通讯
        /// </summary>
        public static readonly IConnectDLL.IConnectT<string> tcpConnect_Camera
            = ConnectBuilder.GetConnectInstance<string>("视觉相机", CommunicationType.TCPClient);


        ///// <summary>
        ///// 前壳站RFID
        ///// </summary>
        //public static readonly IConnectDLL.IConnect serialPort_RFID1
        //     = ConnectBuilder.GetConnectInstance("前壳站RFID", CommunicationType.RFID, "RFID");
        ///// <summary>
        ///// PCB站RFID
        ///// </summary>
        //public static readonly IConnectDLL.IConnect serialPort_RFID2
        //      = ConnectBuilder.GetConnectInstance("PCB站RFID", CommunicationType.RFID, "RFID");



        //public static ModbusGripper_Hsl FrontShellGripper_Hsl = new ModbusGripper_Hsl(1, "前壳夹爪");
        //public static ModbusGripper_Hsl PCBGripper_Hsl = new ModbusGripper_Hsl(1, "PCB夹爪");
        // public static DHClampExtender FrontShellGripper_Hsl = new DHClampExtender("前壳夹爪", "Clamp_Grip.ini");

        public static ClampRotatedAxisPCB PCBGripper_HslNew = new ClampRotatedAxisPCB();
        public static ClampRotatedAxisShell FrontShellGripper_New = new ClampRotatedAxisShell();
        /// <summary>
        /// 下流水线和后一台设备通信
        /// </summary>
        public static readonly IConnect Connect_DownlineNextSite
          = ConnectBuilder.GetConnectInstance("和后一台设备通信", CommunicationType.TCPServer, "前后站");


        public static readonly Iodll.IoControlContainer CommunicationIoMsgNext = new Iodll.IoControlContainer();

        public static AM.Communication.IO.CommunicationIOInfoHandler InfoHandlerNext
            = new AM.Communication.IO.CommunicationIOInfoHandler(ConnectFactory.Connect_DownlineNextSite,
            () => ConnectFactory.Connect_DownlineNextSite.IsConnected);


        public static USR_RfidAynettek.ConnectClientHelper RFID_Shell = CreateInstrance_client("shellRFID", "RFID_Shell");
        public static USR_RfidAynettek.ConnectClientHelper RFID_PCB = CreateInstrance_client("PCBRFID", "RFID_PCB");
        private static USR_RfidAynettek.ConnectClientHelper CreateInstrance_client(string name, string str)
        {
            string _tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.Cfg, str); 
            return new USR_RfidAynettek.ConnectClientHelper(name, _tempPath);
        }



        public static readonly IoControl CommunicationIO_Next = new AM.Communication.IO.IOHelper(
            "和后一台设备通信",
            InfoHandlerNext,
            CommunicationIoMsgNext
            );

        public static readonly IoOutHepler 输送线本机台请求出料
          = new IoOutHepler(CommunicationIO_Next, "输送线本机台请求出料", 0);
        public static readonly IoOutHepler 回流线本机台允许进料
          = new IoOutHepler(CommunicationIO_Next, "回流线本机台允许进料", 2);



        public static readonly IoInHelper 输送线后机台允许进料
           = new IoInHelper(CommunicationIO_Next, "输送线后机台允许进料", 0);
        public static readonly IoInHelper 回流线后机台请求出料
          = new IoInHelper(CommunicationIO_Next, "回流线后机台请求出料", 2);


        //public static readonly IoOutHepler 当站回流要料信号
        // = new IoOutHepler(CommunicationIO_Next, "当站回流要料信号", 2);
        //public static readonly IoOutHepler 当站回流完成信号
        //  = new IoOutHepler(CommunicationIO_Next, "当站回流完成信号", 3);

        //public static readonly IoInHelper 后站回料准备信号
        //  = new IoInHelper(CommunicationIO_Next, "后站回料准备信号", 2);
        //public static readonly IoInHelper 后站回料完成信号
        //  = new IoInHelper(CommunicationIO_Next, "后站回料完成信号", 3);

        // (Connect_DownlineNextSite as IOHelper).SetDataInfoToLocal(0,"11112");写条码demo


        /// <summary>
        /// 通讯对象管理器
        /// </summary>
        static IConnectDLL.ConnectManagerHelper connectHelper => ConnectBuilder.ConnectHelper;

        public static void Initial()
        {
            if (IsInitialed) return;
            connectHelper.Open();
            connectHelper.StartReLink();
            IsInitialed = true;
            //mes登录
            //if (Handler.Funs.FunSelection.Cur.IsUseMes.GetValue)
            //{
            //    Thread.Sleep(2000);
            //    var ret = MESHelper.Cur.CheckLogin("A001", "A001", out string error);
            //    if (ret == false)
            //    {
            //        MessageBox.Show("MES登录失败！！");
            //    }
            //}
        }

        public static void Close()
        {
            connectHelper.Close();
        }

    }
}
