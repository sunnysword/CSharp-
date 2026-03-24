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
    internal class Process_回流流入站 : WorkStationBase
    {


        public static readonly Process_回流流入站 Cur = new Process_回流流入站();

        private Process_回流流入站() : base("回流流入站")
        {

        }

        protected override void Action()
        {

        }

        public bool 回流流入站请求流出 = false;
        public bool 流水线外壳站请求流出 = false;

        void StationClear()
        {
            回流流入站请求流出 = false;
            流水线外壳站请求流出 = false;
        }

        protected override void ImplementRunAction()
        {
            Ref_Step("是否空跑");
            StationClear();

            while (true)
            {
                //if (isPause)
                //{
                //    StaticIOHelper.后站交互本站暂停.Out_ON();
                //    Sleep(100);
                //}
                //else
                //{
                //    StaticIOHelper.后站交互本站暂停.Out_OFF();//
                //    Sleep(100);
                //}
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
                            Ref_Step("判断回流流入站流进光电是否有感应");
                        }
                        break;

                    #region 旧
                    //case "判断后站是否暂停":
                    //    if (StaticIOHelper.后站交互后站停止.In())
                    //    {
                    //        WriteToUser("后站停止，等待后站启动");

                    //    }
                    //    else 
                    //    {
                    //        Ref_Step("本站向后站要料");
                    //    }
                    //    break;
                    #endregion

                    case "判断回流流入站流进光电是否有感应"://只有初始化后，工位存在第一板载具使用
                        if (StaticIOHelper.回流线前段缓存位流进检测.In())
                        {
                            回流流入站请求流出 = true;
                            Ref_Step("等待回流流出站允许流入");
                        }
                        else
                        {
                            Ref_Step("判断后站联机是否开启");
                        }
                        break;

                    case "判断后站联机是否开启":
                        if (Funs.FunCommon.Cur.BackConnect.GetValue)//后站联机
                        {
                            Ref_Step("回流线等待后站请求流出");
                        }
                        else
                        {
                            Ref_Step("等待人工放载具到位");
                        }
                        break;

                    case "回流线等待后站请求流出":
                        if (ConnectFactory.回流线后机台请求出料.In())
                        {
                            StreamLine.Cur.ReturnFrontConveyor.RequestRun(Direction.Forward);
                            ConnectFactory.回流线本机台允许进料.Out_ON();
                            Ref_Step("回流线等待交互完成");
                        }
                        break;

                    case "回流线等待交互完成":
                        if (!ConnectFactory.回流线后机台请求出料.In())
                        {
                            ConnectFactory.回流线本机台允许进料.Out_OFF();
                            Ref_Step("等到后站载具流入到位");
                        }
                        break;

                    case "等到后站载具流入到位":
                        if (StaticIOHelper.回流线前段缓存位流进检测.In())//载具流入到位
                        {
                            回流流入站请求流出 = true;
                            Ref_Step("等待回流流出站允许流入");
                        }
                        break;

                    case "等待人工放载具到位":
                        if (StaticIOHelper.回流线前段缓存位流进检测.In())//载具流入到位
                        {

                            回流流入站请求流出 = true;
                            Ref_Step("等待回流流出站允许流入");
                        }
                        break;

                    case "等待回流流出站允许流入":
                        StreamLine.Cur.ReturnFrontConveyor.RequestStop();
                        Ref_Step("等待回流流出站允许流入1");
                        break;

                    case "等待回流流出站允许流入1":
                        if (Process_回流流出站.Cur.回流流出站允许流入)//允许流入为false时表示已经流到下一站并到位
                        {
                            Process_回流流出站.Cur.回流流出站允许流入 = false;//载具到位后再置false
                            if (!WaitIO(StaticIOHelper.回流线前段阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("回流线前段阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ReturnFrontConveyor.RequestRun(Direction.Forward);
                            Sleep(500);
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if (!StaticIOHelper.回流线前段缓存位流出检测.In() && !Process_回流流出站.Cur.回流流出站允许流入)//载具已经流出并到位
                        {
                            if (Funs.FunCommon.Cur.BackConnect.GetValue)//后站联机
                            {
                                if (!WaitIO(StaticIOHelper.回流线前段阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                                {
                                    WriteErrToUser("回流线前段阻挡气缸工作位未到位，请检查！");
                                    break;
                                }
                                Ref_Step("回流线等待后站请求流出");
                            }
                            else
                            {
                                if (!WaitIO(StaticIOHelper.回流线前段阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                                {
                                    WriteErrToUser("回流线前段阻挡气缸工作位未到位，请检查！");
                                    break;
                                }
                                Ref_Step("等待人工放载具到位");

                            }
                            Sleep(400);
                            StreamLine.Cur.ReturnFrontConveyor.RequestStop();
                        }
                        break;
                }
                Sleep(10);
            }
        }
    }
}
