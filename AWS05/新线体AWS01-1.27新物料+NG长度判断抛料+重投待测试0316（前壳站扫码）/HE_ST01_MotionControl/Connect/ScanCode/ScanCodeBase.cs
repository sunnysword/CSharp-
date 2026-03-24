using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.ScanCode
{
    public class ScanCodeBase
    {
        public ScanCodeBase(IConnectDLL.IConnect connect)
        {
            this.Connect = connect;
            this.Connect.RecevieMsgActionEventHander += Connect_RecevieMsgActionEventHander;
        }

        private void Connect_RecevieMsgActionEventHander(string obj)
        {
            ReceiveMsg = obj;
            if (Process.RunMode.WorkModeManager.Cur.SelectWorkMode.Fun_IsUseMES())
            {
                if (CheckMES(ReceiveMsg))
                {
                    MESCheckOK = true;
                }
                else
                {
                    MESCheckOK = false;
                }
            }
            else
            {
                MESCheckOK = true;
            }
        }

        public IConnectDLL.IConnect Connect;

        public bool MESCheckOK { get; set; }

        public string ReceiveMsg { get; protected set; }
        public string MESMsg { get; protected set; }

        public Func<string,(bool,string)> CheckMESEventHandler;

        public void ClearMsg()
        {
            ReceiveMsg = string.Empty;
            MESMsg = string.Empty;
            MESCheckOK = false;
        }

        public bool CheckMES(string msg)
        {
            if (CheckMESEventHandler == null)
                return true;
            else
            {
                var ret = CheckMESEventHandler.Invoke(msg);
                this.MESMsg = ret.Item2;
                return ret.Item1;
            }
        }
    }
}
