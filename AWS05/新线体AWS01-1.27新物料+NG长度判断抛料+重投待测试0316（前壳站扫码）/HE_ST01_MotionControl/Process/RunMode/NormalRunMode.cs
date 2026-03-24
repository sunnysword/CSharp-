using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
    /// <summary>
    /// 正常运行模式
    /// </summary>
    class NormalRunMode : StationSelectFunsBase
    {
        public NormalRunMode():base("自动模式",WorkMode.Normal)
        {

        }

        public override bool IsConnectWithFrontStation()
        {
            return true;
        }

        public override bool IsConnectWithBackStation()
        {
            return true;
        }

       
        public override bool Fun_IsUseMES()
        {
            return Funs.FunSelection.Cur.IsUseMes.GetValue;
        }


        public override bool Fun_LoadAutoStart()
        {
            return false;
        }

        public override bool Fun_LoadCheckLensHolder()
        {
            return true;
        }


        public override bool Fun_AAGripLensBack()
        {
            return false;
        }
        public override bool Fun_NeedMoveBackStation()
        {
            return true;
        }
    }
}
