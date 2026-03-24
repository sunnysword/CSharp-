using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Connect;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;

namespace Handler.Process.Station
{
    internal class Process_PCB缓存站 : WorkStationBase
    {
        public static readonly Process_PCB缓存站 Cur = new Process_PCB缓存站();

        private Process_PCB缓存站() : base("PCB缓存站")
        {

        }

        protected override void Action()
        {

        }

        public bool PCB缓存站允许流入 = false;
        public bool PCB缓存站请求流出 = false;
        private DateTime PCBoutstartTime;

        void StationClear()
        {
            PCB缓存站允许流入 = false;
            PCB缓存站请求流出 = false;
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
                        if (WorkModeManager.Cur.SelectWorkMode.Mode == WorkMode.TestRun)
                        {
                            //空跑
                            break;
                        }
                        else
                        {
                            Ref_Step("判断外壳缓存位是否有载具");
                        }
                        break;

                    case "判断外壳缓存位是否有载具":
                        if (StaticIOHelper.PCB缓存位载具流进检测.In())
                        {
                            Ref_Step("等待载具流入到位");
                        }
                        else
                        {
                            Ref_Step("等待PCB站请求流出");
                        }
                        break;

                    case "等待PCB站请求流出":
                        if (Process_PCB站.Cur.PCB站请求流出)
                        {
                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            Process_PCB站.Cur.PCB站请求流出 = false;
                            PCB缓存站允许流入 = true;
                            WriteToUser("PCB缓存站允许流入");
                            Ref_Step("等待载具流入到位");
                            PCBoutstartTime = DateTime.Now;
                        }
                        break;

                    case "等待载具流入到位":
                        if (StaticIOHelper.PCB缓存位载具流进检测.In())//载具到位
                        {
                            PCB缓存站允许流入 = false;
                            WriteToUser("PCB缓存站载具到位");
                            StreamLine.Cur.ConveyorBack.RequestStop();
                            Ref_Step("是否开启后站联机");
                        }
                        if ((DateTime.Now - PCBoutstartTime).TotalMilliseconds > 8000)
                        {
                            WriteErrToUser("PCB缓存位到位超时");

                        }
                        break;

                    case "是否开启后站联机":
                        if (Funs.FunCommon.Cur.BackConnect.GetValue)//开启后站联机
                        {
                            WriteToUser("联机已开启，输送线本站请求流出，等待后站允许流入");
                            ConnectFactory.输送线本机台请求出料.Out_ON();
                            Ref_Step("输送线等待后站允许放料");
                        }
                        else
                        {
                            //人工拿走载具
                            WriteToUser("未开启后站联机！");
                            if (!WaitIO(StaticIOHelper.PCB缓存位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("PCB缓存位阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            Sleep(3000);
                            try
                            {
                                if (!WaitIO(StaticIOHelper.PCB缓存位阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                                {
                                    WriteErrToUser("PCB缓存位阻挡气缸工作位未到位，请检查！");

                                    break;
                                }
                            }
                            finally
                            {
                                StreamLine.Cur.ConveyorBack.RequestStop();
                            }
                            Ref_Step("等待PCB站请求流出");
                        }
                        break;

                    case "输送线等待后站允许放料":
                        if (ConnectFactory.输送线后机台允许进料.In())
                        {
                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            WriteToUser("PCB缓存站载具准备流出");
                            if (!WaitIO(StaticIOHelper.PCB缓存位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("PCB缓存位阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if (!StaticIOHelper.PCB缓存位载具流进检测.In() && !StaticIOHelper.PCB缓存位载具流出检测.In())
                        {
                            Sleep(200);
                            if (!WaitIO(StaticIOHelper.PCB缓存位阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("PCB缓存位阻挡气缸工作位未到位，请检查！");
                                break;
                            }
                            ConnectFactory.输送线本机台请求出料.Out_OFF();
                            Ref_Step("等待后站交互信号");
                        }
                        break;

                    case "等待后站交互信号":
                        if (!ConnectFactory.输送线后机台允许进料.In())//检测后站交互信关闭
                        {
                            StreamLine.Cur.ConveyorBack.RequestStop();
                            Ref_Step("等待PCB站请求流出");
                        }
                        break;
                }
                Sleep(10);
            }
        }
    }
}
