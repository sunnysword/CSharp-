using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
  
    class ReflingMode : StationSelectFunsBase
    {
        public ReflingMode() : base("重投模式", WorkMode.RefilRun)
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
