using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RM_dll2;
using System.ComponentModel;
using Handler.Motion;
using Handler.Product;
using Handler.Process.RunMode;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Handler.Connect.RFID;

namespace Handler.Process.Station
{
    public abstract class StationBase : AM.Core.Process.ProcessIPauseBaseExtend, INotifyPropertyChanged
    {

        public StationBase(string name) : base(name, StaticInitial.Motion, true)
        {
        }

        public StationBase(string name, bool IsCreateLog = false) : base(name, StaticInitial.Motion, IsCreateLog)
        {
        }

        /// <summary>
        /// 工序代号
        /// </summary>
        public Func<string> GetProcessCodeFunc { get; set; }
        public Func<string> GetLastProcessCodeFunc { get; set; }
        /// <summary>
        /// 当前工位是否允许治具流入
        /// </summary>
        public bool IsReady { get; set; }
        /// <summary>
        /// 当前工位是否应该有治具流入
        /// </summary>
        public bool IsLoad { get; set; }

        /// <summary>
        /// 治具到位，用来测试
        /// </summary>
        public bool IsAtPosTest { get; set; }

        /// <summary>
        /// 产品是否OK
        /// </summary>
        public bool IsProductOK { get; set; }

        /// <summary>
        /// 是否检测本站状态，如下流水线允许新增治具，那么下流水线检测到治具就不用判断是否是从上一站流过来的
        /// </summary>
        public bool CheckCurrentStationState { get; set; } = true;

        /// <summary>
        /// 产品到位信号：传感器信号
        /// </summary>
        public Func<bool> ProductFlowOnPosSignalEventHandler;
        /// <summary>
        /// 产品离开信号：传感器信号
        /// </summary>
        public Func<bool> ProductLeaveSignalEventHandler;
        /// <summary>
        /// 判断下一站Ready额外的方法
        /// </summary>
        public Func<(bool, string)> CheckNextStationReadyAdditionalEventHandler;

        /// <summary>
        /// 流水线正转
        /// </summary>
        public Action LineForwardEventHandler;
        /// <summary>
        /// 流水线反转
        /// </summary>
        public Action LineReversalEventHandler;
        /// <summary>
        /// 流水线停止
        /// </summary>
        public Action LineStopEventHandler;

        /// <summary>
        /// 托盘到位延时
        /// </summary>
        public Action TrayOnPosDelayEventHandler;

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 当前产品信息集合
        /// </summary>
        public static ObservableCollection<ProductInfo> ProductInfoList = new ObservableCollection<ProductInfo>();



        public override void Stop()
        {
            base.Stop();
            CheckActionHelper.Stop();
        }

        /// <summary>
        /// 当前测试的产品
        /// </summary>
        public ProductInfo CurrentProduct { get; set; }



        //public StationBase NextStation
        //{
        //    get
        //    {
        //        var item = StationManger.Cur.StationList.Find(this);
        //        if (item.Next == null)
        //        {
        //            return StationManger.Cur.StationList.First.Value;
        //        }
        //        else
        //        {
        //            return item.Next.Value;
        //        }
        //    }
        //}
        /// <summary>
        /// 当前站是否启用
        /// </summary>
        /// <returns></returns>
        public virtual bool Fun_StationIsUse()
        {
            return true;
        }



        /// <summary>
        /// 装载产品
        /// </summary>
        /// <param name="product"></param>
        public void LoadProduct(ProductInfo product)
        {
            this.CurrentProduct = product;
            IsLoad = true;
        }


        /// <summary>
        /// 清楚产品信息
        /// </summary>
        public void ClearProduct()
        {
            this.CurrentProduct = null;
            IsLoad = false;
        }

        protected virtual bool CheckProductFlowOnPos()
        {
            if (ProductFlowOnPosSignalEventHandler.Invoke())
            {
                TrayOnPosDelayEventHandler?.Invoke();
                if (ProductFlowOnPosSignalEventHandler.Invoke())
                {
                    return true;
                }
            }
            return false;
        }

 
        /// <summary>
        /// 读取RFid的方法
        /// </summary>
        /// <returns></returns>
        public virtual string ReadRFID()
        {
            return "";
        }

        public virtual bool CurrentStationHaveRFID()
        {
            return false;
        }

        /// <summary>
        /// 读取RFID
        /// </summary>
        /// <returns></returns>
        public string Fun_StationReadRFID()
        {
            string temp = "";
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        temp = "";
                        temp = ReadRFID();
                    }
                    catch
                    {
                        Sleep(100);
                        temp = "";
                    }
                }
                if (!string.IsNullOrEmpty(temp))
                {
                    return temp;
                }
                else
                {
                    throw new Exception("RFID读取失败!");
                }

            }
            catch (Exception ex)
            {
                WriteErrToUser(ex.Message);
                //弹出画面
                bool currentWndIsClose = false;

                App.Current.Dispatcher.Invoke(() =>
                {
                    RFIDTestDemo.Wnd_RFID_ReadFailedView wnd_RFID_ReadFailedView = new RFIDTestDemo.Wnd_RFID_ReadFailedView();
                    wnd_RFID_ReadFailedView.Owner = App.Current.MainWindow;
                    wnd_RFID_ReadFailedView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                    wnd_RFID_ReadFailedView.Topmost = true;
                    wnd_RFID_ReadFailedView.Title = $"{Name} RFID 读取失败";
                    wnd_RFID_ReadFailedView.funcReadRFIDEventHandler += ReadRFID;
                    wnd_RFID_ReadFailedView.Closed += (e, r) =>
                    {
                        Motion.CommitClearErrOrder();
                        Sleep(100);
                        int i_curent = 0;
                        while (Motion.CurOrder.IsErr)
                        {
                            Sleep(100);
                            if (i_curent > 30)
                            {
                                break;
                            }
                            i_curent++;
                        }
                        if (IsProcessRunning)
                        {
                            Motion.CommitStartRunOrder();

                        }
                        currentWndIsClose = true;
                    };
                    wnd_RFID_ReadFailedView.actionGetRFIDEventHandler += s =>
                    {
                        temp = s;

                    };
                    wnd_RFID_ReadFailedView.Show();
                    wnd_RFID_ReadFailedView.Topmost = false;

                });


                while (true)
                {
                    if (currentWndIsClose || !IsProcessRunning)
                    {
                        break;
                    }
                    Sleep(100);
                }

                return temp;
            }
        }

        public static void Add(ProductInfo productInfo)
        {
            if (productInfo == null) return;
            App.Current.Dispatcher.Invoke(() =>
            {
                Remove(productInfo.RFID);
                ProductInfoList.Add(productInfo);
            });
        }

        static readonly object ob_lock = new object();
        //public static void Clear()
        //{
        //    lock (ob_lock)
        //    {
        //        ProductInfoList.Clear();
        //    }
        //}

        public static void Remove(ProductInfo productInfo)
        {
            if (productInfo == null) return;
            lock (ob_lock)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (ProductInfoList.Contains(productInfo))
                    {
                        ProductInfoList.Remove(productInfo);
                    }
                });

            }
        }

        public static void Remove(string rfid)
        {

            lock (ob_lock)
            {
                foreach (var item in ProductInfoList)
                {
                    if (item.RFID == rfid)
                    {
                        ProductInfoList.Remove(item);
                        return;
                    }
                }
            }
        }

        public static ProductInfo FindProductBaseRFID(string rfid)
        {
            lock (ob_lock)
            {
                foreach (var item in ProductInfoList)
                {
                    if (item.RFID == rfid)
                    {

                        return item;
                    }
                }
            }
            return null;
        }

        public static bool CheckAnyProcessIsPause()
        {
            for (int i = 0; i < RM_dll2.Process.ProcessBase.processBasesList.Count; i++)
            {
                if (RM_dll2.Process.ProcessBase.processBasesList[i] is RM_dll2.Process.ProcessIPauseBase process)
                {
                    if (process.CheckPause())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
