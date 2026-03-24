using AccustomeAttributedll;
using AixCommInfo;
using Handler.Funs;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Funs
{
    class FunCommunicator : FunsSelectionHelperbase
    {
        public static readonly FunCommunicator Cur = new FunCommunicator();

        private FunCommunicator() : base("其他通讯设置", true)
        {
            Read();
        }
        public static FunCommunicator CreateInstance()
        {
            return Cur;
        }

        [SaveRemark]
        [TextBoxRemark("前壳RFID")]
        public ParamObjectText<string> 前壳RFID = new ParamObjectText<string>() { GetValue = "" };

        [SaveRemark]
        [TextBoxRemark("PCBRFID")]
        public ParamObjectText<string> PCBRFID = new ParamObjectText<string>() { GetValue = "" };

        [SaveRemark]
        [TextBoxRemark("前壳RFIDPort")]
        public ParamObjectText<string> 前壳RFIDPort = new ParamObjectText<string>() { GetValue = "" };

        [SaveRemark]
        [TextBoxRemark("PCBRFIDPort")]
        public ParamObjectText<string> PCBRFIDPort = new ParamObjectText<string>() { GetValue = "" };

        /// <summary>
        /// 是否是tcprfid
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("前壳是否是TCPRFID", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> ShellTCPRfid = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 是否是tcprfid
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("PCB是否是TCPRFID", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCBTCPRfid = new ParamStructText<bool>() { GetValue = true };

    }
}
