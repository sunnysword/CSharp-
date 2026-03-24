using Handler.Motion;
using Handler.Process.Station;
using Handler.Product;
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

namespace FrontShellLocking.View
{
    /// <summary>
    /// Unit_OffLineModelView.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_LockAttachmentManual : UserControl
    {
        public Unit_LockAttachmentManual()
        {
            InitializeComponent();
            if (StaticInitial.Motion.CurOrder.IsAutoRunning)
            {
                textBlockState.Text = "当前自动流程中，不允许启动手动锁附";
                textBlockState.Foreground = System.Windows.Media.Brushes.Red;
                this.IsEnabled = false;
            }
        }

        private async void btn_StartScrew1(object sender, RoutedEventArgs e)
        {

            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                await Task.Run(() =>
                {
   
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }

        }


        private void btn_PauseScrew1(object sender, RoutedEventArgs e)
        {
          
        }

        private void btn_ContinueScrew1(object sender, RoutedEventArgs e)
        {
            StaticInitial.Motion.CommitClearErrOrder();
            System.Threading.Thread.Sleep(200);
  
        }

        private void btn_StopScrew1(object sender, RoutedEventArgs e)
        {
            StaticInitial.Motion.CommitStopOrder();
        }

        private async void LanButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                await Task.Run(() =>
                {
                   // Station2.Cur.Test(true);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void LanButton_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                var product = ProductInfo.GetInitialProduct();
                product.SN = this.textBoxSN.Text;
                await Task.Run(() =>
                {
                    //Station5.Cur.LoadProduct(product);
                    //Station5.Cur.Test(true);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void LanButton_Click_2(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                await Task.Run(() =>
                {
                    //Process_FrontHolderPickPlace.Cur.CanSkipStep = true;
                    //Process_FrontHolderPickPlace.Cur.Run();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
           
        }

        private async void LanButton_Click_3(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                await Task.Run(() =>
                {
                    //Process_PCBPickPlace.Cur.CanSkipStep = true;
                    //Process_PCBPickPlace.Cur.Run();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
           
        }

        private async void btn_StartScrew2(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            try
            {
                button.IsEnabled = false;
                await Task.Run(() =>
                {
    
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }
    }
}
