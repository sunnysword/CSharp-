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

    public class ThreeAxis前壳上料 : ThreeAxisBase
    {
        private ThreeAxis前壳上料() : base("前壳上料工站三轴",
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("前壳上料工站搬运X轴")),
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("前壳上料工站搬运Y轴")),
            new AixCreateBase(AM.Core.Axis.AxisInfoManger.GetAxisInfo("前壳上料工站搬运Z轴")))

        {

        }
        public static readonly ThreeAxis前壳上料 Cur = new ThreeAxis前壳上料();

        public static ThreeAxis前壳上料 CreateInstrance()
        {
            return Cur;
        }



        #region 功能实例
        [SaveRemark]
        [TextBoxRemark("载具外壳穴位放料点")]
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

        [SaveRemark]
        [TextBoxRemark("放载具外壳前拍照点")]
        public ThreeAixsPosMsgILineZMov PosCameraBeforePlace = new ThreeAixsPosMsgILineZMov(() => Cur,
          () => Cur.Aix_1.GetCmdPos(true),
          () => Cur.Aix_2.GetCmdPos(true),
          () => Cur.Aix_Z.GetCmdPos(true),
          true
          );


        [SaveRemark]
        [TextBoxRemark("放载具外壳后拍照点")]
        public ThreeAixsPosMsgILineZMov PosCameraAfterPlace = new ThreeAixsPosMsgILineZMov(() => Cur,
      () => Cur.Aix_1.GetCmdPos(true),
      () => Cur.Aix_2.GetCmdPos(true),
      () => Cur.Aix_Z.GetCmdPos(true),
      true
      );
        [SaveRemark]
        [TextBoxRemark("重投物料扫码点")]
        public ThreeAixsPosMsgILineZMov ScanPos = new ThreeAixsPosMsgILineZMov(() => Cur,
       () => Cur.Aix_1.GetCmdPos(true),
       () => Cur.Aix_2.GetCmdPos(true),
       () => Cur.Aix_Z.GetCmdPos(true),
       true
       );

        [SaveRemark]
        [TextBoxRemark("待删除点")]
        public ThreeAixsPosMsgILineZMov cameraNG1 = new ThreeAixsPosMsgILineZMov(() => Cur,
        () => Cur.Aix_1.GetCmdPos(true),
        () => Cur.Aix_2.GetCmdPos(true),
        () => Cur.Aix_Z.GetCmdPos(true),
        true
        );
        #endregion

    }

}
