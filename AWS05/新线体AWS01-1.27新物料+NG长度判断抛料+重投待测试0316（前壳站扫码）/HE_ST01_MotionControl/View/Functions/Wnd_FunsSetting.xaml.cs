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

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Wnd_FunsSetting.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_FunsSetting : Window
    {
        public Wnd_FunsSetting()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lsb_Menu.ItemsSource = Unit_FunsViewHelper.demoViewModels;
            lsb_Menu.DisplayMemberPath = "Name";
            lsb_Menu.SelectedIndex = 0;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in Funs.FunsSelectionHelperbase.FunctionsList)
                {
                    item.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
