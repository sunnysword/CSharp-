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
using Handler.Motion.Process.Station;
using Handler.Product;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_ProductListMsg.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_ProductListMsg : UserControl
    {
        public Unit_ProductListMsg()
        {
            InitializeComponent();
           
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            //ProductInfo.Fun_ClearAll();
        }
    }
}
