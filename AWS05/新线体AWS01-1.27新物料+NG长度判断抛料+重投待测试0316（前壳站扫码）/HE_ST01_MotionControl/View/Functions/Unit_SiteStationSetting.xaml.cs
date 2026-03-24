using Handler.Process.Station;
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
    /// Unit_SiteStationSetting.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_SiteStationSetting : UserControl
    {
        public Unit_SiteStationSetting()
        {
            InitializeComponent();
        }


        void Init()
        {
            this.gridSocketSettings.Children.Clear();


            for (int i = 0; i < Funs.FunSocketSetting.Cur.SocketIsUseArray.Length; i++)
            {
                //工位设置
                int index = i;

                //治具设置
                CheckBox checkBoxSocket = new CheckBox();
                checkBoxSocket.Margin = new Thickness(5);
                //checkBoxSocket.Content = $"启用治具{HE_ST01_MotionControl.Motion.Axis.AxisSingle.RotatedMovNextHelper.SocketNameArray[i]}";
                checkBoxSocket.IsChecked = Funs.FunSocketSetting.Cur.SocketIsUseArray[i].GetValue;
                checkBoxSocket.Click += (sender, args) =>
                {
                    CheckBox checkBox = sender as CheckBox;
                    Funs.FunSocketSetting.Cur.SocketIsUseArray[index].GetValue = checkBox.IsChecked == true;
                    Funs.FunSocketSetting.Cur.Save();
                    Funs.FunSocketSetting.Cur.SocketLimitStatusChangedEventHandler?.Invoke(index, checkBox.IsChecked == true);
                };
                Grid.SetColumn(checkBoxSocket, 0);
                gridSocketSettings.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(checkBoxSocket, i);
                gridSocketSettings.Children.Add(checkBoxSocket);
                //治具开合感应设置
                CheckBox checkBoxSocketSensor = new CheckBox();
                checkBoxSocketSensor.Margin = new Thickness(5);
                checkBoxSocketSensor.Content = $"启用工位{i+1}产品有无检测";
                checkBoxSocketSensor.IsChecked = Funs.FunSocketSetting.Cur.StationSensorIsUseArray[i].GetValue;
                checkBoxSocketSensor.Click += (sender, args) =>
                {
                    CheckBox checkBox = sender as CheckBox;
                    Funs.FunSocketSetting.Cur.StationSensorIsUseArray[index].GetValue = checkBox.IsChecked == true;
                    Funs.FunSocketSetting.Cur.Save();
                };
                Grid.SetColumn(checkBoxSocketSensor, 1);
                gridSocketSettings.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(checkBoxSocketSensor, i);
                gridSocketSettings.Children.Add(checkBoxSocketSensor);
            }
        }

        private void SelectionChanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (this.IsInitialized == false) return;

                Funs.FunSocketSetting.Cur.Save();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}
