using RM_dll2;
using RM_dll2.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Handler.Connect;
using Handler.Funs;
using Handler.Motion.IO;
using Handler.Motion.Process.Station;
using Handler.Motion;
using Handler.Motion.Axis;
using Handler.Process.Station;

using Handler.MES;
using AM.Core.Extension;
using HE_ST01_MotionControl.OtherHelpers;
using HE_ST01_MotionControl.Motion.Axis.AxisGroup;
using HE_ST01_MotionControl.Motion.Axis.AxisSingle;
using Handler.Process.Station.TrayLoad;
using Handler.Process.RunMode;
using HE_ST01_MotionControl.Connect.Modbus;
using System.Windows;

namespace Handler.Process
{
    /// <summary>
    /// 机器复位
    /// </summary>
    class Process_ResMachine : RM_dll2.Process.ProcessResMachineBase
    {
        public static readonly Process_ResMachine Cur = new Process_ResMachine();

        private Process_ResMachine() : base("机器复位", StaticInitial.Motion)
        {

        }

        bool IsStop = false;

        public override void Stop()
        {
            IsStop = true;
            base.Stop();
        }

        /// <summary>
        /// 初始化流水线有无载具
        /// </summary>
        /// <returns></returns>
        public bool CarrierCheck()
        {
            //if (StaticIOHelper.前回流移栽载具流进光电检测.In()
            //    || StaticIOHelper.输送线前段皮带线载具流入检测.In()
            //    || StaticIOHelper.输送线前段皮带线载具流出检测.In()
            //    || StaticIOHelper.输送线后段皮带线载具流入检测.In()
            //    || StaticIOHelper.输送线后段皮带线载具流出检测.In()
            //    || StaticIOHelper.回流线前段皮带线载具流入检测.In()
            //    || StaticIOHelper.回流线前段皮带线载具流出检测.In()
            //    || StaticIOHelper.回流线后段皮带线载具流入检测.In()
            //    || StaticIOHelper.回流线后段皮带线载具流出检测.In()
            //    || StaticIOHelper.PCB缓存位载具流进检测.In()
            //    || StaticIOHelper.PCB缓存位载具流出检测.In()
            //    || StaticIOHelper.PCB上料位载具流进检测.In()
            //    || StaticIOHelper.PCB上料位载具流出检测.In()
            //    || StaticIOHelper.外壳上料位载具流进检测.In()
            //    || StaticIOHelper.外壳上料位载具流出检测.In()
            //    || StaticIOHelper.外壳缓存位载具流进检测.In()
            //    || StaticIOHelper.外壳缓存位载具流出检测.In()
            //    || StaticIOHelper.回流线前段缓存位流进检测.In()
            //    || StaticIOHelper.回流线前段缓存位流出检测.In()
            //    || StaticIOHelper.回流线后段缓存位流进检测.In()
            //    || StaticIOHelper.回流线后段缓存位流出检测.In()
            //    || StaticIOHelper.前回流移栽载具流进光电检测.In())
            //{
            //    WriteErrToUser("检测到流水线有载具，请全部取出！");
            //    CarrierCheck();
            //    return false;
            //}
            //else
            {
                StreamLine.Cur.ReturnBackConveyor.RequestStop();
                StreamLine.Cur.ReturnFrontConveyor.RequestStop();
                StreamLine.Cur.ConveyorFront.RequestStop();
                StreamLine.Cur.ConveyorBack.RequestStop();

                //StaticIOHelper.前段输送线启停.Out_OFF();
                //StaticIOHelper.后段输送线启停.Out_OFF();
                //StaticIOHelper.前段回流线启停.Out_OFF();
                //StaticIOHelper.后段回流线启停.Out_OFF();
                StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                return true;
            }
        }

        protected override ProcessResMachineArgs ImplementImplementRunAction()
        {
            ProcessResMachineArgs args = new ProcessResMachineArgs();
            args.IsExitOK = true;
            try
            {
                IsStop = false;
                StaticIOHelper.ResInitial();
                StaticIOHelper.复位按钮指示灯.Out_ON();
                CheckActionHelper checkAction = new CheckActionHelper();
                Ref_Step("判断IO");
                ConnectFactory.输送线本机台请求出料.Out_OFF();
                ConnectFactory.回流线本机台允许进料.Out_OFF();
                bool IsHomeOk = true;

                while (Motion.CurOrder.IsResing)
                {
                    if (IsStop)
                    {
                        IsStop = false;
                        break;
                    }

                    switch (Step)
                    {
                        case "判断IO":
                            if (WorkModeManager.Cur.SelectWorkMode.Mode != WorkMode.TestRun)
                            {
                                if (!ClampRotatedAxisShell.Cur.IsConnected)
                                {
                                    ClampRotatedAxisShell.Cur.Connect();
                                }
                                if (!ClampRotatedAxisPCB.Cur.IsConnected)
                                {
                                    ClampRotatedAxisPCB.Cur.Connect();
                                }

                                ClampRotatedAxisShell.Cur.Rotated_MovAbs(0, true);
                                Process_前壳Tray抓取.Cur.ClampProductCheckGripper = ClampRotatedAxisShell.Cur.In_ClampGrip();

                                ClampRotatedAxisShell.Cur.Rotated_MovAbs(0, true);
                                Process_PCBTray抓取.Cur.ClampProductCheckGripper = ClampRotatedAxisPCB.Cur.In_ClampGrip();

                                if (Process_前壳Tray抓取.Cur.ClampProductCheckGripper == true )
                                {
                                    throw new Exception("前壳取料夹爪状态检测到有产品，请将产品取走");
                                }
                                if (Process_PCBTray抓取.Cur.ClampProductCheckGripper == true )
                                {
                                    throw new Exception("PCB取料夹爪状态检测到有产品，请将产品取走");
                                }
                            }
                            StaticIOHelper.总气源.Out_ON();
                            Ref_Step("复位气缸");
                            break;

                        case "复位气缸":
                            if (CarrierCheck())
                            {
                                StaticIOHelper.CylinderReset();
                                Ref_Step("复位各个轴");
                            }
                            break;

                        case "复位各个轴":
                            Parallel.Invoke(
                                () => ThreeAxisPCB上料.Cur.Home(),
                                () => ThreeAxis前壳上料.Cur.Home());
                            Ref_Step("复位夹爪");
                            break;

                        case "复位夹爪":
                            StaticIOHelper.GripperReset();
                            Ref_Step("复位料仓Z轴");
                            break;

                        case "复位料仓Z轴":
                            Parallel.Invoke(
                             AxisPCB上料工站Tray.Cur.Home,
                             Axis前壳上料工站Tray.Cur.Home
                             );
                            Ref_Step("复位剩余气缸");
                            break;

                        case "复位剩余气缸":
                            StaticIOHelper.ResetOther();
                            Ref_Step("判断每个轴是否复位成功");
                            break;

                        case "判断每个轴是否复位成功":
                            foreach (var item in StaticAixInfoRegisterManager.aixRegisterInfo.CheckAixIsHomeOK)
                            {
                                if (item.GetIsBusy())
                                {
                                    WriteToUser($"{item.Name} 回原点未完成,开始进入等待...", Brushes.Red, false);
                                    if (checkAction.CheckAction(() => item.GetIsBusy() == false, 5000))
                                    {
                                        if (item.GetIsHomeOK())
                                        {
                                            WriteToUser($"{item.Name} 回原点完成...", Brushes.White, false);
                                        }
                                        else
                                        {
                                            IsHomeOk = false;
                                            WriteToUser($"{item.Name} 回原点失败！", Brushes.Red, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.GetIsHomeOK())
                                    {
                                        WriteToUser($"{item.Name} 回原点完成...", Brushes.White, false);
                                    }
                                    else
                                    {
                                        IsHomeOk = false;
                                        WriteToUser($"{item.Name} 回原点失败", Brushes.Red, false);
                                    }
                                }
                            }

                            if (IsHomeOk)
                            {
                                WriteToUser("所有轴已复位成功...", Brushes.Green, false);
                                Ref_Step("轴移动到等待位");
                            }
                            else
                            {
                                throw new Exception("复位失败：存在轴未能复位成功!");
                            }
                            break;

                        case "轴移动到等待位":
                            //AxisRotate转盘工站.Cur.WaitPos.Mov();

                            //var argReset = Handler.Motion.Process.Sub.SubProcessReset.Cur.Run();
                            //if (!argReset.IsExitOK)
                            //{
                            //    throw new Exception($"设备进入等待状态流程失败:{argReset.ErrInfo}");
                            //}
                            //皮带转1s保证所有载具都到位

                            var ret = MessageBox.Show("流线皮带即将转动请确认载具位置！！！", "提示");
                            if (ret == MessageBoxResult.OK)
                            {

                            }

                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            StreamLine.Cur.ConveyorFront.RequestRun(Direction.Forward);
                            StreamLine.Cur.ReturnFrontConveyor.RequestRun(Direction.Forward);
                            StreamLine.Cur.ReturnBackConveyor.RequestRun(Direction.Forward);
                            Sleep(1000);
                            StreamLine.Cur.ConveyorBack.RequestStop();
                            StreamLine.Cur.ConveyorFront.RequestStop();
                            StreamLine.Cur.ReturnFrontConveyor.RequestStop();
                            StreamLine.Cur.ReturnBackConveyor.RequestStop();

                            //通讯IO全部清零
                            ConnectFactory.输送线本机台请求出料.Out_OFF();
                            ConnectFactory.回流线本机台允许进料.Out_OFF();
                            Ref_Step("退出");
                            break;

                        case "退出":
                            args.IsResOK = true;
                            args.IsExitOK = true;
                            IsStop = true;
                            WriteToUser("复位成功", Brushes.Green, true);
                            StaticIOHelper.复位按钮指示灯.Out_OFF();
                            //MESHelper.Cur.MesConnect.Send($"ff0x09;Command:Reset;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=OK;#end");
                            break;
                    }
                    Sleep(10);
                }
            }
            catch (Exception ex)
            {
                WriteToUser($"{Name}:出现异常，{AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex)}");
                StaticIOHelper.复位按钮指示灯.Out_OFF();
                args.IsResOK = false;
                args.IsExitOK = false;
                args.ErrInfo = AM.Tools.ExceptionHelper.GetInnerExceptionMessage(ex);
            }
            return args;
        }
    }
}
