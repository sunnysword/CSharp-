using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Handler.Connect;
using Handler.Connect.RFID;

using Handler.Motion.Axis;
using Handler.Process;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.View;
using HE_ST01_MotionControl.Motion.Axis.AxisSingle;
using HE_ST01_MotionControl.OtherHelpers;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;

namespace Handler.Motion
{
    /// <summary>
    /// 静态初始化类
    /// 来初始化控制器，所有轴对象的创建、参数的读取、外部设备的链接
    /// </summary>
    public static class StaticInitial
    {
        /// <summary>
        /// 这里用来创建所有对象
        /// </summary>
        static StaticInitial()
        {
            //创建RM_2 
            StandardOrder = new RM_dll2.StandardOrderHelper(StaticAixInfoRegisterManager.aixRegisterInfo);
            Motion = new RM_dll2.MotionHelper(StandardOrder);
            Motion.MotionCfg.IsErrAllToPause = true;

        }


        #region 对象
        public static readonly RM_dll2.MotionHelper Motion;
        public static readonly RM_dll2.StandardOrderHelper StandardOrder;

        /// <summary>
        /// 离焦测试
        /// </summary>
        public static RM_dll2.Process.IActionTrigger DefocusTest;

        #endregion


        public static void Initial()
        {
            ConnectFactory.Initial();

            ////打开控制卡
            //读取所有参数
            ISaveReaddll.SaveReadManagerHelper.Fun_ReadAll();

            //设置RM复位、自动运行对象
            Motion.SetProcessResMachine(Process_ResMachine.Cur);
            Motion.SetProcessAutoRun(Process_AutoRun.Cur);

            //初始化IO
            IO.StaticIOHelper.Initial();
            IO.StaticIOHelper.Register();
            //初始化其它

            PalletManger.Initial();//tray作为并行task

            //相机
            //Thread.Sleep(5000);
            //try
            //{
            //    if (Funs.FunSelection.Cur.camPrpogramName.GetValue == "")
            //    {
            //        Handler.Motion.StaticInitial.Motion.WriteErrToUser("切换相机模板失败:" + "相机模板为");
            //    }
            //    Connect.ConnectFactory.tcpConnect_Camera?.Send(Funs.FunSelection.Cur.camPrpogramName.GetValue);

            //}
            //catch (Exception ex)
            //{
            //    Handler.Motion.StaticInitial.Motion.WriteErrToUser("相机换型失败:" + ex.Message);

            //}

            //try
            //{
            //    CheckCarrierdll.CarrierCheckManger.Cur.SetGetModelNameFunc(() => WorkItemManagerHelper.LoadedName);
            //    CheckCarrierdll.CarrierCheckManger.Cur.Initial();
            //    WorkItemManagerHelper.workIterm.LoadedWorkItemEventHandler += () =>
            //    {
            //        CheckCarrierdll.CarrierCheckManger.Cur.ChangeModel(WorkItemManagerHelper.LoadedName);
            //    };
            //}
            //catch (Exception ex)
            //{
            //    Handler.Motion.StaticInitial.Motion.WriteToUser(AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
            //    MessageBox.Show(ex.Message);
            //}

        }

        public static void Close()
        {
            void SafeAction(params Action[] actions)
            {
                foreach (var item in actions)
                {
                    try
                    {
                        item();
                    }
                    catch
                    {


                    }
                }
            }

            SafeAction(Motion.Close,
            IO.StaticIOHelper.Clsoe,
            ConnectFactory.Close
            );






        }

    }
}
