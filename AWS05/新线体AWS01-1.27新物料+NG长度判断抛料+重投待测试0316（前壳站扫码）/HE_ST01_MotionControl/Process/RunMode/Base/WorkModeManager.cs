using Handler.Motion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.RunMode
{
    /// <summary>
    /// 工作模式管理者
    /// </summary>
    class WorkModeManager
    {
        public static readonly WorkModeManager Cur = new WorkModeManager();
        private WorkModeManager()
        {
            WorkModeList.Add(new NormalRunMode());
            WorkModeList.Add(new LineTestMode());
            WorkModeList.Add(new TestRunMode());
            WorkModeList.Add(new ReflingMode());
            SelectWorkMode = WorkModeList[0];
        }



        /// <summary>
        /// 工作模式集合
        /// </summary>
        public readonly ObservableCollection<StationSelectFunsBase> WorkModeList = new ObservableCollection<StationSelectFunsBase>();

        /// <summary>
        /// 当前选择的工作模式
        /// </summary>
        public StationSelectFunsBase SelectWorkMode { get; private set; }


        public void SetWorkMode(StationSelectFunsBase stationSelectFunsBase)
        {
            SelectWorkMode = stationSelectFunsBase;
            if (SelectWorkMode.Mode == WorkMode.RefilRun)
            {
                Funs.FunStationSetting.Cur.是否启用pcb工位.GetValue = false;
                Funs.FunStationSetting.Cur.是否启用前壳工位.GetValue = false;
                //Funs.FunLaser.Cur.IsLaserStationUseCode.GetValue = false;
            }
          
            Motion.StaticInitial.Motion.CommitStopOrder();
            StaticInitial.Motion.CurOrder.ResOKFlag.ResetFlag();
        }



    }
}
