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
    internal class FunMESCheck : FunsSelectionHelperbase
    {
        public static readonly FunMESCheck Cur = new FunMESCheck();

        private FunMESCheck() : base("MES校验", true)
        {
            SaveRead = new SaveReaddll.SaveReadHeler(
              () => System.IO.Path.Combine(Motion.ParamPath_Motion.SelectedDirFullPath, "Settings.ini"), () => this);

            Read();
        }
        [SaveRemark]
        [TextBoxRemark("MES条码校验头")]
        public ParamObjectText<string> CheckHeader = new ParamObjectText<string>() { GetValue = "" };

        [SaveRemark]
        [TextBoxRemark("MES条码版本号")]
        public ParamObjectText<string> SnVersion = new ParamObjectText<string>() { GetValue = "" };

        [SaveRemark]
        [TextBoxRemark("物料条码截取后位数最小长度")]
        public ParamObjectText<int> SplitSnMaxLength = new ParamObjectText<int>() { GetValue = 6};


        public static FunMESCheck CreateInstance()
        {
            return Cur;
        }

    }
}
