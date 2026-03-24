using AbstractSingleAixdll;
using AccustomeAttributedll;
using AixCommInfo;
using AM.Core.Extension;
using Handler.Motion.Axis;
using ISaveReaddll;
using LanCustomControldllExtend.FourAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Motion.Axis.Base.FourAxis
{
    public class FourAxisBase : LineAixZCreateBase, AixCommInfo.IProxyLineZMov
    {
        public FourAxisBase(string name, ThreeAxisBase threeAxis, AbstractSingleAix axisFour, string keyName = null)
            : this(name, threeAxis.Aix_1, threeAxis.Aix_2, threeAxis.Aix_Z, axisFour, keyName)
        {

        }

        public FourAxisBase(string name, AbstractSingleAixdll.AbstractSingleAix singleAix1,
            AbstractSingleAixdll.AbstractSingleAix singleAix2, AbstractSingleAixdll.AbstractSingleAix singleAix3, AbstractSingleAixdll.AbstractSingleAix singleAix4,
            string keyName = null)
            : base(singleAix1, singleAix2, singleAix3, keyName)
        {
            this.Name = name;
            PosWait = new FourAxisPosMsgILineZMov(() => this,
            () => this.Aix_1.GetCmdPos(true),
            () => this.Aix_2.GetCmdPos(true),
            () => this.Aix_Z.GetCmdPos(true),
            () => this.Axis4.GetCmdPos(true),
            true
            );
            Handler.Motion.StaticInitial.Motion.actionStopEventHandler += () =>
            {
                IsStop = true;
            };
            this.Axis4 = singleAix4;
            this.Read();
        }
        /// <summary>
        /// 第四轴，一般是旋转轴
        /// </summary>
        public readonly AbstractSingleAix Axis4;


        public string Name { get; set; }

        public bool IsStop { get; set; } = false;

        [SaveRemark]
        [TextBoxRemark("等待点")]
        [OrderIdRemark(0)]
        public FourAxisPosMsgILineZMov PosWait;


        public override void SafeCheck()
        {
            base.SafeCheck();
            if (this.Aix_Z.CheckPos_GetPos() > this.PosWait.PosMsg_3.GetValue + 3)
            {
                throw new Exception($"{this.Name}操作失败，Z轴不在等待位或等待位上方");
            }
        }

        /// <summary>
        /// XY轴移动到等待点
        /// </summary>
        [ButtonRemark("XY等待点", EnumColor.YellowGreen)]
        public void XY_MovWait()
        {
            Parallel.Invoke(() => XY_MovAbs(PosWait));
            Axis4.MovAbs(PosWait.PosMsg_4.GetValue);
        }

        /// <summary>
        /// Z轴移动到等待点
        /// </summary>
        [ButtonRemark("Z等待点", EnumColor.YellowGreen)]
        public void Z_MovWait()
        {
            Z_MovAbs(PosWait);
        }
        public void XYU_MovAbs(FourAxisPosMsg fourAixsPosMsg)
        {
            IsStop = false;
            Z_MovWait();
            if (IsStop) return;
            Parallel.Invoke(() => XY_MovAbs(fourAixsPosMsg.PosMsg_1.GetValue, fourAixsPosMsg.PosMsg_2.GetValue),
                () => Axis4.MovAbs(fourAixsPosMsg.PosMsg_4.GetValue));
        }

        public void XYU_MovAbs(double posX, double posY, double posU)
        {
            IsStop = false;
            Z_MovWait();
            if (IsStop) return;
            Parallel.Invoke(() => XY_MovAbs(posX, posY),
                () => Axis4.MovAbs(posU));
        }

        public override void StopXYZ()
        {
            IsStop = true;
            base.StopXYZ();
            Axis4.StopSlowDown();
        }

        /// <summary>
        /// 覆写后，总是将Z轴台到等待点在移动
        /// </summary>
        /// <param name="threeAixsPosMsg"></param>
        public override void XY_MovAbs(ThreeAixsPosMsg threeAixsPosMsg)
        {
            IsStop = false;
            Z_MovWait();
            if (IsStop) return;
            Parallel.Invoke(() => base.XY_MovAbs(threeAixsPosMsg));
        }

        /// <summary>
        /// 总是将Z轴台到等待点在移动
        /// </summary>
        /// <param name="dis1"></param>
        /// <param name="dis2"></param>
        public void XY_MovAbs(double dis1, double dis2)
        {
            IsStop = false;
            Z_MovWait();
            if (IsStop) return;
            Parallel.Invoke(() => base.LineMovAbs(dis1, dis2));
        }

        public void Z_MovAbs(double dis)
        {
            Aix_Z.MovAbs(dis, Z_MovSpeed.Speed, Z_MovSpeed.Acc, Z_MovSpeed.Dec, true, true);

        }

        public override void XY_MovAbs(TwoAixsPosMsg twoAixsPosMsg)
        {
            IsStop = false;
            Z_MovWait();
            if (IsStop) return;
            Parallel.Invoke(() => base.XY_MovAbs(twoAixsPosMsg));
        }

        public void ZProxyMovAbs(double pos, bool checkdone)
        {
            this.Z_MovAbs(pos);
        }

        public void ZProxyWait()
        {
            this.Z_MovWait();
        }

        public void LineProxyMovAbs(double pos1, double pos2, bool checkdone)
        {
            Parallel.Invoke(() => this.XY_MovAbs(pos1, pos2));
        }


        public virtual void Home()
        {
            void AxisHome(AbstractSingleAix axis)
            {
                axis.Home();
                if (axis.IsHomeOK == false)
                {
                    throw new Exception($"{axis.Name}回原失败");
                }
            }
            IsStop = false;

            AxisHome(Aix_Z);
            if (IsStop) return;
            List<Task> list = new List<Task>();
            list.Add(Task.Run(() => AxisHome(Aix_1)));
            list.Add(Task.Run(() => AxisHome(Aix_2)));
            list.Add(Task.Run(() => AxisHome(Axis4)));
            Task.WaitAll(list.ToArray());

        }

        public virtual void GoWaitPos()
        {
            void AxisHome(AbstractSingleAix axis, double pos)
            {
                axis.MovAbs(pos);
                if (!axis.MovWithCheck(pos))
                {
                    throw new Exception($"{axis.Name}移动失败");
                }
            }
            IsStop = false;

            AxisHome(Aix_Z, this.PosWait.PosMsg_3.GetValue);
            if (IsStop) return;
            List<Task> list = new List<Task>();
            list.Add(Task.Run(() => AxisHome(Aix_1, this.PosWait.PosMsg_1.GetValue)));
            list.Add(Task.Run(() => AxisHome(Aix_2, this.PosWait.PosMsg_2.GetValue)));
            Task.WaitAll(list.ToArray());
        }

    }
}
