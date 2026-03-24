using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    /// <summary>
    /// 产品测试测试站信息
    /// </summary>
    public class ProductTestStation
    {
        public ProductTestStation()
        {
            StationTestItems = new List<ProductTestStationTestItem>();
        }
        public bool IsUse { get; set; } = true;

        /// <summary>
        /// 测试站
        /// </summary>
        public string TestStationName { get; set; }

        /// <summary>
        /// 测试总结果：0失败，1成功
        /// </summary>
        public string Result { get; set; } = "0";


        /// <summary>
        /// 失败的原因
        /// </summary>
        public string FailedMsg { get; set; }
        /// <summary>
        /// 测试的数据项
        /// </summary>
        public List<ProductTestStationTestItem> StationTestItems { get; set; }
      
    }
}
