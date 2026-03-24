using AccustomeAttributedll;
using AixCommInfo;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Funs
{
    public class FunScrewGun : FunsSelectionHelperbase
    {
        public static readonly FunScrewGun Cur = new FunScrewGun();

        private FunScrewGun() : base("锁附工艺设置")
        {
            Read();
        }
        public static FunScrewGun CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 螺丝枪程序号
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪程序号")]
        public ParamStructText<int> ProgramNo = new ParamStructText<int>();

        /// <summary>
        /// 螺丝枪程序号
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪点检程序号")]
        public ParamStructText<int> ProgramNo2 = new ParamStructText<int>();

        /// <summary>
        /// 螺丝对射光纤
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("螺丝对射光纤", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseScrewCheck = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 螺丝枪测高
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("螺丝枪测高", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseHeightTest = new ParamStructText<bool>();

        /// <summary>
        /// 螺丝枪测高前延时
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测高前延时(ms)")]
        public ParamStructText<int> HeightTestDealy = new ParamStructText<int>();

        /// <summary>
        /// 螺丝枪测高前延时
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测要钉延时(ms)")]
        public ParamStructText<int> AskScrwDealy = new ParamStructText<int>();

        /// <summary>
        /// 螺丝枪测高上限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测高1上限值")]
        public ParamStructText<double> HeightTestUpLimit = new ParamStructText<double>();

        /// <summary>
        /// 螺丝枪测高下限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测高1下限值")]
        public ParamStructText<double> HeightTestDownLimit = new ParamStructText<double>();

        /// <summary>
        /// 螺丝枪测高上限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测高2上限值")]
        public ParamStructText<double> HeightTestUpLimit2 = new ParamStructText<double>();

        /// <summary>
        /// 螺丝枪测高下限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝枪测高2下限值")]
        public ParamStructText<double> HeightTestDownLimit2 = new ParamStructText<double>();

        /// <summary>
        /// 螺丝枪扭力点检
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("螺丝枪扭力点检", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseForqueTest = new ParamStructText<bool>();

        /// <summary>
        /// 螺丝枪扭力点检
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("螺丝枪程序判断", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUsePorJudg = new ParamStructText<bool>() { GetValue = true };

        /// <summary>
        /// 扭力点检上限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("扭力点检上限值")]
        public ParamStructText<double> ForqueTestUpLimit = new ParamStructText<double>();

        /// <summary>
        /// 扭力点检下限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("扭力点检下限值")]
        public ParamStructText<double> ForqueTestDownLimit = new ParamStructText<double>();

        /// <summary>
        /// 扭力下限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("扭力下限值")]
        public ParamStructText<double> ForqueWorkDownLimit = new ParamStructText<double>();
       
        /// <summary>
        /// 扭力上限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("扭力上限值")]
        public ParamStructText<double> ForqueDownDownLimit = new ParamStructText<double>();


        /// <summary>
        /// 角度下限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("角度下限值")]
        public ParamStructText<double> AngelWorkDownLimit = new ParamStructText<double>();

        /// <summary>
        /// 角度上限值
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("角度上限值")]
        public ParamStructText<double> AngelDownDownLimit = new ParamStructText<double>();

        /// <summary>
        /// 手动单螺丝锁附
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("手动单螺丝锁附", new string[] { "螺丝1", "螺丝2" }, 0)]
        public ParamStructText<bool> isUseJustOne = new ParamStructText<bool>();


        /// <summary>
        /// 手动单螺丝锁附
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("螺丝锁个数")]
        public ParamStructText<int> LockScrewPoint = new ParamStructText<int>() { GetValue=2};

    }
}
