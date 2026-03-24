using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;

namespace Handler.Process.Station
{
    internal class Process_回流流出站 : WorkStationBase
    {


        public static readonly Process_回流流出站 Cur = new Process_回流流出站();

        private Process_回流流出站() : base("回流流出站")
        {


        }

        protected override void Action()
        {

        }

        public bool 回流流出站允许流入 = false;
        public bool 回流流出站请求流出 = false;

        void StationClear()
        {
            回流流出站允许流入 = false;
            回流流出站请求流出 = false;
        }

        private readonly Stopwatch stopWatch = new Stopwatch();

        protected override void ImplementRunAction()
        {
            Ref_Step("是否空跑");
            StationClear();

            while (true)
            {
                //WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }

                switch (Step)
                {
                    case "是否空跑":
                        if (WorkModeManager.Cur.SelectWorkMode.Mode == WorkMode.TestRun)
                        {
                            break;
                        }
                        else
                        {
                            Ref_Step("判断回流流出站流进光电是否有感应");
                        }
                        break;

                    case "判断回流流出站流进光电是否有感应":
                        if (StaticIOHelper.回流线后段缓存位流进检测.In())
                        {
                            回流流出站请求流出 = true;
                            Ref_Step("等待回流移栽站允许流入");
                        }
                        else
                        {
                            Ref_Step("等待流入站请求流出");
                        }
                        break;

                    case "等待流入站请求流出":
                        if (Process_回流流入站.Cur.回流流入站请求流出)
                        {
                            Process_回流流入站.Cur.回流流入站请求流出 = false;
                            StreamLine.Cur.ReturnBackConveyor.RequestRun(Direction.Forward);
                            回流流出站允许流入 = true;
                            Ref_Step("等待载具流入到位");
                        }
                        break;

                    case "等待载具流入到位":
                        if (StaticIOHelper.回流线后段缓存位流进检测.In())//载具到位
                        {
                            回流流出站请求流出 = true;
                            Sleep(500);
                            StreamLine.Cur.ReturnBackConveyor.RequestStop();
                            Ref_Step("等待回流移栽站允许流入");
                        }
                        break;

                    case "等待回流移栽站允许流入":
                        if (Process_回流移栽站.Cur.移栽站允许流入)
                        {
                            Process_回流移栽站.Cur.移栽站允许流入 = false;
                            if (!WaitIO(StaticIOHelper.回流线后段阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("回流线后段阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ReturnBackConveyor.RequestRun(Direction.Forward);
                            Sleep(500);
                            stopWatch.Restart();
                            singalRepeat = 0;
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if (stopWatch.ElapsedMilliseconds > 10000)
                        {
                            WriteToUser("回流流出到移栽超时未完成，请检查传感器信号后继续");
                            stopWatch.Restart();
                            singalRepeat++;
                            if (singalRepeat > 2)
                            {
                                WriteErrToUser("回流流出到移栽超时未完成，请检查传感器信号后继续");
                                singalRepeat = 0;
                            }
                            break;
                        }
                        if (!StaticIOHelper.回流线后段缓存位流出检测.In() && !Process_回流移栽站.Cur.移栽站允许流入 && !StaticIOHelper.回流线后段皮带线载具流出检测.In())//载具已经流出并到位
                        {
                            if (!WaitIO(StaticIOHelper.回流线后段阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("回流线后段阻挡气缸工作位未到位，请检查！");
                                break;
                            }
                            Sleep(400);
                            StreamLine.Cur.ReturnBackConveyor.RequestStop();
                            Ref_Step("等待流入站请求流出");
                        }
                        break;
                }
                Sleep(10);
            }
        }
        int singalRepeat = 0;
    }
}
