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
    /// 通用项设置
    /// </summary>
    public class FunSelection : FunsSelectionHelperbase
    {
        public static readonly FunSelection Cur = new FunSelection();

        private FunSelection() : base("功能设置")
        {
            Read();
        }
        public static FunSelection CreateInstance()
        {
            return Cur;
        }

        [SaveRemark]
        [FunctionSelectionRemark("前壳放料前相机引导", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 前壳放料前相机引导 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("前壳放料后相机复检", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 前壳放料后相机复检 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("前壳上料夹爪夹持或旋转状态检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> FrontHolderCheckProduct = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("PCB上料夹爪夹持或旋转状态检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCBCheckProduct = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("前壳上料夹爪物料光电检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 前壳上料夹爪物料光电检测 = new ParamStructText<bool>();


        [SaveRemark]
        [FunctionSelectionRemark("PCB上料夹爪物料光电检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCB上料夹爪物料光电检测 = new ParamStructText<bool>();


        [SaveRemark]
        [FunctionSelectionRemark("前壳上料夹爪限位保护传感器", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseFrontHolderLimitSensor = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("PCB放料前相机引导", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCB放料前相机引导 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("PCB放料后相机复检", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 是否启用PCB放料后相机复检 = new ParamStructText<bool>();


        [SaveRemark]
        [FunctionSelectionRemark("PCB上料夹爪限位保护传感器", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUsePCBLimitSensor = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("是否启用外壳工位载具物料光电检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseSensorCheckFrontHolderCoil = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("是否启用PCB工位载具物料光电检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseSensorCheckPCBCoil = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("回流载具穴位有外壳是否跳过上料", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseFrontHolderCoilPass = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("PCBNG放料是否启用传感器", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCBNG放料是否启用传感器 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("PCB启用扫码", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> PCB是否启用扫码 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("前壳启用扫码", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> 前壳是否启用扫码 = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("RFID", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseRFID = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("安全门", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseSafeDoor = new ParamStructText<bool>();

        [SaveRemark]
        [FunctionSelectionRemark("MES", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseMes = new ParamStructText<bool>();


        /// <summary>
        /// 是否进行气源信号检测
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("气源信号检测", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsCheckAirInput = new ParamStructText<bool>();

        /// <summary>
        /// 相机检测型号
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("相机检测型号")]
        public ParamObjectText<string> camPrpogramName = new ParamObjectText<string> { GetValue = "8M30" };

        /// <summary>
        /// 半成品检测型号
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("半成品检测型号")]
        public ParamObjectText<string> testPrpogramName = new ParamObjectText<string> { GetValue = "haoen_8M_120" };

    }
}
