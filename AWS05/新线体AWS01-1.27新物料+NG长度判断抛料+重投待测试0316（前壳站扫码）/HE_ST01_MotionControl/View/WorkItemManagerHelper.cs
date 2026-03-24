using Handler.Motion;
using HE_ST01_MotionControl.OtherHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkItemHelper2;

namespace Handler.View
{
    /// <summary>
    /// 工单/换型 管理类
    /// </summary>
    public static class WorkItemManagerHelper
    {
        static WorkItemManagerHelper()
        {
            WorkItemHelper2.BackUp.BackUpZipHelper.SoureDirList.Add("Bas");
            //WorkItemHelper2.BackUp.BackUpZipHelper.SoureDirList.Add("AviewConfigs");
            WorkItemHelper2.BackUp.BackUpZipHelper.SoureDirList.Add("AviewConfigs");
            WorkItemHelper2.BackUp.BackUpZipHelper.SoureDirList.Add("CommunicationCfg");
            WorkItemHelper2.BackUp.BackUpZipHelper.SoureDirList.Add("SysCfg");
            workIterm = new ParamWorkIterm(ParamPath_Motion.ModelRoot,
                () => ParamPath_Motion.SelectedDirPath,
    ParamPath_Motion.ResetLoadFold,
    () => ParamPath_Motion.SelectedDirFullPath,
    ParamPath_Motion.ResetLoadFoldFull);


            workIterm.LoadedWorkItemEventHandler += ISaveReaddll.SaveReadManagerHelper.Fun_ReadAll;

            WorkItemManagerHelper.workIterm.LoadedWorkItemEventHandler += () =>
            {
                try
                {
                    var sendStr = Funs.FunSelection.Cur.camPrpogramName.GetValue.ToUpper();
                    if (sendStr == "")
                    {
                        Handler.Motion.StaticInitial.Motion.WriteToUser("视觉换型发送失败,当前为设定视觉方案");
                    }
                    Motion.StaticInitial.Motion.WriteToUser("向相机发送换型命令：" + sendStr);
                    Connect.ConnectFactory.tcpConnect_Camera?.Send(sendStr);
                    Motion.StaticInitial.Motion.WriteToUser("向相机发送换型命令完成：" + sendStr);
                }
                catch (Exception ex)
                {
                    Handler.Motion.StaticInitial.Motion.WriteErrToUser("视觉换型发送失败:" + ex.Message);
                }
            };
            WorkItemManagerHelper.workIterm.LoadedWorkItemEventHandler += () =>
            {
                try
                {
                    if (Funs.FunScrewGun.Cur.ProgramNo.GetValue <= 0)
                    {
                        Handler.Motion.StaticInitial.Motion.WriteErrToUser("螺丝枪换型失败:" + "螺丝枪程序号不正确");
                    }
                    // Connect.ConnectFactory.ScrewGunTCP.ChangeProgram();
                }
                catch (Exception ex)
                {
                    Handler.Motion.StaticInitial.Motion.WriteErrToUser("螺丝枪换型失败:" + ex.Message);
                }
            };
            //WorkItemManagerHelper.workIterm.LoadedWorkItemEventHandler += () =>
            //{
            //    try
            //    {
            //        if (Funs.FunSocketSetting.Cur.SocketIsUseArray != null)
            //        {
            //            Funs.FunSocketSetting.Cur.ChangProgramSocket();
            //        }
            //    }
            //    catch
            //    {

            //    }
            //};

            workIterm.LoadedWorkItemEventHandler += Handler.Motion.StaticInitial.Motion.CurOrder.ResOKFlag.ResetFlag;

            //workIterm.LoadedWorkItemEventHandler += () => {
            //    AADemoControl.ShareStructHelper.TypeName_SetValue(LoadedName);
            //    AADemoControl.ShareStructHelper.aAShareMem.RAM_INT_Set(61, 999);
            //};
            //AADemoControl.ShareStructHelper.TypeName_SetValue(LoadedName);
            //AADemoControl.ShareStructHelper.aAShareMem.RAM_INT_Set(61, 999);
            workIterm.LoadedWorkItemEventHandler += () => Task.Run(async () =>
            {
                await MES.MESHelper2.Cur.SwitchModel();
            });

        }

        public static readonly WorkItemHelper2.ParamWorkIterm workIterm;

        /// <summary>
        /// 当前加载的参数文件的名字
        /// </summary>
        public static string LoadedName
        {
            get { return Motion.ParamPath_Motion.SelectedDirFullName; }
        }
    }
}
