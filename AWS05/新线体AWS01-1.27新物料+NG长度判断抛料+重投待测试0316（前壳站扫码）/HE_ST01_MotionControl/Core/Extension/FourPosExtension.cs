using AM.Core.Extension;
using Handler.Motion.Axis;
using LanCustomControldllExtend.FourAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Core.Extension
{
    public static class FourPosExtension
    {
        /// <summary>
        /// 移动到指定点位并进行位置检查
        /// </summary>
        /// <param name="posMsg">点位信息</param>
        /// <param name="error">若移动失败，返回的错误信息</param>
        /// <param name="moveType">移动轴类型,XY轴运动是XYU轴运动</param>
        /// <param name="zHeightAbs">z轴预先上抬高度，若为null则上抬至等待位</param>
        /// <param name="range">轴到位判断误差范围</param>
        /// <param name="afterXYMoveBeforeZMoveAction">最后一次z轴动作前需要触发的事件</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool MovWithCheck(this FourAxisPosMsgILineZMov posMsg, out string error, AxisMoveType moveType = AxisMoveType.AxisXYZ, double? zHeightAbs = null, double range = 0.1, Action afterXYMoveBeforeZMoveAction = null)
        {
            error = string.Empty;
            return false;
        }



    }
}
