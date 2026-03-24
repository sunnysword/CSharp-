using Handler.Connect;
using Handler.Connect.RFID;
using Handler.Funs;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;
using Handler.View.RFID;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Handler.Product;

namespace Handler.Process.Station
{
    internal class Process_前壳站 : WorkStationBase
    {


        public static readonly Process_前壳站 Cur = new Process_前壳站();

        private Process_前壳站() : base("前壳站")
        {
            
        }

        protected override void Action()
        {

        }

        public bool 外壳站允许流入 = false;
        public bool 外壳站允许三轴放料 = false;
        public bool 外壳站请求流出 = false;
        public bool 三轴数据传递完成 = false;
        void StationClear()
        {
            外壳站允许流入 = false;
            外壳站允许三轴放料 = false;
            外壳站请求流出 = false;
            三轴数据传递完成 = false;
        }

        private RFIDModel model = new RFIDModel();
        private bool IsOldShellProduct = false;

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
                            Ref_Step("判断前壳工位流进光电是否有感应");
                        }
                        break;

                    case "判断前壳工位流进光电是否有感应"://只有初始化后，工位存在第一板载具使用
                        if (StaticIOHelper.外壳上料位载具流进检测.In())
                        {
                            Ref_Step("等待载具流入到位");
                        }
                        else
                        {
                            Ref_Step("等待移栽站请求流出");
                        }
                        break;

                    case "等待移栽站请求流出":
                        if (Process_回流移栽站.Cur.移栽站请求流出)
                        {
                            Process_回流移栽站.Cur.移栽站请求流出 = false;
                            外壳站允许流入 = true;
                            StreamLine.Cur.ConveyorFront.RequestRun(Direction.Forward);
                            Ref_Step("等待载具流入到位");
                        }
                        break;

                    case "等待载具流入到位":
                        IsOldShellProduct = false;
                        if (StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.输送线前段皮带线载具流入检测.In())//载具到位
                        {
                            CurrentProduct = ProductInfo.GetInitialProduct();//初始化对象;
                            外壳站允许流入 = false;
                            if (FunStationSetting.Cur.是否启用前壳工位.GetValue && !Process_AutoRun.Cur.IsClearModeWithCountForSHell())
                            {
                                Sleep(200);
                                if (!WaitIO(StaticIOHelper.外壳上料位顶升缸, AM.Core.IO.CylinderActionType.WorkPos))
                                {
                                    WriteErrToUser("外壳上料位顶升缸工作位未到位，请检查！");
                                    break;
                                }
                                //if (!WaitIO(StaticIOHelper.外壳上料位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                                //{
                                //    WriteErrToUser("外壳上料位阻挡气缸原点位未到位，请检查！");
                                //    break;
                                //}
                                StreamLine.Cur.ConveyorFront.RequestStop();
                                Ref_Step("判断上料前载具外壳穴位是否有料");
                            }
                            else
                            {
                                WriteToUser("未启用前壳工位，载具等待后站允许流入", Brushes.Yellow, false);
                                CurrentProduct.StartTimeForCT = DateTime.Now;
                                StreamLine.Cur.ConveyorFront.RequestStop();
                                Ref_Step("解绑RFID");
                            }
                        }
                        break;

                    case "判断上料前载具外壳穴位是否有料":
                        if (FunSelection.Cur.IsUseFrontHolderCoilPass.GetValue)
                        {
                            if (StaticIOHelper.外壳上料位物料检测有无.In())//有物料，直接跳过上料
                            {
                                IsOldShellProduct = true;
                                WriteToUser("外壳放料前载具穴位检测到有外壳，跳过上料流程");
                                Ref_Step("解绑RFID");
                                break;
                            }
                        }
                        else
                        {
                            WriteToUser("未启用回流载具穴位有外壳是否跳过上料功能！");
                            if (FunSelection.Cur.IsUseSensorCheckFrontHolderCoil.GetValue)
                            {
                                if (StaticIOHelper.外壳上料位物料检测有无.In())//有物料，报警
                                {
                                    WriteErrToUser("外壳放料前载具穴位检测到有外壳，请取走！");
                                    break;
                                }
                            }
                        }
                        Ref_Step("允许前壳三轴放料");
                        break;

                    case "允许前壳三轴放料":
                        CurrentProduct.StartTimeForCT = DateTime.Now;
                        外壳站允许三轴放料 = true;
                        Ref_Step("等待前壳放料完成");
                        break;

                    case "等待前壳放料完成":
                        if (Process_AutoRun.Cur.IsClearModeWithCountForSHell())
                        {
                            外壳站允许三轴放料 = false;
                            WriteToUser("清料模式被打开！", Brushes.Yellow, false);
                            Ref_Step("解绑RFID");
                        }
                        if (Process_前壳Tray抓取.Cur.外壳放料完成)
                        {
                            Process_前壳Tray抓取.Cur.外壳放料完成 = false;
                            if (Process_AutoRun.Cur.isReProduce)
                            {
                                Ref_Step("等待重投数据传递完成");
                                break;
                            }
                            Ref_Step("判断上料后载具上外壳是否放好");
                        }
                        break;
                    case "等待重投数据传递完成":
                        CurrentProduct.EndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        CurrentProduct.EndTimeForCT = DateTime.Now;
                        CurrentProduct.Result = Product.ProductInfo.ResultOK;
                        CurrentProduct.SN = Process_前壳Tray抓取.Cur.CurrentProduct.SN;
                        CurrentProduct.PCBSN = Process_前壳Tray抓取.Cur.CurrentProduct.PCBSN;
                        三轴数据传递完成 = true;
                        Ref_Step("判断上料后载具上外壳是否放好");
                        break;
                    case "判断上料后载具上外壳是否放好":
                        if (FunSelection.Cur.IsUseSensorCheckFrontHolderCoil.GetValue)
                        {
                            if (!StaticIOHelper.外壳上料位物料检测有无.In())
                            {
                                WriteErrToUser("外壳上料位检测到外壳穴位无物料，请放好！");
                                break;
                            }
                        }
                        else
                        {
                            WriteToUser("未启用外壳工位载具物料光电检测功能！");
                        }
                        Ref_Step("解绑RFID");
                        break;

                    case "解绑RFID":
                        string error = "";
                        if (FunSelection.Cur.IsUseRFID.GetValue)//启用rfid
                        {
                            if (!WaitIO(StaticIOHelper.外壳上料位顶升缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("外壳上料位顶升缸原点位未到位，请检查！");
                                break;
                            }
                            bool ret = false;

                            if (FunCommunicator.Cur.ShellTCPRfid.GetValue)
                            {
                                ret = RFIDDataHelperTCP.UnbindingRfid(ConnectFactory.RFID_Shell, out error);
                            }
                            else
                            {
                                ret = RFIDDataHelper.UnbindingRfid(RFIDManager.Shell_RFID, out error);
                            }

                            if (!ret)
                            {
                                WriteErrToUser($"{Name}:解绑RFID失败:{error}");
                                break;
                            }
                            model = RFIDDataHelper.GetRfid(RFIDManager.Shell_RFID, out error);
                            if (model == null)
                            {
                                WriteErrToUser($"{Name}:读取RFID失败:{error}");
                                break;
                            }
                            else
                            {
                                WriteToUser(error);
                            }
                            if (model.ProductID != FunMES.Cur.线体名称.GetValue)
                            {
                                WriteErrToUser($"当前rfid记录线体号和当前线体不符，请更换rfid或者手动写入rfid正确的线体号:rfid记录线体：{model.ProductID}:实际线体号：{FunMES.Cur.线体名称.GetValue}");
                                break;
                            }

                            if (IsOldShellProduct)
                            {
                                if (Process_AutoRun.Cur.isReProduce)//重投模式写入数据到RFID
                                {

                                    model.Result = CurrentProduct.Result;
                                    model.Barcode = CurrentProduct.SN;
                                    ret = false;
                                    if (FunCommunicator.Cur.ShellTCPRfid.GetValue)
                                    {
                                        ret = RFIDDataHelperTCP.SetRfid(ConnectFactory.RFID_Shell, model, out error);
                                    }
                                    else
                                    {
                                        ret = RFIDDataHelper.SetRfid(RFIDManager.Shell_RFID, model, out error);
                                    }

                                    if (!ret)
                                    {
                                        WriteErrToUser($"{Name}:重投数据写入RFID失败:{error}");
                                        break;
                                    }
                                    else
                                    {
                                        WriteToUser(error);
                                    }
                                }
                            }
                            else if (/*!FunStationSetting.Cur.是否启用前壳工位.GetValue ||*/ Process_AutoRun.Cur.IsClearModeWithCountForSHell())//不启用或者清料模式todo
                            {

                                if (Process_AutoRun.Cur.IsClearModeWithCountForSHell())
                                {
                                    WriteToUser("清料模式被打开！", Brushes.Yellow, false);
                                }

                                model.Result = "2";//无料
                                model.FormateStationState[RFIDModel.SHELLST] = 0;
                                ret = false;
                                if (FunCommunicator.Cur.ShellTCPRfid.GetValue)
                                {
                                    ret = RFIDDataHelperTCP.SetRfid(ConnectFactory.RFID_Shell, model, out error);
                                }
                                else
                                {
                                    ret = RFIDDataHelper.SetRfid(RFIDManager.Shell_RFID, model, out error);
                                }

                                if (!ret)
                                {
                                    WriteErrToUser($"{Name}:写入RFID失败:{error}");
                                    break;
                                }
                                else
                                {
                                    WriteToUser(error);
                                }
                            }

                            CurrentProduct.RFID = model.CarrierID;
                        }
                        else
                        {
                            WriteToUser("RFID未启用!", Brushes.Yellow, false);
                        }

                        外壳站请求流出 = true;
                        CurrentProduct.EndTimeForCT = DateTime.Now;
                        Ref_Step("等待前壳缓存站允许流入");
                        break;

                    case "等待前壳缓存站允许流入":
                        if (Process_前壳缓存站.Cur.前壳缓存站允许流入)
                        {
                            //Process_前壳缓存站.Cur.前壳缓存站允许流入 = false;
                            if (!WaitIO(StaticIOHelper.外壳上料位顶升缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("外壳上料位顶升缸原点位未到位，请检查！");
                                break;
                            }
                            if (!WaitIO(StaticIOHelper.外壳上料位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("外壳上料位阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorFront.RequestRun(Direction.Forward);
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if (!StaticIOHelper.外壳缓存位载具流出检测.In() && !Process_前壳缓存站.Cur.前壳缓存站允许流入)//载具流出
                        {
                            if (!WaitIO(StaticIOHelper.外壳上料位阻挡气缸, AM.Core.IO.CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("外壳上料位阻挡气缸工作位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorFront.RequestStop();
                            Ref_Step("等待移栽站请求流出");
                        }
                        break;
                }
                Sleep(10);
            }
        }
    }
}
