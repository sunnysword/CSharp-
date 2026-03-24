using AccustomeAttributedll;
using AixCommInfo;
using ISaveReaddll;
using Handler.Funs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Motion;

namespace Handler.Funs
{
    /// <summary>
    /// 基本设置
    /// </summary>
    class FunMES : FunsSelectionHelperbase
    {
        public static readonly FunMES Cur = new FunMES();

        private FunMES() : base("MES设置", true)
        {
            Read();
        }
        public static FunMES CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 线体名称
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("线体名称")]
        public ParamObjectText<string> 线体名称 = new ParamObjectText<string>() { GetValue = "" };

        /// <summary>
        /// MES工位名称
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("MES工位名称")]
        public ParamObjectText<string> MESStationName = new ParamObjectText<string>() { GetValue = "" };

        /// <summary>
        /// MES其他机型
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("MES其他机型")]
        public ParamObjectText<string> MES其他机型 = new ParamObjectText<string>() { GetValue = "" };


        [SaveRemark]
        [FunctionSelectionRemark("MES是否将前壳和PCB绑定", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsBindPCBSN = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("MES检测SN失败需人工确认", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsCheckSNFailNeedManualConfirm = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("MES过站失败需人工确认", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsDataUpFailNeedManualConfirm = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("MES是否使用其他机型上传", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> MES是否使用其他机型上传 = new ParamStructText<bool>();

        /// <summary>
        /// MES超时时间
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("MES超时时间")]
        public ParamObjectText<int> MESOverTime = new ParamObjectText<int>() { GetValue = 10000 };

        [SaveRemark]
        [FunctionSelectionRemark("MES异常信息上传", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsDataUpErrorMsgs = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("MES信息仅保存", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsDataUpSaveOnly = new ParamStructText<bool>();

    }

}
