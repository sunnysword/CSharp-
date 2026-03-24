using AccustomeAttributedll;
using AixCommInfo;
using Handler.Motion.Axis;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Motion.Axis.AxisSingle
{
    public class Axis前壳取料U : AxisCreateAbsEncoderBase
    {
        private Axis前壳取料U()
            : base(AM.Core.Axis.AxisInfoManger.GetAxisInfo
                  ("前壳取料四轴U"), needCheckSLimit: false)
        {
            WaitPos.MovFinishedAction += () =>
            {
                RotatedMovNextHelper.Reset();
            };
        }
        public static readonly Axis前壳取料U Cur = new Axis前壳取料U();
        public static Axis前壳取料U CreateInstrance()
        {
            return Cur;
        }

        #region 覆写方法区
        public override void Mov(double dis)
        {
            SafeCheck();
            base.Mov(dis);
        }
        public override void JOGP()
        {
            SafeCheck();
            base.JOGP();
        }

        public override void JOGN()
        {
            SafeCheck();
            base.JOGN();
        }

        public override void SafeCheck()
        {

        }

        public bool CheckRange(LineAixZCreateBase threeaxis, ThreeAixsPosMsg pos)
        {
            if (threeaxis.Aix_1.CheckPos(pos.PosMsg_1.GetValue, 2)
                && threeaxis.Aix_2.CheckPos(pos.PosMsg_2.GetValue, 2)
                && threeaxis.Aix_Z.CheckPos(pos.PosMsg_3.GetValue, 2))
            {
                return true;
            }
            return false;
        }

        [ButtonRemark("回原点", EnumColor.LightGreen)]
        public override void Home()
        {
            RotatedMovNextHelper.Reset();
            base.Home();

        }
        #endregion

        #region 功能实例

        [SaveRemark]
        [TextBoxRemark("等待位")]
        [ButtonRemark("等待位")]
        /// <summary>
        /// 等待位
        /// </summary>
        public SingleAixPosAndSpeedMsgAndIMov WaitPos = new SingleAixPosAndSpeedMsgAndIMov(() => Cur);

        #endregion

        #region 方法操作
        [ButtonRemark("下一工位")]
        public void MovNext()
        {
            SafeCheck();
            RotatedMovNextHelper.MovePosition(1);
        }

        [ButtonRemark("上一工位")]
        public void MovLast()
        {
            SafeCheck();
            RotatedMovNextHelper.MovePosition(-1);
        }

        //[ButtonRemark("一直转")]
        //public void MovForever()
        //{
        //    while (true)
        //    {
        //        for (int i = 0; i < 20; i++)
        //        {
        //            SafeCheck();
        //            RotatedMovNextHelper.MovePosition(1);
        //            System.Threading.Thread.Sleep(100);
        //        }

        //        for (int i = 0; i < 5; i++)
        //        {
        //            SafeCheck();
        //            RotatedMovNextHelper.MovePosition(1,-1);
        //            System.Threading.Thread.Sleep(100);
        //        }
        //    }

        //}
        #endregion

    }
}
