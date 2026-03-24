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
    /// <summary>
    /// 工位设置
    /// </summary>
    public class FunStationSetting : FunsSelectionHelperbase
    {
        public static readonly FunStationSetting Cur = new FunStationSetting();

        private FunStationSetting() : base("工位设置")
        {
            Read();
        }
        public static FunStationSetting CreateInstance()
        {
            return Cur;
        }

        [SaveRemark]
        [FunctionSelectionRemark("工位一取前壳", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 是否启用前壳工位 = new ParamStructText<bool>();
        /// <summary>
        /// 启用工位三取PCB
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("工位二取PCB", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 是否启用pcb工位 = new ParamStructText<bool>();


    }
}
