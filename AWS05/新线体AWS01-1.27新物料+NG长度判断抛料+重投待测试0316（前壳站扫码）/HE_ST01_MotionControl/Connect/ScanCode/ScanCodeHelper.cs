using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect
{
    internal class ScanCodeHelper
    {
        public static readonly ScanCodeHelper Cur =new ScanCodeHelper();


        private ScanCodeHelper()
        {
            //ScanCode1.CheckMESEventHandler += CheckSN1;
        }
        //public ScanCode.ScanCodeBase ScanCode1 = new ScanCode.ScanCodeBase(Connect.ConnectFactory.tcpConnect_ScanCode1);

        public (bool,string) CheckSN1(string msg)
        {
            if(MES.MESHelper.Cur.CheckSN(msg,out string error) == true)
            {
                return (true,string.Empty);
            }
            return (false,error);
        }
    }
}
