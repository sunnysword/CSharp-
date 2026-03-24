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
    public abstract class FunsSelectionHelperbase
    {
        public FunsSelectionHelperbase(string headerName,bool isGlobal=false)
        {
            HeaderName = headerName;
            if (isGlobal)
            {
                SaveRead = new SaveReaddll.SaveReadHeler(() => System.IO.Path.Combine(System.Environment.CurrentDirectory, "SysCfg","SystemConfig.ini"), () => this);
            }
            else
            {
                SaveRead = new SaveReaddll.SaveReadHeler(
                    () =>System.IO.Path.Combine(Motion.ParamPath_Motion.SelectedDirPath,"Settings.ini"), () => this);
            }
            FunctionsList.Add(this);
            this.Read();
        }

        public string HeaderName { get; set; }

        public static List<FunsSelectionHelperbase> FunctionsList = new List<FunsSelectionHelperbase>();


        protected SaveReaddll.SaveReadHeler SaveRead;


        public void Save()
        {
            SaveRead?.Save();
        }

        public void Read()
        {
            SaveRead?.Read();
        }
    }
}
