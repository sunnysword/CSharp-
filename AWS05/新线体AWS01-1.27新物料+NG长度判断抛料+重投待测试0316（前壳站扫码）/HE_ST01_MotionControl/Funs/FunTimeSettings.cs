using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Funs;
using AccustomeAttributedll;
using AixCommInfo;
using ISaveReaddll;

namespace Handler.Funs
{
    internal class FunTimeSettings : FunsSelectionHelperbase
    {
        public static readonly FunTimeSettings Cur = new FunTimeSettings();

        private FunTimeSettings() : base("时间设置")
        {
            Read();
        }

        public static FunTimeSettings CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 相机检测超时时间
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("相机检测超时时间(ms)")]
        public ParamStructText<int> DispenserCheckOverTime = new ParamStructText<int>() { GetValue = 10000 };

        /// <summary>
        /// RFID读取延时 2023/10/26 wfh
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("RFID读取延时(ms)")]
        public ParamStructText<int> RFIDReadDelayTime = new ParamStructText<int>() { GetValue = 500 };

        /// <summary>
        /// RFID读取延时 2023/10/26 wfh
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("RFID写入前延时(ms)")]
        public ParamStructText<int> RFIDWriteDelayTime = new ParamStructText<int>() { GetValue = 500 };

        /// <summary>
        /// 夹爪取放延时
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("夹爪取放延时(ms)")]
        public ParamStructText<int> 夹爪取放延时 = new ParamStructText<int>() { GetValue = 200 };
    }
}