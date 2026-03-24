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
    class FunCommon : FunsSelectionHelperbase
    {
        public static readonly FunCommon Cur = new FunCommon();

        private FunCommon() : base("基本设置",true)
        {
            Read();
            AM.Core.Process.ProcessIPauseBaseExtend.GetMaxNGProductCountEventHandler += 
                () => Handler.Funs.FunCommon.Cur.ErrContinueNum.GetValue;


        }
        public static FunCommon CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 蜂鸣器响应时间
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("蜂鸣器器响应时间(ms)")]
        public ParamStructText<int> BuzzerActionTime = new ParamStructText<int>();

        /// <summary>
        /// 连续不良报警数
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("连续不良报警功能", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> ErrContinueNumIsUse = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 蜂鸣器
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("蜂鸣器", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> BuzzerIsUse = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 后站联机
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("后站联机", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> BackConnect = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 连续不良报警数
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("连续不良报警数")]
        public ParamStructText<int> ErrContinueNum = new ParamStructText<int>() { GetValue = 100 };





    }

}
