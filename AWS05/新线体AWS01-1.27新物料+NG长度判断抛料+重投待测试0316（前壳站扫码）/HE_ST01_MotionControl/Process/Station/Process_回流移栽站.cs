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
    internal class Process_回流移栽站 : WorkStationBase
    {


        public static readonly Process_回流移栽站 Cur = new Process_回流移栽站();

        private Process_回流移栽站() : base("回流移栽站")
        {

        }

        protected override void Action()
        {

        }

        public bool 移栽站允许流入 = false;
        public bool 移栽站请求流出 = false;

        void StationClear()
        {
            移栽站允许流入 = false;
            移栽站请求流出 = false;
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
                            break;
                        }
                        else
                        {
                            StaticIOHelper.回流移栽皮带线启停.Out_ON();
                            Sleep(1000);
                            StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                            Ref_Step("初始化判断移栽站有无载具");
                        }
                        break;

                    case "初始化判断移栽站有无载具":
                        if (StaticIOHelper.前回流移栽载具流进光电检测.In() || StaticIOHelper.前回流移栽载具流出光电检测.In())
                        {
                            Ref_Step("等待载具流入到位");
                        }
                        else
                        {
                            Ref_Step("判断回流流出站是否请求流出");
                        }
                        break;

                    case "判断回流流出站是否请求流出":
                        if (Process_回流流出站.Cur.回流流出站请求流出)
                        {
                            Ref_Step("判断移栽是否有载具");
                        }
                        else//人工放载具并载具到位
                        {
                            if (!StaticIOHelper.前回流移栽载具流进光电检测.In() && StaticIOHelper.前回流移栽载具流出光电检测.In())
                            {
                                if (!WaitIO(StaticIOHelper.回流移栽气缸, AM.Core.IO.CylinderActionType.WorkPos))
                                {
                                    WriteErrToUser("回流移栽气缸工作位未到位，请检查！");
                                    break;
                                }
                                移栽站请求流出 = true;
                                Ref_Step("等待前壳站允许流入");
                            }
                        }
                        break;

                    case "判断移栽是否有载具":
                        if (StaticIOHelper.前回流移栽载具流进光电检测.In() || StaticIOHelper.前回流移栽载具流出光电检测.In())
                        {
                            WriteToUser("回流流出站请求流出，但人工有放载具，需等待流出！");
                            StaticIOHelper.回流移栽皮带线正反转.Out_OFF();
                            Ref_Step("等待载具流入到位");
                        }
                        else//移栽无载具，等待回流流出站流出载具
                        {
                            Ref_Step("等待流出站请求流出");
                        }
                        break;

                    case "等待流出站请求流出":
                        if (Process_回流流出站.Cur.回流流出站请求流出)
                        {
                            Process_回流流出站.Cur.回流流出站请求流出 = false;
                            移栽站允许流入 = true;
                            StaticIOHelper.回流移栽皮带线启停.Out_ON();
                            Ref_Step("等待载具流入到位");
                        }
                        break;

                    case "等待载具流入到位":
                        if (!StaticIOHelper.前回流移栽载具流进光电检测.In() && StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.回流线后段皮带线载具流出检测.In())//载具到位
                        {
                            //移栽移动到输送线
                            if (!WaitIO(StaticIOHelper.回流移栽气缸, AM.Core.IO.CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("回流移栽气缸工作位未到位，请检查！");
                                break;
                            }
                            StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                            移栽站请求流出 = true;
                            Ref_Step("等待前壳站允许流入");
                        }
                        break;

                    case "等待前壳站允许流入":
                        if (Process_前壳站.Cur.外壳站允许流入)
                        {
                            StaticIOHelper.回流移栽皮带线正反转.Out_ON();//电机反转
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if ((!StaticIOHelper.前回流移栽载具流进光电检测.In() && !StaticIOHelper.前回流移栽载具流出光电检测.In()) && !Process_前壳站.Cur.外壳站允许流入)//载具已经流出并到位
                        {
                            //移栽移动到回流线
                            if (!WaitIO(StaticIOHelper.回流移栽气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("回流移栽气缸原点位未到位，请检查！");
                                break;
                            }
                            StaticIOHelper.回流移栽皮带线正反转.Out_OFF();
                            Ref_Step("判断回流流出站是否请求流出");
                        }
                        break;
                }
                Sleep(10);
            }
        }
    }
}
