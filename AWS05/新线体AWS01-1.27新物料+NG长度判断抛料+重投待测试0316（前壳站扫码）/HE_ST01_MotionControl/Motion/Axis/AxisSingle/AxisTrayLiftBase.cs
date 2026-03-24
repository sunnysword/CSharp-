using AccustomeAttributedll;
using AixCommInfo;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis
{
    /// <summary>
    /// 料仓Z轴基类
    /// </summary>
    public class AxisTrayLiftBase : AixCreateBase
    {
        public AxisTrayLiftBase(ApplicationSettingsBase applicationSettingsBase, string keyName = null)
            : base(applicationSettingsBase)
        {
            PosWait = new SingleAixPosAndSpeedMsgAndIMov(() => this);           
            Read();
        }
    

        #region 覆写方法区

        public override void SafeCheck()
        {


        }

        #endregion

        #region 功能实例

        [SaveRemark]
        [TextBoxRemark("上料盘等待点")]
        [ButtonRemark("上料盘等待点")]
        public SingleAixPosAndSpeedMsgAndIMov PosWait;

        #endregion

    }
}
