using AccustomeAttributedll;
using AixCommInfo;
using Handler.Motion.Axis;
using HE_ST01_MotionControl.Motion.Axis.Base.FourAxis;
using ISaveReaddll;
using LanCustomControldllExtend.FourAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Motion.Axis.AxisGroup
{
    public class ThreeAxisPCB上料 : ThreeAxisBase
    {
        private ThreeAxisPCB上料() : base("PCB上料工站三轴",
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("PCB上料工站搬运X轴")),
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("PCB上料工站搬运Y轴")),
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("PCB上料工站搬运Z轴")))
        {

        }
        public static readonly ThreeAxisPCB上料 Cur = new ThreeAxisPCB上料();

        public static ThreeAxisPCB上料 CreateInstrance()
        {
            return Cur;
        }


        [SaveRemark]
        [TextBoxRemark("放载具PCB点")]
        public ThreeAixsPosMsgILineZMov PosPlace = new ThreeAixsPosMsgILineZMov(() => Cur,
           () => Cur.Aix_1.GetCmdPos(true),
           () => Cur.Aix_2.GetCmdPos(true),
           () => Cur.Aix_Z.GetCmdPos(true),
           true
           );

        [SaveRemark]
        [TextBoxRemark("取料点Z高度点")]
        public ThreeAixsPosMsgILineZMov PosFetchCoilPlace = new ThreeAixsPosMsgILineZMov(() => Cur,
            () => Cur.Aix_1.GetCmdPos(true),
            () => Cur.Aix_2.GetCmdPos(true),
            () => Cur.Aix_Z.GetCmdPos(true),
            true
            );

        //[SaveRemark]
        //[TextBoxRemark("放PCBNG位Z高度")]
        //public ThreeAixsPosMsgILineZMov PCBNG_Z_Place = new ThreeAixsPosMsgILineZMov(() => Cur,
        //    () => Cur.Aix_1.GetCmdPos(true),
        //    () => Cur.Aix_2.GetCmdPos(true),
        //    () => Cur.Aix_Z.GetCmdPos(true),
        //    true
        //    );

        [SaveRemark]
        [TextBoxRemark("放载具PCB前拍照点")]
        public ThreeAixsPosMsgILineZMov PosCameraBeforePlace = new ThreeAixsPosMsgILineZMov(() => Cur,
          () => Cur.Aix_1.GetCmdPos(true),
          () => Cur.Aix_2.GetCmdPos(true),
          () => Cur.Aix_Z.GetCmdPos(true),
          true
          );

        [SaveRemark]
        [TextBoxRemark("放载具PCB后拍照点")]
        public ThreeAixsPosMsgILineZMov PosCameraAfterPlace = new ThreeAixsPosMsgILineZMov(() => Cur,
        () => Cur.Aix_1.GetCmdPos(true),
        () => Cur.Aix_2.GetCmdPos(true),
        () => Cur.Aix_Z.GetCmdPos(true),
        true
        );

        //[SaveRemark]
        //[TextBoxRemark("去扫码位")]
        //public ThreeAixsPosMsgILineZMov scanPos = new ThreeAixsPosMsgILineZMov(() => Cur,
        //() => Cur.Aix_1.GetCmdPos(true),
        //() => Cur.Aix_2.GetCmdPos(true),
        //() => Cur.Aix_Z.GetCmdPos(true),
        //true
        //);

        [SaveRemark]
        [TextBoxRemark("NG料位传感器检测位XY首位坐标")]
        public ThreeAixsPosMsgILineZMov ngSensorPos = new ThreeAixsPosMsgILineZMov(() => Cur,
         () => Cur.Aix_1.GetCmdPos(true),
         () => Cur.Aix_2.GetCmdPos(true),
         () => Cur.Aix_Z.GetCmdPos(true),
         true
         );

        [SaveRemark]
        [TextBoxRemark("NG料位传感器检测Z")]
        public ThreeAixsPosMsgILineZMov ngSensorPosZ = new ThreeAixsPosMsgILineZMov(() => Cur,
        () => Cur.Aix_1.GetCmdPos(true),
        () => Cur.Aix_2.GetCmdPos(true),
        () => Cur.Aix_Z.GetCmdPos(true),
        true
        );


    }
}
