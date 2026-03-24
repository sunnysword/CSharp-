using AixCommInfo;
 
using Handler.Motion.Axis;
using HaoEn_G.Connect;
using LanCustomControldllExtend.FourAxis;
using Newtonsoft.Json;
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
using System.Windows.Shapes;

namespace Handler.View
{
    /// <summary>
    /// AdminTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdminTestWindow : Window
    {
        public AdminTestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("是否需要不复位就进入复位完成状态，供临时调试使用", "确认", MessageBoxButton.YesNo))
            {
                Motion.StaticInitial.Motion.CurOrder.ResOKFlag.SetFlag();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
      

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Connect.ConnectFactory.ScrewGunTCP.ProjectNums;转换扭力
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
        
        }

        private void SetNG(string str)
        {
            new System.Threading.Thread(() =>
            {
                Params.ParamsNGTray.Cur.AddNGInfoOnce(str);
            }).Start();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SetNG("扫码NG");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            SetNG("螺丝锁附NG");

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            SetNG("螺丝高度NG");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            SetNG("半成品测试NG");
        }
    }
}
