using AixCommInfo;
using AM.Core.Extension;
using AM.Core.IO;
using Handler.Connect;
using Handler.Connect.Camera;
using Handler.Core.Process;
using Handler.Funs;
using Handler.Modbus;
using Handler.Motion.IO;
using Handler.Motion.PalletTray;
using Handler.Params;
using Handler.Process;
using Handler.Process.Station;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Product;
using HE_ST01_MotionControl.Connect.Modbus;
using HE_ST01_MotionControl.Motion.Axis.AxisGroup;
using Hg_MN200_dll2;
using Iodll;
using LanCustomControldllExtend.FourAxis;
using Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using System.Diagnostics;
using Handler.View;

namespace Handler.Process.Station.TrayLoad
{

    class Process_前壳Tray抓取 : StationBase
    {
        public static readonly Process_前壳Tray抓取 Cur = new Process_前壳Tray抓取();

        private Process_前壳Tray抓取() : base("前壳Tray抓取三轴")
        {
            ConnectFactory.前壳Connect_ScanCode.RecevieTActionEventHander += Connect_ScanCode_RecevieTActionEventHander;
        }

        /// <summary>
        /// 扫码枪扫到的二维码
        /// </summary>
        public string TempSN { get; set; }
        private readonly Stopwatch stopWatch = new Stopwatch();

        private void Connect_ScanCode_RecevieTActionEventHander(string obj)
        {
            ReportMsg("扫码枪收到消息:" + obj);
            if (obj.ToUpper() != "NR" || obj.ToUpper() != "NOREAD")
            {
                TempSN = obj;
            }
        }

        private string lastBarcode;
        private Task task1;
        public IoOutHepler Out_CameraLight { get; set; }
        public ThreeAxis前壳上料 ThreeAxis => ThreeAxis前壳上料.Cur;
        public CameraHelper Camera { get; set; }

        /// <summary>
        /// 前壳料盘
        /// </summary>
        public PalletInfoOnePoint Pallet { get; set; }

        public PalletInfoOnePoint ScanPcbShellPallet { get; set; }

        //public ModbusGripper_Hsl Gripper { get; set; }
        public ClampRotatedAxisShell Gripper { get; set; }
        public IoInHelper ClampProductCheckSensor { get; set; }
        public bool ClampProductCheckGripper { get; set; }

        /// <summary>
        /// 料仓类
        /// </summary>
        public Process_TrayLoad TrayProcess { get; set; }

        private Task task;
        /// <summary>
        /// 载具外壳穴位放料点
        /// </summary>
        public ThreeAixsPosMsgILineZMov PosPlace { get; set; }
        /// <summary>
        /// 外壳三轴等待点
        /// </summary>
        public ThreeAixsPosMsgILineZMov 安全位置 { get; set; }

        public bool 外壳放料完成 { get; set; }
        public Func<bool> 是否开启前壳夹爪旋转或夹持状态检测 { get; set; }
        public bool CanSkipStep { get; set; } = false;
        public int ngIndex { get; set; }
        public void FlushTray()
        {
            WriteToUser("料仓事件清除料盘索引一次");
            this.Pallet.Clear();
        }

        /// <summary>
        /// 获取当前轴位置
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPos()
        {
            return $"{ThreeAxis.Aix_1.GetCmdPos()},{ThreeAxis.Aix_2.GetCmdPos()}";
        }

        public bool WaitScanBarcode()
        {
            try
            {
                DateTime t1 = DateTime.Now;
                TempSN = "";
                while ((DateTime.Now - t1).TotalSeconds < 5)
                {
                    if (TempSN != "") return true;
                    Thread.Sleep(100);
                }
                WriteErrToUser("超时未收到条码！");
            }
            finally
            {
                ConnectFactory.前壳Connect_ScanCode.Send("stop");
            }
            return false;
        }

        protected override void ImplementRunAction()
        {

            //TrayProcess.FlushTrayAction -= FlushTray;
            //TrayProcess.FlushTrayAction += FlushTray;
            IsReady = false;
            string[] CamValue = new string[] { "", "", "", "" };
            string error = string.Empty;
            bool ScanResult = false;
            TrayPointZU visionPoint = null;//料盘位置对象
            //声明两个轴位置变量pos，pos2，pos是取料位置存储对象
            ThreeAixsPosMsgILineZMov pos = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
            () => this.ThreeAxis.Aix_1.GetCmdPos(true),
            () => this.ThreeAxis.Aix_2.GetCmdPos(true),
            () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
            true
            );
            //pos2是放料前定位后位置存储对象
            ThreeAixsPosMsgILineZMov pos2 = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
            () => this.ThreeAxis.Aix_1.GetCmdPos(true),
            () => this.ThreeAxis.Aix_2.GetCmdPos(true),
            () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
            true
            );
            //声明一个相机对象，用来存数据
            CameraCmd cameraCmd = new CameraCmd();
            //获取设置的连续不良报警数
            ContinuousNGStatistics ngCount = new ContinuousNGStatistics("前壳相机定位", () => Funs.FunCommon.Cur.ErrContinueNum.GetValue);
            Ref_Step("Z轴移动到等待位");
            外壳放料完成 = false;
            int errorCount = 0;

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }

                switch (Step)
                {
                    case "Z轴移动到等待位":
                        if (!FunStationSetting.Cur.是否启用前壳工位.GetValue)
                        {
                            Ref_Step("Z轴移动到等待位");
                            WriteToUser("当前已屏蔽前壳取料功能", Brushes.Yellow, false);
                            Sleep(10000);
                            break;
                        }
                        if (Process_AutoRun.Cur.IsClearModeWithCount())
                        {
                            Sleep(10000);
                            break;
                        }
                        if (WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            //if (CanSkipStep)
                            //{
                            //    SleepLoopCheck(2000);
                            //}

                            // 判断模式是否为清料模式 
                            // 判断夹爪|| 工位1检测 || 工位2检测 || 工位3检测
                            // 当前工位1,2,3存在物料时,正常执行提前取前壳 ，否则停止取料
                            //if (Process_AutoRun.Cur.IsClearMode)
                            //{
                            //    //WriteToUser($"1----清料模式,判断当前工位3是否存在物料", Brushes.Yellow, false);
                            //    //Sleep(1000);
                            //    //if (PlaceStation.Allow)
                            //    //{
                            //    //    WriteToUser($"2----清料模式,当前工位不允许放料", Brushes.Yellow, false);
                            //    //    break;
                            //    //}
                            //    //else
                            //    //{
                            //    //    break;
                            //    //}

                            //}
                            Ref_Step("判断料盘是否允许取料");
                        }
                        break;

                    case "判断料盘是否允许取料":
                        if (TrayProcess.Load_state == ENUM_Load_State.Ready)//料盘已经抽出并到位
                        {
                            Ref_Step("判断是否是重投模式");
                        }
                        break;
                    case "判断是否是重投模式":
                        if (Process.Process_AutoRun.Cur.isReProduce)
                        {
                            //默认需要扫码
                            CurrentProduct = ProductInfo.GetInitialProduct();//初始化对象
                            visionPoint = new TrayPointZU();
                            visionPoint = ScanPcbShellPallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                            pos.PosMsg_1.GetValue = visionPoint.X;
                            pos.PosMsg_2.GetValue = visionPoint.Y;
                            pos.PosMsg_3.GetValue = visionPoint.Z;
                            if (WaitAxisPoint(pos,AxisMoveType.AxisXYZ))
                            {
                                Ref_Step("重投扫码");//扫码位置是锁附完的PCB码
                            }
                        }
                        else
                        {
                            Ref_Step("计算点位");
                        }
                        break;
                    case "重投扫码":
                        TempSN = string.Empty;
                        ConnectFactory.前壳Connect_ScanCode.Send("start");
                        ScanResult = false;
                        ScanResult = WaitScanBarcode();
                        Ref_Step("扫码结果判断处理");
                        break;
                    case "扫码结果判断处理":
                        if (ScanResult)
                        {
                            if (CurrentProduct != null)
                            {
                                TempSN = TempSN == null ? "" : TempSN.Split(';')[0];
                                CurrentProduct.PCBSN = TempSN;
                            }
                            WriteToUser($"扫码枪扫到的PCB码为:{TempSN}", Brushes.Green, false);
                            var 机种 = FunMES.Cur.MES是否使用其他机型上传.GetValue ? FunMES.Cur.MES其他机型.GetValue : WorkItemManagerHelper.LoadedName;
                            var checkHeader = FunMESCheck.Cur.CheckHeader.GetValue;
                            var version = FunMESCheck.Cur.SnVersion.GetValue;
                            var checkResult = TempSN.Length > checkHeader.Length && TempSN.Substring(0, checkHeader.Length) == checkHeader;
                            var formateBarcode = $"{机种}{version}{TempSN.Substring(8)}";
                            CurrentProduct.SN = formateBarcode;
                            if (CurrentProduct.SN != "" && CurrentProduct.SN != null)
                            {
                                if (CurrentProduct.SN.Count() > 19)
                                {
                                    CurrentProduct.SN = null;
                                    ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                    WriteErrToUser($"SN位数大于19位！视为NG");
                                    break;
                                }
                            }
                            if (lastBarcode == TempSN)
                            {
                                CurrentProduct.SN = null;
                                Params.ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                WriteErrToUser($"PCB码重复！上一个条码为{lastBarcode}当前条码为{TempSN}");
                                break;
                            }
                            else
                            {
                                lastBarcode = TempSN;
                            }

                            if (!checkResult)
                            {
                                WriteErrToUser($"PCB头校验失败！{TempSN}");
                                CurrentProduct.SN = null;
                                ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                Ref_Step("重投扫码");
                                break;

                            }
                            Ref_Step("计算点位");
                        }
                        else
                        {
                            WriteToUser("扫码失败！", Brushes.Red, false);
                            CurrentProduct.SN = null;
                            if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))//移动到安全点
                            {
                                Ref_Step("判断是否是重投模式");
                                break;
                            }
                        }
                        break;
                    case "计算点位":
                        if (CurrentProduct==null)
                        {
                            CurrentProduct = ProductInfo.GetInitialProduct();
                        }
                        visionPoint=new TrayPointZU();
                        visionPoint = Pallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                        if (visionPoint == null)
                        {
                            ReportMsg("料盘取完");
                            Ref_Step("通知更换料盘");
                        }
                        else
                        {
                            Ref_Step("判断获取点位方式");
                        }
                        break;

                    case "判断获取点位方式":
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过直接走固定点位取料", Brushes.Yellow, false);
                            Ref_Step("移动到取料点位");
                            break;
                        }
                        if (Pallet.IsPointByCamera)
                        {
                            Ref_Step("移动到视觉拍照点");
                        }
                        else
                        {
                            Ref_Step("移动到取料点位");
                        }
                        break;

                    case "移动到视觉拍照点":
                        //先保存料盘当前取料穴位位置
                        pos.PosMsg_1.GetValue = visionPoint.X;
                        pos.PosMsg_2.GetValue = visionPoint.Y;
                        pos.PosMsg_3.GetValue = visionPoint.Z;
                        Out_CameraLight.Out_ON();//开光源
                                                 //移动到该固定位置拍照并把当前位置发给相机
                        task1 = new Task(() =>
                        {
                            Gripper.Rotated_MovAbs((short)Gripper.前壳夹爪Tray取料旋转.PosMsg.GetValue, false);
                        });
                        task1.Start();
                        if (WaitAxisPoint(pos, AxisMoveType.AxisXYZ))
                        {
                            Ref_Step("通知相机拍照");
                        }
                        break;

                    case "通知相机拍照":
                        var cameraResult = Camera.SendAndCheck("T2", out cameraCmd, out error,
                            GetCurrentPos(), process: this);

                        if (cameraResult == true)
                        {
                            Ref_Step("相机结果判断");
                        }
                        else if (cameraResult == false)
                        {
                            WriteErrToUser(error);
                        }
                        break;

                    case "相机结果判断":
                        if (cameraCmd.Result == CameraCmd.Result_OK || cameraCmd.Result == "OK")
                        {
                            Out_CameraLight.Out_OFF();
                            ngCount.ClearNGProductCount();//产品抓边无问题，则清除当前连续抓边NG报警计数
                            Ref_Step("相机OK处理");
                        }
                        else
                        {
                            Ref_Step("相机NG处理");
                            errorCount++;
                            if (errorCount >= FunCommon.Cur.ErrContinueNum.GetValue)
                            {
                                WriteErrToUser("相机NG超过次数！");
                            }
                            errorCount = 0;
                            ///前壳扫码-失败NG
                            //ParamsNGTray.Cur.AddScan前壳Ng();
                        }
                        break;

                    case "相机OK处理":
                        try
                        {
                            if (string.IsNullOrEmpty(cameraCmd.Message))
                            {
                                ErrorDialog("相机结果OK，但未收到坐标信息，无法取料！相机发送信息:" + cameraCmd.FullMessage,
                                   ("重新拍照", () =>
                                   {
                                       Ref_Step("计算点位");
                                   }
                                ),
                                   ("跳过此点", () =>
                                   {
                                       Pallet.AddIndex();
                                       Ref_Step("Z轴移动到等待位");
                                   }
                                )
                                );
                            }
                            else
                            {
                                try
                                {
                                    var array = cameraCmd.Message.Split(',');
                                    pos.PosMsg_1.GetValue = double.Parse(array[0]);
                                    pos.PosMsg_2.GetValue = double.Parse(array[1]);
                                    //取料Z轴高度使用固定点高度
                                    pos.PosMsg_3.GetValue = ThreeAxis前壳上料.Cur.PosFetchCoilPlace.PosMsg_3.GetValue;
                                    Ref_Step("移动到取料点位");
                                }
                                catch (Exception ex)
                                {
                                    WriteErrToUser($"收到相机消息后解析异常，消息内容:{cameraCmd.FullMessage},异常原因:" + ex.Message);
                                    Ref_Step("移动到视觉拍照点");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteErrToUser("处理相机回复消息异常，相机回复内容：" + cameraCmd.FullMessage + "异常原因:" + ex.Message);
                            break;
                        }
                        break;

                    case "移动到取料点位":
                        if (!Pallet.IsPointByCamera)//启用的是固定点位
                        {
                            pos.PosMsg_1.GetValue = visionPoint.X;
                            pos.PosMsg_2.GetValue = visionPoint.Y;
                            pos.PosMsg_3.GetValue = ThreeAxis前壳上料.Cur.PosFetchCoilPlace.PosMsg_3.GetValue;
                        }
                        Pallet.AddIndex();
                        Ref_Step("开始夹取");
                        break;

                    case "开始夹取":
                        task1?.Wait();
                        Gripper.Rotated_MovAbsCheck((short)Gripper.前壳夹爪Tray取料旋转.PosMsg.GetValue);
                        Gripper.Wait_RotatedDone();
                        if (!Gripper.Rotated_MovAbsCheck((short)Gripper.前壳夹爪Tray取料旋转.PosMsg.GetValue))
                        {
                            if (stopWatch.ElapsedMilliseconds > 10000)
                            {
                                WriteErrToUser("夹爪运动超时未完成，请检查前壳夹爪COM口后继续");
                                stopWatch.Restart();
                                break;
                            }
                            break;
                        }
                        ClampProductCheckGripper = false;
                        Gripper.Clamp_OFF();
                        if (!WaitAxisPoint(pos, AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("夹爪夹紧");
                        break;

                    case "夹爪夹紧":
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)//空跑直接抬起夹爪
                        {
                            Sleep(200);
                            Ref_Step("夹爪上升");
                            break;
                        }
                        if (!Gripper.Clamp_ON(o => WriteErrToUser(o)))
                        {
                            ErrorDialog("前壳夹爪夹紧检测异常，请选择后续操作",
                               ("跳过这一个", () =>
                               {

                                   Ref_Step("Z轴移动到等待位");
                               }
                            ),
                               ("重新抓取", () =>
                               {
                                   Ref_Step("开始夹取");
                               }
                            )
                                );
                            break;
                        }
                        Ref_Step("夹爪上升");
                        break;

                    case "夹爪上升":
                        if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        Ref_Step("判断夹爪产品有无检测");
                        break;

                    case "判断夹爪产品有无检测":
                        //空跑模式
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过产品有无检测结果", Brushes.Yellow, false);
                            Ref_Step("允许放料前准备动作");
                            break;
                        }
                        if (!是否开启前壳夹爪旋转或夹持状态检测())
                        {
                            ReportMsg("有无检测未启用");
                            WriteToUser("夹爪有无物料检测未开启！", Brushes.Yellow, false);
                            Ref_Step("允许放料前准备动作");
                            break;
                        }
                        //ClampProductCheckGripper = CheckGripperClamp(Gripper, Funs.Fun夹爪Setting.Cur.前壳夹爪加紧距离.GetValue);
                        ClampProductCheckGripper = Gripper.In_ClampGrip();

                        if (ClampProductCheckGripper == false)//未感应到物料
                        {
                            ErrorDialog("夹爪夹取产品后，传感器未检测到产品，请选择后续操作",
                                   ("重新检测", null),
                                   ("重新抓取", () =>
                                   {
                                       Ref_Step("开始夹取");
                                   }
                            )
                                );
                        }
                        else
                        {
                            Ref_Step("允许放料前准备动作");
                        }
                        break;

                    case "允许放料前准备动作":
                        visionPoint = Pallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                        if (visionPoint == null)
                        {
                            ReportMsg("料盘取完");
                            TrayProcess.Load_state = ENUM_Load_State.AllTakeOver;
                            Pallet.Clear();
                        }
                        Ref_Step("三轴到等待点");
                        break;

                    case "三轴到等待点":
                        task = new Task(() =>
                        {
                            Gripper.Rotated_MovAbs((short)Gripper.前壳夹爪Tray放料旋转.PosMsg.GetValue, false);
                        });
                        task.Start();
                        if (!WaitAxisPoint(ThreeAxis.PosPlace, AxisMoveType.AxisXY))
                        {
                            break;
                        }
                        Ref_Step("等待允许放料");
                        break;

                    case "等待允许放料":
                        if (Process_前壳站.Cur.外壳站允许三轴放料)
                        {
                            Process_前壳站.Cur.外壳站允许三轴放料 = false;
                            if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                            {
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue - 10;//空跑时夹爪放料抬高10毫米
                                Ref_Step("三轴放料前旋转");
                            }
                            else
                            {
                                if (FunSelection.Cur.前壳放料前相机引导.GetValue)//上料穴位是否需要相机定位拍照引导
                                {
                                    Ref_Step("前往放料前定位拍照位");
                                }
                                else//走固定位置穴位放料
                                {
                                    pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                                    pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                                    pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                                    Ref_Step("三轴放料前旋转");
                                }
                            }
                        }
                        break;

                    case "前往放料前定位拍照位":
                        Out_CameraLight.Out_ON();
                        if (!WaitAxisPoint(ThreeAxis.PosCameraBeforePlace, AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("相机定位");
                        break;

                    case "相机定位":
                        if (!FunSelection.Cur.前壳放料前相机引导.GetValue)
                        {
                            WriteToUser("未启用相机定位");
                            pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                            pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                            pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                            Ref_Step("三轴放料前旋转");
                            break;
                        }
                        cameraResult = Camera.SendAndCheck("cam4", out cameraCmd, out error,
                            process: this);

                        if (cameraResult == true)
                        {
                            if (cameraCmd.IsPass)
                            {
                                Out_CameraLight.Out_OFF();
                                Ref_Step("相机放料前定位OK处理");
                            }
                            else
                            {
                                Ref_Step("相机放料前定位NG处理");
                                break;
                            }
                        }
                        else if (cameraResult == false)
                        {
                            WriteErrToUser(error);
                        }
                        break;

                    case "相机放料前定位NG处理":
                        ErrorDialog("前壳放入前壳前相机定位失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机定位");
                                  }
                        ), ("壳体作为NG品", () =>
                        {
                            CurrentProduct.SetProductNG("前壳放料前定位NG");
                            ParamsNGTray.Cur.AddShellBeforePlaceNg();
                            Ref_Step("放料完成前Z轴上升到等待位");
                        }
                        )
                               );
                        break;

                    case "相机放料前定位OK处理":
                        if (string.IsNullOrEmpty(cameraCmd.Message))
                        {
                            WriteErrToUser("前壳相机结果OK，但未收到坐标信息，无法放料！相机发送信息:" + cameraCmd.FullMessage);
                            Ref_Step("相机定位");
                            break;
                        }
                        else
                        {
                            try
                            {
                                char s = ';';
                                var arrayss = cameraCmd.FullMessage.Split(s);
                                switch (arrayss[1].ToUpper())
                                {
                                    case "CAM4":
                                        CamValue[0] = arrayss[4];
                                        CamValue[1] = arrayss[5];
                                        break;
                                }
                                WriteToUser($"当前产品的视觉检测值为{CamValue[0]} -- {CamValue[1]}");
                                var array = cameraCmd.Message.Split(';');
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue + double.Parse(array[0]);
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue + double.Parse(array[1]);
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;

                                Ref_Step("三轴放料前旋转");
                            }
                            catch (Exception ex)
                            {
                                WriteErrToUser($"收到前壳相机消息后解析异常，消息内容:{cameraCmd.FullMessage},异常原因:" + ex.Message);
                                Ref_Step("相机定位");
                            }
                        }
                        break;

                    case "三轴放料前旋转":
                        task?.Wait();
                        Gripper.Rotated_MovAbsCheck((short)Gripper.前壳夹爪Tray放料旋转.PosMsg.GetValue);
                        Gripper.Wait_RotatedDone();
                        if (!Gripper.Rotated_MovAbsCheck((short)Gripper.前壳夹爪Tray放料旋转.PosMsg.GetValue))
                        {
                            break;
                        }
                        Ref_Step("三轴放料");
                        break;

                    case "三轴放料":
                        if (!WaitAxisPoint(pos2, AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("放料后夹爪松开");
                        break;

                    case "放料后夹爪松开":
                        if (!Gripper.Clamp_OFF(o => WriteErrToUser(o)))
                        {
                            break;
                        }
                        ClampProductCheckGripper = false;
                        Ref_Step("放料后夹爪上升");
                        break;

                    case "放料后夹爪上升":
                        if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("当前空跑模式，放料后不使用相机检测");
                            Ref_Step("放料完成前Z轴上升到等待位");
                        }
                        else
                        {
                            if (!FunSelection.Cur.前壳放料后相机复检.GetValue)
                            {
                                WriteToUser("放料后相机复检未启用");
                                Ref_Step("放料完成前Z轴上升到等待位");
                            }
                            else
                            {
                                Ref_Step("前往放料后相机拍照位");
                            }
                        }
                        break;

                    case "前往放料后相机拍照位":
                        Out_CameraLight.Out_ON();
                        if (!WaitAxisPoint(ThreeAxis.PosCameraAfterPlace, AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("相机检测");
                        break;

                    case "相机检测":
                        if (!FunSelection.Cur.前壳放料后相机复检.GetValue)
                        {
                            WriteToUser("未启用相机检测");
                            Ref_Step("放料完成前Z轴上升到等待位");
                            break;
                        }
                        cameraResult = Camera.SendAndCheck("cam5", out cameraCmd, out error,
                            process: this);

                        if (cameraResult == true)
                        {
                            Out_CameraLight.Out_OFF();

                            if (cameraCmd.IsPass)
                            {
                                char s = ';';
                                var arrayss = cameraCmd.FullMessage.Split(s);
                                switch (arrayss[1].ToUpper())
                                {
                                    case "CAM5":
                                        CamValue[2] = arrayss[2];
                                        CamValue[3] = arrayss[3];
                                        break;
                                }
                                WriteToUser($"当前产品的视觉检测值为{CamValue[2]} -- {CamValue[3]}");

                                Ref_Step("放料完成前Z轴上升到等待位");
                            }
                            else
                            {
                                WriteErrToUser("转盘工位三前壳放料完成后相机检测不合格，请人工查看是否产品位置不正确");
                                Ref_Step("相机放料后检测NG处理");
                            }
                        }
                        else if (cameraResult == false)
                        {
                            WriteErrToUser(error);
                        }
                        break;

                    case "相机放料后检测NG处理":
                        ErrorDialog("前壳放入前壳后相机检测失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机检测");
                                  }
                        ),
                                  ("壳体作为NG品", () =>
                                  {
                                      CurrentProduct.SetProductNG("前壳放料后定位NG");
                                      ParamsNGTray.Cur.AddShellAfterPlaceNg();
                                      Ref_Step("放料完成前Z轴上升到等待位");
                                  }
                        )
                               );
                        break;

                    case "放料完成前Z轴上升到等待位":
                        if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        Ref_Step("通知放料完成");
                        break;

                    case "通知放料完成":
                    
                        外壳放料完成 = true;
                        if (Process_AutoRun.Cur.isReProduce)
                        {
                            Ref_Step("等待重投数据传递完成");
                            break;
                        }
                        Ref_Step("Z轴移动到等待位");
                        break;
                    case "等待重投数据传递完成":
                        if (Process_前壳站.Cur.三轴数据传递完成)
                        {
                            Process_前壳站.Cur.三轴数据传递完成 = false;
                        }
                        Ref_Step("Z轴移动到等待位");
                        break;
                    case "相机NG处理":
                        if (ngCount.AddAndCheckNGProductCountOverMax(out error))
                        {
                            ErrorDialog($"视觉拍照连续NG，{error}",
                                ("继续拍照", () =>
                                {
                                    Pallet.AddIndex();
                                    ngCount.ClearNGProductCount();
                                    Ref_Step("Z轴移动到等待位");
                                }
                            ),
                                ("置为空料盘", () =>
                                {
                                    Ref_Step("通知更换料盘");
                                }
                            )
                             );
                        }
                        else
                        {
                            Pallet.AddIndex();
                            Ref_Step("Z轴移动到等待位");
                        }
                        break;

                    case "通知更换料盘":
                        TrayProcess.Load_state = ENUM_Load_State.AllTakeOver;
                        Pallet.Clear();
                        Ref_Step("Z轴移动到等待位");
                        break;

                    default:
                        throw new Exception($"{Name}:当前步骤不存在，当前步骤：{Step}");
                }
                Sleep(10);
            }
        }
    }
}
