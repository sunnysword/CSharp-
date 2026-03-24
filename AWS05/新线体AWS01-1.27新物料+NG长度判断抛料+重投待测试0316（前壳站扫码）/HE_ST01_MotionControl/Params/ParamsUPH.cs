using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Params
{
    public class ParamsUPH : ParamsBase
    {
        public static readonly ParamsUPH Cur = new ParamsUPH();

        private ParamsUPH() : base()
        {
            Read();
        }
        public static ParamsUPH CreateInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 上一时间
        /// </summary>
        public DateTime? StartTime = null;

        /// <summary>
        /// 后一时间
        /// </summary>
        public DateTime? EndTime = null;

        [SaveRemark]
        public int UPH = 0;


        public void Init()
        {
            StartTime = null;
            EndTime = null;
        }

        /// <summary>
        /// 记录来料时间
        /// </summary>
        public void SetTime()
        {
            if (StartTime == null)  //第一次上料
            {
                StartTime = DateTime.Now;
            }
            else if (EndTime == null) //第二次上料
            {
                EndTime = DateTime.Now;
            }
            else  //后续上料，时间互换
            {
                StartTime = EndTime;
                EndTime = DateTime.Now;
            }
        }


        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public int DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return Convert.ToInt32(secondSpan.TotalSeconds);
        }

        /// <summary>
        /// 计算UPH
        /// </summary>
        public void GetUPH()
        {

            try
            {
                SetTime();
                if (StartTime != null && EndTime != null)
                {
                    UPH = (3600 / (DiffSeconds((DateTime)StartTime, (DateTime)EndTime)));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
