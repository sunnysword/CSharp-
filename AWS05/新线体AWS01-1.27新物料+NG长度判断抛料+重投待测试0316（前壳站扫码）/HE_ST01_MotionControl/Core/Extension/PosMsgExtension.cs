using Handler.Motion;
using Handler.Motion.Axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Core.Extension
{

    public enum AxisMoveType
    {
        AxisX,
        AxisY,
        AxisZ,
        AxisXY,
        AxisXYZ,
    }
    public static class PosMsgExtension
    {
        public static bool CheckPos(this AixCommInfo.ThreeAixsPosMsg posMsg, int axisIndex, double range = 0.1)
        {
            double realPos = default;
            double pos = default;
            switch (axisIndex)
            {
                case 1:
                    realPos = posMsg.GetPos_1Func().Invoke();
                    pos = posMsg.PosMsg_1.GetValue;
                    break;
                case 2:
                    realPos = posMsg.GetPos_2Func().Invoke();
                    pos = posMsg.PosMsg_2.GetValue;
                    break;
                case 3:
                    realPos = posMsg.GetPos_3Func().Invoke();
                    pos = posMsg.PosMsg_3.GetValue;
                    break;
                default:
                    throw new ArgumentException("index无效，无法判断轴位置", nameof(axisIndex));
            }
            return Math.Abs(pos - realPos) < Math.Abs(range);
        }

        public static bool CheckPos(this AixCommInfo.SingleAixPosAndSpeedMsgAndIMov posMsg, double range = 0.1)
        {
            double realPos = default;
            double pos = default;
            if (posMsg.GetProxyMov() is AixCreateBase axis)
            {
                realPos = axis.CheckPos_GetPos();
                pos = posMsg.PosMsg.GetValue;
                return Math.Abs(pos - realPos) < Math.Abs(range);
            }
            else
            {
                throw new Exception("SingleAixPosAndSpeedMsgAndIMov类型转换失败，无法CheckPos");
            }
        }

        public static bool MovWithCheck(this AixCommInfo.SingleAixPosAndSpeedMsgAndIMov posMsg, out string error, double range = 0.1)
        {
            error = string.Empty;
            double realPos = default;
            double pos = default;
            posMsg.Mov();

            if (posMsg.GetProxyMov() is AixCreateBase axis)
            {
                realPos = axis.CheckPos_GetPos();
                pos = posMsg.PosMsg.GetValue;
                if (!(Math.Abs(pos - realPos) < Math.Abs(range)))
                {
                    error = $"{axis.Name}:移动失败，移动到位位置超出允许范围值";
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("SingleAixPosAndSpeedMsgAndIMov类型转换失败，无法CheckPos");
            }
        }


    }
}
