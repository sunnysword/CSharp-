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
using Handler.Product;
using AviewMotionUI.ProductionStatistics;
using Handler.Params;
using System.Collections.ObjectModel;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_ProductInfo.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_ProductInfo : UserControl, IShow
    {
        public Unit_ProductInfo()
        {
            InitializeComponent();
        }
        private void Btn_ClearProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to empty the current production data?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ProductNumberManager.Cur.Clear();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public void ShowUI()
        {
            tb_OkProductNum.Text = ProductionStatisticsManger.Current.TotalYieldArray.YieldPercent.KeyCount.ToString();
            tb_NgProductNum.Text = ProductionStatisticsManger.Current.TotalYieldArray.YieldPercent.SpareCount.ToString();
            tb_TotalProductNum.Text = ProductionStatisticsManger.Current.TotalYieldArray.YieldPercent.TotalCount.ToString();

            tb_OKPercent.Text = ProductionStatisticsManger.Current.TotalYieldArray.YieldPercent.Yield.ToString("F2") + "%";
            tb_UPH.Text = ParamsUPH.Cur.UPH.ToString();
        }


        private void Btn_NgProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Wnd_ProductMsg wnd_ProductMsg = Wnd_ProductMsg.CreateInstrance();
                //wnd_ProductMsg.Loaded += (s, r) => btn_NgProduct.IsEnabled = false;
                //wnd_ProductMsg.Closed += (s, r) => btn_NgProduct.IsEnabled = true;
                wnd_ProductMsg.WindowState = WindowState.Normal;
                wnd_ProductMsg.ShowDialog();
               
                
            }
            catch (Exception ex)
            {
                //btn_NgProduct.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定清空生产计数吗？", "请确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AviewMotionUI.ProductionStatistics.ProductionStatisticsManger.Current.Clear();
                ParamsNGTray.Cur.Clear();
                ParamsNGTray.Cur.ClearIndex();
                AviewMotionUI.ProductionStatistics.ProductionStatisticsManger.Current.Clear();
                ParamsNGTray.Cur.Clear();
                ParamsNGTray.Cur.ClearIndex();
            }
        }
        #region 原版TOP3NG
        //public void FlushNGInfo(List<NGInfo> items)
        //{
        //    if (items == null)
        //    {
        //        return;
        //    }
        //    var items2 = items.Take(3);
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        this.tb_NGTOP.Text = string.Empty;
        //        var str = string.Empty;
        //        foreach (var item in items2)
        //        {
        //            str += item.Info;
        //            str += " : ";
        //            str += item.Count.ToString();
        //            str += "\n";
        //        }
        //        this.tb_NGTOP.Text = str;
        //    });
        //}
        #endregion
        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            Window window2 = new Window();
            window2.Content = new AviewMotionUI.Views.ProductionStatistics.UCProductionStatistics();
            window2.Show();
        }
    }
}
