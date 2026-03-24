using AixCommInfo;
using LanCustomControldll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Handler.View.Pallet
{

    /// <summary>
    /// UC_PalletEdit_AAUnload.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PalletEdit : UserControl
    {
        private UC_PalletEdit()
        {
            InitializeComponent();
        }

        public UC_PalletEdit(PalletInfo palletInfo, LineAixZCreateBase _lineAix) :this()
        {
            CurTray = palletInfo;
            lineAix = _lineAix;
            Fun_SetBinding();

            
        }
        PalletInfo CurTray;
        LineAixZCreateBase lineAix;

        

        private void Fun_SetBinding()
        {
            void TextBinding<T>(ParamStructText<T> paramStructText, TextBox textBox) where T:struct
            {
                Binding binding = new Binding();
                binding.Source = paramStructText;
                binding.Path = new PropertyPath("GetValue");
                textBox.SetBinding(TextBox.TextProperty, binding);

            }
            TextBinding(uc_Boxs.Index, tb_PointIndex);


            TextBinding(CurTray.RowNums, tb_Dims_X);
            TextBinding(CurTray.ColNums, tb_Dims_Y);

            void TextPointBinding(TrayPoint trayPoint, UC_TextBoxAndButton uC_TextBoxAnd_X, UC_TextBoxAndButton uC_TextBoxAnd_Y)
            {
                Binding binding_x = new Binding();
                binding_x.Source = trayPoint;
                binding_x.Path = new PropertyPath("X");
                uC_TextBoxAnd_X.tb_Msg.SetBinding(TextBox.TextProperty, binding_x);

                Binding binding_y = new Binding();
                binding_y.Source = trayPoint;
                binding_y.Path = new PropertyPath("Y");
                uC_TextBoxAnd_Y.tb_Msg.SetBinding(TextBox.TextProperty, binding_y);

                uC_TextBoxAnd_X.btn_GetPos.Click += (s, e) =>
                {
                    try
                    {
                        uC_TextBoxAnd_X.tb_Msg.Text = lineAix.Aix_1.GetCmdPos().ToString();
                        uC_TextBoxAnd_X.tb_Msg.Focus();
                        Thread.Sleep(100);
                        uC_TextBoxAnd_X.btn_GetPos.Focus();

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                };
                uC_TextBoxAnd_Y.btn_GetPos.Click += (s, e) =>
                {
                    try
                    {
                        uC_TextBoxAnd_Y.tb_Msg.Text = lineAix.Aix_2.GetCmdPos().ToString();
                        uC_TextBoxAnd_Y.tb_Msg.Focus();
                        Thread.Sleep(100);
                        uC_TextBoxAnd_Y.btn_GetPos.Focus();

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                };
            }

            TextPointBinding(CurTray.Control_Point1, tb_Control_1_X, tb_Control_1_Y);
            TextPointBinding(CurTray.Control_Point2, tb_Control_2_X, tb_Control_2_Y);
            TextPointBinding(CurTray.Control_Point3, tb_Control_3_X, tb_Control_3_Y);
            TextPointBinding(CurTray.Control_Point4, tb_Control_4_X, tb_Control_4_Y);

            //Binding binding3 = new Binding();
            //binding3.Source = CurTray.Z_Standard;
            //binding3.Path = new PropertyPath("GetValue");
            //tb_Z_Standard.tb_Msg.SetBinding(TextBox.TextProperty, binding3);
            //tb_Z_Standard.btn_GetPos.Click += (s, e) =>
            //{
            //    try
            //    {
            //        tb_Z_Standard.tb_Msg.Text = lineAix.Aix_Z.GetCmdPos().ToString();
            //        tb_Z_Standard.tb_Msg.Focus();
            //        Thread.Sleep(100);
            //        tb_Z_Standard.btn_GetPos.Focus();

            //    }
            //    catch (Exception ex)
            //    {

            //        MessageBox.Show(ex.Message);
            //    }
            //};

            Dg_Msg.ItemsSource = CurTray.Points;


        }

        private void Fun_ReCheckControlPoints()
        {
            if (CurTray.RowNums.GetValue == 1 && CurTray.ColNums.GetValue == 1)
            {
                tb_Control_1_X.IsEnabled = true;
                tb_Control_2_X.IsEnabled = false;
                tb_Control_3_X.IsEnabled = false;
                tb_Control_4_X.IsEnabled = false;
                img_ControlPoint_Tip.Source = null;
            }
            else if (CurTray.RowNums.GetValue == 1 && CurTray.ColNums.GetValue > 1)
            {
                tb_Control_1_X.IsEnabled = true;
                tb_Control_2_X.IsEnabled = true;
                tb_Control_3_X.IsEnabled = false;
                tb_Control_4_X.IsEnabled = false;
                img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_1.jpg", UriKind.Relative));
            }
            else if (CurTray.RowNums.GetValue > 1 && CurTray.ColNums.GetValue == 1)
            {
                tb_Control_1_X.IsEnabled = true;
                tb_Control_2_X.IsEnabled = false;
                tb_Control_3_X.IsEnabled = true;
                tb_Control_4_X.IsEnabled = false;
                img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_2.jpg", UriKind.Relative));
            }
            else if (CurTray.RowNums.GetValue > 1 && CurTray.ColNums.GetValue > 1)
            {
                tb_Control_1_X.IsEnabled = true;
                tb_Control_2_X.IsEnabled = true;
                tb_Control_3_X.IsEnabled = true;
                tb_Control_4_X.IsEnabled = true;
                img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_3.jpg", UriKind.Relative));
            }
            else
            {
                tb_Control_1_X.IsEnabled = false;
                tb_Control_2_X.IsEnabled = false;
                tb_Control_3_X.IsEnabled = false;
                tb_Control_4_X.IsEnabled = false;
                img_ControlPoint_Tip.Source = null;

            }
        }



        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurTray.Save();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btn_GenPalletPoints_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurTray.Fun_CalcPalletPoints();
                //uc_Boxs.Draw_Boxs(CurTray,lineAix);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            LanCustomControldll.Wnd_CheckLogin wnd_CheckLogin = new LanCustomControldll.Wnd_CheckLogin();

            wnd_CheckLogin.SetCheckLogin((s) => 
            {
                if (string.IsNullOrEmpty(s)) return false;
                if (s.Trim() == "123123") return true;
                return false;
            });
            wnd_CheckLogin.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            wnd_CheckLogin.ShowDialog();
            if(wnd_CheckLogin.IsLoginOK)
            {
                gd_RowColSettings.IsEnabled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurTray.RowNums.PropertyValueMustChanged += Fun_ReCheckControlPoints;
            CurTray.ColNums.PropertyValueMustChanged += Fun_ReCheckControlPoints;
            Fun_ReCheckControlPoints();
            //uc_Boxs.Draw_Boxs(CurTray,lineAix);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CurTray.RowNums.PropertyValueMustChanged -= Fun_ReCheckControlPoints;
            CurTray.ColNums.PropertyValueMustChanged -= Fun_ReCheckControlPoints;
        }

        private async void btn_FirstPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurTray.Points.Count == 0) return;
                this.IsEnabled = false;
                
                await Task.Run(() =>
                {
                    try
                    {
                        lineAix.LineMovAbs(CurTray.Points[0].X, CurTray.Points[0].Y);
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue=0;
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(ex.Message);
                        });

                    }
                });


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private async void btn_LastPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurTray.Points.Count == 0) return;
                this.IsEnabled = false;

                await Task.Run(() =>
                {
                    try
                    {
                        lineAix.LineMovAbs(CurTray.Points[CurTray.Points.Count - 1].X, CurTray.Points[CurTray.Points.Count - 1].Y);
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue = CurTray.Points.Count-1;
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(ex.Message);
                        });

                    }
                });


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.IsEnabled = true;
            }

        }

        private async void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i=uc_Boxs.Index.GetValue+1;

                await Task.Run(()=>
                {
                    try
                    {
                        if(i< CurTray.Points.Count)
                        {
                            lineAix.LineMovAbs(CurTray.Points[i].X, CurTray.Points[i].Y);
                           
                        }
                        else
                        {
                            lineAix.LineMovAbs(CurTray.Points[0].X, CurTray.Points[0].Y);
                            i = 0;
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue = i;
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(()=>
                        {
                            MessageBox.Show(ex.Message);
                        });
                        
                    }
                });


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private async void btn_Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i = uc_Boxs.Index.GetValue-1;

                await Task.Run(() =>
                {
                    try
                    {
                        if (i >= 0)
                        {
                            lineAix.LineMovAbs(CurTray.Points[i].X, CurTray.Points[i].Y);
                           
                        }
                        else
                        {
                            lineAix.LineMovAbs(CurTray.Points[CurTray.Points.Count-1].X, CurTray.Points[CurTray.Points.Count - 1].Y);
                            i = CurTray.Points.Count - 1;
                           
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue = i;
                        });

                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(ex.Message);
                        });

                    }
                });


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        public void MovIndexPoint()
        {
            int i = uc_Boxs.Index.GetValue;
            lineAix.LineMovAbs(CurTray.Points[i].X, CurTray.Points[i].Y);
        }

        public event Action<int> ActionAutoCamTakeEvent;
        //private async void btn_AutoCamTake_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        this.IsEnabled = false;
        //        int i = uc_Boxs.Index.GetValue;
        //        await Task.Run(() =>
        //        {
        //            try
        //            {
        //                ActionAutoCamTakeEvent?.Invoke(i);

        //            }
        //            catch (Exception ex)
        //            {
        //                this.Dispatcher.Invoke(() =>
        //                {
        //                    MessageBox.Show(ex.Message);
        //                });

        //            }
        //        });


        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        this.IsEnabled = true;
        //    }
        //}
    }
}
