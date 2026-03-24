using AccustomeAttributedll;
using AixCommInfo;
using AM.Core.Extension;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handler.Motion.Axis
{
    /// <summary>
    /// UV灯轴
    /// </summary>
    public class AxisCreateAbsEncoderBase : AixCreateBase
    {
        public static List<AxisCreateAbsEncoderBase> AbsEncoderMotorList = new List<AxisCreateAbsEncoderBase>();

        public static void SetAllMotorSoftLimit()
        {
            foreach (var item in AbsEncoderMotorList)
            {
                item.SetSoftLimit();
            }
        }

        public AxisCreateAbsEncoderBase(ApplicationSettingsBase applicationSettingsBase, string keyName = null,bool needCheckSLimit=true)
            : base(applicationSettingsBase,keyName)
        {
            AbsEncoderMotorList.Add(this);

            PosFowardSoftLimit = new SingleAixPosAndSpeedMsgAndIMov(() => this);
            PosReversalSoftLimit = new SingleAixPosAndSpeedMsgAndIMov(() => this);
            PosHome = new SingleAixPosAndSpeedMsgAndIMov(() => this);

            Read();
            if (needCheckSLimit)
            {
                StaticAixInfoRegisterManager.aixRegisterInfo.CheckAixLimitList.Add(
             new AixRegisterInfodll.AixLimitStateInfo(
                 new AixRegisterInfodll.IoCheck(() =>
                 {
                     if (CardInital.card.IsCardReady)
                     {
                         return this.ZMotionEx_GetAxisSELP() || this.ZMotionEx_GetAxisSELN();
                     }
                     else
                     {
                         return false;
                     }
                 }),
             () =>
             {
                 if (this.ZMotionEx_GetAxisSELP())
                 {
                     return $"{this.Name}：软极限正极限触发";
                 }
                 else
                 {
                     return $"{this.Name}：软极限负极限触发";
                 }
             }));
            }
            else
            {
                PosFowardSoftLimit.PosMsg.GetValue = 200000000;
                PosReversalSoftLimit.PosMsg.GetValue = -200000000;
                SetSoftLimit();
                Save();
            }
         

        }
        #region 覆写方法区
        public override void Home()
        {
            SafeCheck();
            for (int i = 0; i < 3; i++)
            {
                var pulseAbs = this.ZMotionEx_GetAxisEncoderValue();
                //double pos = ((double)pulseAbs) / this.PPU_pulsRound_CMD * this.PPU_helicalPitch;
                while (pulseAbs >= 18000)
                {
                    pulseAbs = pulseAbs - 18000;
                }
                while (pulseAbs <= -18000)
                {
                    pulseAbs = pulseAbs + 18000;
                }
                this.ZMotionEx_SetDPos((float)pulseAbs);
                this.ZMotionEx_SetMPos((float)pulseAbs);
                System.Threading.Thread.Sleep(50);
            }
            this.MovAbs(this.PosHome.PosMsg.GetValue, this.HomeSpeed.Speed, this.HomeSpeed.Acc, this.HomeSpeed.Dec, true, true);
            this.IsHomeOK = true;
        }

        public override double GetCmdPos()
        {
            return base.GetEncPos();
        }
        #endregion

        #region 功能实例

        /// <summary>
        /// 原点位
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("原点位")]
        //[ButtonRemark("正限位")]
        public SingleAixPosAndSpeedMsgAndIMov PosHome;


        /// <summary>
        /// 正限位
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("正限位")]
        //[ButtonRemark("正限位")]
        public SingleAixPosAndSpeedMsgAndIMov PosFowardSoftLimit;

        /// <summary>
        /// 负限位
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("负限位")]
        //[ButtonRemark("负限位")]
        public SingleAixPosAndSpeedMsgAndIMov PosReversalSoftLimit;

        #endregion

        #region 方法操作

        [ButtonRemark("设置正负限位", enumColor: EnumColor.Yellow)]
        public void Button_SetSoftLimit()
        {
            var ret = MessageBox.Show("确定设置该轴的正负软极限位吗", "请确认", MessageBoxButton.YesNo);
            if (ret == MessageBoxResult.No)
                return;

            SetSoftLimit();
        }

        //[ButtonRemark("设置原点", enumColor: EnumColor.Yellow)]
        //public void Button_SetHome()
        //{
        //    var ret = MessageBox.Show("确定设置当前位置为当前轴的原点位吗", "请确认", MessageBoxButton.YesNo);
        //    if (ret == MessageBoxResult.No)
        //        return;

        //    SetSoftLimit();
        //}


        public void SetSoftLimit()
        {
            try
            {
                if (PosFowardSoftLimit.PosMsg.GetValue == PosReversalSoftLimit.PosMsg.GetValue)
                {
                    PosFowardSoftLimit.PosMsg.GetValue = 1000;
                    PosReversalSoftLimit.PosMsg.GetValue = -1000;
                }
                this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().SetFsLimit((float)this.ConvertToCmdPuls(PosFowardSoftLimit.PosMsg.GetValue));
                this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().SetRsLimit((float)this.ConvertToCmdPuls(PosReversalSoftLimit.PosMsg.GetValue));

                this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().SetFsLimit((float)this.ConvertToCmdPuls(PosFowardSoftLimit.PosMsg.GetValue));
                this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().SetRsLimit((float)this.ConvertToCmdPuls(PosReversalSoftLimit.PosMsg.GetValue));


                this.Save();
                //this.ZMotionEx_SetSoftLimitForward((float)this.ConvertToCmdPuls(PosFowardSoftLimit.PosMsg.GetValue));
                //this.ZMotionEx_SetSoftLimitReversal((float)this.ConvertToCmdPuls(PosReversalSoftLimit.PosMsg.GetValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //float pos1=0f,pos2=0f;
            //var ret = cszmcaux.zmcaux.ZAux_Direct_GetFsLimit(
            // this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
            // this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,ref pos1);

            //var ret2 = cszmcaux.zmcaux.ZAux_Direct_GetRsLimit(
            //this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
            //this.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo, ref pos2);
        }
        #endregion

    }
}
