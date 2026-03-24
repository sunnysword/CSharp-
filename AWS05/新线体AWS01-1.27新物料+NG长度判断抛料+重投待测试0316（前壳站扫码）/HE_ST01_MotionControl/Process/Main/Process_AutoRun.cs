using RM_dll2.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Connect;
using Handler.Motion.IO;
using Handler.Motion.Process.Station;
using Handler.Product;
using Handler.Motion;
using Handler.Process.Station;
using Handler.Connect.FrontBack;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Connect.Camera;
using Handler.Process.Station.TrayLoad;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;

namespace Handler.Process
{
    class Process_AutoRun : RM_dll2.Process.ProcessAutoRunBase
    {
        public static readonly Process_AutoRun Cur = new Process_AutoRun();

        private Process_AutoRun() : base("自动运行", StaticInitial.Motion)
        {
        }

        /// <summary>
        /// 当前是否是清料模式
        /// </summary>
        public bool IsClearMode { get; set; } = false;

        public bool IsClearModeNew { get; set; } = false;


        /// <summary>
        /// 当前是否是重投模式
        /// </summary>
        public bool isReProduce { get; set; }=false;



        public int ClearCount = 0;

        public bool IsClearModeWithCount()
        {
            return IsClearModeNew && ClearCount <= 0;
        }

        public bool IsClearModeWithCountForSHell()
        {
            if (ClearCount == 1)
            {
                //当清零数量为1时，为了避免这个时候有在做的pcb，所以一定要等pcb工位状态空闲才可以下一步
                while (!Process_PCB站.Cur.IsReady)
                {
                    Sleep(1000);
                    WriteToUser("等待pcb工位允许流入！");
                }
            }
            return IsClearModeNew && ClearCount <= 0;
        }

        public void SetClearCount()
        {
            if (ClearCount > 0 && IsClearModeNew)
            {
                ClearCount--;
            }

        }

        public static readonly object obj_lock = new object();
        bool IsExit = false;

        public override void Stop()
        {
            base.Stop();
            CameraHelper.IsStop = true;
            RM_dll2.CheckActionHelper.Stop();
            Motion.CurOrder.ResOKFlag.ResetFlag();

            //if (FrontBackHelper.Upline1 != null)
            //{
            //    FrontBackHelper.Upline1?.Reset_ProductIsLoad();
            //    FrontBackHelper.Upline1?.Init();
            //    FrontBackHelper.Upline1.IsRunning = false;
            //}

            //if (FrontBackHelper.Downline1 != null)
            //{
            //    FrontBackHelper.Downline1.Reset_ProductIsLoad();
            //    FrontBackHelper.Downline1.Init();
            //    FrontBackHelper.Downline1.IsRunning = false;
            //}

            //重置 下料轴放料信号
            // StationRightUpDown.Cur.Allow = false;
        }

        private bool CheckBeforeAutoRun()
        {
            IsClearMode = false;
            return true;
        }

        protected override ProcessAutoRunArgs ImplementImplementRunAction()
        {
            ProcessAutoRunArgs processAutoRunArgs = new ProcessAutoRunArgs() { IsExitOK = true };
            if (!CheckBeforeAutoRun())
            {
                processAutoRunArgs.IsExitOK = true;
                return processAutoRunArgs;
            }
            MES.MESHelper2.Cur.resList.Clear();
            //var argReset = Handler.Motion.Process.Sub.SubProcessReset.Cur.Run();
            //if (!argReset.IsExitOK)
            //{
            //    WriteErrToUser($"设备进入等待状态流程失败:{argReset.ErrInfo}");
            //    processAutoRunArgs.IsExitOK = false;
            //    processAutoRunArgs.ErrInfo = $"设备进入等待状态流程失败:{argReset.ErrInfo}";
            //    return processAutoRunArgs;
            //}
            if (!IsProcessRunning) return processAutoRunArgs;

            List<Task> taskAutoRun = new List<Task>();
            void RunProcess(RM_dll2.Process.ProcessBase process)
            {
                taskAutoRun.Add(Task.Run(() =>
                {
                    var args = process.Run();
                    if (args.IsExitOK == false)
                    {
                        throw new Exception(args.ErrInfo);
                    }
                }));
            }

            try
            {
                App.Current.Dispatcher.Invoke(() => WorkStationBase.ProductInfoList.Clear());
                //foreach (var item in StationManger.Cur.RotateAxisStationList)
                //{
                //    item.Allow = false;
                //    item.AllowFinish = false;
                //    item.IsAtPosTest = false;
                //    item.IsReady = false;
                //    item.IsLoad = false;
                //    item.ClearProduct();

                //}

                RunProcess(Process_PCBTray抓取.Cur);
                RunProcess(Process_前壳Tray抓取.Cur);
                RunProcess(Process_PCB缓存站.Cur);
                RunProcess(Process_PCB站.Cur);
                RunProcess(Process_前壳缓存站.Cur);
                RunProcess(Process_前壳站.Cur);
                RunProcess(Process_回流移栽站.Cur);
                RunProcess(Process_回流流出站.Cur);
                RunProcess(Process_回流流入站.Cur);
                RunProcess(PalletManger.Process_PCB);
                RunProcess(PalletManger.Process_前壳);
                Task.WaitAll(taskAutoRun.ToArray());
                processAutoRunArgs.IsExitOK = true;
            }
            catch (Exception ex)
            {
                WriteErrToUser($"{Name}:出现异常，{AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex)}");
                WriteErrToUser($"{Name}:出现异常，{AM.Tools.ExceptionHelper.GetInnerExceptionMessage(ex)}");
                processAutoRunArgs.IsExitOK = false;
                processAutoRunArgs.ErrInfo = AM.Tools.ExceptionHelper.GetInnerExceptionMessage(ex); ;
                Motion.CommitStopOrder();
            }
            finally
            {
            }
            return processAutoRunArgs;
        }
    }
}
