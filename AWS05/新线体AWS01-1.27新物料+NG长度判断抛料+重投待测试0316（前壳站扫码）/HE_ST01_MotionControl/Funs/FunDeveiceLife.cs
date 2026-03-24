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
    internal class FunDeveiceLife : FunsSelectionHelperbase
    {
        public static readonly FunDeveiceLife Cur = new FunDeveiceLife();

        private FunDeveiceLife() : base("寿命设置")
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => System.IO.Path.Combine(System.Environment.CurrentDirectory, "SysCfg", "DeviceLife.ini"), () => this);

            Read();
        }
        public static FunDeveiceLife createInstance()
        {
            return Cur;
        }

        /// <summary>
        /// 探针使用计数
        /// </summary>
        [SaveRemark]
        public int StationProbeUsageCurrenCount = 0;
        
        /// <summary>
        /// 探针寿命统计报警
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("探针寿命统计报警", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseDeveiceLife = new ParamStructText<bool>();

        /// <summary>
        /// 探针最大使用次数
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("探针最大使用次数")]
        public ParamStructText<int> StationProbeMaxCount = new ParamStructText<int>() { GetValue = 20000 };

        /// <summary>
        /// 探针报警返回信息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CheckProbeMaxLife(out string error)
        {
            error = string.Empty;
            if (!IsUseDeveiceLife.GetValue)
            {
                return false;
            }
            else
            {
                if(StationProbeUsageCurrenCount>= StationProbeMaxCount.GetValue)
                {
                    error = "探针寿命超过最大使用次数，请更换并重置次数";
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 探针计数+1
        /// </summary>
        public void AddProbeCountOnce()
        {
            StationProbeUsageCurrenCount++;
            this.Save();
        }



        #region 批头计数

        /// <summary>
        /// 批头使用计数
        /// </summary>
        [SaveRemark]
        public int StationBitsUsageCurrenCount = 0;

        /// <summary>
        /// 批头寿命统计报警
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("批头寿命统计报警", new string[] { "启用", "禁用" }, 0)]
        public ParamStructText<bool> IsUseBitsDeveiceLife = new ParamStructText<bool>();

        /// <summary>
        /// 批头最大使用次数
        /// </summary>
        [SaveRemark]
        [TextBoxRemark("批头最大使用次数")]
        public ParamStructText<int> StationBitsMaxCount = new ParamStructText<int>() { GetValue = 20000 };

        /// <summary>
        /// 批头报警返回信息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CheckBitsMaxLife(out string error)
        {
            error = string.Empty;
            if (!IsUseBitsDeveiceLife.GetValue)
            {
                return false;
            }
            else
            {
                if (StationBitsUsageCurrenCount >= StationBitsMaxCount.GetValue)
                {
                    error = "批头寿命超过最大使用次数，请更换并重置次数";
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 批头计数+1
        /// </summary>
        public void AddBitsCountOnce()
        {
            StationBitsUsageCurrenCount++;
            this.Save();
        }



        #endregion
    }
}
