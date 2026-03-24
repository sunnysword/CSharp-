using AixRegisterInfodll;
using Handler.Core.Extension;
 
using Handler.Motion.Axis;
using Handler.Process.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Handler.Funs;
using RM_dll2.Light;

namespace Handler.Motion.IO
{
    /// <summary>
    /// 静态的IO 额外扫描
    /// 在这里可以添加你所想要的IO状态扫描
    /// </summary>
    public static class IOScanHelper
    {

        static IOScanHelper()
        {
           
        }



        static RM_dll2.MotionHelper motion => StaticInitial.Motion;


        static readonly IoCheck ioCheck_IsAutoRunning = new IoCheck(() => motion.CurOrder.IsAutoRunning);
        static readonly IoCheck ioCheck_IsResing = new IoCheck(() => motion.CurOrder.IsResing);
        static readonly IoCheck ioCheck_IsPause = new IoCheck(() => motion.CurOrder.IsPause);
        static readonly IoCheck ioCheck_IsErr = new IoCheck(() => motion.CurOrder.IsErr);

        public static readonly IoCheck ioCheck_DispenserZ;

        static RM_dll2.ExtraMsgHelper msgHelper_Door = new RM_dll2.ExtraMsgHelper();

        static bool IsDoorCheck = false;
        static bool Reset_Enabled = false;
        static bool Start_Enabled = false;
        static bool Pause_Enabled = false;
        static bool Clear_Enabled = false;


        /// <summary>
        /// 扫描方法
        /// </summary>
        public static void Scan()
        {
            //if (ioCheck_Start.RisingCheck())
            //{
            //    StationLoad.Cur.IsStart = true;
            //}

            //if (!StaticIOHelper.CheckAirSourceInput() && !motion.CurOrder.IsErr)
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    if (!StaticIOHelper.CheckAirSourceInput() && !motion.CurOrder.IsErr)
            //    {
            //        System.Threading.Thread.Sleep(1000);
            //        motion.WriteErrToUser("气源无输入信号,请检查气压");
            //    }
            //}

            //if (Funs.FunSelection.Cur.IsUseFrontHolderLimitSensor.GetValue)
            //{
            //    if (StaticIOHelper.In_TheFrontShellClampingClawDetectionMechanismIsInPlace.In() && !motion.CurOrder.IsErr)
            //    {
            //        FourAxisHolderLoad.Cur.Aix_Z.StopSlowDown();
            //        motion.WriteErrToUser($"{FourAxisHolderLoad.Cur.Aix_Z.Name}运动失败，夹爪限位传感器被触发!({StaticIOHelper.In_TheFrontShellClampingClawDetectionMechanismIsInPlace.GetInfo()}");
            //    }
            //}
            //if (Funs.FunSelection.Cur.IsUsePCBLimitSensor.GetValue)
            //{
            //    if (StaticIOHelper.In_PcbGripperDetectionMechanismInPlace.In() && !motion.CurOrder.IsErr)
            //    {
            //        FourAxisPCBLoad.Cur.Aix_Z.StopSlowDown();
            //        motion.WriteErrToUser($"{FourAxisPCBLoad.Cur.Aix_Z.Name}运动失败，夹爪限位传感器被触发!({StaticIOHelper.In_PcbGripperDetectionMechanismInPlace.GetInfo()}");
            //    }
            //}
            //if (Funs.FunSelection.Cur.IsUseUnloadLimitSensor.GetValue)
            //{
            //    if (StaticIOHelper.In_TheCuttingClampClawDetectionMechanismIsInPlace.In() && !motion.CurOrder.IsErr)
            //    {
            //        FourAxisUnload.Cur.Aix_Z.StopSlowDown();
            //        motion.WriteErrToUser($"{FourAxisUnload.Cur.Aix_Z.Name}运动失败，夹爪限位传感器被触发!({StaticIOHelper.In_TheCuttingClampClawDetectionMechanismIsInPlace.GetInfo()}");
            //    }
            //}

            //IsDoorCheck = StaticIOHelper.插销门锁监控1.In() && StaticIOHelper.插销门锁监控2.In() && StaticIOHelper.插销门锁监控3.In() && StaticIOHelper.门禁1.In() && StaticIOHelper.门禁2.In() ;
            //if (motion.CurOrder.IsAutoRunning)
            //{
            //    if (!motion.CurOrder.IsPause)
            //    {
            //        if (!IsDoorCheck)
            //        {
            //            if(false)//todo现在门监控无用
            //            {
            //                motion.CommitPauseOrder();
            //            }

            //        }
            //    }
            //}
            //if (IsDoorCheck)
            //{
            //    msgHelper_Door.Info = "";
            //}
            //else
            //{
            //    msgHelper_Door.Info = "安全门打开，请注意安全!!!";
            //}

            if (motion.CurOrder.IsErr || motion.CurOrder.IsAlarm || motion.CurOrder.IsEMG || motion.CurOrder.IsErrPause || motion.CurOrder.IsPause)
            {
                if (Handler.Funs.FunSelection.Cur.IsUseSafeDoor.GetValue) 
                {
                    StaticIOHelper.前安全门锁.Out_OFF();
                    StaticIOHelper.左安全门锁.Out_OFF();
                    StaticIOHelper.后安全门锁.Out_OFF();
                }
                
            }


            #region 操作面板动作
            //if (StaticIOHelper.上电按钮?.In() == true && StaticIOHelper.下电按钮?.In() == true)//
            //{
            //    StaticIOHelper.启动按钮指示灯.Out_ON();//启动按钮灯亮
            //    //StaticIOHelper.Out_PowerOffButtonLight.Out_OFF();
            //}

            //if (StaticIOHelper.上电按钮?.In() == false && StaticIOHelper.下电按钮?.In() == false)//
            //{
            //    StaticIOHelper.启动按钮指示灯.Out_OFF();
            //    //StaticIOHelper.Out_PowerOffButtonLight.Out_ON();
            //}

            if (motion.CurOrder.IsErr || motion.CurOrder.IsAlarm || motion.CurOrder.IsEMG || motion.CurOrder.IsResing || motion.CurOrder.IsAutoRunning)
            {
                Reset_Enabled = false;

            }
            else
            {
                Reset_Enabled = true;//判断什么状态下置复位
            }

            if (motion.CurOrder.IsErr || motion.CurOrder.IsAlarm || motion.CurOrder.IsEMG || motion.CurOrder.IsResing || !motion.CurOrder.IsResOK)
            {
                Start_Enabled = false;
            }
            else
            {
                if (motion.CurOrder.IsAutoRunning && !(motion.CurOrder.IsErrPause || motion.CurOrder.IsPause))
                {
                    if (StationBase.CheckAnyProcessIsPause() && motion.CurOrder.IsAutoRunning)
                    {
                        Start_Enabled = true;
                    }
                    else
                    {
                        Start_Enabled = false;
                    }
                }
                else
                {
                    Start_Enabled = true;
                }
            }

            if (motion.CurOrder.IsAutoRunning && !motion.CurOrder.IsPause)
            {
                Pause_Enabled = true;
            }
            else
            {
                Pause_Enabled = false;
            }

            if (motion.CurOrder.IsErr || motion.CurOrder.IsAlarm || motion.CurOrder.IsEMG)
            {
                Clear_Enabled = true;
            }
            else
            {
                Clear_Enabled = false;
            }

            if (StaticIOHelper.启动?.In() == true)//启动按钮被按下
            {
                if (Start_Enabled == true)
                {
                    StaticIOHelper.启动按钮指示灯.Out_ON();
                    StaticIOHelper.复位按钮指示灯.Out_OFF();
                    StaticIOHelper.初始化按钮指示灯.Out_OFF();
                    StaticIOHelper.停止按钮指示灯.Out_OFF();
                    StaticIOHelper.暂停按钮指示灯.Out_OFF();
                    //StaticIOHelper.前段输送线启停.Out_ON();
                    //StaticIOHelper.后段输送线启停.Out_ON();
                  
                    if (Handler.Funs.FunSelection.Cur.IsUseSafeDoor.GetValue) 
                    {
                        StaticIOHelper.前安全门锁.Out_ON();
                        StaticIOHelper.左安全门锁.Out_ON();
                        StaticIOHelper.后安全门锁.Out_ON();
                    }
                    
                    StaticInitial.Motion.CommitStartRunOrder();
                }
            }

            if (StaticIOHelper.复位?.In() == true)//复位清除报警
            {
                if (Clear_Enabled == true)
                {
                    //将下列按钮置为复位状态效果，但未复位
                    StaticIOHelper.启动按钮指示灯.Out_OFF();
                    StaticIOHelper.复位按钮指示灯.Out_ON();
                    StaticIOHelper.初始化按钮指示灯.Out_OFF();
                    StaticIOHelper.停止按钮指示灯.Out_OFF();
                    StaticIOHelper.暂停按钮指示灯.Out_OFF();
                  
                    StaticInitial.Motion.CommitClearErrOrder();
                }
            }

            if (StaticIOHelper.初始化?.In() == true)//
            {
                if (Reset_Enabled == true)
                {
                    StaticIOHelper.启动按钮指示灯.Out_OFF();
                    StaticIOHelper.复位按钮指示灯.Out_OFF();
                    StaticIOHelper.初始化按钮指示灯.Out_ON();
                    StaticIOHelper.停止按钮指示灯.Out_OFF();
                    StaticIOHelper.暂停按钮指示灯.Out_OFF();

                    //启动复位，机台初始化
                    StaticInitial.Motion.CommitResOrder();
                }
            }

            if (StaticIOHelper.停止?.In() == true)
            {
                StaticIOHelper.启动按钮指示灯.Out_OFF();
                StaticIOHelper.复位按钮指示灯.Out_OFF();
                StaticIOHelper.初始化按钮指示灯.Out_ON();
                StaticIOHelper.停止按钮指示灯.Out_OFF();
                StaticIOHelper.暂停按钮指示灯.Out_OFF();

                StaticInitial.Motion.CommitStopOrder();
                StaticInitial.Motion.CurOrder.ResOKFlag.ResetFlag();
            }

            if (StaticIOHelper.暂停?.In() == true)
            {
                if (Pause_Enabled == true)
                {
                    StaticIOHelper.启动按钮指示灯.Out_OFF();
                    StaticIOHelper.复位按钮指示灯.Out_OFF();
                    StaticIOHelper.初始化按钮指示灯.Out_OFF();
                    StaticIOHelper.停止按钮指示灯.Out_OFF();
                    StaticIOHelper.暂停按钮指示灯.Out_ON();
                    //StaticIOHelper.前段输送线启停.Out_OFF();
                    //StaticIOHelper.后段输送线启停.Out_OFF();
                
                    StaticInitial.Motion.CommitPauseOrder();
                }
            }

            #endregion 操作面板动作
            IsDoorCheck = StaticIOHelper.前安全门锁开关信号.In() && StaticIOHelper.左安全门锁开关信号.In()&&StaticIOHelper.左边门接近感应器.In();
            if (motion.CurOrder.IsAutoRunning)
            {
                if (!motion.CurOrder.IsPause)
                {
                    //加启用安全门
                    if (FunSelection.Cur.IsUseSafeDoor.GetValue && !IsDoorCheck&&!Process_TrayLoad.isAllow)
                    {
                        motion.CommitPauseOrder();
                    }
                }
            }
            if (IsDoorCheck)
            {
                msgHelper_Door.Info = "";
            }
            else
            {
                //加启用安全门
                if (FunSelection.Cur.IsUseSafeDoor.GetValue)
                {
                    msgHelper_Door.Info = "安全门打开，请注意安全!!!";
                }
            }
        }
    }
}
