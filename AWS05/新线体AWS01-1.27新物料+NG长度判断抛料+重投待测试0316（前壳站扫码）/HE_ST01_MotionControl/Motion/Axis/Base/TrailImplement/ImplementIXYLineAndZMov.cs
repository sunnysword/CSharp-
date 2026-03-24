using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis.TrailImplement
{
    class ImplementIXYLineAndZMov : GlueIndustryModuledll.ImplementForApplication.IXYLineAndZMov
    {

        public ImplementIXYLineAndZMov(LineAixZCreateBase lineAixCreate)
        {
            LineAix = lineAixCreate;
        }

        readonly  LineAixZCreateBase LineAix;
        public void XY_LineMov(double dis_x, double dis_y, double speed, double acc, double dec, bool checkdone)
        {
            LineAix.CancleOneSafeCheck();
            LineAix.LineMovBasePuls(dis_x, dis_y, speed, acc, dec, checkdone);
        }

        public void XY_LineMovAbs(double pos_x, double pos_y, double speed, double acc, double dec, bool checkdone)
        {
            LineAix.CancleOneSafeCheck();
            LineAix.LineMovAbsBasePuls(pos_x, pos_y, speed, acc, dec, checkdone);
        }

        public void Z_Mov(double dis, double speed, double acc, double dec, bool checkdone)
        {

            LineAix.Aix_Z.MovBasePuls(dis, speed, acc, dec, checkdone);
        }

        public void Z_MovAbs(double pos, double speed, double acc, double dec, bool checkdone)
        {
            LineAix.Aix_Z.CancleOneSafeCheck();
            LineAix.Aix_Z.MovAbsBasePuls(pos, speed, acc, dec, checkdone);
        }

        public void Z_WaitDone()
        {
            LineAix.Aix_Z.WaitDone();
        }

        public void XY_WaitDone()
        {
            LineAix.WaitDone();
        }
    }
}
