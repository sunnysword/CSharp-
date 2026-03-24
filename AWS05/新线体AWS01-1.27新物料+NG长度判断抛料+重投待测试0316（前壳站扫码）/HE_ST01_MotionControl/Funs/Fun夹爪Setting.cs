using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccustomeAttributedll;
using AixCommInfo;
using Handler.Funs;
using ISaveReaddll;

namespace Handler.Funs
{
    internal class Fun夹爪Setting : FunsSelectionHelperbase
    {
        public static readonly Fun夹爪Setting Cur = new Fun夹爪Setting();

        private Fun夹爪Setting() : base("夹爪设置")
        {
            Read();
        }

        public static Fun夹爪Setting CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// PCB夹爪加紧距离
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪加紧距离(mm)")]
        public ParamStructText<short> PCB夹爪加紧距离 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// PCB夹爪张开距离
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪张开距离(mm)")]
        public ParamStructText<short> PCB夹爪张开距离 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// PCB夹爪Tray取料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪Tray取料旋转(°）")]
        public ParamStructText<short> PCB夹爪Tray取料旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// PCB夹爪Tray放料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪Tray放料旋转(°)")]
        public ParamStructText<short> PCB夹爪Tray放料旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// PCB夹爪速度
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪速度")]
        public ParamStructText<byte> PCB夹爪速度 = new ParamStructText<byte>() { GetValue = 100 };

        /// <summary>
        /// PCB夹爪夹持力
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪夹持力")]
        public ParamStructText<byte> PCB夹爪夹持力 = new ParamStructText<byte>() { GetValue = 100 };


        /// <summary>
        /// PCB夹爪扫码旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCB夹爪扫码旋转(°)")]
        public ParamStructText<short> PCB夹爪扫码旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// PCBNG放料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("PCBNG放料旋转(°)")]
        public ParamStructText<short> PCBNG放料旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳夹爪加紧距离
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪加紧距离(mm)")]
        public ParamStructText<short> 前壳夹爪加紧距离 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳夹爪张开距离
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪张开距离(mm)")]
        public ParamStructText<short> 前壳夹爪张开距离 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳夹爪Tray取料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪Tray取料旋转(°）")]
        public ParamStructText<short> 前壳夹爪Tray取料旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳夹爪Tray放料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪Tray放料旋转(°)")]
        public ParamStructText<short> 前壳夹爪Tray放料旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳夹爪扫码旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪扫码旋转(°)")]
        public ParamStructText<short> 前壳夹爪扫码旋转 = new ParamStructText<short>() { GetValue = 500 };

        /// <summary>
        /// 前壳NG放料旋转
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳NG放料旋转(°)")]
        public ParamStructText<short> 前壳NG放料旋转 = new ParamStructText<short>() { GetValue = 500 };


        /// <summary>
        /// 前壳夹爪速度
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪速度")]
        public ParamStructText<byte> 前壳夹爪速度 = new ParamStructText<byte>() { GetValue = 100 };

        /// <summary>
        /// 前壳夹爪夹持力
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("前壳夹爪夹持力")]
        public ParamStructText<byte> 前壳夹爪夹持力 = new ParamStructText<byte>() { GetValue = 100 };


    }
}
