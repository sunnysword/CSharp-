using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
    /// <summary>
    /// 空跑模式
    /// </summary>
    class TestRunMode : StationSelectFunsBase
    {
        public TestRunMode() : base("空跑模式", WorkMode.TestRun)
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
            return false;
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
