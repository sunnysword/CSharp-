using AM.Core.IO;
using Handler.Connect;
using Handler.Connect.RFID;
using Handler.Funs;
using Handler.MES;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;
using Handler.Product;
using Handler.View.RFID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handler.View; 
using System.Threading.Tasks;
using System.Windows.Media;

namespace Handler.Process.Station
{
    internal class Process_PCB站 : WorkStationBase
    {
        public static readonly Process_PCB站 Cur = new Process_PCB站();

        private Process_PCB站() : base("PCB站")
        {

        }

        protected override void Action()
        {

        }

        public bool PCB站允许流入 = false;
        public bool 允许PCB三轴放料 = false;
        public bool PCB站请求流出 = false;
        public bool 三轴数据传递完成 = false;
        /// <summary>
        /// 人工扫码
        /// </summary>

        public string ArtificialSN { get; set; }

        void StationClear()
        {
            PCB站允许流入 = false;
            允许PCB三轴放料 = false;
            PCB站请求流出 = false;
            三轴数据传递完成 = false;
        }

        private RFIDModel model = new RFIDModel();

        protected override void ImplementRunAction()
        {
            Ref_Step("是否空跑");
            StationClear();
            string error = string.Empty;
            string RFID_Info = string.Empty;

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
                            Ref_Step("判断PCB工位流进光电是否有感应");
                        }
                        break;

                    case "判断PCB工位流进光电是否有感应":
                        if (StaticIOHelper.PCB上料位载具流进检测.In())
                        {
                            Ref_Step("等待载具流入到位");
                        }
                        else
                        {
                            Ref_Step("等待前壳缓存站请求流出");
                        }
                        break;

                    case "等待前壳缓存站请求流出":
                        if (Process_前壳缓存站.Cur.前壳缓存站请求流出)
                        {
                            Process_前壳缓存站.Cur.前壳缓存站请求流出 = false;
                            PCB站允许流入 = true;
                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            Ref_Step("等待载具流入到位");
                        }
                        break;

                    case "等待载具流入到位":
                        if (StaticIOHelper.PCB上料位载具流进检测.In())//载具到位
                        {
                            PCB站允许流入 = false;
                            if (FunStationSetting.Cur.是否启用pcb工位.GetValue && !Process_AutoRun.Cur.IsClearModeWithCount())
                            {
                                Sleep(500);
                                //if (!WaitIO(StaticIOHelper.PCB上料位顶升缸, AM.Core.IO.CylinderActionType.WorkPos))
                                //{
                                //    WriteErrToUser("PCB上料位顶升缸工作位未到位，请检查！");
                                //    break;
                                //}
                                //if (!WaitIO(StaticIOHelper.PCB上料位阻挡气缸, AM.Core.IO.CylinderActionType.OriginPos))
                                //{
                                //    WriteErrToUser("PCB上料位阻挡气缸原点位未到位，请检查！");
                                //    break;
                                //}
                                Ref_Step("判断RFID中产品状态");
                            }
                            else if (Process_AutoRun.Cur.IsClearModeWithCount())
                            {
                                WriteToUser("清料模式被打开！", Brushes.Yellow, false);
                                Sleep(500);
                                if (FunSelection.Cur.IsUseSensorCheckPCBCoil.GetValue)
                                {
                                    if (StaticIOHelper.PCB上料位外壳检测有无.In())
                                    {
                                        WriteErrToUser("清料模式检测到PCB上料位外壳穴位有外壳，请先手动移除！");
                                        break;
                                    }
                                }
                                model = RFIDDataHelper.GetRfid(RFIDManager.PCB_RFID, out error);
                                if (model == null)
                                {
                                    WriteErrToUser($"{Name}:读取RFID失败:{error}");
                                    break;
                                }
                                StreamLine.Cur.ConveyorBack.RequestStop();
                                Ref_Step("RFID写入");
                            }
                            else
                            {
                                WriteToUser("未启用pcb工位，载具等待后站允许流入", Brushes.Yellow, false);
                                StreamLine.Cur.ConveyorBack.RequestStop();
                                Ref_Step("判断RFID中产品状态");
                            }
                        }
                        break;

                    case "判断RFID中产品状态":
                        if (FunSelection.Cur.IsUseRFID.GetValue)
                        {
                            if (FunCommunicator.Cur.PCBTCPRfid.GetValue)
                            {
                                model = RFIDDataHelperTCP.GetRfid(ConnectFactory.RFID_PCB, out error);
                            }
                            else
                            {
                                model = RFIDDataHelper.GetRfid(RFIDManager.PCB_RFID, out error);
                            }

                            if (model == null)
                            {
                                WriteErrToUser($"{Name}:读取RFID失败:{error}");
                                break;
                            }
                            else
                            {
                                WriteToUser(error);
                            }
                            if (model.Result != "1")
                            {
                                WriteToUser("当前载具无产品或产品NG不做料！");
                                StreamLine.Cur.ConveyorBack.RequestStop();
                                Ref_Step("RFID写入");
                                break;
                            }
                            if (Process_AutoRun.Cur.isReProduce) 
                            {
                                WriteToUser("当前为重投模式，直接出站！");
                                StreamLine.Cur.ConveyorBack.RequestStop();
                                Ref_Step("RFID写入");
                                break;
                            }

                            if (model.FormateStationState["ST01-2"] == 1)
                            {
                                WriteToUser("当前产品已做料，直接出站！");
                                StreamLine.Cur.ConveyorBack.RequestStop();
                                Ref_Step("RFID写入");
                                break;
                            }
                        }
                        else
                        {
                            WriteToUser("RFID未启用!", Brushes.Yellow, false);
                        }
                        if (!WaitIO(StaticIOHelper.PCB上料位顶升缸, CylinderActionType.WorkPos))
                        {
                            WriteErrToUser("PCB上料位顶升缸工作位未到位，请检查！");
                            break;
                        }
                        StreamLine.Cur.ConveyorBack.RequestStop();
                        //if (Process_AutoRun.Cur.isReProduce)
                        //{
                        //    Ref_Step("等待人工扫码");
                        //    break;
                        //}
                        Ref_Step("判断上料前载具上是否有PCB和外壳");
                        break;
                    case "判断上料前载具上是否有PCB和外壳":
                        if (FunSelection.Cur.IsUseSensorCheckPCBCoil.GetValue)
                        {
                            if (StaticIOHelper.PCB上料位PCB检测有无.In())
                            {
                                WriteErrToUser("PCB上料位PCB穴位检测到有PCB，请取走！");
                                break;
                            }
                            if (!StaticIOHelper.PCB上料位外壳检测有无.In())
                            {
                                WriteErrToUser("PCB上料位外壳穴位未检测到有外壳，请检查并放好！");
                                break;
                            }
                        }
                        Ref_Step("允许PCB三轴放料");
                        break;
                    case "等待人工扫码":
                        ArtificialSN = string.Empty;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ArtificialSN = Microsoft.VisualBasic.Interaction.InputBox("请输入SN：", "提示");
                        });

                        if (string.IsNullOrEmpty(ArtificialSN))
                        {
                            WriteErrToUser("输入的SN为空，请重新输入");
                            break;
                        }
                        else
                        {
                            Ref_Step("人工扫码转型");
                        }
                        break;
                    case "人工扫码转型":
                        WriteToUser($"人工扫到的码是：{ArtificialSN}");
                        var 机种 = FunMES.Cur.MES是否使用其他机型上传.GetValue ? FunMES.Cur.MES其他机型.GetValue : WorkItemManagerHelper.LoadedName;
                        var checkHeader = FunMESCheck.Cur.CheckHeader.GetValue;
                        var version = FunMESCheck.Cur.SnVersion.GetValue;
                        var checkResult = ArtificialSN.Length > checkHeader.Length && ArtificialSN.Substring(0, checkHeader.Length) == checkHeader;
                        var formateBarcode = $"{机种}{version}{ArtificialSN.Substring(8)}";
                        CurrentProduct = ProductInfo.GetInitialProduct();//初始化对象
                        CurrentProduct.SN = formateBarcode;

                        if (CurrentProduct.SN != "" && CurrentProduct.SN != null)
                        {
                            if (CurrentProduct.SN.Count() > 19)
                            {
                                CurrentProduct.SN = null;
                                WriteErrToUser($"SN位数大于19位！视为NG");
                            }
                        }

                        if (!checkResult)
                        {
                            WriteErrToUser($"PCB头校验失败！{ArtificialSN},请重新输入");
                            Ref_Step("等待人工扫码");
                            break;

                        }

                        CurrentProduct.StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        CurrentProduct.StartTimeForCT = DateTime.Now;
                        CurrentProduct.EndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        CurrentProduct.EndTimeForCT = DateTime.Now;
                        CurrentProduct.Result = ProductInfo.ResultOK;
                        CurrentProduct.PCBSN = ArtificialSN;
                        CurrentProduct.RFID = model.CarrierID;
                        RFID_Info = ProductInfoConverter.GetStringFromProduct(CurrentProduct);
                        if (Process_AutoRun.Cur.isReProduce)
                        {
                            Ref_Step("RFID写入");
                            break;
                        }
                        break;
                    case "允许PCB三轴放料":
                        允许PCB三轴放料 = true;
                        CurrentProduct = ProductInfo.GetInitialProduct();//初始化对象
                        CurrentProduct.StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        CurrentProduct.StartTimeForCT = DateTime.Now;
                        Ref_Step("等待PCB放料完成");
                        break;

                    case "等待PCB放料完成":
                        if (Process_PCBTray抓取.Cur.PCB放料完成)
                        {
                            CurrentProduct.EndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                            CurrentProduct.EndTimeForCT = DateTime.Now;
                            CurrentProduct.Result = Process_PCBTray抓取.Cur.CurrentProduct.Result;
                            CurrentProduct.SN = Process_PCBTray抓取.Cur.CurrentProduct.SN;
                            CurrentProduct.PCBSN = Process_PCBTray抓取.Cur.CurrentProduct.PCBSN;
                            CurrentProduct.RFID = model.CarrierID;
                            Process_PCBTray抓取.Cur.PCB放料完成 = false;
                            RFID_Info = ProductInfoConverter.GetStringFromProduct(CurrentProduct);
                            三轴数据传递完成 = true;
                            Ref_Step("判断上料后载具上外壳和PCB是否放好");
                        }

                        if (Process_AutoRun.Cur.IsClearModeWithCount())
                        {
                            允许PCB三轴放料 = false;
                            WriteToUser("清料模式被打开！", Brushes.Yellow, false);
                            Ref_Step("RFID写入");
                        }
                        break;

                    case "判断上料后载具上外壳和PCB是否放好":
                        if (FunSelection.Cur.IsUseSensorCheckPCBCoil.GetValue && !Process_AutoRun.Cur.IsClearModeWithCount())
                        {
                            if (!StaticIOHelper.PCB上料位外壳检测有无.In())
                            {
                                WriteErrToUser("PCB上料位放料后检测到外壳穴位无物料，请检查并放好！");
                                break;
                            }
                            if (!StaticIOHelper.PCB上料位PCB检测有无.In())
                            {
                                WriteErrToUser("PCB上料位放料后检测到PCB穴位无物料，请检查并放好！");
                                break;
                            }
                        }
                        else
                        {
                            WriteToUser("未启用PCB工位载具物料光电检测功能！");
                        }
                        Ref_Step("上传MES");
                        break;

                    case "上传MES":
                        if (Funs.FunSelection.Cur.IsUseMes.GetValue)//todo MES
                        {
                            WriteToUser("启动mes，上传pcb码");
                            var onlySave = FunMES.Cur.IsDataUpSaveOnly.GetValue;
                            var ret = MESHelper2.Cur.DataUp(CurrentProduct, !onlySave, out error);
                            if (ret == false)
                            {
                                ErrorDialog("PCB站MES数据上传失败，请选择后续操作",
                                   ("重新上传", () =>
                                   {
                                       MESHelper2.Cur.RemoveFailItem(MESHelper2.Cur.resList, CurrentProduct.SN, "0x04");
                                   }
                                ),
                                   ("人工NG", () =>
                                   {
                                       Params.ParamsNGTray.Cur.AddMesCheckNGNumNg();
                                       Params.ParamsNGTray.Cur.AddNGInfoOnce("MES出站NG");
                                       CurrentProduct.Result = "2";
                                       WriteErrToUser($"PCB站MES数据上传失败！{error}");
                                       Ref_Step("RFID写入");
                                   }
                                )
                                );
                                break;
                            }
                        }
                        Ref_Step("RFID写入");
                        break;

                    case "RFID写入":
                        if (FunSelection.Cur.IsUseRFID.GetValue)//启用rfid
                        {
                            if (!WaitIO(StaticIOHelper.PCB上料位顶升缸, CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("PCB上料位顶升缸原点位未到位，请检查！");
                                break;
                            }
                            if ((FunStationSetting.Cur.是否启用pcb工位.GetValue && !Process_AutoRun.Cur.IsClearModeWithCount())|| Process_AutoRun.Cur.isReProduce)
                            {
                                if (model.Result == "1")
                                {
                                    model.Barcode = CurrentProduct.SN;
                                    model.Result = CurrentProduct.Result == "1" ? "1" : "-1";
                                    model.FormateStationState[RFIDModel.PCBST] = CurrentProduct.Result == "1" ? 1 : 2;
                                }
                                else if (model.Result == "2")
                                {
                                    model.FormateStationState[RFIDModel.PCBST] = 0;
                                    //if (StaticIOHelper.PCB上料位PCB检测有无.In())
                                    //{
                                    //    WriteErrToUser("PCB上料位PCB穴位检测到有PCB，但是现在是清料模式，请取走！");
                                    //    break;
                                    //}
                                    //if (StaticIOHelper.PCB上料位外壳检测有无.In())
                                    //{
                                    //    WriteErrToUser("PCB上料位外壳穴位检测到有外壳，但是现在是清料模式，请取走！");
                                    //    break;
                                    //}
                                }
                                else
                                {
                                    model.Result = "-1";
                                    model.FormateStationState[RFIDModel.PCBST] = 0;
                                }
                            }
                            else
                            {
                                model.Result = "2";
                                model.FormateStationState[RFIDModel.PCBST] = 0;
                                if (StaticIOHelper.PCB上料位PCB检测有无.In())
                                {
                                    WriteErrToUser("PCB上料位PCB穴位检测到有PCB，但是现在是清料模式，请取走！");
                                    break;
                                }
                                if (StaticIOHelper.PCB上料位外壳检测有无.In())
                                {
                                    WriteErrToUser("PCB上料位外壳穴位检测到有外壳，但是现在是清料模式，请取走！");
                                    break;
                                }
                            }
                            bool ret = true;
                            if (FunCommunicator.Cur.PCBTCPRfid.GetValue)
                            {
                                ret = RFIDDataHelperTCP.SetRfid(ConnectFactory.RFID_PCB, model, out error);
                            }
                            else
                            {
                                ret = RFIDDataHelper.SetRfid(RFIDManager.PCB_RFID, model, out error);
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
                        else
                        {
                            WriteToUser("RFID未启用!", Brushes.Yellow, false);
                        }
                        PCB站请求流出 = true;
                        Process_AutoRun.Cur.SetClearCount();
                        Ref_Step("等待PCB缓存站允许流入");
                        break;

                    case "等待PCB缓存站允许流入":
                        if (Process_PCB缓存站.Cur.PCB缓存站允许流入)
                        {
                            if (!WaitIO(StaticIOHelper.PCB上料位顶升缸, CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("PCB上料位顶升缸原点位未到位，请检查！");
                                break;
                            }
                            if (!WaitIO(StaticIOHelper.PCB上料位阻挡气缸, CylinderActionType.OriginPos))
                            {
                                WriteErrToUser("PCB上料位阻挡气缸原点位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorBack.RequestRun(Direction.Forward);
                            Ref_Step("等待载具流出");
                        }
                        break;

                    case "等待载具流出":
                        if (!StaticIOHelper.PCB上料位载具流出检测.In() && !Process_PCB缓存站.Cur.PCB缓存站允许流入)
                        {
                            if (!WaitIO(StaticIOHelper.PCB上料位阻挡气缸, CylinderActionType.WorkPos))
                            {
                                WriteErrToUser("PCB上料位阻挡气缸工作位未到位，请检查！");
                                break;
                            }
                            StreamLine.Cur.ConveyorBack.RequestStop();
                            Ref_Step("等待前壳缓存站请求流出");
                        }
                        break;
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
