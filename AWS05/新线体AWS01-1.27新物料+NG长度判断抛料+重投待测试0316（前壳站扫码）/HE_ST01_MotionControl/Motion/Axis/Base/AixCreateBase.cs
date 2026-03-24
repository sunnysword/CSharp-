using AbstractSingleAixdll;
using AccustomeAttributedll;
using AixCommInfo;
using SingleAixdll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handler.Motion.Axis
{
    public class AixCreateBase : AbstractSingleAixdll.AbstractSingleAix, IProxyMov, ISaveReaddll.ISaveRead
    {

        class ISingleAixCfgForMN200
        {
            public string Name { get; set; }

            public short LineNo { get; set; }

            public short DevNo { get; set; }
            public SingleAixdll.SvonLogic svonLogic { get; set; }
            public bool isCheckINP { get; set; }

        }

        class ISingleAixCfgForZMotion
        {
            public string IP { get; set; }

            public string Name { get; set; }

            public short DevNo { get; set; }

            public SingleAixdll.SvonLogic SvonLogic { get; set; }
        }


        public AixCreateBase(ApplicationSettingsBase applicationSettingsBase, string keyName = null) : base(CreateSingleZMotionAix(applicationSettingsBase), CreateAbstractAixCreateArgs(applicationSettingsBase), StaticAixInfoRegisterManager.aixRegisterInfo)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                SaveRead = new SaveReaddll.SaveReadHeler(() => ParamPath_Motion.CurFileFullPath, () => this);
            }
            else
            {
                SaveRead = new SaveReaddll.SaveReadHeler(() => ParamPath_Motion.CurFileFullPath, () => this, section: keyName);
            }
        }


        SaveReaddll.SaveReadHeler SaveRead;

        [ButtonRemark("回原点", EnumColor.LightGreen)]
        public override void Home()
        {
            base.Home();
        }

        public Action SafeCheckEventHandler;

        public override void SafeCheck()
        {
            base.SafeCheck();
            SafeCheckEventHandler?.Invoke();
        }
        private static ISingleAix CreateSingleZMotionAix(ApplicationSettingsBase applicationSettingsBase)
        {


            return CreateSingleAix<ISingleAixCfgForZMotion>(applicationSettingsBase, s =>
            {
                return new ZMotionHelper.SingleAix.SingleAixHelper
                (
                    s.IP,
                    s.Name,
                    s.DevNo,
                    s.SvonLogic

                    );
            });
        }

        //private static ISingleAix CreateMN200SingleAix(ApplicationSettingsBase applicationSettingsBase)
        //{
        //    return CreateSingleAix<ISingleAixCfgForMN200>(applicationSettingsBase, s =>
        //    {
        //        return new Hg_MN200_dll2.SingleAixHelper
        //        (
        //            s.Name,
        //            s.LineNo,
        //            s.DevNo,
        //            s.svonLogic,
        //            s.isCheckINP
        //            );
        //    });
        //}

        public virtual void MovAbs(double dis, bool checkdone)
        {
            base.MovAbs(dis, checkdone, true);
        }

        public virtual void MovAbs(double dis, double speed, double acc, double dec, bool checkdone)
        {
            base.MovAbs(dis, speed, acc, dec, checkdone, true);
        }

        public void Read()
        {
            SaveRead?.Read();
        }

        public void Save()
        {
            SaveRead.Save();
        }
    }
}
