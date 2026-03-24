using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
    /// <summary>
    /// 站位基础功能控制：是否使用SN;是否上传MES...
    /// </summary>
    public abstract class StationSelectFunsBase
    {

        public StationSelectFunsBase(string name, WorkMode workMode)
        {
            this.Name = name;
            this.Mode = workMode;
        }

        /// <summary>
        /// 当前模式的名字
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 当前工作模式
        /// </summary>
        public WorkMode Mode { get; }

        /// <summary>
        /// 是否和前站联动
        /// </summary>
        /// <returns></returns>
        public abstract bool IsConnectWithFrontStation();

        /// <summary>
        /// 是否和后站联动
        /// </summary>
        /// <returns></returns>
        public abstract bool IsConnectWithBackStation();

        /// <summary>
        /// AA是否将Lens回放
        /// </summary>
        /// <returns></returns>
        public abstract bool Fun_AAGripLensBack();


        /// <summary>
        /// 是否上传MES
        /// </summary>
        /// <returns></returns>
        public abstract bool Fun_IsUseMES();


        /// <summary>
        /// 上料时是否检测Lens/Holder
        /// </summary>
        /// <returns></returns>
        public abstract bool Fun_LoadCheckLensHolder();


        /// <summary>
        /// 上料时是否自动有效
        /// </summary>
        /// <returns></returns>
        public abstract bool Fun_LoadAutoStart();


        /// <summary>
        /// 下料到流水线上后是否需要往后站流，如果是单工位空跑，就不需要往后流
        /// </summary>
        /// <returns></returns>
        public abstract bool Fun_NeedMoveBackStation();

        public List<KeyValuePair<string, bool>> GetFunsSettings()
        {
            List<KeyValuePair<string, bool>> pairs = new List<KeyValuePair<string, bool>>();

            pairs.Add(new KeyValuePair<string, bool>("是否上传MES", Fun_IsUseMES()));
            pairs.Add(new KeyValuePair<string, bool>("上料时是否检测Lens/Holder", Fun_LoadCheckLensHolder()));
            pairs.Add(new KeyValuePair<string, bool>("上料时是否启动按钮自动有效", Fun_LoadAutoStart()));
            pairs.Add(new KeyValuePair<string, bool>("是否AA回放", Fun_AAGripLensBack()));

            return pairs;
        }

    }

    /// <summary>
    /// 工作模式
    /// </summary>
    public enum WorkMode
    {
        /// <summary>
        /// 正常工作模式
        /// </summary>
        Normal = 1,

        /// <summary>
        /// AA空跑模式
        /// </summary>
        AATestRun = 2,

        /// <summary>
        /// 空跑模式
        /// </summary>
        TestRun = 3,

        /// <summary>
        /// 重投模式
        /// </summary>
        RefilRun = 4,
        /// <summary>
        /// 脏污白板点检
        /// </summary>
        WhiteDailyCheck,
        SingleRun,
        AADailyCheck,
        /// <summary>
        /// 独立打样模式
        /// </summary>
        ProofingMode,
        JustNextMode,
        LineTestMode
    }

}
