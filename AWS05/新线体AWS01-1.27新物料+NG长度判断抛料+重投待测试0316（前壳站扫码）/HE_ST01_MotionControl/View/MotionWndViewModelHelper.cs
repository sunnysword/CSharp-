using DH_ClampRotatedAxisDemo;
using DS_SorterAndOven.View.MainUnits;
using Handler.Connect;
using Handler.Connect.Camera;
using Handler.Motion.Axis;
using Handler.Motion.IO;
using Handler.Process.Station;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.View.Gripper;
using Handler.View.Laser;
using Handler.View.Pallet;
using Handler.View.RFID;
using HE_ST01_MotionControl.Connect.Modbus;
using HE_ST01_MotionControl.Motion.Axis.AxisGroup;
using HE_ST01_MotionControl.Motion.Axis.AxisSingle;
using HE_ST01_MotionControl.OtherHelpers;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;
using Iodll;
using LanCustomControldllExtend.Unit_LineAix;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static LanCustomControldll.Unit_Aix.UC_SimpleCalcMode;

namespace Handler.View
{
    /// <summary>
    /// Motion 窗口，界面管理
    /// </summary>
    class MotionWndViewModelHelper : LanguageControlBase.Wpf.BindableObject
    {
        static MotionWndViewModelHelper Cur = new MotionWndViewModelHelper();

        //这里添加用户界面
        private MotionWndViewModelHelper()
        {
            //注册按钮按下发生时，获取对应按钮的界面
            Unit_UserViewModel.actionButtonDownEventHandler += () => { UserControlCurShow = Unit_UserViewModel.userControl_CurShow.userControl; };

            void AddPage(AbstractSingleAixdll.AbstractSingleAix mSingleAix, string buttonCaption)
            {
                LanCustomControldll.Unit_Aix.UC_DemoAix demoAixViewModel = new LanCustomControldll.Unit_Aix.UC_DemoAix();
                demoAixViewModel.SetAixInfo(mSingleAix);
                Unit_UserViewModel unit_UserViewModel_DemoAix = new Unit_UserViewModel(buttonCaption, demoAixViewModel, demoAixViewModel);
            }

            LanCustomControldllExtend.Unit_LineAix.UC_DemoLineAixs PCB上料 = new LanCustomControldllExtend.Unit_LineAix.UC_DemoLineAixs();
            PCB上料.SetLineAixsInfo(ThreeAxisPCB上料.Cur);
            //PCB上料.AddOutputButton(StaticIOHelper.PCB取料相机光源);
            Unit_UserViewModel unit_UserViewModel_Plasma2 = new Unit_UserViewModel("PCB上料三轴", PCB上料, PCB上料);
            PCB上料.sp_TrailContent.Visibility = Visibility.Hidden;
            ButtonGroupView groupView_PCBLoad = new ButtonGroupView();
            UC_PalletEdit_OnePoint UC_PalletEdit_PCBLoad = new Pallet.UC_PalletEdit_OnePoint(PalletManger.PalletPCB, ThreeAxisPCB上料.Cur);
            UC_PalletEdit_PCBLoad.Gripper = Process_PCBTray抓取.Cur.Gripper;
            groupView_PCBLoad.AddButton("PCB料盘设置", UC_PalletEdit_PCBLoad);

            UC_PalletEdit_OnePoint UC_PalletEdit_PCBNG = new Pallet.UC_PalletEdit_OnePoint(PalletManger.PalletPCBNG, ThreeAxisPCB上料.Cur);
            UC_PalletEdit_PCBNG.Gripper = Process_PCBTray抓取.Cur.Gripper;
            groupView_PCBLoad.AddButton("PCBNG料盘设置", UC_PalletEdit_PCBNG);

            UC_PalletEdit_OnePoint UC_PalletEdit_ScanPCB = new Pallet.UC_PalletEdit_OnePoint(PalletManger.ScanPCB, ThreeAxisPCB上料.Cur);
            UC_PalletEdit_ScanPCB.Gripper = Process_PCBTray抓取.Cur.Gripper;
            groupView_PCBLoad.AddButton("PCB扫码料盘设置", UC_PalletEdit_ScanPCB);

            UC_PalletEdit_CameraCalib calibView_PCBLoad = new UC_PalletEdit_CameraCalib(PalletManger.PalletCalibPCB, ThreeAxisPCB上料.Cur, CameraManager.CameraPCB);
            groupView_PCBLoad.AddButton("PCB料盘标定", calibView_PCBLoad);
            PCB上料.SetComponent(groupView_PCBLoad, null);


            LanCustomControldllExtend.Unit_LineAix.UC_DemoLineAixs 前壳上料 = new LanCustomControldllExtend.Unit_LineAix.UC_DemoLineAixs();
            前壳上料.SetLineAixsInfo(ThreeAxis前壳上料.Cur);
            //前壳上料.AddOutputButton(StaticIOHelper.前壳取料相机光源);
            Unit_UserViewModel unit_UserViewModel_Plasma4 = new Unit_UserViewModel("前壳上料三轴", 前壳上料, 前壳上料);
            前壳上料.sp_TrailContent.Visibility = Visibility.Hidden;
            ButtonGroupView groupView_PCBLoad3 = new ButtonGroupView();
            UC_PalletEdit_OnePoint UC_PalletEdit_PCBLoad3 = new Pallet.UC_PalletEdit_OnePoint(PalletManger.Pallet前壳, ThreeAxis前壳上料.Cur);
            UC_PalletEdit_PCBLoad3.Gripper = Process_前壳Tray抓取.Cur.Gripper;

            UC_PalletEdit_OnePoint UC_PalletEdit_Scan前壳PCB = new Pallet.UC_PalletEdit_OnePoint(PalletManger.ScanPcbShell, ThreeAxis前壳上料.Cur);
            UC_PalletEdit_Scan前壳PCB.Gripper = Process_前壳Tray抓取.Cur.Gripper;
            groupView_PCBLoad.AddButton("前壳PCB扫码料盘设置", UC_PalletEdit_Scan前壳PCB);



            UC_PalletEdit_CameraCalib calibView_PCBLoad3 = new UC_PalletEdit_CameraCalib(PalletManger.PalletCalib前壳, ThreeAxis前壳上料.Cur, CameraManager.CameraFrontHolder);
            groupView_PCBLoad3.AddButton("前壳料盘设置", UC_PalletEdit_PCBLoad3);


            groupView_PCBLoad3.AddButton("前壳料盘标定", calibView_PCBLoad3);
            前壳上料.SetComponent(groupView_PCBLoad3, null);

            AddPage(AxisPCB上料工站Tray.Cur, AxisPCB上料工站Tray.Cur.Name);
            AddPage(Axis前壳上料工站Tray.Cur, Axis前壳上料工站Tray.Cur.Name);

            UC_ClampAxisView viewNewGrip = new UC_ClampAxisView();
            viewNewGrip.SetAxisInfo(ClampRotatedAxisShell.Cur);
            new Unit_UserViewModel("前壳夹爪测试", viewNewGrip, viewNewGrip);

            UC_ClampAxisView viewNewGripPCB = new UC_ClampAxisView();
            viewNewGripPCB.SetAxisInfo(ClampRotatedAxisPCB.Cur);
            new Unit_UserViewModel("PCB夹爪测试", viewNewGripPCB, viewNewGripPCB);
            //IO 界面
            LanCustomControldllExtend.Unit_Io.UC_DemoIOList_Format5_Extern uC_DemoIOList_Format1 = new LanCustomControldllExtend.Unit_Io.UC_DemoIOList_Format5_Extern(new StaticIOHelper());
            uC_DemoIOList_Format1.SetIoPages(AM.Core.IO.IOBuilder.IoMsg);

            Unit_UserViewModel unit_UserViewModel_io = new Unit_UserViewModel("IO", uC_DemoIOList_Format1, uC_DemoIOList_Format1);


            //FinalTestMachine.Views.UserControlMESConfig viewMes = new FinalTestMachine.Views.UserControlMESConfig();
            //Unit_UserViewModel unit_UserViewModel_Mes = new Unit_UserViewModel("Mes测试", viewMes);

            ViewRFID viewRFID = new ViewRFID();
            Unit_UserViewModel unit_UserViewModel_RFID = new Unit_UserViewModel("RFID测试", viewRFID);
            //设置默认显示
            unit_UserViewModelsList[0].SetShow();
            UserControlCurShow = unit_UserViewModelsList[0].userControl;
        }



        public static MotionWndViewModelHelper CreateInstrance()
        {
            return Cur;
        }

        /// <summary>
        /// 已经创建的界面集合
        /// </summary>
        public ObservableCollection<Unit_UserViewModel> unit_UserViewModelsList => Unit_UserViewModel.userControlsListHaved;
        bool IsExit = false;

        /// <summary>
        /// 额外的界面刷新事件
        /// </summary>
        public event Action ExtraShowUIEventHandler;


        UserControl userControlCurShow;
        public UserControl UserControlCurShow
        {
            get { return userControlCurShow; }
            set
            {
                SetProperty(ref userControlCurShow, value);
            }
        }


        public bool IsCanShow { get; set; }

        public void ShowUI()
        {


            if (!IsCanShow) return;

            try
            {
                Unit_UserViewModel.Fun_AllShow();
                ExtraShowUIEventHandler?.Invoke();

            }
            catch (Exception ex)
            {
                //Motion.StaticInitial.Motion.WriteErrToUser(ex.Message);
            }

        }

        //void ShowUI2()
        //{

        //    try
        //    {
        //        bool IsScan = true;
        //        while (true)
        //        {
        //            if (IsExit)
        //            {
        //                IsExit = false;
        //                return;
        //            }
        //            if (IsScan)
        //            {
        //                IsScan = false;

        //                foreach (var item in ExtraShowUIEventHandler.GetInvocationList())
        //                {
        //                    if(item!=null)
        //                    {

        //                        App.Current.Dispatcher.Invoke(() =>
        //                        {
        //                            try
        //                            {
        //                                item.DynamicInvoke();

        //                            }
        //                            catch (Exception ex)
        //                            {

        //                                Motion.StaticInitial.Motion.WriteErrToUser("UI Scan Err:" + ex.Message);
        //                            }


        //                        });
        //                        Thread.Sleep(10);
        //                    }
        //                }
        //                IsScan = true;
        //            }
        //            Thread.Sleep(10);




        //        }
        //    }
        //    finally
        //    {
        //        task_Curr = null;
        //    }

        //}



    }
}
