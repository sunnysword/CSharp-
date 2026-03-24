using Handler.Motion.Axis;
using Handler.Motion.IO;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Motion.Axis.AxisSingle
{
    public class Axis前壳上料工站Tray : AxisTrayLiftBase
    {
        private Axis前壳上料工站Tray()
            : base(AM.Core.Axis.AxisInfoManger.GetAxisInfo
                  ("前壳上料工站Tray盘轴Z"))
        {
            Read();

        }
        public static readonly Axis前壳上料工站Tray Cur = new Axis前壳上料工站Tray();
        public static Axis前壳上料工站Tray CreateInstrance()
        {
            return Cur;
        }

        #region 覆写方法区


        #endregion

        #region 功能实例

        #endregion

    }
}
