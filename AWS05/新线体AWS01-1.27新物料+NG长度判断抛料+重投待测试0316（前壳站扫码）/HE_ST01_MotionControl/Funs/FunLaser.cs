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
    public class FunLaser : FunsSelectionHelperbase
    {
        public static readonly FunLaser Cur = new FunLaser();

        private FunLaser() : base("镭雕站设置")
        {
            Read();
        }
        public static FunLaser CreateInstance()
        {
            return Cur;
        }

        [SaveRemark]
        [FunctionSelectionRemark("镭雕SN获取方式", new string[] { "MES获取", "软件生成" }, 0)]
        public ParamStructText<bool> LaserSNGetTypeIsMes = new ParamStructText<bool>();

        [SaveRemark]
        [TextBoxRemark("镭雕SN若软件生成,SN前缀")]
        public ParamObjectText<string> LaserCodePrefix = new ParamObjectText<string>();

        /// <summary>
        /// 镭雕SN若软件生成，保存的流水号
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("镭雕SN若软件生成,当前流水号起始数字")]
        public ParamObjectText<int> LaserCodeSerialNumber = new ParamObjectText<int>() { GetValue = 1 };


        [SaveRemark]
        [TextBoxRemark("激光镭雕模板名称")] 
        public ParamObjectText<string> LaserCodeModuleName = new ParamObjectText<string>();

        [SaveRemark]
        [TextBoxRemark("激光清洗模板名称")]
        public ParamObjectText<string> LaserCleanModuleName = new ParamObjectText<string>();

        [SaveRemark]
        [TextBoxRemark("镭雕站当前功率")]
        public ParamObjectText<string> LaserCleanPower = new ParamObjectText<string>();

        [SaveRemark]
        [FunctionSelectionRemark("镭雕站镭雕", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsLaserStationUseCode = new ParamStructText<bool>();


        [SaveRemark]
        [FunctionSelectionRemark("镭雕站激光清洁", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsLaserStationUseLaserClean = new ParamStructText<bool>();




        /// <summary>
        /// 日期前缀
        /// </summary>
        [SaveRemark]
        public string DateTimess = "";


        public string GetSerialNumber()
        {

            string s2 = DateTime.Now.ToString("yyMMdd");
            if (DateTimess == "")
            {
                DateTimess = s2;
            }
            //日期数不一样就重置序号为0
            if (DateTimess != s2)
            {
                LaserCodeSerialNumber.GetValue = 0;
                DateTimess = s2;
            }
            string str = LaserCodeSerialNumber.GetValue.ToString().PadLeft(5, '0');
            LaserCodeSerialNumber.GetValue++;
            this.Save();
            return $"{this.LaserCodePrefix.GetValue}{s2}{str}";
        }

    }
}
