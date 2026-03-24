using AM.Communication.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.Connect.Powerss
{
    /// <summary>
    /// Plasma功率
    /// </summary>
    public class PowerHelper
    {

        public static PowerHelper Cur = new PowerHelper();
        private PowerHelper()
        {

        }



        public static PowerHelper CreateInstrance()
        {
            return Cur;
        }


        ProgrammablePowerEntity programmablePower = ConnectFactory.serialPortPower;



        static object _lock = new object();


        public bool Start = true;

        /// <summary>
        /// 通道1电流
        /// </summary>
        public string current_value_A = "0";

        /// <summary>
        /// 通道2电流
        /// </summary>
        public string current_value_B = "0";


        /// <summary>
        /// 读取电流
        /// </summary>
        public void ReadPower()
        {
            lock (_lock)
            {
                try
                {
                    string current = string.Empty;
                    string volt = string.Empty;
                    programmablePower.CheckCurrentAndGetVoltage(Power3Demo.ChannelIndex.First, out current, out volt);
                    current_value_A = current;
                    programmablePower.CheckCurrentAndGetVoltage(Power3Demo.ChannelIndex.Second, out current, out volt);
                    current_value_B = current;
                }
                catch (Exception)
                {


                }


            }


        }


        Thread thread = new Thread(() =>
        {
            while (true)
            {
                if (Cur.Start)
                {
                    Cur.ReadPower();
                }
                Thread.Sleep(100);
            }
        });

        public void init()
        {
            thread.IsBackground = true;
            thread.Start();
        }

        public void closed()
        {
            thread.Abort();
        }





    }
}
