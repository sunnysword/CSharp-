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
using Handler.Funs;

namespace Handler.View.RunModes
{
    /// <summary>
    /// Wnd_RunModeSetting.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_RunModeSetting : Window
    {
        private Wnd_RunModeSetting()
        {
            InitializeComponent();
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(this, IUserLimitdll.UserLevelModes.ModifyAndOperation);


            cbb_RunModes.IsEnabled = true;
        }
        static Wnd_RunModeSetting Cur = null;
        static readonly object ob_lock = new object();

        public static Wnd_RunModeSetting CreateInstrance()
        {
            if (Cur == null)
            {
                lock (ob_lock)
                {
                    if (Cur == null)
                    {
                        Cur = new Wnd_RunModeSetting();
                    }
                }
            }

            return Cur;
        }

        public static void SingleShow()
        {
            Window window = CreateInstrance();
            window.Topmost = false;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowState = WindowState.Normal;
            window.ShowDialog();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbb_RunModes.ItemsSource = Process.RunMode.WorkModeManager.Cur.WorkModeList;
                cbb_RunModes.DisplayMemberPath = "Name";
                cbb_RunModes.SelectedItem = Process.RunMode.WorkModeManager.Cur.SelectWorkMode;

                lb_Funs.ItemsSource = Process.RunMode.WorkModeManager.Cur.SelectWorkMode.GetFunsSettings();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Cur = null;

        }

        private void cbb_RunModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsInitialized == false || IsLoaded == false) return;
            try
            {
                Process.RunMode.StationSelectFunsBase temp = cbb_RunModes.SelectedItem as Process.RunMode.StationSelectFunsBase;

                if (temp != Process.RunMode.WorkModeManager.Cur.SelectWorkMode)
                {
                    Process.RunMode.WorkModeManager.Cur.SetWorkMode(temp);
                    lb_Funs.ItemsSource = null;
                    lb_Funs.ItemsSource = Process.RunMode.WorkModeManager.Cur.SelectWorkMode.GetFunsSettings();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
