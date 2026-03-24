using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;

namespace Handler.Process.Station
{
    internal class Process_前壳缓存站 : WorkStationBase
    {
        public static readonly Process_前壳缓存站 Cur = new Process_前壳缓存站();

        private Process_前壳缓存站() : base("前壳缓存站")
        {

        }

        protected override void Action()
        {

        }

        public bool 前壳缓存站允许流入 = false;
        public bool 前壳缓存站请求流出 = false;

        void StationClear()
        {
            前壳缓存站允许流入 = false;
            前壳缓存站请求流出 = false;
        }

        protected override void ImplementRunAction()
        {
            Ref_Step("是否空跑");
            StationClear();

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }

                switch (Step)
                {
                    case "是否空跑":
                        if (WorkModeManager.Cur.SelectWorkMode.Mode == Process.RunMode.WorkMode.TestRun)
                        {
                            break;
                        }
                        else
                        {
                            Ref_Step("判断外壳缓存位是否有载具");
                        }
                        break;

                    case "判断外壳缓存位是否有载具":
                        if (StaticIOHelper.外壳缓存位载具流进检测.In())
                        {
                            Ref_Step("等待载具流入到位");
                        }
                        else
                        {
                            Ref_Step("等待前壳站请求流出");
                        }
                        break;

                    case "等待前壳站请求流出":
                        if (Process_前壳站.Cur.外壳站请求流出)
                        {
                            StreamLine.Cur.ConveyorFront.RequestRun(Direction.Forward);
                            Process_前壳站.Cur.外壳站请求流出 = false;
                            前壳缓存站允许流入 = true;
                            Ref_Step("等待载具流入到位");
                        }
                        break;

                    case "等待载具流入到位":
                        if (StaticIOHelper.外壳缓存位载具流进检测.In())//载具到位
                        {
                            前壳缓存站允许流入 = false;
                            Ref_Step("等待PCB站允许流入");
                            StreamLine.Cur.ConveyorFront.RequestStop();
                            前壳缓存站请求流出 = true;
                        }
                        break;

                    case "等待PCB站允许流入":
                        if (Process_PCB站.Cur.PCB站允许流入)
                        {
                            if (!WaitIO(StaticIOHelper.外壳缓存位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("外壳缓存位阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorFront.RequestRun(Direction.Forward);
                            Ref_Step("等待PCB站载具流入到位");
                        }
                        break;

                    case "等待PCB站载具流入到位":
                        if (!StaticIOHelper.外壳缓存位载具流出检测.In() && !Process_PCB站.Cur.PCB站允许流入)
                        {
                            if (!WaitIO(StaticIOHelper.外壳缓存位阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("外壳缓存位阻挡气缸工作位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorFront.RequestStop();
                            Ref_Step("等待前壳站请求流出");
                        }
                        break;
                }
                Sleep(10);
            }
        }
    }
}
