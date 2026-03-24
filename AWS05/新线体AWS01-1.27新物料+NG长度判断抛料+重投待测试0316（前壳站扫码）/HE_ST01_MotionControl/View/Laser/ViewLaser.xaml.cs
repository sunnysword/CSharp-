using Handler.Connect;
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

namespace Handler.View.Laser
{
    /// <summary>
    /// ViewLaser.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLaser : UserControl
    {
        public ViewLaser()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Funs.FunLaser.Cur.LaserCodeModuleName.GetValue))
            {
                MessageBox.Show("请先在设置中设置镭雕模板名称");
                return;
            }
            var ui = sender as Button;
            ui.IsEnabled = false;
            await Task.Run(() =>
            {
                var ret = LaserManager.Default.Laser1.Execute(LaserCmdType.Initialize,
                    Funs.FunLaser.Cur.LaserCodeModuleName.GetValue,
                    out string error);
                if (ret==true)
                {
                    MessageBox.Show("初始化成功");
                }
                else if (ret==false)
                {
                    MessageBox.Show("初始化失败！"+error);
                }
            });
            ui.IsEnabled = true;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxSN.Text))
            {
                MessageBox.Show("请先输入SN");
                return;
            }
            var ui = sender as Button;
            ui.IsEnabled = false;
            var sn = this.textBoxSN.Text;
            await Task.Run(() =>
            {
                var ret = LaserManager.Default.Laser1.Execute(LaserCmdType.Data,
                   sn,
                    out string error);
                if (ret == true)
                {
                }
                else if (ret == false)
                {
                    MessageBox.Show("数据发送失败！" + error);
                }
            });
            ui.IsEnabled = true;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var ui = sender as Button;
            ui.IsEnabled = false;
            await Task.Run(() =>
            {
                var ret = LaserManager.Default.Laser1.Execute(LaserCmdType.MarkStart,
                    string.Empty,
                    out string error);
                if (ret == true)
                {
                    MessageBox.Show("打标成功！" + error);

                }
                else if (ret == false)
                {
                    MessageBox.Show("打标失败！" + error);
                }
            });
            ui.IsEnabled = true;
        }
    }
}
