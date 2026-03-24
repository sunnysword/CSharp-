using AixCommInfo;
using AM.Core.Extension;
using AM.Core.IO;
using Handler.Connect;
using Handler.Connect.Camera;
using Handler.Core.Process;
using Handler.FileOperate;
using Handler.Funs;
using Handler.MES;
using Handler.Modbus;
using Handler.Motion.IO;
using Handler.Motion.PalletTray;
using Handler.Params;
using Handler.Process;
using Handler.Process.Station;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Product;
using Handler.View;
using HE_ST01_MotionControl.Connect.Modbus;
using HE_ST01_MotionControl.Motion.Axis.AxisGroup;
using Hg_MN200_dll2;
using Iodll;
using LanCustomControldllExtend.FourAxis;
using Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using System.Diagnostics;
using AixRegisterInfodll;
using Microsoft.VisualBasic;

namespace Handler.Process.Station.TrayLoad
{
    class Process_PCBTray抓取 : StationBase
    {
        public static readonly Process_PCBTray抓取 Cur = new Process_PCBTray抓取();

        private Process_PCBTray抓取() : base("PCBTray抓取三轴")
        {
            ConnectFactory.PCBConnect_ScanCode.RecevieTActionEventHander += Connect_ScanCode_RecevieTActionEventHander;
        }

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
        public static bool isEmptyRun = true;
        public static bool isOnlyTray = false;

        public IoOutHepler Out_CameraLight { get; set; }
        public ThreeAxisPCB上料 ThreeAxis => ThreeAxisPCB上料.Cur;
        public CameraHelper Camera { get; set; }
        public PalletInfoOnePoint Pallet { get; set; }
        public PalletInfoOnePoint NGPallet { get; set; }
        public PalletInfoOnePoint ScanPallet { get; set; }
        public ClampRotatedAxisPCB Gripper { get; set; }
        public IoInHelper ClampProductCheckSensor { get; set; }
        public bool ClampProductCheckGripper { get; set; }

        /// <summary>
        /// PCB料仓类
        /// </summary>
        public Process_TrayLoad TrayProcess { get; set; }
        public ThreeAixsPosMsgILineZMov PosPlace { get; set; }
        public ThreeAixsPosMsgILineZMov 安全位置 { get; set; }

        public bool PCB放料完成 { get; set; }
        public Func<bool> 是否开启PCB夹爪旋转或夹持状态检测 { get; set; }

        int ngIndex = 0;//ng料盘的计数点位
        public bool CanSkipStep { get; set; } = false;

        public bool WaitScanBarcode()
        {
            try
            {
                DateTime t1 = DateTime.Now;
                // TempSN = "";
                while ((DateTime.Now - t1).TotalSeconds < 5)
                {
                    if (TempSN != "" && TempSN != "NoRead") return true;
                    Thread.Sleep(100);
                }
                WriteToUser("扫码枪扫码超时！", Brushes.Red, false);
            }
            finally
            {
                ConnectFactory.PCBConnect_ScanCode.Send("stop");
            }
            return false;
        }

        public void FlushTray()
        {
            WriteToUser("料仓事件清除料盘索引一次");
            this.Pallet.Clear();
        }

        private Task task;
        private Task task1;

        public string GetCurrentPos()
        {
            return $"{ThreeAxis.Aix_1.GetCmdPos()},{ThreeAxis.Aix_2.GetCmdPos()}";//获取三轴当前位置
        }

        protected override void ImplementRunAction()
        {
            TrayProcess.FlushTrayAction -= FlushTray;
            TrayProcess.FlushTrayAction += FlushTray;
            IsReady = false;
            string[] CamValue = new string[] { "", "", "", "" };
            string error = string.Empty;
            TrayPointZU visionPoint = null;//视觉点位
            TrayPointZU ScanPoint = null;//扫码点位
            TrayPointZU NGPoint = null;//NG点位
            PCB放料完成 = false;
            string PCB_SN = string.Empty;
            bool judgeResult = false;
            bool ScanResult = false;
            //声明两个轴位置变量pos，pos2
            ThreeAixsPosMsgILineZMov pos = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
            () => this.ThreeAxis.Aix_1.GetCmdPos(true),
            () => this.ThreeAxis.Aix_2.GetCmdPos(true),
            () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
            true
            );
            ThreeAixsPosMsgILineZMov pos2 = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
            () => this.ThreeAxis.Aix_1.GetCmdPos(true),
            () => this.ThreeAxis.Aix_2.GetCmdPos(true),
            () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
            true
            );
            ThreeAixsPosMsgILineZMov NGpos = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
           () => this.ThreeAxis.Aix_1.GetCmdPos(true),
           () => this.ThreeAxis.Aix_2.GetCmdPos(true),
           () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
           true
           );
            ThreeAixsPosMsgILineZMov ScanPos = new ThreeAixsPosMsgILineZMov(() => ThreeAxis,
          () => this.ThreeAxis.Aix_1.GetCmdPos(true),
          () => this.ThreeAxis.Aix_2.GetCmdPos(true),
          () => this.ThreeAxis.Aix_Z.GetCmdPos(true),
          true
          );

            CameraCmd cameraCmd = new CameraCmd();
            ContinuousNGStatistics ngCount = new ContinuousNGStatistics("PCB相机定位", () => Funs.FunCommon.Cur.ErrContinueNum.GetValue);
            Ref_Step("NG盘到位");
            int errorCount = 0;
            bool ChangeNGTray = false;
            IoCheck inputButton = new IoCheck(() => StaticIOHelper.NG料仓人工按钮.In());

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }

                while (ChangeNGTray)
                {
                    if (inputButton.RisingCheck() && StaticIOHelper.NG料仓人工推拉到位接近.In())
                    {
                        if (!WaitIO(StaticIOHelper.NG移载锁紧气缸, CylinderActionType.WorkPos))
                        {
                            WriteErrToUser("NG移载锁紧气缸去动点位超时，请检查");
                            continue;
                        }
                        if (!WaitIO(StaticIOHelper.NG料仓移载气缸, CylinderActionType.WorkPos))
                        {
                            WriteErrToUser("NG料仓移载气缸去动点位超时，请检查");
                            continue;
                        }
                        if (!StaticIOHelper.NG料仓载具到位光电.In())
                        {
                            WriteErrToUser("PCBNG区载具推到位检测无信号，请检查有无料盘");
                            continue;
                        }
                        StaticIOHelper.NG料仓人工按钮灯.Out_OFF();
                        NGPallet.Clear();
                        ChangeNGTray = false;
                    }
                    else
                    {
                        Sleep(100);
                        continue;
                    }
                }

                switch (Step)
                {
                    case "NG盘到位":
                        if (!StaticIOHelper.NG料仓人工推拉到位接近.In())
                        {
                            WriteErrToUser("NG料仓人工推拉到位接近无信号，请检查");
                            break;
                        }
                        if (!WaitIO(StaticIOHelper.NG移载锁紧气缸, CylinderActionType.WorkPos))
                        {
                            WriteErrToUser("NG移载锁紧气缸去动点位超时，请检查");
                            break;
                        }
                        if (!WaitIO(StaticIOHelper.NG料仓移载气缸, CylinderActionType.WorkPos))
                        {
                            WriteErrToUser("NG料仓移载气缸去动点位超时，请检查");
                            break;
                        }
                        if (!StaticIOHelper.NG料仓载具到位光电.In())
                        {
                            WriteErrToUser("PCBNG区载具推到位检测无信号，请检查有无料盘");
                            break;
                        }
                        Ref_Step("Z轴移动到等待位");
                        break;

                    case "Z轴移动到等待位":
                        if (!FunStationSetting.Cur.是否启用pcb工位.GetValue)//PCB工位是否启用
                        {
                            Ref_Step("Z轴移动到等待位");
                            WriteToUser("当前已屏蔽PCB取料功能", Brushes.Yellow, false);
                            Sleep(10000);
                            break;
                        }
                        if (WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            //if (CanSkipStep)
                            //{
                            //    SleepLoopCheck(500);
                            //}

                            // 判断模式是否为清料模式 
                            // 判断夹爪|| 工位1检测 || 工位2检测 || 工位3检测
                            // 当前工位1,2,3存在物料时,正常执行提前取PCB ，否则停止取料
                            //if (Process_AutoRun.Cur.IsClearMode)
                            //{
                            //    WriteToUser($"1----清料模式,判断当前工位3是否存在物料", Brushes.Yellow, false);
                            //    Sleep(1000);
                            //if (PlaceStation.Allow)
                            //{
                            //    WriteToUser($"2----清料模式,判断当前工位3是允许放料", Brushes.Yellow, false);
                            //    //if (/*是否存在前壳料()*/)
                            //    //{
                            //    //    WriteToUser($"3----清料模式,判断当前工位3感应到产品准备放料", Brushes.Yellow, false);

                            //    //    Ref_Step("判断料盘是否允许取料");
                            //    //    break;
                            //    //}
                            //    //else
                            //    //{
                            //    //    break;
                            //    //}
                            //}
                            //else
                            //{
                            //    break;
                            //}

                            //}
                            Ref_Step("判断料盘是否允许取料");
                        }
                        break;
                    case "判断料盘是否允许取料":
                        if (TrayProcess.Load_state == ENUM_Load_State.Ready)
                        {
                            Ref_Step("判断是否需要扫码");
                        }
                        break;

                    case "判断是否需要扫码":
                        CurrentProduct = ProductInfo.GetInitialProduct();//初始化对象
                        visionPoint = Pallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                        if (FunSelection.Cur.PCB是否启用扫码.GetValue)
                        {
                            Ref_Step("移动到PCB扫码点");
                        }
                        else
                        {
                            WriteToUser("未启用扫码功能！", Brushes.Yellow, false);
                            CurrentProduct.SN = DateTime.Now.ToString("yyyyMMddHHmmss");
                            if (visionPoint == null)
                            {
                                ReportMsg("料盘取完");
                                Ref_Step("通知更换料盘");
                                break;
                            }
                            Ref_Step("判断获取点位方式");
                        }
                        break;
                    case "移动到PCB扫码点":
                        if (Process_AutoRun.Cur.IsClearModeWithCount())
                        {
                            Sleep(10000);
                            break;
                        }
                        ScanPallet.Index = Pallet.Index;
                        ScanPoint = ScanPallet.GetTrayPointWithZU();
                        if (ScanPoint == null)
                        {
                            ReportMsg("料盘取完");
                            Ref_Step("通知更换料盘");
                            break;
                        }
                        ScanPos.PosMsg_1.GetValue = ScanPoint.X;
                        ScanPos.PosMsg_2.GetValue = ScanPoint.Y;
                        ScanPos.PosMsg_3.GetValue = ScanPoint.Z;
                        task1 = new Task(() =>
                        {
                            Gripper.Rotated_MovAbs((short)Gripper.PCB夹爪Tray取料旋转.PosMsg.GetValue, false);
                        });
                        task1.Start();
                        if (!WaitAxisPoint(ScanPos, AxisMoveType.AxisXYZ)) //移动到扫码点
                        {
                            break;
                        }
                        Ref_Step("PCB扫码");
                        break;
                    case "PCB扫码":
                        TempSN = string.Empty;
                        judgeResult = false;
                        ConnectFactory.PCBConnect_ScanCode.Send("start");
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
                            var 机种 = FunMES.Cur.MES是否使用其他机型上传.GetValue ? FunMES.Cur.MES其他机型.GetValue: WorkItemManagerHelper.LoadedName;
                            var checkHeader = FunMESCheck.Cur.CheckHeader.GetValue;
                            var version = FunMESCheck.Cur.SnVersion.GetValue;
                            var checkResult = TempSN.Length > checkHeader.Length && TempSN.Substring(0, checkHeader.Length) == checkHeader;
                            var formateBarcode = $"{机种}{version}{TempSN.Substring(8)}";
                            judgeResult = TempSN.Substring(8).Length>=Funs.FunMESCheck.Cur.SplitSnMaxLength.GetValue;
                            CurrentProduct.SN = formateBarcode;
                            if (CurrentProduct.SN != "" && CurrentProduct.SN != null)
                            {
                                if (CurrentProduct.SN.Count() > 19)
                                {
                                    CurrentProduct.SN = null;
                                    ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                    WriteErrToUser($"SN位数大于19位！视为NG");
                                }
                            }
                            if (lastBarcode == TempSN)
                            {
                                CurrentProduct.SN = null;
                                Params.ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                WriteErrToUser($"PCB码重复！上一个条码为{lastBarcode}当前条码为{TempSN}");
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
                                break;

                            }
                            else
                            {
                                if (FunSelection.Cur.IsUseMes.GetValue)//todo MES
                                {
                                    WriteToUser("启用MES");
                                    Ref_Step("上传MES");
                                }
                                else
                                {
                                    WriteToUser("未启用MES");
                                    Ref_Step("判断获取点位方式");
                                }
                            }
                        }
                        else
                        {
                            WriteToUser("扫码失败！", Brushes.Red, false);
                            //ScanPallet.AddIndex();//扫码失败取下一个
                            //Pallet.Index = ScanPallet.Index;
                            CurrentProduct.SN = null;
                            Params.ParamsNGTray.Cur.AddScanPCBNg();//PCB扫码NG计数
                            if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))//移动到安全点
                            {
                                Ref_Step("移动到PCB扫码点");
                                break;
                            }
                        }
                        break;
                    case "上传MES":
                        var ret=false;
                        if (FunMES.Cur.MES是否使用其他机型上传.GetValue)
                        {  //使用另一型号上传，先上传0x03且只传一个长码,再上传0x02
                            ret = MESHelper2.Cur.Bind(CurrentProduct.SN, "", out error);
                            if (!ret)
                            {
                                WriteErrToUser($"其他机型转型码：{CurrentProduct.SN}，MES绑定失败！{error}");
                                CurrentProduct.SN = null;
                                ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                break;
                            }
                            //todo改成检测头校验
                            ret = MESHelper2.Cur.CheckSN(TempSN, out error);
                            if (!ret)
                            {
                                WriteErrToUser($"PCB码：{TempSN}，MES校验失败！{error}");
                                CurrentProduct.SN = null;
                                ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                break;
                            }

                        }
                        else
                        { //正常使用型号上传，先上传0x02,再上传0x03
                            //todo改成检测头校验
                            ret = MESHelper2.Cur.CheckSN(TempSN, out error);
                            if (!ret)
                            {
                                WriteErrToUser($"PCB站MES校验失败！{error}");
                                CurrentProduct.SN = null;
                                ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                break;
                            }
                            ret = MESHelper2.Cur.Bind(CurrentProduct.SN, TempSN, out error);
                            if (!ret)
                            {
                                WriteErrToUser($"PCB站MES绑定失败！{error}");
                                CurrentProduct.SN = null;
                                ParamsNGTray.Cur.AddMesCheckNGNumNg();//PCB扫码NG计数
                                break;
                            }

                        }
                        Ref_Step("判断获取点位方式");
                        break;

                    case "判断获取点位方式":
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式走固定点位取料", Brushes.Yellow, false);
                            Ref_Step("移动到取料点位");
                        }
                        else
                        {
                            if (Pallet.IsPointByCamera)
                            {
                                Ref_Step("移动到视觉拍照点");
                            }
                            else
                            {
                                WriteToUser("固定点取料，跳过直接走固定点位取料", Brushes.Yellow, false);
                                Ref_Step("移动到取料点位");
                            }
                        }
                        break;

                    case "移动到视觉拍照点":
                        //先保存料盘当前取料穴位位置
                        visionPoint = new TrayPointZU();
                        visionPoint = Pallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                        pos.PosMsg_1.GetValue = visionPoint.X;//料盘固定点位为视觉拍照点
                        pos.PosMsg_2.GetValue = visionPoint.Y;
                        pos.PosMsg_3.GetValue = visionPoint.Z;
                        Out_CameraLight.Out_ON();
                        if (WaitAxisPoint(pos, AxisMoveType.AxisXYZ))
                        {
                            Ref_Step("通知相机拍照");
                        }
                        break;

                    case "通知相机拍照":
                        var cameraResult = Camera.SendAndCheck("T1", out cameraCmd, out error,
                            $"{GetCurrentPos()},{CurrentProduct.SN}", process: this);
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
                            ngCount.ClearNGProductCount();
                            Ref_Step("相机OK处理");
                        }
                        else
                        {
                            if (Process_AutoRun.Cur.IsClearModeWithCount())
                            {
                                Sleep(10000);
                                break;
                            }
                            errorCount++;
                            if (errorCount >= FunCommon.Cur.ErrContinueNum.GetValue)
                            {
                                WriteErrToUser("相机NG超过次数！");
                                errorCount = 0;

                            }
                            Ref_Step("相机NG处理");
                            //ParamsNGTray.Cur.AddPCBLocateNg();
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
                                       Ref_Step("移动到视觉拍照点");
                                   }
                                ),
                                   ("跳过此点", () =>
                                   {
                                       Pallet.AddIndex();
                                       ScanPallet.Index = Pallet.Index;
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
                                    //PCB_SN = array[2].ToString();//相机返回的数组第三个元素为相机解析的SN
                                    //取料Z轴高度使用固定点高度
                                    pos.PosMsg_3.GetValue = ThreeAxisPCB上料.Cur.PosFetchCoilPlace.PosMsg_3.GetValue;

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
                        visionPoint = new TrayPointZU();
                        visionPoint = Pallet.GetTrayPointWithZU();//获取当前料盘的物料穴位索引
                        if (!Pallet.IsPointByCamera)
                        {
                            //取固定点位
                            pos.PosMsg_1.GetValue = visionPoint.X;
                            pos.PosMsg_2.GetValue = visionPoint.Y;
                            pos.PosMsg_3.GetValue = ThreeAxisPCB上料.Cur.PosFetchCoilPlace.PosMsg_3.GetValue;
                        }
                        Pallet.AddIndex();
                        ScanPallet.Index = Pallet.Index;
                        Ref_Step("开始夹取");
                        break;

                    case "开始夹取":
                        task1?.Wait();
                        Gripper.Rotated_MovAbsCheck((short)Gripper.PCB夹爪Tray取料旋转.PosMsg.GetValue);
                        Gripper.Wait_RotatedDone();
                        if (!Gripper.Rotated_MovAbsCheck((short)Gripper.PCB夹爪Tray取料旋转.PosMsg.GetValue))
                        {
                            if (stopWatch.ElapsedMilliseconds > 10000)
                            {
                                WriteErrToUser("夹爪运动超时未完成，请检查PCB夹爪COM口后继续");
                                stopWatch.Restart();
                                break;
                            }
                            break;
                        }
                        if (!Gripper.Clamp_OFF(o => WriteErrToUser(o)))
                        {
                            break;
                        }
                        ClampProductCheckGripper = false;
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
                        if (stopWatch.ElapsedMilliseconds > 5000)
                        {
                            WriteErrToUser("夹爪运动超时未完成，请检查PCB夹爪COM口后继续");
                            stopWatch.Restart();
                            break;
                        }
                        if (!Gripper.Clamp_ON(o => WriteErrToUser(o)))
                        {
                            ErrorDialog("PCB夹爪夹紧检测异常，请选择后续操作",
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
                        ClampProductCheckGripper = Gripper.In_ClampGrip();
                        Sleep(FunTimeSettings.Cur.夹爪取放延时.GetValue);
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
                        if (FunSelection.Cur.PCB上料夹爪物料光电检测.GetValue)
                        {
                            if (!StaticIOHelper.PCB电动夹抓物料检测.In())
                            {
                                ErrorDialog("夹爪夹取产品后，光电未检测到产品，请选择后续操作",
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
                                Ref_Step("判断扫码是否OK");
                            }
                        }
                        else
                        {
                            ReportMsg("PCB上料夹爪物料光电检测未开启！");
                            WriteToUser("PCB上料夹爪物料光电检测未开启！", Brushes.Yellow, false);
                            Ref_Step("判断扫码是否OK");
                            break;
                        }
                        break;

                    case "判断扫码是否OK":
                        if (!judgeResult)
                        {
                            Ref_Step("移动到扫码NG位置");
                            break;
                        }
                        if (CurrentProduct.SN != null)//暂时的条件是扫码不为空
                        {
                            CurrentProduct.Result = ProductInfo.ResultOK;
                            Ref_Step("允许放料前准备动作");
                        }
                        else
                        {
                            //CurrentProduct.SN=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            CurrentProduct.Result = ProductInfo.ResultNG;
                            Ref_Step("移动到扫码NG位置");
                        }
                        break;

                    case "移动到扫码NG位置":
                        NGPoint = NGPallet.GetTrayPointWithZU();
                        if (NGPoint == null)
                        {
                            if (!WaitIO(StaticIOHelper.NG料仓移载气缸, CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("NG料仓移载气缸去原点位超时，请检查");
                                break;
                            }
                            if (!WaitIO(StaticIOHelper.NG移载锁紧气缸, CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("NG移载锁紧气缸去原点位超时，请检查");
                                break;
                            }
                            StaticIOHelper.NG料仓人工按钮灯.Out_ON();
                            inputButton.Reset();
                            ChangeNGTray = true;
                            WriteErrToUser("PCBNG料盘满，请取走NG料");
                            break;
                        }

                        if (FunSelection.Cur.PCBNG放料是否启用传感器.GetValue)
                        {
                            NGPoint = NGPallet.GetTrayPointWithZUFirstOffset(ThreeAxisPCB上料.Cur.ngSensorPos);
                        }

                        NGpos.PosMsg_1.GetValue = NGPoint.X;
                        NGpos.PosMsg_2.GetValue = NGPoint.Y;
                        NGpos.PosMsg_3.GetValue = NGPoint.Z;

                        //实际高度可能很高，先将Z抬升至安全高度
                        if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        //移动到NG穴位
                        if (!WaitAxisPoint(NGpos, AxisMoveType.AxisXY))
                        {
                            break;
                        }

                        Gripper.Rotated_MovAbsCheck((short)Gripper.PCBNG放料旋转.PosMsg.GetValue);
                        Gripper.Wait_RotatedDone();
                        if (!Gripper.Rotated_MovAbsCheck((short)Gripper.PCBNG放料旋转.PosMsg.GetValue))
                        {
                            break;
                        }
                        if (!WaitAxisPoint(NGpos, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        if (!Gripper.Clamp_OFF(o => WriteErrToUser(o)))
                        {
                            break;
                        }
                        //放完NG料回安全高度
                        if (!WaitAxisPoint(ThreeAxis.PosWait, AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        NGPallet.AddIndex();
                        Ref_Step("判断料盘是否允许取料");
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
                            Gripper.Rotated_MovAbs((short)Gripper.PCB夹爪Tray放料旋转.PosMsg.GetValue, false);
                        });
                        task.Start();
                        if (!WaitAxisPoint(ThreeAxis.PosPlace, AxisMoveType.AxisXY))
                        {
                            break;
                        }
                        Ref_Step("等待允许放料");
                        break;

                    case "等待允许放料":
                        if (Process_PCB站.Cur.允许PCB三轴放料)
                        {
                            Process_PCB站.Cur.允许PCB三轴放料 = false;
                            if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                            {
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue - 10;//空跑时夹爪放料抬高10毫米
                                Ref_Step("三轴放料前旋转");
                            }
                            else
                            {
                                if (FunSelection.Cur.PCB放料前相机引导.GetValue)
                                {
                                    Ref_Step("前往放料前定位拍照位");
                                }
                                else
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
                        if (!FunSelection.Cur.PCB放料前相机引导.GetValue)
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
                                Out_CameraLight.Out_ON();
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
                        ErrorDialog("PCB放入相机定位失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机定位");
                                  }
                        ), ("壳体作为NG品", () =>
                        {
                            //CurrentProduct.SetProductNG("PCB放料前定位NG");
                            ParamsNGTray.Cur.AddPCBBeforePlaceNg();
                            Ref_Step("放料完成前Z轴上升到等待位");
                        }
                        )
                               );
                        break;

                    case "相机放料前定位OK处理":
                        if (string.IsNullOrEmpty(cameraCmd.Message))
                        {
                            WriteErrToUser("PCB相机结果OK，但未收到坐标信息，无法放料！相机发送信息:" + cameraCmd.FullMessage);
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
                                //暂定：视觉定位放料视觉发来的是偏移值
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue + double.Parse(array[0]);
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue + double.Parse(array[1]);
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                                Ref_Step("三轴放料前旋转");
                            }
                            catch (Exception ex)
                            {
                                WriteErrToUser($"收到PCB相机消息后解析异常，消息内容:{cameraCmd.FullMessage},异常原因:" + ex.Message);
                                Ref_Step("相机定位");
                            }
                        }
                        break;

                    case "三轴放料前旋转":
                        task?.Wait();
                        Gripper.Rotated_MovAbs((short)Gripper.PCB夹爪Tray放料旋转.PosMsg.GetValue, false);
                        Gripper.Wait_RotatedDone();
                        if (!Gripper.Rotated_MovAbsCheck((short)Gripper.PCB夹爪Tray放料旋转.PosMsg.GetValue))
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
                        Sleep(FunTimeSettings.Cur.夹爪取放延时.GetValue);
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
                            if (!FunSelection.Cur.是否启用PCB放料后相机复检.GetValue)
                            {
                                WriteToUser("未启用相机检测");
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
                        if (!FunSelection.Cur.是否启用PCB放料后相机复检.GetValue)
                        {
                            WriteToUser("未启用相机检测");
                            Ref_Step("放料完成前Z轴上升到等待位");
                            break;
                        }
                        cameraResult = Camera.SendAndCheck("cam5", out cameraCmd, out error,
                            process: this);
                        if (cameraResult == true)
                        {
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
                                WriteErrToUser("转盘工位三PCB放料完成后相机检测不合格，请人工查看是否产品位置不正确");
                                Ref_Step("相机放料后检测NG处理");
                            }
                        }
                        else if (cameraResult == false)
                        {
                            WriteErrToUser(error);
                        }
                        break;

                    case "相机放料后检测NG处理":
                        ErrorDialog("PCB放入后相机检测失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机检测");
                                  }
                        ),
                                  ("壳体作为NG品", () =>
                                  {
                                      //CurrentProduct.SetProductNG("PCB放料后定位NG");
                                      ParamsNGTray.Cur.AddPCBAfterPlaceNg();
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
                        Ref_Step("更新UI数据并保存本地数据");
                        break;

                    case "更新UI数据并保存本地数据":
                        //生产数据信息
                        ParamsUPH.Cur.GetUPH();
                        AviewMotionUI.ProductionStatistics.ProductResult resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.OK;
                        if (CurrentProduct.Result == ProductInfo.ResultOK)
                        {
                            resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.OK;
                        }
                        else
                        {
                            resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.NG;
                            ParamsNGTray.Cur.AddNGInfoOnce(CurrentProduct.FailedMsg);
                        }
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                AviewMotionUI.ProductionStatistics.ProductionStatisticsManger.Current.AddCountOnce(resultEnum,
                                         CurrentProduct.RFID, ProductConverter.GetProductStationResultItems(CurrentProduct).ToArray());
                            }
                            catch (Exception ex)
                            {
                                WriteErrToUser("生产统计更新异常:" + ex.Message);
                            }

                        });

                        var timeNow = DateTime.Now;
                        CsvHelper csv = new CsvHelper(
                        () => $"..//data//ST01数据//{timeNow.ToString("yyyy")}//{timeNow.ToString("yyyy-MM-dd")}",
                        () => "上料数据.csv");
                        csv.SetCSVHeader("时间", "PCB_SN");
                        csv.WriteLine($"{timeNow.ToString("yyyy-MM-dd HH:MM:ss")},E{CurrentProduct.SN}");
                        PCB放料完成 = true;//通知放料完成
                        Ref_Step("等待数据传递完成");
                        break;

                    case "等待数据传递完成":
                        if (Process_PCB站.Cur.三轴数据传递完成)
                        {
                            Process_PCB站.Cur.三轴数据传递完成 = false;
                            Ref_Step("Z轴移动到等待位");
                        }
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
