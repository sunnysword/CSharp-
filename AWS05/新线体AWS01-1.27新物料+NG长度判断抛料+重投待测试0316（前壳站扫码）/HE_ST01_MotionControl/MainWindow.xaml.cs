using Handler.Connect;
using Handler.Connect.Camera;
using Handler.Motion;
using Handler.Motion.Axis;
using Handler.Motion.IO;
using Handler.Motion.Process;
using Handler.Motion.Process.Station;
using Handler.Params;
using Handler.Process;
using Handler.Process.Station;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Product;
using Handler.View;
using HE_ST01_MotionControl.OtherHelpers;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;
using LanCustomControldll;
using RM_dll2;
using RM_dll2.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThreadInfoRecorddll;

namespace Handler
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //LanguageControlBase.Wpf.BindableObject.SetSafeRefUIForonPropertyChanged(s =>
            //{
            //    this.Dispatcher.BeginInvoke(new Action(()=>s?.Invoke()));
            //});
            InitializeComponent();

            App.Current.MainWindow = this;

            uc_ErrLog.SetMaxListMsgNum(3);
            //uc_ProductInfo.SetMaxListMsgNum(3);

            uc_ErrLog.SetHeader("错误信息");
            //uc_ProductInfo.SetHeader("产品信息");

            Thread.CurrentThread.Name = $"{Thread.CurrentThread.ManagedThreadId} UI_Dispatcher";
            ThreadInfoRecord.WriteInfo();

            AddLog();

            Wpf_LoginWnd.UserSetting.Cur.RegisterMenu(Menu_Main.TopMenu);

        }

        private readonly WndStartPositionHelper wndStartPositionHelper = new WndStartPositionHelper();
        private bool IsExit = false;

        #region 流程信息框

        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_main = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log1 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log2 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log3 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log4 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log5 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log6 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();

        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log7 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log8 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log9 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log10 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();
        private LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log11 = new LanCustomControldll.Unit_ListMsg.UC_ListMsg();

        private void AddLog()
        {
            void TempAddLog(string name, LanCustomControldll.Unit_ListMsg.UC_ListMsg uC_ListMsg)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = name;
                uC_ListMsg.SetMaxListMsgNum(10);
                uC_ListMsg.SetHeader("");
                tabItem.Content = uC_ListMsg;

                tb_ProcessList.Items.Add(tabItem);
            }
            TempAddLog("主流程", uc_main);
            TempAddLog("外壳三轴抓取", uc_log1);
            TempAddLog("PCB三轴抓取", uc_log2);
            TempAddLog("外壳Tray料仓", uc_log3);
            TempAddLog("PCBTray料仓", uc_log4);
            TempAddLog("回流流入站", uc_log5);
            TempAddLog("回流流出站", uc_log6);
            TempAddLog("回流移栽站", uc_log7);
            TempAddLog("外壳站", uc_log8);
            TempAddLog("外壳缓存站", uc_log9);
            TempAddLog("PCB站", uc_log10);
            TempAddLog("PCB缓存站", uc_log11);


        }

        private void LogRegister()
        {
            //log待加，todo
            #region 注册流程日志

            addLog(uc_log1, Process_前壳Tray抓取.Cur);
            addLog(uc_log2, Process_PCBTray抓取.Cur);
            addLog(uc_log3, PalletManger.Process_前壳);
            addLog(uc_log4, PalletManger.Process_PCB);
            addLog(uc_log5, Process_回流流入站.Cur);
            addLog(uc_log6, Process_回流流出站.Cur);
            addLog(uc_log7, Process_回流移栽站.Cur);
            addLog(uc_log8, Process_前壳站.Cur);
            addLog(uc_log9, Process_前壳缓存站.Cur);
            addLog(uc_log10, Process_PCB站.Cur);
            addLog(uc_log10, Process_PCB缓存站.Cur);

            var uc_main_log = uc_main;

            Process_ResMachine.Cur.WriteToUserEvent += uc_main_log.WriteToUser;

            Motion.StaticInitial.Motion.LanWriteToUserEvent += uc_main_log.WriteToUser;
            Motion.StaticInitial.Motion.WriteToUserEvent += uc_main_log.WriteToUser;

            RM_dll2.AllErrInfoRecord.WriteErrToUserEvent += s => uc_ErrLog.LanWriteToUser(s, Brushes.Red, false);
            StaticInitial.Motion.actionClearErrEventHandler += uc_ErrLog.ClearMsg;

            //CameraManager.CameraMoving.WriteToUserEventHandler += SubMovingStationUpLevel.Cur.WriteToUser;
            //Process_Assembling.Cur.WriteToUserEvent += uc_log2.WriteToUser;
            //Process_LenFeeding.Cur.WriteToUserEvent += uc_log1.WriteToUser;
            #endregion 注册流程日志
        }
        #endregion 流程信息框

        private void ShowUI()
        {
            Thread.CurrentThread.Name = $"{Thread.CurrentThread.ManagedThreadId} MainWindow_ShowUI";
            ThreadInfoRecord.WriteInfo();
            MotionWndViewModelHelper motionWindowViewModel = MotionWndViewModelHelper.CreateInstrance();

            while (true)
            {
                if (IsExit)
                {
                    return;
                }
                this.Dispatcher.Invoke(() =>
                {
                    //tb_MachineState.Text = Motion.StaticInitial.Motion.MotionStateInfo.MotionState;
                    //Border_MachineBackClolor.Background = Motion.StaticInitial.Motion.MotionStateInfo.MotionStateColor;
                    Main_TopPart.ShowUI();
                    Main_rightPart.ShowUI();
                    uc_link.ShowUI();
                    tb_NoticeBar.Text = "";
                    if (RM_dll2.Process.ProcessBase.Fun_IsAnyProcessRun())
                    {
                        tb_NoticeBar.Text = "【功能块运行中...】|";
                    }
                    if (PalletManger.Process_前壳.IsWarning || PalletManger.Process_PCB.IsWarning)
                    {
                        tb_MachineState.LanText = "料仓缺料请上料！";
                        Border_MachineBackClolor.Background = Brushes.Yellow;
                    }
                    else
                    {
                        tb_MachineState.LanText = Motion.StaticInitial.Motion.MotionStateInfo.MotionState;
                        Border_MachineBackClolor.Background = Motion.StaticInitial.Motion.MotionStateInfo.MotionStateColor;
                    }

                    tb_NoticeBar.Text += RM_dll2.ExtraMsgHelper.GetAllMsg();
                    motionWindowViewModel.ShowUI();
                    //textBoxNGIndex.Text = ParamsNGTray.Cur.ScanPCBNgNum.ToString();
                    viewLife.ShowUI();

                    //重新调用刷新UI 
                    //ShowStationLimit();

                    //todo 料仓料盘计数待测试
                    ShowTrayNumber();

                    //主界面刷新
                    tb_ShellRFID.Text = Process_前壳站.Cur.CurrentProduct?.RFID;
                    if (Process_前壳站.Cur.CurrentProduct != null)
                    {
                        var timeoff = (Process_前壳站.Cur.CurrentProduct.EndTimeForCT - Process_前壳站.Cur.CurrentProduct.StartTimeForCT).TotalMilliseconds / 1000;
                        timeoff = timeoff < 0 ? 0 : timeoff;
                        tb_ShellCT.Text = Process_前壳站.Cur.CurrentProduct != null ? timeoff.ToString() : "";
                    }


                    tb_PCBBarcode.Text = Process_PCB站.Cur.CurrentProduct?.PCBSN;
                    tb_PCBBarcodeFormate.Text = Process_PCB站.Cur.CurrentProduct?.SN;
                    tb_PCBRFID.Text = Process_PCB站.Cur.CurrentProduct?.RFID;
                    if (Process_PCB站.Cur.CurrentProduct != null)
                    {
                        var timeoff = (Process_PCB站.Cur.CurrentProduct.EndTimeForCT - Process_PCB站.Cur.CurrentProduct.StartTimeForCT).TotalMilliseconds / 1000;
                        timeoff = timeoff < 0 ? 0 : timeoff;
                        tb_PCBCT.Text = Process_PCB站.Cur.CurrentProduct != null ? (timeoff.ToString()) : "";
                    }

                    tb_Check.Text = Funs.FunMESCheck.Cur.CheckHeader.GetValue;
                    tb_Version.Text = Funs.FunMESCheck.Cur.SnVersion.GetValue;

                });
                Thread.Sleep(100);

            }
        }

        /// <summary>
        /// 料仓层数、料盘取料计数、NG数显示
        /// </summary>
        public void ShowTrayNumber()
        {
            //读料仓文件有可能两线程会抢资源，可以加锁
            textBlockHolderLayers.Text = (PalletManger.前壳Trayer.Index + 1).ToString();//料仓层数
            textBoxHolderIndex.Text = Process_前壳Tray抓取.Cur.Pallet.Index.ToString();
            textBlockPCBLayers.Text = (PalletManger.PCBTrayer.Index + 1).ToString();
            textBoxPCBIndex.Text = Process_PCBTray抓取.Cur.Pallet.Index.ToString();
            textBoxNGIndex.Text = Process_PCBTray抓取.Cur.NGPallet.Index.ToString();//暂定只有扫码NG才会丢NG料盘
        }

        public void ShowStationLimit()
        {
            void SetTextBlock(TextBlock text, int index)
            {
                if (Funs.FunSocketSetting.Cur.SocketIsUseArray[index].GetValue)
                {
                    text.Text = "启用中";
                    text.Background = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    text.Text = "禁用中";
                    text.Background = System.Windows.Media.Brushes.Gray;
                }
                Funs.FunSocketSetting.Cur.SocketIsUseArray[index].PropertyValueMustChanged += () =>
                {
                    if (Funs.FunSocketSetting.Cur.SocketIsUseArray[index].GetValue)
                    {
                        text.Text = "启用中";
                        text.Background = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        text.Text = "禁用中";
                        text.Background = System.Windows.Media.Brushes.Gray;
                    }
                };
            }
            void SetTextBlock2(TextBlock text, int index)
            {
                if (Funs.FunSocketSetting.Cur.StationSensorIsUseArray[index].GetValue)
                {
                    text.Text = "启用中";
                    text.Background = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    text.Text = "禁用中";
                    text.Background = System.Windows.Media.Brushes.Gray;
                }
                Funs.FunSocketSetting.Cur.StationSensorIsUseArray[index].PropertyValueMustChanged += () =>
                {
                    if (Funs.FunSocketSetting.Cur.StationSensorIsUseArray[index].GetValue)
                    {
                        text.Text = "启用中";
                        text.Background = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        text.Text = "禁用中";
                        text.Background = System.Windows.Media.Brushes.Gray;
                    }
                };
            }
            SetTextBlock(this.textBlockSocketAState, 0);
            SetTextBlock(this.textBlockSocketBState, 1);
            SetTextBlock(this.textBlockSocketCState, 2);
            SetTextBlock(this.textBlockSocketDState, 3);
            SetTextBlock(this.textBlockSocketEState, 4);
            SetTextBlock(this.textBlockSocketFState, 5);
            SetTextBlock2(this.textBlockSite1State, 0);
            SetTextBlock2(this.textBlockSite2State, 1);
            SetTextBlock2(this.textBlockSite3State, 2);
            SetTextBlock2(this.textBlockSite4State, 3);
            SetTextBlock2(this.textBlockSite5State, 4);
            SetTextBlock2(this.textBlockSite6State, 5);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                wndStartPositionHelper.SaveWndCurrentPosition(this);
                IsExit = true;
                StaticInitial.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                App.Current.Shutdown(0);
                Environment.Exit(0);
            }
        }

        private async void MotionInitial()
        {
            Motion.CardInital.Initial();
            HE_ST01_MotionControl.Properties.Settings.Default.NeedInitial = true;
            await Task.Run(() =>
            {
                try
                {
                    Motion.CardInital.CardInit();
                    Motion.StaticInitial.Initial();

                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (Wnd_ProcessBar.Cur != null)
                        {
                        }
                        Wnd_ProcessBar.SingleClose();
                        MessageBox.Show(AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                    });
                }
            });

            if (Wnd_ProcessBar.Cur != null)
            {
            }

            Wnd_ProcessBar.SingleClose();
            this.IsEnabled = true;

            LogRegister();

            AM.Communication.ConnectUIBuilder.SetUIWithConnect(Menu_Main.menu_ConSettings, AM.Communication.ConnectBuilder.ConnectInfoList);

            StationManger.Cur.Register();//子程序加载进来
            MotionWndViewModelHelper motionWindowViewModel = MotionWndViewModelHelper.CreateInstrance();
            Thread thread = new Thread(ShowUI);

            thread.IsBackground = true;
            thread.Start();
            if (Motion.CardInital.card.IsCardReady)
            {
                Motion.CardInital.card.InitialSucessedInvoke();
                AxisCreateAbsEncoderBase.SetAllMotorSoftLimit();
            }
            ////打开控制卡
            //读取所有参数
            ISaveReaddll.SaveReadManagerHelper.Fun_ReadAll();

            StaticInitial.Motion.Initial();

            //初始化RM
            this.unit_ProductInfoList.Dg_Msg.ItemsSource = WorkStationBase.ProductInfoList;
            //Process_RotateAxis.Cur.FlushDataEventHandler += viewResultList.FlushData;
            //ShowStationLimit();
            uc_link.SetViewBinding();

            //初始计数top3
            //ParamsNGTray.Cur.FlushNGInfo +=s=> this.Main_TopPart.uc_Product.FlushNGInfo(s);

        }


        private void addLog(LanCustomControldll.Unit_ListMsg.UC_ListMsg uc_log, ProcessBase processGrounFirst, params ProcessBase[] processBases)
        {
            processGrounFirst.WriteToUserEvent += uc_log.WriteToUser;

            foreach (var item in processBases)
            {
                item.WriteToUserEvent += processGrounFirst.WriteToUser;
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                wndStartPositionHelper.SetWndStartPosition(this);
                Wnd_ProcessBar.SingleShow();
                Motion.CardInital.ReportInfoProcessBarEvent += Wnd_ProcessBar.CreateInstrance().WriteProcessLog;

                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                var versionDescriptions = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false);
                string description = ((System.Reflection.AssemblyDescriptionAttribute)versionDescriptions[0]).Description;
                this.Title = $"德赛上料站" + "    版本:" + version + "   版本日期:" + description;//修改名字
                MotionInitial();
                Main_TopPart.uc_ngTop.Set();
            }
            catch (Exception ex)
            {
                MessageBox.Show(AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
            }
        }

        //public void WriteProductInfo(string info, Brush brush)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        uc_ProductInfo.WriteToUser(info, brush, true);
        //    });
        //}

        //public void ClearSN()
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        Main_rightPart.tb_SN.Text = "";
        //    });
        //}

        //void ReadSN(string sn)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        Main_rightPart.tb_SN.Text = sn;
        //    });
        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认关闭软件吗？", "请确认", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.A))
            {
                AdminTestWindow window = new AdminTestWindow();
                window.Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("请确认清除壳体料盘计数吗", "确认", MessageBoxButton.YesNo))
            {
                return;
            }
            else
            {
                Process_前壳Tray抓取.Cur.Pallet.Clear();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("请确认清除PCB料盘计数吗", "确认", MessageBoxButton.YesNo))
            {
                return;
            }
            else
            {
                Process_PCBTray抓取.Cur.Pallet.Clear();
                Process_PCBTray抓取.Cur.ScanPallet.Clear();
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("请确认清除NG料盘计数吗", "确认", MessageBoxButton.YesNo))
            {

                return;
            }
            else
            {

                ParamsNGTray.Cur.ScanPCBNgNum = 0;
                ParamsNGTray.Cur.MesCheckNGsNum = 0;
                Process_PCBTray抓取.Cur.NGPallet.Clear();
            }

        }
        PrinterHelper ph = null;

        private async void TestPrint_Btn(object sender, RoutedEventArgs e)
        {
            if (ph == null)
            {
                ph = new PrinterHelper("打印测试", "C:\\Users\\Lenovo\\Desktop\\标签模板\\标签模板.btw", "打印机名称");
            }
            if (!ph.IsReadyForPrint())
            {
                MessageBox.Show("打印机未准备就绪，请稍后....");
                return;
            }
            await ph.Print(new PrintParameter() { BarCode1 = "123", BarCode2 = "234", BarCode3 = "345", BarCode4 = "456", BarCode5 = "567" }, 4);
            await Task.Delay(3000);
            ph.Close();
        }

        private void ButtonEnd_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("请确认PCB料盘计数到最后吗", "确认", MessageBoxButton.YesNo))
            {
                return;
            }
            else
            {
                Process_PCBTray抓取.Cur.Pallet.EndIndex();
                Process_PCBTray抓取.Cur.ScanPallet.EndIndex();
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("请确认壳体料盘计数到最后吗", "确认", MessageBoxButton.YesNo))
            {
                return;
            }
            else
            {
                Process_前壳Tray抓取.Cur.Pallet.EndIndex();

            }
        }
        private void FrontDoor_Click(object sender, RoutedEventArgs e)
        {
            if (FrontDoor.Content.ToString() == "开启前门锁")
            {
                StaticIOHelper.前安全门锁.Out_ON();
                FrontDoor.Content = "关闭前门锁";
            }
            else if (FrontDoor.Content.ToString() == "关闭前门锁")
            {
                StaticIOHelper.前安全门锁.Out_OFF();
                FrontDoor.Content = "开启前门锁";

            }

        }

        private void BackDoor_Click(object sender, RoutedEventArgs e)
        {
            if (BackDoor.Content.ToString() == "开启后门锁")
            {
                StaticIOHelper.后安全门锁.Out_ON();
                BackDoor.Content = "关闭后门锁";
            }
            else if (BackDoor.Content.ToString() == "关闭后门锁")
            {
                StaticIOHelper.后安全门锁.Out_OFF();
                BackDoor.Content = "开启后门锁";

            }
        }
        private void LeftDoor_Click(object sender, RoutedEventArgs e)
        {
            if (LeftDoor.Content.ToString() == "开启左门锁")
            {
                StaticIOHelper.左安全门锁.Out_ON();
                LeftDoor.Content = "关闭左门锁";
            }
            else if (LeftDoor.Content.ToString() == "关闭左门锁")
            {
                StaticIOHelper.左安全门锁.Out_OFF();
                LeftDoor.Content = "开启左门锁";

            }
        }
    }
}