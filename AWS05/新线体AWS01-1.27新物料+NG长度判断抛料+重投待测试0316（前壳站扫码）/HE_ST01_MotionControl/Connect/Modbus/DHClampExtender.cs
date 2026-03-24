using DH_ClampRotatedAxisDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Modbus
{
    public class DHClampExtender:DHClampRotatedAxisHelper
    {
        public DHClampExtender(string name, string portNameCfgName):base(name, portNameCfgName) { }

        public void SetMaxDis(ushort dis,byte speed,byte force=100)
        {
            this.ClampOpenPercentSetting = dis;
            this.ClampForce = force;
            this.ClampSpeed = speed;
            this.RotatedSpeed = speed;
            this.SaveCfg();
        }


    }
}
