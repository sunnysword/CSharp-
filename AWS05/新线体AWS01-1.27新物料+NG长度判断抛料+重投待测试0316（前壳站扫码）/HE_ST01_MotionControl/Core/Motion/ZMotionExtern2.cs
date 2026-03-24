
using Handler.Motion.Axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Core.Extension
{
    public static class ZMotionExtern2
    {
        public static double ZMotionEx_GetAxisEncoderValue(this AixCreateBase axis)
        {
            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            float pos = 0f;
            var ret = cszmcaux.zmcaux.ZAux_Direct_GetEncoder(
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,
                ref pos);
            axis.SafeCheckAction(ret);
            return pos;
        }

        public static void ZMotionEx_SetDPos(this AixCreateBase axis,float pulse)
        {
            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            var ret = cszmcaux.zmcaux.ZAux_Direct_SetDpos(
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,
                pulse);
            axis.SafeCheckAction(ret);
        }

        public static void ZMotionEx_SetMPos(this AixCreateBase axis, float pulse)
        {
            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            var ret = cszmcaux.zmcaux.ZAux_Direct_SetMpos(
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,
                pulse);
            axis.SafeCheckAction(ret);
        }

        public static bool ZMotionEx_GetAxisSELP(this AixCreateBase axis)
        {
            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            int ipvalue = 0;
            var ret  = cszmcaux.zmcaux.ZAux_Direct_GetAxisStatus(
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
                 axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,
                 ref ipvalue);
            axis.SafeCheckAction(ret);
            return ipvalue == 512;

        }

        public static bool ZMotionEx_GetAxisSELN(this AixCreateBase axis)
        {
            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            int ipvalue = 0;
            var ret = cszmcaux.zmcaux.ZAux_Direct_GetAxisStatus(
                axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().ZMotion.GetControllerHandle(),
                 axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo,
                 ref ipvalue);
            axis.SafeCheckAction(ret);
            return ipvalue==1024;
        }

        public static void SafeCheckAction(this AixCreateBase axis, int ret)
        {

            if (!axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().IsConnected)
            {
                throw new Exception($"Name={axis.Name}," +
                    $"轴号={axis.GetImplementAix<ZMotionHelper.SingleAix.SingleAixHelper>().AixNo} 掉线");
            }
            int nErrCode = ret;
            if (nErrCode != 0)
            {
                throw new Exception("ErrCode=" + nErrCode);
            }

        }
    }
}
