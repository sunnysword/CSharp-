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
    // <summary>
    // 送料流水线控制
    // </summary>
    class StationFrontLine : WorkStationBase
    {
        public static readonly StationFrontLine Cur = new StationFrontLine();

        private StationFrontLine() : base("送料流水线站")
        {
            this.CheckCurrentStationState = false;
        }

        protected override void Action()
        {

        }

        public bool 流水线外壳站允许放料 = false;

        void StationClear()
        {
            流水线外壳站允许放料 = false;
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
                            //空跑
                        }
                        else
                        {
                            if (Funs.FunCommon.Cur.BackConnect.GetValue)//后站联机
                            {
                            }
                            else
                            {
                                //StaticIOHelper.前段输送线启停.Out_ON();//开启流水线等待载具到位
                                Ref_Step("等待载具流入到位");
                            }
                        }
                        break;

                    case "等待载具流入到位":
                        if (StaticIOHelper.外壳上料位载具流出检测.In())//载具到位
                        {
                            StaticIOHelper.外壳上料位顶升缸.WorkPos();
                            StaticIOHelper.外壳上料位阻挡气缸.OriginPos();
                            流水线外壳站允许放料 = true;
                            Ref_Step("等待前壳放料完成");
                        }
                        break;

                    case "等待前壳放料完成":
                        if (Process_前壳Tray抓取.Cur.外壳放料完成)
                        {
                            Process_前壳Tray抓取.Cur.外壳放料完成 = false;

                        }
                        break;

                    case "读取RFID":
                        if (this.CurrentStationHaveRFID())
                        {
                            if (Funs.FunSelection.Cur.IsUseRFID.GetValue)
                            {
                                //tempRfid = Fun_StationReadRFID();

                                //ReportMsg("读取的RFID：" + tempRfid);
                                //CurrentProduct = FindProductBaseRFID(tempRfid);
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Remove(CurrentProduct);
                                });
                                Ref_Step("请求允许流出");
                            }
                            else
                            {
                                ReportMsg("RFID禁用");
                                Ref_Step("请求允许流出");
                            }
                        }
                        else
                        {
                            ReportMsg("本站无RFID");
                            Ref_Step("请求允许流出");
                        }
                        break;

                    case "请求允许流出":
                        Ref_Step("判断下一站是否Ready");
                        break;

                    case "判断下一站是否Ready":
                        //if (NextStation.IsReady)
                        //{
                        //    if (this.ProductFlowOnPosSignalEventHandler.Invoke())
                        //    {
                        //        NextStation.LoadProduct(CurrentProduct);
                        //        ClearProduct();
                        //        tempRfid = "";
                        //        Ref_Step("挡停气缸下降");

                        //    }
                        //    else
                        //    {
                        //        WriteErrToUser("下流水线治具准备流向下一站时，未感应到治具");
                        //    }

                        //}
                        break;

                    case "挡停气缸下降":
                        //if (StaticIOHelper.CylinderDownLineStopCy.Out(out errorCylinder))
                        //{
                        //    Ref_Step("等待托盘流走");
                        //}
                        //else
                        //{
                        //    WriteErrToUser(errorCylinder);
                        //}
                        break;

                    case "等待托盘流走":
                        //if (CheckAction.CheckAction(() => NextStation.ProductFlowOnPosSignalEventHandler.Invoke() && NextStation.IsReady == false, 10000))
                        //{
                        //    Ref_Step("挡停气缸上升");
                        //}
                        //else
                        //{
                        //    if (!IsProcessRunning) break;
                        //    WriteErrToUser($"{Name}:下层流水线治具流向下一站超时");
                        //}
                        break;

                    case "挡停气缸上升":
                        //if (StaticIOHelper.CylinderDownLineStopCy.Back(out errorCylinder))
                        //{
                        //    Ref_Step("判断状态");
                        //}
                        //else
                        //{
                        //    WriteErrToUser(errorCylinder);
                        //}
                        break;

                    default:
                        throw new Exception($"{Name}:当前步骤不存在，当前步骤：{Step}");
                }
                Sleep(10);
            }
        }
    }
}
