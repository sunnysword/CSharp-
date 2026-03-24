using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis.TrailImplement
{
    class ImplementIPointDispenserIOTriggerBase : GlueIndustryModuledll.ImplementForApplication.ImplementPoint.IPointDispenserIOTriggerBase
    {

        public ImplementIPointDispenserIOTriggerBase(Iodll.IoOutHepler outHepler)
        {
            IoOut = outHepler;
        }
        readonly IoOutHepler IoOut;

        public void Out_OFF()
        {
            IoOut.Out_OFF();
        }

        public void Out_ON()
        {
            IoOut.Out_ON();
        }
    }
}
