using LanCustomControldll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Handler.Motion.Process;
using Handler.Process.Station;
using Handler.Connect;
using Handler.Motion.IO;
using AixCommInfo;
using Handler.Motion.Axis;
using Handler.Process;
using Handler.Motion;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_MainBtns_1.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_MainBtns_1 : UserControl, IShow
    {
        public Unit_MainBtns_1()
        {
            InitializeComponent();


            //Binding binding = new Binding();
            //binding.Source = Connect.ConnectToRobot.IsRobotOnLine;
            //binding.Path = new PropertyPath("GetValue");
            //chb_RobotSelect.SetBinding(CheckBox.IsCheckedProperty, binding);
            ClearCount.Text = "0";
        }

        private void Btn_ResMachine_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBoxResult.Yes == MessageBox.Show("请先确认机台安全，确认后点击YES按钮进行复位，点击NO按钮取消复位", "确认", MessageBoxButton.YesNo))
            {
                Motion.StaticInitial.Motion.CommitResOrder();
            }

        }

        private void Btn_ClearErr_Click(object sender, RoutedEventArgs e)
        {
            Motion.StaticInitial.Motion.CommitClearErrOrder();
        }

        //private void Btn_Pause_Click(object sender, RoutedEventArgs e)
        //{
        //    Motion.StaticInitial.Motion.CommitPauseOrder();
        //}

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBoxResult.Yes == MessageBox.Show("是否确认停止，确认后点击YES按钮进行停止，点击NO按钮取消停止", "确认", MessageBoxButton.YesNo))
            {
                StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                Motion.StaticInitial.Motion.CommitStopOrder();
                StaticInitial.Motion.CurOrder.ResOKFlag.ResetFlag();
            }

            //StaticIOHelper.回流移栽皮带线启停.Out_OFF();
            //StaticIOHelper.前段回流线启停.Out_OFF();
            //StaticIOHelper.后段回流线启停.Out_OFF();
            //StaticIOHelper.前段输送线启停.Out_OFF();
            //StaticIOHelper.后段输送线启停.Out_OFF();
            //Motion.StaticInitial.Motion.CommitStopOrder();
            //StaticInitial.Motion.CurOrder.ResOKFlag.ResetFlag();
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            StaticIOHelper.启动按钮指示灯.Out_ON();
            StaticIOHelper.复位按钮指示灯.Out_OFF();
            StaticIOHelper.初始化按钮指示灯.Out_OFF();
            StaticIOHelper.停止按钮指示灯.Out_OFF();
            StaticIOHelper.暂停按钮指示灯.Out_OFF();
            //StaticIOHelper.前段输送线启停.Out_ON();
            //StaticIOHelper.后段输送线启停.Out_ON();

            if (Handler.Funs.FunSelection.Cur.IsUseSafeDoor.GetValue)
            {
                StaticIOHelper.前安全门锁.Out_ON();
                StaticIOHelper.左安全门锁.Out_ON();
                StaticIOHelper.后安全门锁.Out_ON();
            }
            Motion.StaticInitial.Motion.CommitStartRunOrder();


        }

        private void Btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            Motion.StaticInitial.Motion.CommitPauseOrder();
            StaticIOHelper.启动按钮指示灯.Out_OFF();
            StaticIOHelper.复位按钮指示灯.Out_OFF();
            StaticIOHelper.初始化按钮指示灯.Out_OFF();
            StaticIOHelper.停止按钮指示灯.Out_OFF();
            StaticIOHelper.暂停按钮指示灯.Out_ON();
            //StaticIOHelper.前段输送线启停.Out_OFF();
            //StaticIOHelper.后段输送线启停.Out_OFF();

        }


        RM_dll2.StandardOrderHelper StandardOrderHelper => Motion.StaticInitial.StandardOrder;


        public void ShowUI()
        {
            if (StandardOrderHelper.IsErr || StandardOrderHelper.IsAlarm ||
                StandardOrderHelper.IsEMG || StandardOrderHelper.IsResing ||
                StandardOrderHelper.IsAutoRunning)
            {
                btn_ResMachine.IsEnabled = false;

            }
            else
            {
                btn_ResMachine.IsEnabled = true;
            }

            if (StandardOrderHelper.IsErr || StandardOrderHelper.IsAlarm ||
                  StandardOrderHelper.IsEMG || StandardOrderHelper.IsResing || !StandardOrderHelper.IsResOK)
            {
                btn_Start.IsEnabled = false;
            }
            else
            {
                if (StandardOrderHelper.IsAutoRunning && !(StandardOrderHelper.IsErrPause || StandardOrderHelper.IsPause))
                {
                    if (StationBase.CheckAnyProcessIsPause() && StandardOrderHelper.IsAutoRunning)
                    {
                        btn_Start.IsEnabled = true;
                    }
                    else
                    {
                        btn_Start.IsEnabled = false;
                    }
                }
                else
                {
                    btn_Start.IsEnabled = true;
                }
            }

            if (StandardOrderHelper.IsAutoRunning && !StandardOrderHelper.IsPause)
            {
                btn_Pause.IsEnabled = true;
            }
            else
            {
                btn_Pause.IsEnabled = false;
            }
            Process.Process_AutoRun.Cur.IsClearModeNew = chb_IsLClearMode.IsChecked == true;
            if (StandardOrderHelper.IsErr || StandardOrderHelper.IsAlarm || StandardOrderHelper.IsEMG)
            {
                btn_ClearErr.IsEnabled = true;
            }
            else
            {
                btn_ClearErr.IsEnabled = false;
            }
            if (!ClearCount.IsFocused && !chb_IsLClearMode.IsFocused)
            {
                ClearCount.Text = Process.Process_AutoRun.Cur.ClearCount.ToString();
            }


            //if(Connect.ConnectFactory.tcp_Robot.IsConnected)
            //{
            //    if (!chb_RobotSelect.IsEnabled)
            //    {
            //        chb_RobotSelect.IsEnabled = true;
            //    }

            //}
            //else
            //{
            //    if (chb_RobotSelect.IsEnabled)
            //    {
            //        chb_RobotSelect.IsEnabled = false;
            //        chb_RobotSelect.IsChecked = false;
            //    }
            //}

            if (StandardOrderHelper.IsAutoRunning)
            {
                if (StandardOrderHelper.IsPause || StationBase.CheckAnyProcessIsPause() && StandardOrderHelper.IsAutoRunning)
                {
                    btn_Start.LanContent = "继续";
                }
                else
                {
                    btn_Start.LanContent = "启动";
                }
            }
            else
            {
                btn_Start.LanContent = "启动";
                chb_IsLClearMode.IsChecked = false;

                //chb_IsLClearMode.IsChecked = false;

            }

        }
        private void btn_Lamp_Click(object sender, RoutedEventArgs e)
        {
            if (btn_Lamp.LanContent == "开启照明")
            {
                StaticIOHelper.机台内日光灯控制中继?.Out_ON();
                btn_Lamp.LanContent = "关闭照明";
            }
            else if (btn_Lamp.LanContent == "关闭照明")
            {
                StaticIOHelper.机台内日光灯控制中继?.Out_OFF();
                btn_Lamp.LanContent = "开启照明";
            }
        }
        private void btn_OpenSafeDoor_Click(object sender, RoutedEventArgs e)
        {
            if (btn_OpenSafeDoor.LanContent == "开启安全门")
            {
                Funs.FunSelection.Cur.IsUseSafeDoor.GetValue = true;
                btn_OpenSafeDoor.LanContent = "关闭安全门";
            }
            else if (btn_OpenSafeDoor.LanContent == "关闭安全门")
            {
                Funs.FunSelection.Cur.IsUseSafeDoor.GetValue = false;
                btn_OpenSafeDoor.LanContent = "开启安全门";
            }
        }
        private void Chb_IsLClearMode_Click(object sender, RoutedEventArgs e)
        {
            Process.Process_AutoRun.Cur.IsClearModeNew = chb_IsLClearMode.IsChecked == true;
            Int32.TryParse(ClearCount.Text, out int clearCount);
            Process.Process_AutoRun.Cur.ClearCount = clearCount;
        }

        private void chb_Reload_Click(object sender, RoutedEventArgs e)
        {
            //Process_MovRequest.Cur.IsReLoad.GetValue = chb_Reload.IsChecked == true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Binding binding = new Binding();
            //binding.Source = Process_MovRequest.Cur.IsReLoad;
            //binding.Path = new PropertyPath("GetValue");
            //chb_Reload.SetBinding(CheckBox.IsCheckedProperty, binding);
        }

        private void chb_IsLReProduce_Click(object sender, RoutedEventArgs e)
        {
            Process_AutoRun.Cur.isReProduce= chb_IsLReProduce.IsChecked==true;

        }
    }
}
