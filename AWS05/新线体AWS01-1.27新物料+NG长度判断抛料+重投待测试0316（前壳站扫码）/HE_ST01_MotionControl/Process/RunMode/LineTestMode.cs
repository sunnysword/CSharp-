using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
    /// <summary>
    /// 单机模式
    /// </summary>
    class LineTestMode : StationSelectFunsBase
    {
        public LineTestMode() : base("单机模式", WorkMode.LineTestMode)
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
            return false;
        }


        public override bool Fun_LoadAutoStart()
        {
            return true;
        }

        public override bool Fun_LoadCheckLensHolder()
        {
            return true;
        }

        public override bool Fun_AAGripLensBack()
        {
            return true;
        }

        public override bool Fun_NeedMoveBackStation()
        {
            return false;
        }
    }
}
