using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Motion.IO;

namespace Handler.Process.Station
{
    // <summary>
    // 回流流水线控制
    // </summary>
    // </summary>
    class StationBackLine : WorkStationBase
    {
        public static readonly StationBackLine Cur = new StationBackLine();

        private StationBackLine() : base("回流流水线站")
        {
            this.CheckCurrentStationState = false;
        }

        protected override void Action()
        {

        }

        protected override void ImplementRunAction()
        {
            IsReady = false;
            string errorCylinder = string.Empty;
            Ref_Step("判断状态");
            string tempRfid = "";

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }

                switch (Step)
                {
                    case "判断状态":
                        if (this.CheckProductFlowOnPos())
                        {
                            IsReady = false;
                            Ref_Step("读取RFID");
                        }
                        else
                        {
                            IsReady = true;
                        }
                        break;

                    case "读取RFID":
                        if (this.CurrentStationHaveRFID())
                        {
                            if (Funs.FunSelection.Cur.IsUseRFID.GetValue)
                            {
                                tempRfid = Fun_StationReadRFID();

                                ReportMsg("读取的RFID：" + tempRfid);
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
                    //break;

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
