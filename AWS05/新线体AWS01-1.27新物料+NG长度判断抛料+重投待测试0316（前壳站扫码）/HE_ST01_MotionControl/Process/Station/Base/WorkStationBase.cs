using RM_dll2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Product;
using Handler.Process.RunMode;
using Handler.Motion.IO;
using Handler.Connect.FrontBack;
using Platform;
using Handler.Core.Process;
using Handler.Process.Station.Base;
using AM.Core.IO;
using Handler.Motion;
using Handler.Connect.RFID;
using Handler.View.RFID;

namespace Handler.Process.Station
{
    /// <summary>
    /// 每站的抽象基类
    /// </summary>
    public abstract class WorkStationBase : StationBase
    {

        public WorkStationBase(string name) : base(name)
        {
            this.IsCheckMotionErr = false;
        }

        public Platform.ElevatingPlatform FrontNextConnect;
        public Platform.CommunicationProductInfo RemoteProductInfo;
        /// <summary>
        /// 是否是上层气缸降下来的产品
        /// </summary>
        public bool IsUpLevelProduct { get; set; } = false;

        /// <summary>
        /// 该工位是否是上下双层气缸
        /// </summary>
        public bool IsDoubleLevelCylinder { get; set; }
        protected abstract void Action();

        public AM.Core.IO.Cylinder CylinderUp;
        public AM.Core.IO.Cylinder CylinderStop;
        public AM.Core.IO.Cylinder CylinderPin;

        public bool IsLastStation { get; set; }

        public override void Stop()
        {
            IsUpLevelProduct = false;
            base.Stop();
        }

        public virtual bool IsContinueIfProductNG()
        {
            return false;
        }
        /// <summary>
        /// 是否只执行一次流程,用于单机模式下第一个工作作为上料工位，而不是前站传递
        /// </summary>
        /// <returns></returns>
        public virtual bool IsFirstStationWhenSingleRun()
        {
            return false;
        }

        public bool CylinderWorkPos(out string error)
        {
            error = string.Empty;
            if (CylinderUp != null)
            {
                if (!CylinderUp.WorkPos(out error))
                {
                    return false;
                }
            }
            if (CylinderPin != null)
            {
                if (!CylinderPin.WorkPos(out error))
                {
                    return false;
                }
            }
            //Task.Run(() => { CylinderStop.OriginPos(out _); });
            return true;
        }

        public bool CylinderOriginPos(bool isWaitSignal, out string error)
        {
            error = string.Empty;
            if (CylinderPin != null)
            {
                if (!CylinderPin.OriginPos(out error, isWaitSignal))
                {
                    return false;
                }
            }
            Task.Run(() => { CylinderStop.WorkPos(out _, isWaitSignal); });
            if (CylinderUp != null)
            {
                if (!CylinderUp.OriginPos(out error, isWaitSignal))
                {
                    return false;
                }
            }
            return true;
        }


        public bool HasUnlock = false;
        public void Unlock()
        {
            if (!HasUnlock)
            {
                HasUnlock = true;
                this.FrontNextConnect.UnLock();
            }
        }

        protected override void ImplementRunAction()
        {
            if (FrontNextConnect != null)
            {
                FrontNextConnect.IsRunning = false;
            }
            HasUnlock = false;
            IsReady = false;
            RemoteProductInfo = null;
            bool result = false;
            string error = string.Empty;
            string rfidInfo = string.Empty;
            string remoteStr = string.Empty;
            CurrentProduct = null;
            bool isMesOK = false;
            string mesContent = string.Empty;
            bool isCarrierExist = false;    //流程刚开始是否有治具，有的话需要先排空治具
            string tempRfid = string.Empty;
            string errorCylinder = string.Empty;
            SampleProductInfo remoteProduct = null;

            RunTimeCirculate = "";
            Sleep(3000);
            Ref_Step("判断状态");

            //while (true)
            //{
            //    if (!IsProcessRunning)
            //    {
            //        break;
            //    }
            //    WaitPause();
            //    if (!IsProcessRunning)
            //    {
            //        break;
            //    }
            //    switch (Step)
            //    {
            //        case "判断状态":
            //            if (CheckProductFlowOnPos())
            //            {
            //                if (!CurrentWorkMode.IsConnectWithFrontStation())
            //                {
            //                    if (IsFirstStationWhenSingleRun())
            //                    {
            //                        Ref_Step("等待载具进入工作位");
            //                    }
            //                    else
            //                    {
            //                        WriteErrToUser("当前为单机模式，只允许本站第一工位存在治具，请将其他工位治具移走");
            //                        break;
            //                    }
            //                }
            //                else
            //                {
            //                    isCarrierExist = true;
            //                    this.LoadProduct(null);
            //                    Ref_Step("顶升气缸升起");
            //                }
            //            }
            //            else
            //            {
            //                IsReady = true;
            //                Ref_Step("判断顶升气缸状态");
            //            }
            //            break;

            //        case "判断顶升气缸状态":
            //            if (this.Name == ProcessParams.Name_StationMoving)
            //            {
            //                if (StaticIOHelper.CyMovingStationUpDown.GetOriginPosState() == false)
            //                {
            //                    Ref_Step("清空治具流程搬运工位顶升气缸下降");
            //                    break;
            //                }
            //            }
            //            Ref_Step("设置本站允许流入");
            //            break;

            //        case "清空治具流程搬运工位顶升气缸下降":
            //            if (WaitIO(StaticIOHelper.CyMovingStationUpDown, CylinderActionType.OriginPos))
            //            {
            //                if (this.ProductFlowOnPosSignalEventHandler?.Invoke() == true)
            //                {
            //                    if (!CurrentWorkMode.IsConnectWithFrontStation() && !IsFirstStationWhenSingleRun())
            //                    {
            //                        WriteErrToUser("当前为单机模式，只允许本站第一工位存在治具，请将其他工位治具移走");
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        isCarrierExist = false;
            //                        Ref_Step("顶升气缸升起");
            //                        break;
            //                    }
            //                }
            //            }
            //            Ref_Step("设置本站允许流入");
            //            break;





            //        case "设置本站允许流入":
            //            HasUnlock = false;
            //            Ref_Step("等待载具进入工作位");
            //            break;

            //        case "等待载具进入工作位":
            //            if (this.CheckProductFlowOnPos())
            //            {
            //                if (this.ProductLeaveSignalEventHandler?.Invoke() == true)
            //                {
            //                    ReportMsg("治具到位传感器感应到治具，但此时治具离开传感器也仍能感应到治具");
            //                    break;
            //                }
            //                if (IsUpLevelProduct)
            //                {
            //                    IsUpLevelProduct = false;
            //                    ReportMsg("搬运工位上层治具下降");
            //                    Ref_Step("MES过站");
            //                    break;
            //                }
            //                if (CurrentWorkMode.IsConnectWithBackStation())
            //                {
            //                    FrontNextConnect.Lock();
            //                    IsReady = false;


            //                }

            //                if (!CheckCurrentStationState)
            //                {
            //                    Ref_Step("顶升气缸升起");
            //                    break;
            //                }
            //                if (FrontNextConnect.ProductIsLoad || (IsFirstStationWhenSingleRun() && !CurrentWorkMode.IsConnectWithFrontStation()))
            //                {
            //                    FrontNextConnect.Reset_ProductIsLoad();
            //                    IsLoad = false;
            //                    IsReady = false;
            //                    Ref_Step("获取产品信息");

            //                }
            //                else
            //                {
            //                    HasUnlock = false;
            //                    if (!IsProcessRunning) break;
            //                    if (CheckAction.CheckAction(() => this.ProductFlowOnPosSignalEventHandler.Invoke(), 1000))
            //                    {
            //                        WriteErrToUser($"{Name}：上一站未放托盘，当前托盘感应检测异常!");
            //                        HasUnlock = false;

            //                        break;
            //                    }
            //                    else
            //                    {
            //                        IsReady = true;
            //                    }
            //                }

            //            }
            //            else
            //            {
            //                Unlock();
            //            }
            //            break;


            //        case "获取产品信息":
            //            if (IsFirstStationWhenSingleRun() && !CurrentWorkMode.IsConnectWithFrontStation())
            //            {
            //                ReportMsg("当前流程只执行一次");
            //                this.LoadProduct(ProductInfo.GetInitialProduct());
            //            }
            //            else
            //            {
            //                remoteStr = FrontNextConnect.ProductInfo;
            //                FrontNextConnect.ProductInfo = string.Empty;
            //                ReportMsg("收到上一站TCP产品信息:" + remoteStr);
            //                remoteProduct = SampleProductInfo.GetProductInfo(remoteStr);
            //                CurrentProduct = ProductInfoConverter.GetProductInfoClass(this, remoteProduct);
            //            }
            //            Ref_Step("顶升气缸升起");
            //            break;

            //        case "顶升气缸升起":
            //            if (!CylinderWorkPos(out errorCylinder))
            //            {
            //                WriteErrToUser(errorCylinder);
            //                break;
            //            }
            //            if (IsFirstStationWhenSingleRun() && !CurrentWorkMode.IsConnectWithFrontStation())
            //            {
            //                ReportMsg("当前流程只执行一次");
            //                this.LoadProduct(ProductInfo.GetInitialProduct());
            //                Ref_Step("判断产品状态");
            //            }
            //            else
            //            {
            //                Ref_Step("读取RFID");
            //            }
            //            break;

            //        case "读取RFID":
            //            if (!Funs.FunSelection.Cur.IsUseRFID)
            //            {
            //                ReportMsg("RFID未启用");
            //                Ref_Step("判断产品状态");
            //                break;
            //            }
            //            if (this.CurrentStationHaveRFID())
            //            {
            //                if (this.RFID.ReadData(out rfidInfo, out error))
            //                {
            //                    ReportMsg("RFID读取:" + rfidInfo);
            //                    Ref_Step("比对TCP通讯和RFID产品内容");
            //                }
            //                else
            //                {
            //                    Ref_Step("RFID读取失败处理");
            //                }
            //            }
            //            else
            //            {
            //                ReportMsg("此站无RFID");
            //                Ref_Step("判断产品状态");
            //            }
            //            break;

            //        case "比对TCP通讯和RFID产品内容":

            //            if (remoteStr == rfidInfo)
            //            {
            //                Ref_Step("根据RFID内容设置产品信息");
            //            }
            //            else
            //            {
            //                Ref_Step("内容比对失败报错");
            //            }

            //            break;

            //        case "内容比对失败报错":
            //            ErrorDialog($"{Name}:RFID读取内容和前一站发送的产品信息内容不一致，RFID读取内容:{rfidInfo} " +
            //                $"前一站发送内容:{remoteStr} 请选择正确的产品信息",
            //                     ("以RFID内容作为产品信息", () => Ref_Step("根据RFID内容设置产品信息")),
            //                     ("以前一站内容作为产品信息", () => Ref_Step("判断产品状态")),
            //                     ("作为NG品", () =>
            //                     {
            //                         if (CurrentProduct != null)
            //                         {
            //                             CurrentProduct.Result = ProductInfo.ResultNG;
            //                         }
            //                         Ref_Step("判断产品状态");
            //                     }
            //            )
            //                     );
            //            break;

            //        case "RFID读取失败处理":
            //            WriteErrToUser($"{Name}:读取RFID失败，失败原因:{error}");
            //            App.Current.Dispatcher.Invoke(() =>
            //            {
            //                RFIDReadErrorWnd errorWnd = new RFIDReadErrorWnd();
            //                errorWnd.ShowAndReturn(this.Name, error,
            //                    () =>
            //                    {
            //                        CylinderUp?.OriginPos(out _, false);
            //                        Ref_Step("等待载具进入工作位");
            //                    },
            //                    () =>
            //                    {
            //                        rfidInfo = errorWnd.ManualInputContent;
            //                        ReportMsg("手动输入RFID内容:" + rfidInfo);
            //                        Ref_Step("根据RFID内容设置产品信息");
            //                    },
            //                    () =>
            //                    {
            //                        Ref_Step("读取RFID");
            //                    });
            //            });

            //            break;

            //        case "根据RFID内容设置产品信息":
            //            CurrentProduct = ProductInfoConverter.GetProductInfoClass(this, SampleProductInfo.GetProductInfo(rfidInfo));
            //            if (GlobalEquipmentInfo.IsFirstAAStation && this.IsFirstStationWhenSingleRun())
            //            {

            //            }
            //            Ref_Step("判断产品信息格式是否正确");
            //            break;

            //        case "判断产品信息格式是否正确":
            //            if (CurrentProduct != null)
            //            {
            //                if (CurrentProduct.IsVerifySuccess == false)
            //                {
            //                    CurrentProduct.Result = ProductInfo.ResultNG;
            //                    Ref_Step("判断下一站是否Ready");
            //                    WriteErrToUser("产品信息格式不正确，此产品将作为NG品");
            //                    break;
            //                }
            //            }
            //            Ref_Step("读取RFID治具编号");
            //            break;

            //        case "读取RFID治具编号":
            //            if (!Funs.FunSelection.Cur.IsUseRFID)
            //            {
            //                ReportMsg("RFID未启用");
            //                Ref_Step("判断产品状态");
            //                break;
            //            }
            //            if (this.CurrentStationHaveRFID())
            //            {
            //                if (this.RFID.ReadRFIDNumber(out tempRfid, out error))
            //                {
            //                    ReportMsg("RFID读取治具编号:" + tempRfid);
            //                    if (CurrentProduct != null)
            //                    {
            //                        CurrentProduct.RFID = tempRfid;
            //                    }
            //                    Ref_Step("判断产品状态");
            //                }
            //                else
            //                {
            //                    WriteErrToUser($"{Name}:读取RFID失败，失败原因:{error}");
            //                }
            //            }
            //            else
            //            {
            //                ReportMsg("此站无RFID");
            //                Ref_Step("判断产品状态");
            //            }
            //            break;


            //        case "判断产品状态":
            //            if (CurrentProduct != null)
            //            {
            //                if (GlobalEquipmentInfo.IsFirstAAStation && IsFirstStationWhenSingleRun())
            //                {
            //                    if (CurrentProduct.Result == ProductInfo.ResultNG)
            //                    {
            //                        CurrentProduct.IsLastStationNG = true;
            //                    }
            //                }
            //            }
            //            if (IsDoubleLevelCylinder)
            //            {
            //                if (FrontNextConnect.IsRunning)
            //                {
            //                    ReportMsg($"本站上层有产品，产品流走下一站");
            //                    Ref_Step("判断下一站是否Ready");
            //                    break;
            //                }
            //            }

            //            if (CurrentProduct != null)
            //            {
            //                ReportMsg($"产品SN:{CurrentProduct?.SN}");

            //                if (CurrentWorkMode.IsConnectWithBackStation())
            //                {
            //                    if (CurrentProduct.IsDone)
            //                    {
            //                        ReportMsg($"产品SN:{CurrentProduct.SN}已经做过，不用在此站生产");
            //                        Sleep(1000);
            //                        Ref_Step("判断下一站是否Ready");
            //                        break;
            //                    }
            //                }
            //                if (CurrentProduct.Result == ProductInfo.ResultNG)
            //                {
            //                    if (IsContinueIfProductNG())
            //                    {
            //                        ReportMsg("产品NG,但允许继续运行本站流程");

            //                        Ref_Step("判断RFID工序");
            //                    }
            //                    else
            //                    {
            //                        ReportMsg("产品NG，跳过本站");
            //                        Sleep(1000);
            //                        Ref_Step("MES过站");
            //                    }

            //                }
            //                else
            //                {
            //                    if (Fun_StationIsUse())
            //                    {
            //                        if (FrontNextConnect.IsProcessStation)
            //                        {
            //                            CurrentProduct.IsDone = true;
            //                        }
            //                        Ref_Step("判断RFID工序");
            //                    }
            //                    else
            //                    {
            //                        if (!CurrentWorkMode.IsConnectWithBackStation() && IsLastStation)
            //                        {
            //                            Ref_Step("单机模式下设备停止");
            //                            break;
            //                        }
            //                        CurrentProduct.GetProductTestStationByName(this.Name).IsUse = false;

            //                        ReportMsg("本站未启用，跳过本站");
            //                        Sleep(1000);

            //                        Ref_Step("判断下一站是否Ready");
            //                    }
            //                    App.Current.Dispatcher.Invoke(() =>
            //                    {
            //                        Add(CurrentProduct);//加入产品队列
            //                    });
            //                }
            //            }
            //            else
            //            {
            //                ReportMsg($"产品为空");
            //                Sleep(1000);

            //                Ref_Step("判断下一站是否Ready");
            //            }

            //            break;

            //        case "判断RFID工序":
            //            if (Funs.FunSelection.Cur.IsUseRFID && Funs.FunRFID.Cur.IsCheckRFIDProcess)
            //            {
            //                if (CurrentProduct != null)
            //                {
            //                    if (CurrentProduct.LastProcessCode != this.GetLastProcessCodeFunc())
            //                    {
            //                        WriteErrToUser($"读取RFID上一段工序:{CurrentProduct.LastProcessCode},当前工站设置上一段工序:{this.GetLastProcessCodeFunc()},工序不匹配，跳过本站");
            //                        Sleep(1000);
            //                        Ref_Step("判断下一站是否Ready");
            //                        break;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                ReportMsg("RFID工序比对未启用");
            //            }
            //            Ref_Step("MES检查SN");
            //            break;

            //        case "MES检查SN":

            //            string sn = CurrentProduct?.SN;
            //            if (WorkModeManager.Cur.SelectWorkMode.Fun_IsUseMES())
            //            {
            //                isMesOK = false;
            //                mesContent = string.Empty;
            //                try
            //                {
            //                    if (MES.MESHelper.Cur.CheckSN(sn, out error) == true)
            //                    {
            //                        this.WriteToUser($"MES检查SN成功，{sn}条码对应产品允许在本站作业",
            //                           System.Windows.Media.Brushes.Green, true);
            //                        Ref_Step("调用功能块");
            //                        isMesOK = true;
            //                    }
            //                    else
            //                    {
            //                        mesContent = $"MES检查SN失败，条码:{sn},失败原因：{error}";
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    mesContent = $"MES检查SN时发生异常：{ex.Message}";
            //                }
            //                if (isMesOK == false)
            //                {
            //                    if (Funs.FunMES.Cur.IsCheckSNFailSetProductResultFail)
            //                    {
            //                        if (CurrentProduct != null)
            //                        {
            //                            CurrentProduct.Result = ProductInfo.ResultNG;
            //                        }
            //                        Ref_Step("调用功能块");
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        ErrorDialog("MES检查SN失败:" + error,
            //                      ("重新检查", () => Ref_Step("MES检查SN")),
            //                      ("作为NG品", () =>
            //                      {
            //                          if (CurrentProduct != null)
            //                          {
            //                              CurrentProduct.Result = ProductInfo.ResultNG;
            //                          }
            //                          Ref_Step("调用功能块");
            //                      }
            //                        )
            //                      );
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                ReportMsg("当前未启用MES");
            //                Ref_Step("调用功能块");
            //            }
            //            break;

            //        case "调用功能块":
            //            if (IsDoubleLevelCylinder)
            //            {
            //                FrontNextConnect.IsRunning = true;
            //                if (WaitIO(StaticIOHelper.CyMovingStationUpDown, CylinderActionType.WorkPos))
            //                {
            //                    FrontNextConnect.UnLock();
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            Action();
            //            if (!CurrentWorkMode.IsConnectWithBackStation() && IsLastStation)
            //            {
            //                StaticIOHelper.Fun_UpLineStop();
            //            }
            //            if (!IsProcessRunning)
            //            {
            //                ReportMsg("IsProcessRunning标志位false，流程退出");
            //                break;
            //            }


            //            //if (IsFirstStationAndDoOnce())
            //            //{
            //            //    ReportMsg("流程只执行一次，流程退出");
            //            //    return;
            //            //}
            //            else
            //            {
            //                Ref_Step("判断是否是双层气缸");
            //            }
            //            break;

            //        case "MES过站":
            //            if (WorkModeManager.Cur.SelectWorkMode.Fun_IsUseMES() == false)
            //            {
            //                ReportMsg("当前未启用MES");
            //                Ref_Step("判断下一站是否Ready");
            //                break;
            //            }
            //            if (CurrentProduct == null)
            //            {
            //                ReportMsg("当前产品为空");
            //                Ref_Step("判断下一站是否Ready");
            //                break;
            //            }
            //            if (IsLastStation == false)
            //            {
            //                ReportMsg("当前不是当前站最后工位，无需上传MES");
            //                Ref_Step("判断下一站是否Ready");
            //                break;
            //            }
            //            if (CurrentProduct.Result == ProductInfo.ResultNG && CurrentProduct.IsLastStationNG)
            //            {
            //                ReportMsg("产品是上一站流入NG品，无需上传MES");
            //                Ref_Step("判断下一站是否Ready");
            //                break;
            //            }
            //            isMesOK = false;
            //            mesContent = string.Empty;
            //            try
            //            {
            //                if (MES.MESHelper.Cur.DataUp(CurrentProduct, out error) == true)
            //                {
            //                    this.WriteToUser($"MES过站成功，{CurrentProduct.SN}条码过站成功",
            //                       System.Windows.Media.Brushes.Green, true);
            //                    isMesOK = true;
            //                    Ref_Step("判断下一站是否Ready");
            //                }
            //                else
            //                {
            //                    mesContent = $"MES检查SN失败，条码:{CurrentProduct.SN},失败原因：{error}";
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                mesContent = $"MES检查SN时发生异常：{ex.Message}";
            //            }
            //            if (isMesOK == false)
            //            {
            //                if (Funs.FunMES.Cur.IsDataUpFailSetProductResultFail)
            //                {
            //                    CurrentProduct.Result = ProductInfo.ResultNG;
            //                    Ref_Step("判断下一站是否Ready");
            //                    break;
            //                }
            //                else
            //                {
            //                    ErrorDialog($"条码:{CurrentProduct.SN}MES过站失败:" + error,
            //                  ("重新检查", () => Ref_Step("MES过站")),
            //                  ("作为NG品", () =>
            //                  {
            //                      CurrentProduct.Result = ProductInfo.ResultNG;
            //                      Ref_Step("判断下一站是否Ready");
            //                  }
            //                    )
            //                  );
            //                }
            //            }

            //            break;

            //        case "判断是否是双层气缸":
            //            if (IsDoubleLevelCylinder)
            //            {
            //                Ref_Step("设置本站允许流入");
            //            }
            //            else
            //            {
            //                Ref_Step("判断下一站是否Ready");
            //            }

            //            break;

            //        case "判断下一站是否Ready":
            //            if (IsLastStation)
            //            {
            //                Ref_Step("生产统计处理");
            //            }
            //            else
            //            {
            //                Ref_Step("治具RFID写入");
            //            }

            //            break;

            //        case "生产统计处理":
            //            if (CurrentProduct == null)
            //            {
            //                Ref_Step("治具RFID写入");
            //                break;
            //            }
            //            var content1 = $"产品SN:{CurrentProduct.SN};{CurrentProduct.SN};产品结果:{CurrentProduct.Result};产品若NG，NG原因:{CurrentProduct.FailedMsg}";
            //            ReportMsg(content1);
            //            //AviewMotionUI.ProductionStatistics.ProductResult resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.OK;
            //            //if (CurrentProduct.Result == ProductInfo.ResultOK)
            //            //{
            //            //    resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.OK;
            //            //}
            //            //else
            //            //{
            //            //    resultEnum = AviewMotionUI.ProductionStatistics.ProductResult.NG;
            //            //}
            //            //App.Current.Dispatcher.Invoke(() =>
            //            //{
            //            //    try
            //            //    {
            //            //        AviewMotionUI.ProductionStatistics.ProductionStatisticsManger.Current.AddCountOnce(resultEnum,
            //            //                 CurrentProduct.RFID, ProductConverter.GetProductStationResultItems(CurrentProduct).ToArray());
            //            //    }
            //            //    catch (Exception ex)
            //            //    {
            //            //        WriteErrToUser("生产统计更新异常:" + ex.Message);
            //            //    }

            //            //});
            //            Ref_Step("治具RFID写入");
            //            break;

            //        case "治具RFID写入":
            //            if (!Funs.FunSelection.Cur.IsUseRFID)
            //            {
            //                ReportMsg("RFID未启用，直接流走");
            //                Ref_Step("判断下一站是否Ready2");
            //                break;
            //            }
            //            rfidInfo = SampleProductInfo.GetStringFromProductInfo(ProductInfoConverter.GetSampleProductInfoClass(this, CurrentProduct));
            //            ReportMsg("写入RFID:" + rfidInfo);
            //            if (this.RFID.WriteData(rfidInfo, out error))
            //            {
            //                Ref_Step("判断下一站是否Ready2");
            //            }
            //            else
            //            {
            //                WriteErrToUser($"{Name}:写入RFID失败:{error}，写入内容:" + rfidInfo);
            //            }

            //            break;
            //        case "判断下一站是否Ready2":
            //            if (!CurrentWorkMode.IsConnectWithBackStation() && IsLastStation)
            //            {
            //                Ref_Step("气缸缩回");
            //            }
            //            else
            //            {

            //                bool isDone = false;
            //                if (CurrentProduct == null)
            //                {
            //                    isDone = true;
            //                }
            //                else
            //                {
            //                    isDone = CurrentProduct.IsDone;
            //                }
            //                result = FrontNextConnect.Ask(isDone);
            //                if (result)
            //                {
            //                    Ref_Step("设置产品信息");
            //                }
            //            }


            //            break;

            //        case "设置产品信息":
            //            string infoMsg = string.Empty;
            //            if (GlobalEquipmentInfo.IsLastStation && this.IsLastStation)
            //            {
            //                ReportMsg("AA3设备并且是最后工位");
            //                Platform.CommunicationProductInfo communicationProductInfo = new Platform.CommunicationProductInfo();
            //                communicationProductInfo = ProductInfoConverter.GetCommunicationProductInfoClass(this, CurrentProduct);
            //                infoMsg = Newtonsoft.Json.JsonConvert.SerializeObject(communicationProductInfo); ;
            //            }
            //            else
            //            {
            //                Platform.SampleProductInfo communicationProductInfo = new Platform.SampleProductInfo();
            //                communicationProductInfo = ProductInfoConverter.GetSampleProductInfoClass(this, CurrentProduct);
            //                infoMsg = SampleProductInfo.GetStringFromProductInfo(communicationProductInfo);
            //            }

            //            ReportMsg("向下一站发送产品信息:" + infoMsg);
            //            result = FrontNextConnect.SetProductInfo(infoMsg);
            //            if (result == true)
            //            {
            //                Ref_Step("气缸缩回");
            //            }
            //            break;


            //        case "气缸缩回":
                      
            //            CylinderOriginPos(false, out _);
            //            Sleep(500);
            //            if (this.CheckSignalLoop(() => !this.ProductFlowOnPosSignalEventHandler.Invoke(),
            //                "{Name}治具流向下一站超时", 10000) == true)
            //            {
            //                Sleep(1500);
            //                Ref_Step("挡停气缸上升");
            //            }
            //            break;

            //        case "挡停气缸上升":
            //            if (CylinderStop.OriginPos(out errorCylinder))
            //            {
            //                if (isCarrierExist)
            //                {
            //                    Ref_Step("判断顶升气缸状态");
            //                }
            //                IsReady = true;
            //                Ref_Step("设置本站允许流入");
            //            }
            //            else
            //            {
            //                WriteErrToUser(errorCylinder);
            //            }
            //            break;


            //        case "单机模式下设备停止":
            //            StaticInitial.Motion.CommitStopOrder();
            //            IsProcessRunning = false;
            //            break;


            //        default:
            //            throw new Exception($"{Name}:当前步骤不存在，当前步骤：{Step}");
            //    }
            //    Sleep(10);
            //}
        }






    }
}
