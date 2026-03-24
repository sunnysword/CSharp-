using AixCommInfo;
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
using Handler.Motion.PalletTray;
using AbstractLineAixsdll;
using Handler.Motion.Axis;
using Handler.Process.Station.TrayLoad.Pallet;

namespace Handler.View.Pallet
{
    /// <summary>
    /// UC_Pallet_Box_AAUnload.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Pallet_Box : UserControl
    {
        public UC_Pallet_Box()
        {
            InitializeComponent();

            Index.PropertyChanged += Index_PropertyChanged;
        }

        private void Index_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int i = -1;
            foreach (var item in bordersList)
            {
                i = int.Parse(item.Tag.ToString());
                if(i==Index.GetValue)
                {
                    item.BorderBrush= Brushes.Yellow;
                }
                else
                {
                    item.BorderBrush = Brushes.Transparent;
                }

            }


            
        }

        public ParamStructText<int> Index = new AixCommInfo.ParamStructText<int>() { GetValue=-1};


        List<Border> bordersList = new List<Border>();


        /// <summary>
        /// 绘制阵列的矩形
        /// </summary>
        public void Draw_Boxs(PalletInfo trayPalletHelper_AAUnload, ThreeAxisBase lineAix)
        {
            bordersList.Clear();
           
            gd_1.Children.Clear();
            ushort rows = trayPalletHelper_AAUnload.RowNums.GetValue;
            ushort cols = trayPalletHelper_AAUnload.ColNums.GetValue;
            int box_Max = 50;
            int box_Min = 20;

            gd_1.RowDefinitions.Clear();
            gd_1.ColumnDefinitions.Clear();

           

            if (gd_1.Width < (box_Max + box_Min + 5) * cols)
            {
                gd_1.Width = (box_Max + box_Min + 5) * cols;
            }
            if (gd_1.Height < (box_Max + box_Min) * rows)
            {
                gd_1.Height = (box_Max + box_Min) * rows;
            }


            for (int i = 0; i < rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();

                gd_1.RowDefinitions.Add(rowDefinition);

                for (int j = 0; j < cols; j++)
                {
                    if (i == 0)
                    {
                        ColumnDefinition columnDefinition = new ColumnDefinition();
                        //columnDefinition.Width = GridLength.Auto;
                        gd_1.ColumnDefinitions.Add(columnDefinition);
                    }

                    Border border = new Border();
                    bordersList.Add(border);
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Transparent;
                    border.Tag = (i * cols + j).ToString();
                    border.Cursor = Cursors.Hand;
                    if (trayPalletHelper_AAUnload.Points.Count >= (i * cols + j))
                    {

                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = (i * cols + j).ToString();
                        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        border.Child = textBlock;
                        TrayPointZU trayPoint = trayPalletHelper_AAUnload.Points[i * cols + j];
                        border.ToolTip = $"点{i * cols + j}: X={trayPoint.X},Y={trayPoint.Y}";
                        border.PreviewMouseLeftButtonUp += async (s1, e) =>
                        {
                            try
                            {
                                this.IsEnabled = false;
                                Border border1 = s1 as Border;
                                int r = 0;
                                int c = 0;
                                r = Grid.GetRow(border1);
                                c = Grid.GetColumn(border1);
                                
                                Index.GetValue = r * trayPalletHelper_AAUnload.ColNums.GetValue + c;
                                TrayPointZU point = trayPalletHelper_AAUnload.Points[Index.GetValue];
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        lineAix.Z_MovWait();
                                        lineAix.XY_MovAbs(point.X, point.Y);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });

                                    }
                                });


                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show(ex.Message);
                            }

                            this.IsEnabled = true;


                        };
                    }

                    border.MinHeight = box_Min;
                    border.MinWidth = box_Min + 15;
                    border.MaxHeight = box_Max;
                    border.MaxWidth = box_Max;

                    border.Background = Brushes.Gray;
                    border.Margin = new Thickness(5);


                    gd_1.Children.Add(border);


                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                }
            }

            Index.GetValue = -1;
        }

    }
}
