using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Core.Process
{
    /// <summary>
    /// 连续NG统计类
    /// </summary>
    public class ContinuousNGStatistics
    {
        public ContinuousNGStatistics(string name,Func<int> getMaxNGCount)
        {
            this.Name = name;
            this.GetMaxNGProductCountEventHandler = getMaxNGCount;
        }

        public string Name { get; set; }
        public Func<int> GetMaxNGProductCountEventHandler;

        /// <summary>
        /// NG产品计数，用于连续不良报警统计，可能会被清零，不能用来作为工位的NG计数
        /// </summary>
        public int NGProductCount { get; set; } = 0;

        public void ClearNGProductCount() => this.NGProductCount = 0;

        public bool AddAndCheckNGProductCountOverMax(out string error)
        {
            error = string.Empty;
            if (Funs.FunCommon.Cur.ErrContinueNumIsUse.GetValue == false)
            {
                return false;
            }
            ++this.NGProductCount;
            if (GetMaxNGProductCountEventHandler != null)
            {
                if (this.NGProductCount >= GetMaxNGProductCountEventHandler())
                {
                    error=$"{this.Name}:连续不良报警，连续不良数:{this.NGProductCount},名称:{this.Name}";
                    this.NGProductCount = 0;
                    return true;
                }
            }

            return false;
        }
    }
}
