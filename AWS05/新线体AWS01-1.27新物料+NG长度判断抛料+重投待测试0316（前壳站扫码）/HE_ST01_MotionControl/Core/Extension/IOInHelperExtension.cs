using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Core.Extension
{
    public static class IOInHelperExtension
    {
        public static string GetInfo(this Iodll.IoInHelper item)
        {
            return $"(IO点位名称:{item.Remark},IO模块名称:{item.ioHelper.Name},索引:{string.Join(",", item.InBytes)}";
        }
    }
}
