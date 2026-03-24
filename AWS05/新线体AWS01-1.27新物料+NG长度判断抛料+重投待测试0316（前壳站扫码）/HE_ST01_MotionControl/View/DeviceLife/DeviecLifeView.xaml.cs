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

namespace Handler.View.DeviceLife
{
    /// <summary>
    /// DeviecLifeView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviecLifeView : UserControl
    {
        public DeviecLifeView()
        {
            InitializeComponent();
        }

        private bool _canExit = false;

        public void ShowUI()
        {
            if (_canExit)
            {
                return;
            }
            textBlockRightProbeCount.Text = Funs.FunDeveiceLife.Cur.StationProbeUsageCurrenCount.ToString();
            textBlockRightProbeMaxCount.Text = Funs.FunDeveiceLife.Cur.StationProbeMaxCount.GetValue.ToString();

            textBlockRightBitsCount.Text = Funs.FunDeveiceLife.Cur.StationBitsUsageCurrenCount.ToString();
            textBlockRightBitsMaxCount.Text = Funs.FunDeveiceLife.Cur.StationBitsMaxCount.GetValue.ToString();
            //App.Current.Dispatcher.Invoke(() =>
            //{

            //});
            //System.Threading.Thread.Sleep(1000);

        }

        public void Init()
        {
            var thread = new System.Threading.Thread(ShowUI);
            thread.IsBackground = true;
            thread.Start();
        }

        private void btnRightProbeClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定清空探针计数吗？", "请确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Funs.FunDeveiceLife.Cur.StationProbeUsageCurrenCount = 0;
                Funs.FunDeveiceLife.Cur.Save();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _canExit = true;
        }

        private void btnRightBitsClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定清空批头计数吗？", "请确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Funs.FunDeveiceLife.Cur.StationBitsUsageCurrenCount = 0;
                Funs.FunDeveiceLife.Cur.Save();
            }
        }
    }
}
