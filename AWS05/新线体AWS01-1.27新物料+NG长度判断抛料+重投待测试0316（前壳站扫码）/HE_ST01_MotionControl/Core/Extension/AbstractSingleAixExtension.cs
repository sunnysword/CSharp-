using AbstractSingleAixdll;
using AixCommInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Core.Extension
{
    public static class AbstractSingleAixExtension
    {
        public static bool MovWithCheck(this AbstractSingleAix axis, SingleAixPosAndSpeedMsgAndIMov point)
        {
            point.Mov();
            axis.WaitDone();
            return axis.CheckPos(point.PosMsg.GetValue, 0.01);
        }

        public static bool MovWithCheck(this AbstractSingleAix axis, double point)
        {
            axis.SafeCheck();
            axis.MovAbs(point);
            return axis.CheckPos(point, 0.1);
        }

        public static bool MovWithCheck(this AbstractLineAixsdll.AbstractLineAixs axis, double point1, double point2)
        {
            axis.SafeCheck();
            axis.LineMovAbs(point1, point2);
            return axis.Aix_1.CheckPos(point1, 0.1)
                && axis.Aix_2.CheckPos(point2, 0.1);
        }

        public static bool MovWithCheck(this AbstractLineAixsdll.AbstractLineAixs axis, TwoAixsPosMsg point)
        {
            axis.SafeCheck();
            axis.LineMovAbs(point.PosMsg_1.GetValue, point.PosMsg_2.GetValue);
            return axis.Aix_1.CheckPos(point.PosMsg_1.GetValue, 0.1)
                && axis.Aix_2.CheckPos(point.PosMsg_2.GetValue, 0.1);
        }

        public static bool MovWithCheck(this AbstractLineAixsdll.AbstractLineAixs axis, ThreeAixsPosMsg point)
        {
            axis.SafeCheck();
            axis.LineMovAbs(point.PosMsg_1.GetValue, point.PosMsg_2.GetValue);
            return axis.Aix_1.CheckPos(point.PosMsg_1.GetValue, 0.1)
                && axis.Aix_2.CheckPos(point.PosMsg_2.GetValue, 0.1);
        }
    }
}
