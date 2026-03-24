using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Params
{
    public class ParamsBase
    {
        public ParamsBase()
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => Motion.ParamPath_Motion.CurFileFullPath, () => this);
            this.Read();
        }



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
