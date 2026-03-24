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

using Handler.Motion.Process.Station;
using Handler.Process.Station.TrayLoad.Pallet;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_MainTopPart.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_MainTopPart : UserControl, IShow
    {
        public Unit_MainTopPart()
        {
            InitializeComponent();
        }

        public void ShowUI()
        {
            uc_Product.ShowUI();
            uc_typeName.ShowUI();
            uc_ngTop.ShowUI();
        }

        private void btnPlasmaClear_Click(object sender, RoutedEventArgs e)
        {
            //Funs.FunPlasmaLife.Cur.PlasmaLifeCurrentValue.GetValue = 0;
            //Funs.FunPlasmaLife.Cur.Save();
        }

      
    }
}
