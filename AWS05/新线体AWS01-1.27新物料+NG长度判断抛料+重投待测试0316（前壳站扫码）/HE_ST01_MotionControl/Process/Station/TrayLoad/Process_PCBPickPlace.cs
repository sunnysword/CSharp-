using AM.Core.IO;
using Handler.Connect.Camera;
using Handler.Core.Process;
using Handler.Motion.IO;
using Handler.Motion.PalletTray;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station;
using Handler.Process;
using Handler.Product;
using Iodll;
using LanCustomControldllExtend.FourAxis;
using Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Handler.Motion.Axis;

namespace HE_ST01_MotionControl.Process.Station.TrayLoad
{
    class Process_PCBPickPlace : StationBase
    {
        public static readonly Process_PCBPickPlace Cur = new Process_PCBPickPlace();
        private Process_PCBPickPlace() : base("PCB取料三轴")
        {
        }
        public IoOutHepler Out_CameraLight { get; set; }
        public ThreeAxisForPCB FourAxis => ThreeAxisForPCB.Cur;
        public CameraHelper Camera { get; set; }
        public PalletInfoOnePoint Pallet { get; set; }
        public Cylinder CylinderClamp { get; set; }
        public IoInHelper ClampProductCheckSensor { get; set; }
        public Process_TrayLoad TrayProcess { get; set; }
        public FourAxisPosMsgILineZMov PosPlace { get; set; }
        public FourAxisPosMsgILineZMov PosShowering { get; set; }

        public IAllowPlace PlaceStation { get; set; }
        public Func<bool> IsClampCheckProductSensor { get; set; }

        public bool CanSkipStep { get; set; }



        public void FlushTray()
        {
            WriteToUser("料仓事件清除料盘索引一次");
            this.Pallet.Clear();
        }
        public string GetCurrentPos()
        {
            return $"{FourAxis.Aix_1.GetCmdPos()};{FourAxis.Aix_2.GetCmdPos()}";
        }
        protected override void ImplementRunAction()
        {
            TrayProcess.FlushTrayAction -= FlushTray;
            TrayProcess.FlushTrayAction += FlushTray;
            IsReady = false;
            string[] CamValue = new string[] { "", "", "", "" };
            int _countIndex = 0;
            string error = string.Empty;
            TrayPointZU visionPoint = null;

            FourAxisPosMsgILineZMov pos = new FourAxisPosMsgILineZMov(() => FourAxis,
            () => this.FourAxis.Aix_1.GetCmdPos(true),
            () => this.FourAxis.Aix_2.GetCmdPos(true),
            () => this.FourAxis.Aix_Z.GetCmdPos(true),
            () => this.FourAxis.Axis4.GetCmdPos(true),
            true
            );
            FourAxisPosMsgILineZMov pos2 = new FourAxisPosMsgILineZMov(() => FourAxis,
            () => this.FourAxis.Aix_1.GetCmdPos(true),
            () => this.FourAxis.Aix_2.GetCmdPos(true),
            () => this.FourAxis.Aix_Z.GetCmdPos(true),
            () => this.FourAxis.Axis4.GetCmdPos(true),
            true
            );
            CameraCmd cameraCmd = new CameraCmd();
            ContinuousNGStatistics ngCount = new ContinuousNGStatistics("PCB相机定位", () => Funs.FunCommon.Cur.ErrContinueNum.GetValue);
            Ref_Step("Z轴移动到等待位");

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

                        if (!Funs.FunStationSetting.Cur.IsUseStation3GetPcb.GetValue)
                        {
                            Ref_Step("Z轴移动到等待位");
                            WriteToUser("当前屏蔽PCB取料", Brushes.Yellow, false);
                            Sleep(10000);
                            break;
                        }


                        if (WaitAxisPoint(FourAxis.PosWait, AM.Core.Extension.AxisMoveType.AxisZ))
                        {
                            if (CanSkipStep)
                            {
                                SleepLoopCheck(10000);
                            }

                            // 判断模式是否为清料模式 
                            // 判断夹爪|| 工位1检测 || 工位2检测 || 工位3检测
                            // 当前工位1,2,3存在物料时,正常执行提前取PCB ，否则停止取料
                            if (Process_AutoRun.Cur.IsClearMode)
                            {
                                WriteToUser($"1----清料模式,判断当前工位3是否存在物料", Brushes.Yellow, false);
                                Sleep(1000);
                                if (PlaceStation.Allow)
                                {
                                    WriteToUser($"2----清料模式,判断当前工位3是允许放料", Brushes.Yellow, false);
                                    if (StaticIOHelper.In_IsThereAnyProductDetectedAtStation3OfRotaryTable.In())
                                    {
                                        WriteToUser($"3----清料模式,判断当前工位3感应到产品准备放料", Brushes.Yellow, false);

                                        Ref_Step("判断料盘是否允许取料");
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                            Ref_Step("判断料盘是否允许取料");
                        }
                        break;

                    case "判断料盘是否允许取料":
                        if (TrayProcess.Load_state == ENUM_Load_State.Ready || CanSkipStep)
                        {
                            Ref_Step("计算点位");
                        }
                        break;


                    case "计算点位":
                        CurrentProduct = ProductInfo.GetInitialProduct();
                        visionPoint = Pallet.GetTrayPointWithZU();
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
                        pos.PosMsg_1.GetValue = visionPoint.X;
                        pos.PosMsg_2.GetValue = visionPoint.Y;
                        pos.PosMsg_3.GetValue = visionPoint.Z;
                        pos.PosMsg_4.GetValue = pos.GetPos_4Func().Invoke();
                        Out_CameraLight.Out_ON();
                        if (WaitAxisPoint(pos, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            Ref_Step("通知相机拍照");
                        }
                        break;

                    case "通知相机拍照":
                        var extraMsg = Funs.FunStationSetting.Cur.IsUseStation3PCBSN.GetValue ? "1" : "0";
                        var cameraResult = Camera.SendAndCheck("cam3", out cameraCmd, out error,
                            GetCurrentPos() + $";{extraMsg}", process: this);
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
                        //空跑模式
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过产品相机放料前检测结果", Brushes.Yellow, false);
                            Ref_Step("相机OK处理");
                            break;
                        }

                        if (cameraCmd.Result == CameraCmd.Result_OK)
                        {
                            Out_CameraLight.Out_OFF();
                            ngCount.ClearNGProductCount();
                            Ref_Step("相机OK处理");
                        }
                        else
                        {
                            Ref_Step("相机NG处理");
                            ///Pcb扫码-失败NG
                            //ParamsNGTray.Cur.AddScanPcbNg();
                        }
                        break;

                    case "相机OK处理":
                        try
                        {
                            //空跑模式
                            if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                            {
                                WriteToUser("空跑模式，跳过产品相机放料前检测结果", Brushes.Yellow, false);
                                Ref_Step("移动到取料点位");
                                break;
                            }

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
                                    var array = cameraCmd.Message.Split(';');
                                    pos.PosMsg_1.GetValue = double.Parse(array[0]);
                                    pos.PosMsg_2.GetValue = double.Parse(array[1]);
                                    pos.PosMsg_3.GetValue = double.Parse(array[2]);
                                    pos.PosMsg_4.GetValue = double.Parse(array[3]);
                                    CurrentProduct = ProductInfo.GetInitialProduct();
                                    if (Funs.FunStationSetting.Cur.IsUseStation3PCBSN.GetValue)
                                    {
                                        if (array.Length >= 5)
                                        {
                                            if (array[4] == "" || array[4] == "0")
                                            {
                                                var mesg = ErrorDialog("PCBSN为空!!", "重新拍照", "取下一个");
                                                if (mesg == "重新拍照")
                                                {
                                                    Ref_Step("通知相机拍照");
                                                    break;
                                                }
                                                else
                                                {
                                                    Ref_Step("相机NG处理");
                                                    break;
                                                }
                                            }
                                            CurrentProduct.PCBSN = array[4];
                                        }
                                    }
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

                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过直接走固定点位取料", Brushes.Yellow, false);
                            if (!WaitIO(CylinderClamp, CylinderActionType.OriginPos))
                            {
                                break;
                            }
                            if (!WaitAxisPoint(pos, AM.Core.Extension.AxisMoveType.AxisXYZ))
                            {
                                break;
                            }
                            Ref_Step("夹爪夹紧");
                            break;
                        }

                        if (!Pallet.IsPointByCamera)
                        {
                            pos.PosMsg_1.GetValue = visionPoint.X;
                            pos.PosMsg_2.GetValue = visionPoint.Y;
                            pos.PosMsg_3.GetValue = visionPoint.Z;
                            pos.PosMsg_4.GetValue = visionPoint.U;
                        }
                        if (!WaitIO(CylinderClamp, CylinderActionType.OriginPos))
                        {
                            break;
                        }
                        if (!WaitAxisPoint(pos, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("夹爪夹紧");
                        break;

                    case "夹爪夹紧":
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            Sleep(500);
                            Ref_Step("夹爪上升");
                            break;
                        }

                        if (!WaitIO(CylinderClamp, CylinderActionType.WorkPos))
                        {
                            break;
                        }
                        Sleep(500);
                        Ref_Step("夹爪上升");
                        break;

                    case "夹爪上升":
                        if (!WaitAxisPoint(FourAxis.PosWait, AM.Core.Extension.AxisMoveType.AxisZ))
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
                            Ref_Step("判断是否需要吹淋");
                            break;
                        }

                        if (!IsClampCheckProductSensor())
                        {
                            ReportMsg("有无检测未启用");
                            Ref_Step("判断是否需要吹淋");
                            break;
                        }
                        if (ClampProductCheckSensor.In() == false)
                        {
                            ErrorDialog("夹爪夹取产品后，传感器未检测到产品，请选择后续操作",
                                   ("重新检测", null),
                                   ("重新抓取", () =>
                                   {
                                       Ref_Step("移动到取料点位");
                                   }
                            )
                                );
                        }
                        else
                        {
                            Ref_Step("判断是否需要吹淋");
                        }
                        break;

                    case "判断是否需要吹淋":
                        Pallet.AddIndex();

                        if (!Funs.FunStationSetting.Cur.IsUseStation3Showering.GetValue)
                        {
                            WriteToUser("吹淋未启用");
                            Ref_Step("前往放料点位上方");
                        }
                        else
                        {
                            Ref_Step("前往吹淋点");
                        }
                        break;

                    case "前往吹淋点":
                        if (!WaitAxisPoint(FourAxisPCBLoad.Cur.PosShowering, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("开始吹淋");

                        break;

                    case "开始吹淋":
                        //吹淋
                        StaticIOHelper.Out_BlowShowerOnOffControl.Out_ON();
                        SleepLoopCheck(Funs.FunStationSetting.Cur.Station3ShoweringTime.GetValue);
                        StaticIOHelper.Out_BlowShowerOnOffControl.Out_OFF();
                        Ref_Step("前往放料点位上方");

                        break;

                    case "前往放料点位上方":
                        if (Funs.FunStationSetting.Cur.IsUseStation3CameraLocationBeforePlace.GetValue)
                        {
                            if (!WaitAxisPoint(FourAxis.PosCameraBeforePlace, AM.Core.Extension.AxisMoveType.AxisXY))
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (!WaitAxisPoint(PosPlace, AM.Core.Extension.AxisMoveType.AxisXY))
                            {
                                break;
                            }
                        }

                        Ref_Step("等待允许放料");
                        break;

                    case "等待允许放料":
                        if (PlaceStation.Allow || CanSkipStep)
                        {
                            PlaceStation.Allow = false;
                            if (Funs.FunStationSetting.Cur.IsUseStation3CameraLocationBeforePlace.GetValue)
                            {
                                Ref_Step("前往放料前定位拍照位");
                            }
                            else
                            {
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                                pos2.PosMsg_4.GetValue = PosPlace.PosMsg_4.GetValue;
                                Ref_Step("四轴放料");
                            }
                        }
                        break;

                    case "前往放料前定位拍照位":
                        Out_CameraLight.Out_ON();
                        if (!WaitAxisPoint(FourAxis.PosCameraBeforePlace, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("相机定位");
                        break;

                    case "相机定位":
                        if (!Funs.FunStationSetting.Cur.IsUseStation3CameraLocationBeforePlace.GetValue)
                        {
                            WriteToUser("未启用相机定位");
                            pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue;
                            pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue;
                            pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                            pos2.PosMsg_4.GetValue = PosPlace.PosMsg_4.GetValue;
                            Ref_Step("四轴放料");
                            break;
                        }
                        cameraResult = Camera.SendAndCheck("cam4", out cameraCmd, out error,
                            process: this);

                        //空跑模式
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过产品相机后检测结果", Brushes.Yellow, false);
                            Sleep(100);
                            Out_CameraLight.Out_OFF();

                            Ref_Step("相机放料前定位OK处理");
                            break;
                        }
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
                        ErrorDialog("PCB放入前壳前相机定位失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机定位");
                                  }
                        ),
                                  ("壳体作为NG品", () =>
                                  {
                                      CurrentProduct.SetProductNG("PCB放料前定位NG");
                                      ParamsNGTray.Cur.AddScanPcbNg();
                                      Ref_Step("放料完成前Z轴上升到等待位");
                                  }
                        )
                               );
                        break;


                    case "相机放料前定位OK处理":
                        //空跑模式
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过产品相机后检测结果2", Brushes.Yellow, false);
                            Sleep(100);

                            Ref_Step("四轴放料");
                            break;
                        }

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
                                pos2.PosMsg_1.GetValue = PosPlace.PosMsg_1.GetValue + double.Parse(array[0]);
                                pos2.PosMsg_2.GetValue = PosPlace.PosMsg_2.GetValue + double.Parse(array[1]);
                                pos2.PosMsg_3.GetValue = PosPlace.PosMsg_3.GetValue;
                                pos2.PosMsg_4.GetValue = PosPlace.PosMsg_4.GetValue;

                                Ref_Step("四轴放料");
                            }
                            catch (Exception ex)
                            {
                                WriteErrToUser($"收到PCB相机消息后解析异常，消息内容:{cameraCmd.FullMessage},异常原因:" + ex.Message);
                                Ref_Step("相机定位");
                            }

                        }

                        break;

                    case "四轴放料":
                        if (!WaitAxisPoint(pos2, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("放料后夹爪松开");
                        break;

                    case "放料后夹爪松开":
                        if (!WaitIO(CylinderClamp, CylinderActionType.OriginPos))
                        {
                            break;
                        }
                        Ref_Step("放料后夹爪上升");
                        break;

                    case "放料后夹爪上升":
                        if (!WaitAxisPoint(FourAxis.PosWait, AM.Core.Extension.AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        if (!Funs.FunStationSetting.Cur.IsUseStation3CameraCheckAfterPlace.GetValue)
                        {
                            WriteToUser("未启用相机检测");
                            Ref_Step("放料完成前Z轴上升到等待位");
                        }
                        else
                        {
                            Ref_Step("前往放料后相机拍照位");
                        }
                        break;

                    case "前往放料后相机拍照位":
                        Out_CameraLight.Out_ON();
                        if (!WaitAxisPoint(FourAxis.PosCameraAfterPlace, AM.Core.Extension.AxisMoveType.AxisXYZ))
                        {
                            break;
                        }
                        Ref_Step("相机检测");
                        break;

                    case "相机检测":
                        if (!Funs.FunStationSetting.Cur.IsUseStation3CameraCheckAfterPlace.GetValue)
                        {
                            WriteToUser("未启用相机检测");
                            Ref_Step("放料完成前Z轴上升到等待位");
                            break;
                        }
                        cameraResult = Camera.SendAndCheck("cam5", out cameraCmd, out error,
                            process: this);
                        //空跑模式
                        if (CurrentWorkMode.Mode == RunMode.WorkMode.TestRun)
                        {
                            WriteToUser("空跑模式，跳过产品相机后检测结果3", Brushes.Yellow, false);
                            Sleep(100);
                            Out_CameraLight.Out_OFF();
                            Ref_Step("放料完成前Z轴上升到等待位");
                            break;
                        }


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
                        ErrorDialog("PCB放入前壳后相机检测失败",
                                  ("重新拍照", () =>
                                  {
                                      Ref_Step("相机检测");
                                  }
                        ),
                                  ("壳体作为NG品", () =>
                                  {
                                      CurrentProduct.SetProductNG("PCB放料后定位NG");
                                      ParamsNGTray.Cur.AddPressingNg();
                                      Ref_Step("放料完成前Z轴上升到等待位");
                                  }
                        )
                               );
                        break;

                    case "放料完成前Z轴上升到等待位":
                        if (!WaitAxisPoint(FourAxis.PosWait, AM.Core.Extension.AxisMoveType.AxisZ))
                        {
                            break;
                        }
                        Ref_Step("通知放料完成");
                        break;

                    case "通知放料完成":
                        if (PlaceStation.CurrentProduct != null)
                        {
                            //PlaceStation.CurrentProduct.PCBSN = CurrentProduct?.SN;
                            PlaceStation.CurrentProduct.Result = CurrentProduct?.Result;

                            PlaceStation.CurrentProduct.Cam3PCBValue = CamValue[0];
                            PlaceStation.CurrentProduct.Cam3PCBLimit = CamValue[1];
                            PlaceStation.CurrentProduct.Cam4PCBValue = CamValue[2];
                            PlaceStation.CurrentProduct.Cam4PCBLimit = CamValue[3];
                            PlaceStation.CurrentProduct.PCBSN = CurrentProduct.PCBSN;
                            WriteToUser($"当前产品的视觉检测值为{CurrentProduct.Cam1BlobValue} -- {CurrentProduct.Cam1BlobLimit}");
                            WriteToUser($"当前产品的PCB检测值为{PlaceStation.CurrentProduct.Cam3PCBValue} -- {PlaceStation.CurrentProduct.Cam3PCBLimit}");
                            WriteToUser($"当前产品的PCB放料后检测值为{PlaceStation.CurrentProduct.Cam4PCBValue} -- {PlaceStation.CurrentProduct.Cam4PCBLimit}");
                            WriteToUser($"当前产品的PCB放料后检的PCBSN为{PlaceStation.CurrentProduct.PCBSN}");
                            CamValue[0] = "";
                            CamValue[1] = "";
                            CamValue[2] = "";
                            CamValue[3] = "";
                        }
                        PlaceStation.AllowFinish = true;
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
