
using DS_SorterAndOven.View.MainUnits;
using FrontShellLocking.View;
using Handler.Connect;
using Handler.Connect.RFID;
using Handler.Motion;
using Handler.Motion.IO;
using Handler.Motion.Process;
using Handler.Params;
using Handler.View.DeviceLife;
using Handler.View.RunModes;
using Newtonsoft.Json;
using RM_dll2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
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
using TCPHelper3;
using Wpf_LoginWnd;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_MainTopMenu.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_MainTopMenu : UserControl
    {
        public Unit_MainTopMenu()
        {
            InitializeComponent();


            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_WorkItemManager, IUserLimitdll.UserLevelModes.Operation);
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_ConSettings, IUserLimitdll.UserLevelModes.Top);
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_MainSetting, IUserLimitdll.UserLevelModes.Top);
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_SettingFunc, IUserLimitdll.UserLevelModes.Top);

            //IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_SettingFunc, IUserLimitdll.UserLevelModes.Top);
            //IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(menu_PwdHeader, IUserLimitdll.UserLevelModes.Top);

        }

        private void menu_Motion_Click(object sender, RoutedEventArgs e)
        {
            Wnd_Motion.SingleShow();
        }

        private void menu_WorkItemManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WorkItemHelper2.MainWindow window = new WorkItemHelper2.MainWindow(WorkItemManagerHelper.workIterm);
                window.Owner = Window.GetWindow(this);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ShowDialog();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void menuLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.isirLog)
                {
                    IrisLoginHelper.MainWindow wnd = new IrisLoginHelper.MainWindow();
                    wnd.Owner = Window.GetWindow(this);
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wnd.ShowDialog();
                }
                else
                {
                    Wpf_LoginWnd.LoginMainWnd wnd = new Wpf_LoginWnd.LoginMainWnd();
                    wnd.Owner = Window.GetWindow(this);
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wnd.ShowDialog();
                }




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void menu_BaseFunsSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Wnd_FunsSetting wnd = new Wnd_FunsSetting();
                wnd.Owner = Window.GetWindow(this);
                wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                wnd.Height = 600;
                wnd.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Menu_SettingFunc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.isirLog)
                {
                    IrisLoginHelper.UserList userList = new IrisLoginHelper.UserList();
                    userList.Owner = Window.GetWindow(this);
                    userList.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    userList.ShowDialog();
                }
                else
                {
                    Wpf_LoginWnd.Wnd_LoginFunc wnd = new Wpf_LoginWnd.Wnd_LoginFunc();
                    wnd.Owner = Window.GetWindow(this);
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    wnd.ShowDialog();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void menu_RunModeSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Wnd_RunModeSetting.SingleShow();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void menu_SetDevLife_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var wnd = new DevManager.Wnd_DevLifeSetting();
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void menu_LifeShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var wnd = new DevManager.Wnd_LifeShowManager();
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }




        private void menuLife_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Content = new DeviecLifeView();
            window.Owner = App.Current.MainWindow;
            window.Height = 200;
            window.Width = 300;
            window.Show();
        }



        private void menuWhiteTest_Click(object sender, RoutedEventArgs e)
        {

        }



        private void menuNextStationInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LanCustomControldllExtend.CommunicationIO.CommunicationIOView view = new LanCustomControldllExtend.CommunicationIO.CommunicationIOView(
                 (AM.Communication.IO.IOHelper)ConnectFactory.CommunicationIO_Next, ConnectFactory.CommunicationIoMsgNext);
                Window window = new Window
                {
                    Content = view,
                    Owner = App.Current.MainWindow,
                    Width = 1000,
                    Height = 800
                };
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menuMESParameters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menu_CarrierChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckCarrierdll.WndCarrierCheckCreateView wdCarrierCheckCreateView = new CheckCarrierdll.WndCarrierCheckCreateView();
                wdCarrierCheckCreateView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                wdCarrierCheckCreateView.ShowDialog();

            }
            catch (Exception ex)
            {
                Handler.Motion.StaticInitial.Motion.WriteToUser(AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                MessageBox.Show(ex.Message);
            }
        }

        private void menu_PalletSetting_Click(object sender, RoutedEventArgs e)
        {
            Window window = null;
            if (window == null)
            {
                window = new PALLET_DEMO.MainWindow();
                window.Closed += (s1, s2) =>
                {
                    window = null;
                };
            }
            window.Show();
        }

        private void menu_ManualTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window window = new Window()
                {
                    Owner = App.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                Unit_LockAttachmentManual view = new Unit_LockAttachmentManual();
                window.Content = view;
                window.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void LanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Handler.View.MoudbusRTU.ModbusRTU view = new MoudbusRTU.ModbusRTU(Connect.ConnectFactory.LockAttachmentHeightMeasurement);
                //view.AddButton("测高一次", () =>
                //{
                //    if (ConnectFactory.LockAttachmentHeightMeasurement.GetCurrentHeight(out double height, out string error))
                //    {
                //        MessageBox.Show("高度:" + height.ToString());
                //    }
                //    else
                //    {
                //        MessageBox.Show("获取高度失败:" + error);
                //    }
                //});
                //view.AddButton("清零一次", () =>
                //{
                //    if (ConnectFactory.LockAttachmentHeightMeasurement.Zero(out string error))
                //    {
                //        MessageBox.Show("清零成功:");
                //    }
                //    else
                //    {
                //        MessageBox.Show("清零失败:" + error);
                //    }
                //});
                //view.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LanMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //Handler.View.MoudbusRTU.ModbusRTU view = new MoudbusRTU.ModbusRTU(Connect.ConnectFactory.LockAttachmentTorqueCheck);
                //view.AddButton("测量一次", () =>
                //{
                //    if (ConnectFactory.LockAttachmentTorqueCheck.GetCurrentValue(out double height, out string error))
                //    {
                //        MessageBox.Show("value:" + height.ToString());
                //    }
                //    else
                //    {
                //        MessageBox.Show("获取value失败:" + error);
                //    }
                //});

                //view.AddButton("读取峰值", () =>
                //{
                //    if (ConnectFactory.LockAttachmentTorqueCheck.GetMaxValue(out double height, out string error))
                //    {
                //        MessageBox.Show("value:" + height.ToString());
                //    }
                //    else
                //    {
                //        MessageBox.Show("获取value失败:" + error);
                //    }
                //});

                //view.AddButton("清零", () =>
                //{
                //    if (ConnectFactory.LockAttachmentTorqueCheck.Zero(out string error))
                //    {
                //        MessageBox.Show("清零成功:");
                //    }
                //    else
                //    {
                //        MessageBox.Show("清零失败:" + error);
                //    }
                //});
                //view.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public double NextDouble(Random ran, double minValue, double maxValue, int n)
        {
            double randNum = ran.NextDouble() * (maxValue - minValue) + minValue;
            return Convert.ToDouble(randNum.ToString("f" + n));
        }
        private void LanMenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //TorqueCheckWindow window = new TorqueCheckWindow();
            //Random ran = new Random();
            //window.Flush(
            //    () =>
            //{
            //    //double randNum = NextDouble(ran, 3.16, 7.28, 2);// 保留两位小数
            //    //return randNum;
            //    if (ConnectFactory.LockAttachmentTorqueCheck.GetCurrentValue(out double height, out string error))
            //    {
            //        return height;
            //    }
            //    else
            //    {
            //        return 0.0;
            //    }
            //},
            //() =>
            //{
            //    if (ConnectFactory.LockAttachmentTorqueCheck.GetMaxValue(out double height, out string error))
            //    {
            //        return height;
            //    }
            //    else
            //    {
            //        return 0.0;
            //    }
            //});
            //window.Show();
        }


        private void click_test(object sender, RoutedEventArgs e)
        {
            ParamsNGTray.Cur.Clear();
        }

        private void menuBeforeStationInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Mes_Test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MESTest mESTest = new MESTest();
                mESTest.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mESTest.Focus();
                mESTest.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menu_GripTest_Click(object sender, RoutedEventArgs e)
        {

            //axisHelper.ClampOpenPercentSetting = 100;//张开距离
            //axisHelper.Rotated_MovAbsCheck(100);//旋转角度并等待
            //axisHelper.ClampForce = 100;//夹持力设定
            //axisHelper.Clamp_OFF(s=>WriteToUserEevntHandler(s));//夹爪张开

        }

        private void menu_GripTest_Click_2(object sender, RoutedEventArgs e)
        {
            DH_ClampRotatedAxisDemo.MainWindow viewNewGrip = new DH_ClampRotatedAxisDemo.MainWindow(ConnectFactory.PCBGripper_HslNew);
            viewNewGrip.Show();
        }
    
        private void RFID_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    USR_RfidAynettek.ClientWindow window = new USR_RfidAynettek.ClientWindow(
            //        Connect.ConnectFactory.RFID_Load, null,
            //    null, null, null, null, null, null);

            //    AddClick(window, "btn_readrfid1", "tb_readrfid1", () => RFIDDataHelperTCP.GetRfidS(Connect.ConnectFactory.RFID_Load), null);
            //    AddClick(window, "btn_writerfid1", "tb_writerfid1", null, m => RFIDDataHelperTCP.SetRfidS(Connect.ConnectFactory.RFID_Load, m));
            //    window.ShowDialog();
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message);
            //}
        }

        public static Button GetButton(USR_RfidAynettek.ClientWindow window, string name)
        {
            Type type = window.GetType();

            FieldInfo fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo?.GetValue(window) as Button;
        }
        public static TextBox GetTextBox(USR_RfidAynettek.ClientWindow window, string name)
        {
            Type type = window.GetType();

            FieldInfo fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo?.GetValue(window) as TextBox;
        }
        public static void ReplaceButton(Button oldButton, TextBox textBox, Func<string> func = null, Action<string> action = null)
        {
            var parent = VisualTreeHelper.GetParent(oldButton) as Panel;

            if (parent != null)
            {
                int index = parent.Children.IndexOf(oldButton);
                parent.Children.RemoveAt(index);
                int row = Grid.GetRow(oldButton);
                int column = Grid.GetColumn(oldButton);
                int rowSpan = Grid.GetRowSpan(oldButton);
                int columnSpan = Grid.GetColumnSpan(oldButton);
                Button newButton = new Button
                {
                    Name = oldButton.Name,
                    Content = oldButton.Content,
                    Width = oldButton.Width,
                    Height = oldButton.Height,
                    Margin = oldButton.Margin,
                    HorizontalAlignment = oldButton.HorizontalAlignment,
                    VerticalAlignment = oldButton.VerticalAlignment,

                };
                Grid.SetRow(newButton, row);
                Grid.SetColumn(newButton, column);
                Grid.SetRowSpan(newButton, rowSpan);
                Grid.SetColumnSpan(newButton, columnSpan);
                newButton.Click += (s, e) =>
                {
                    try
                    {
                        if (func != null)
                        {
                            textBox.Text = func();
                        }
                        else if (action != null)
                        {
                            if (textBox.Text != "")
                            {
                                string text = textBox.Text;
                                action(text);
                            }
                            else
                            {
                                MessageBox.Show("DeviceID is Empty!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                parent.Children.Insert(index, newButton);
            }
        }
        public static void AddClick(USR_RfidAynettek.ClientWindow window, string name, string name1, Func<string> func = null, Action<string> action = null)
        {
            Button button = GetButton(window, name);
            TextBox textBox = GetTextBox(window, name1);
            ReplaceButton(button, textBox, func, action);
        }
    }
}
