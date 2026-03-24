using Handler.Funs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect
{
    internal class LaserManager
    {
        public static readonly LaserManager Default = new LaserManager();
        private LaserManager() 
        {
            //Laser1 = new LaserHelper("激光镭雕", Connect.ConnectFactory.ConnectLaser);
            Laser1.IsConnectEventHandler += () => Laser1.Connect.IsConnected;
        }

        public LaserHelper Laser1; 
    }
}
