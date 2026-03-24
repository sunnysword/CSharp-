using Handler.Motion.Axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Core.Extension
{
    public static class ThreePosExtension
    {

        public static bool MovWithCheck(this AixCommInfo.ThreeAixsPosMsgILineZMov posMsg, out string error, AxisMoveType moveType = AxisMoveType.AxisXYZ, double range = 0.1, Action afterXYMoveBeforeZMoveAction = null)
        {
            error = string.Empty;
            if (posMsg.GetIProxyLineZMov() is ThreeAxisBase threeAxis)
            {
                threeAxis.IsStop = false;
                if (!threeAxis.Aix_Z.CheckPos(threeAxis.PosWait.PosMsg_3.GetValue, 0.01))
                {
                    threeAxis.Z_MovAbs(threeAxis.PosWait);
                    System.Threading.Thread.Sleep(60);
                    if (!threeAxis.Aix_Z.CheckPos(threeAxis.PosWait.PosMsg_3.GetValue, range))
                    {
                        error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                        return false;
                    }
                    if (threeAxis.IsStop)
                    {
                        error = $"{threeAxis.Name}: IsStop停止标志位被触发";
                        return false;
                    }
                }
                if (moveType == AxisMoveType.AxisZ)
                {
                    if (posMsg.PosMsg_3.GetValue == threeAxis.PosWait.PosMsg_3.GetValue)
                    {
                        return true;
                    }
                }
                switch (moveType)
                {
                    case AxisMoveType.AxisX:
                        threeAxis.Aix_1.MovAbs(posMsg.PosMsg_1.GetValue);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: X轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisY:
                        threeAxis.Aix_2.MovAbs(posMsg.PosMsg_2.GetValue);
                        if (!threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Y轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisZ:
                        threeAxis.Aix_Z.MovAbs(posMsg.PosMsg_3.GetValue);
                        System.Threading.Thread.Sleep(60);
                        if (!threeAxis.Aix_Z.CheckPos(posMsg.PosMsg_3.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisXY:
                        threeAxis.XY_MovAbs(posMsg);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range)
                            && !threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: XY轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisXYZ:
                        threeAxis.XY_MovAbs(posMsg);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range)
                            && !threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: XY轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        if (threeAxis.IsStop)
                        {
                            error = $"{threeAxis.Name}: IsStop停止标志位被触发";
                            return false;
                        }
                        if (afterXYMoveBeforeZMoveAction != null)
                        {
                            afterXYMoveBeforeZMoveAction();
                        }
                        threeAxis.Z_MovAbs(posMsg);
                        System.Threading.Thread.Sleep(60);
                        if (!threeAxis.Aix_Z.CheckPos(posMsg.PosMsg_3.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;

                    default:
                        throw new Exception("moveType无具体值，无法判断轴移动类型");
                }

            }
            else
            {
                throw new Exception("ThreeAxisBase类型转换失败，无法MovWithCheck");
            }
        }
        /// <summary>
        /// 移动到指定点位并进行位置检查(1.Z轴上抬 2.XY平移 3.Z轴下降)
        /// </summary>
        /// <param name="posMsg"></param>
        /// <param name="zHeight">z轴高度，绝对位置，即z轴先上抬多少绝对高度，然后再移动到xy点位</param>
        /// <param name="error"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool MovWithCheck(this AixCommInfo.ThreeAixsPosMsgILineZMov posMsg, double zHeight, out string error, AxisMoveType moveType = AxisMoveType.AxisXYZ, double range = 0.1)
        {
            error = string.Empty;
            if (posMsg.GetIProxyLineZMov() is ThreeAxisBase threeAxis)
            {
                threeAxis.IsStop = false;
                threeAxis.Z_MovAbs(zHeight);
                System.Threading.Thread.Sleep(60);
                if (!threeAxis.Aix_Z.CheckPos(zHeight, range))
                {
                    error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                    return false;
                }
                if (threeAxis.IsStop)
                {
                    error = $"{threeAxis.Name}: IsStop停止标志位被触发";
                    return false;
                }
                threeAxis.CancleOneSafeCheck();

                switch (moveType)
                {
                    case AxisMoveType.AxisX:
                        threeAxis.Aix_1.MovAbs(posMsg.PosMsg_1.GetValue);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: X轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisY:
                        threeAxis.Aix_2.MovAbs(posMsg.PosMsg_2.GetValue);
                        if (!threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Y轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisZ:
                        threeAxis.Aix_Z.MovAbs(posMsg.PosMsg_3.GetValue);
                        System.Threading.Thread.Sleep(60);
                        if (!threeAxis.Aix_Z.CheckPos(posMsg.PosMsg_3.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisXY:
                        threeAxis.XY_MovAbs(posMsg);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range)
                            && !threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: XY轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;
                    case AxisMoveType.AxisXYZ:
                        threeAxis.XY_MovAbs(posMsg);
                        if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range)
                            && !threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: XY轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        if (threeAxis.IsStop)
                        {
                            error = $"{threeAxis.Name}: IsStop停止标志位被触发";
                            return false;
                        }
                        System.Threading.Thread.Sleep(60);
                        threeAxis.Z_MovAbs(posMsg);
                        if (!threeAxis.Aix_Z.CheckPos(posMsg.PosMsg_3.GetValue, range))
                        {
                            error = $"{threeAxis.Name}: Z轴移动失败，移动距离未达到设定点位";
                            return false;
                        }
                        return true;

                    default:
                        throw new Exception("moveType无具体值，无法判断轴移动类型");
                }
            }
            else
            {
                throw new Exception("ThreeAxisBase类型转换失败，无法MovWithCheck");
            }
        }

        public static bool MovSingleAxisWithCheck(this AixCommInfo.ThreeAixsPosMsgILineZMov posMsg, int axisIndex, out string error, double range = 0.1)
        {
            error = string.Empty;
            if (posMsg.GetIProxyLineZMov() is ThreeAxisBase threeAxis)
            {
                AbstractSingleAixdll.AbstractSingleAix axis = null;
                double posValue = default;
                switch (axisIndex)
                {
                    case 1:
                        axis = threeAxis.Aix_1;
                        posValue = posMsg.PosMsg_1.GetValue;
                        break;
                    case 2:
                        axis = threeAxis.Aix_2;
                        posValue = posMsg.PosMsg_2.GetValue;
                        break;
                    case 3:
                        axis = threeAxis.Aix_Z;
                        posValue = posMsg.PosMsg_3.GetValue;
                        break;
                    default:
                        throw new ArgumentException("index无效，无法判断动作轴", nameof(axisIndex));
                }
                axis.MovAbs(posValue);
                if (!axis.CheckPos(posValue, range))
                {
                    error = $"{threeAxis.Name}: 轴移动失败，移动距离未达到设定点位";
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("ThreeAxisBase类型转换失败，无法MovWithCheck");
            }
        }

        public static bool MovXYAxisWithCheck(this AixCommInfo.ThreeAixsPosMsgILineZMov posMsg, out string error, double range = 0.1)
        {
            error = string.Empty;
            if (posMsg.GetIProxyLineZMov() is ThreeAxisBase threeAxis)
            {
                threeAxis.IsStop = false;
                threeAxis.XY_MovAbs(posMsg);
                if (!threeAxis.Aix_1.CheckPos(posMsg.PosMsg_1.GetValue, range)
                    && !threeAxis.Aix_2.CheckPos(posMsg.PosMsg_2.GetValue, range))
                {
                    error = $"{threeAxis.Name}: XY轴移动失败，移动距离未达到设定点位";
                    return false;
                }
                if (threeAxis.IsStop)
                {
                    error = $"{threeAxis.Name}: IsStop停止标志位被触发";
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("ThreeAxisBase类型转换失败，无法MovWithCheck");
            }
        }
    }
}
