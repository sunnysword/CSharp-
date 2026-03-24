using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis.TrailImplement
{
    class ImplementIConvertToPuls : Wpf_DispenserTrailEditDemo.TrailInfo.IConvertToPuls
    {
        public ImplementIConvertToPuls(LineAixZCreateBase lineAixCreateBase )
        {
            LineAix = lineAixCreateBase;
        }
        readonly LineAixZCreateBase LineAix;
        public double X_ConvertToPuls(double mm)
        {
            return LineAix.Aix_1.ConvertToCmdPuls(mm);
        }

        public double Y_ConvertToPuls(double mm)
        {
            return LineAix.Aix_2.ConvertToCmdPuls(mm);
        }

        public double Z_ConvertToPuls(double mm)
        {
            return LineAix.Aix_Z.ConvertToCmdPuls(mm);
        }
    }
}
