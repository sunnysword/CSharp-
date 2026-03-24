using AixCommInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.PalletTray
{

    /// <summary>
    /// 盘上每个点状态
    /// </summary>
    public enum TrayPointStatus
    {
        /// <summary>
        /// 等待使用
        /// </summary>
        WaitUse=0,


        /// <summary>
        /// 已经使用过了
        /// </summary>
        HaveUsed=1,

        /// <summary>
        /// 空
        /// </summary>
        Empty,

        /// <summary>
        /// 禁用
        /// </summary>
        Limited,
    
    }



    /// <summary>
    /// 盘上的一个点对象
    /// </summary>
    public class TrayPoint:BindableObject
    {


    }
}
