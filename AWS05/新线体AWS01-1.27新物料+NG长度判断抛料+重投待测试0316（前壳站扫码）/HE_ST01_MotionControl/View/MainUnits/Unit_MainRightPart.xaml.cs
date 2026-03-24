using Handler.Params;
using Handler.Process.Station;
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

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_MainRightPart.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_MainRightPart : UserControl, IShow
    {
        public Unit_MainRightPart()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowUI()
        {
           

            uc_buttons.ShowUI();




        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NG1Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("确认清除NG料盘吗", "确认", MessageBoxButton.YesNo))
            {
                ParamsNGTray.Cur.ClearIndex();
            }
           
        }

        private void btn_NG1Clear_Click_1(object sender, RoutedEventArgs e)
        {
        }
    }
}
