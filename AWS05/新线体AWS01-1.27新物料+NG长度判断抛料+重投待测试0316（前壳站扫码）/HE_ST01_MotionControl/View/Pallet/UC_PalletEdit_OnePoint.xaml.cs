using AbstractLineAixsdll;
using AccustomeAttributedll;
using AixCommInfo;
using AM.Core.IO;
using Handler.Connect.Modbus;
using Handler.Modbus;
using Handler.Motion.Axis;
using Handler.Motion.PalletTray;
using HE_ST01_MotionControl.Connect.Modbus;
using LanCustomControldll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


namespace Handler.View.Pallet
{

    /// <summary>
    /// UC_PalletEdit_AAUnload.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PalletEdit_OnePoint : UserControl
    {
        private UC_PalletEdit_OnePoint()
        {
            InitializeComponent();
        }

        public UC_PalletEdit_OnePoint(PalletInfoOnePoint palletInfo, ThreeAxisBase _lineAix) : this()
        {
            CurTray = palletInfo;
            lineAix = _lineAix;
            Fun_SetBinding();


        }
        PalletInfoOnePoint CurTray;
        ThreeAxisBase lineAix;

        //public Cylinder CylinderClmap;
        // public ModbusGripper_Hsl Gripper;
        public ClampRotatedAxisBase Gripper;
        private void Fun_SetBinding()
        {
            void TextBinding<T>(ParamStructText<T> paramStructText, TextBox textBox) where T : struct
            {
                Binding binding = new Binding();
                binding.Source = paramStructText;
                binding.Path = new PropertyPath("GetValue");
                textBox.SetBinding(TextBox.TextProperty, binding);

            }
            TextBinding(uc_Boxs.Index, tb_PointIndex);


            TextBinding(CurTray.RowNums, tb_Dims_X);
            TextBinding(CurTray.ColNums, tb_Dims_Y);

            void TextPointBinding(TrayPoint trayPoint, UC_TextBoxAndButton uC_TextBoxAnd_X, UC_TextBoxAndButton uC_TextBoxAnd_Y,
                UC_TextBoxAndButton uC_TextBoxAnd_Z, UC_TextBoxAndButton uC_TextBoxAnd_U)
            {
                Binding binding_x = new Binding();
                binding_x.Source = trayPoint;
                binding_x.Path = new PropertyPath("X");
                uC_TextBoxAnd_X.tb_Msg.SetBinding(TextBox.TextProperty, binding_x);

                Binding binding_y = new Binding();
                binding_y.Source = trayPoint;
                binding_y.Path = new PropertyPath("Y");
                uC_TextBoxAnd_Y.tb_Msg.SetBinding(TextBox.TextProperty, binding_y);

                Binding binding_z = new Binding();
                binding_z.Source = trayPoint;
                binding_z.Path = new PropertyPath("Z");
                uC_TextBoxAnd_Z.tb_Msg.SetBinding(TextBox.TextProperty, binding_z);

                Binding binding_u = new Binding();
                binding_u.Source = trayPoint;
                binding_u.Path = new PropertyPath("U");
                uC_TextBoxAnd_U.tb_Msg.SetBinding(TextBox.TextProperty, binding_u);

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

                uC_TextBoxAnd_Z.btn_GetPos.Click += (s, e) =>
                {
                    try
                    {
                        uC_TextBoxAnd_Z.tb_Msg.Text = lineAix.Aix_Z.GetCmdPos().ToString();
                        uC_TextBoxAnd_Z.tb_Msg.Focus();
                        Thread.Sleep(100);
                        uC_TextBoxAnd_Z.btn_GetPos.Focus();

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                };

                //uC_TextBoxAnd_U.btn_GetPos.Click += (s, e) =>
                //{
                //    try
                //    {
                //        uC_TextBoxAnd_U.tb_Msg.Text = lineAix.Axis4.GetCmdPos().ToString();
                //        uC_TextBoxAnd_U.tb_Msg.Focus();
                //        Thread.Sleep(100);
                //        uC_TextBoxAnd_U.btn_GetPos.Focus();

                //    }
                //    catch (Exception ex)
                //    {

                //        MessageBox.Show(ex.Message);
                //    }
                //};
            }

            TextPointBinding(CurTray.Control_Point1, tb_Control_1_X, tb_Control_1_Y,
               tb_Control_1_Z, tb_Control_1_U);
            TextPointBinding(CurTray.Control_Point2, tb_Control_2_X, tb_Control_2_Y,
                tb_Control_2_Z, tb_Control_2_U);


            Binding binding1 = new Binding();
            binding1.Source = CurTray.RowSpacing;
            binding1.Path = new PropertyPath("GetValue");
            tb_RowSpacing.SetBinding(TextBox.TextProperty, binding1);

            Binding binding2 = new Binding();
            binding2.Source = CurTray.ColumnSpacing;
            binding2.Path = new PropertyPath("GetValue");
            tb_ColumnSpacing.SetBinding(TextBox.TextProperty, binding2);

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


            //Binding binding4 = new Binding();
            //binding4.Source = CurTray.U_Standard;
            //binding4.Path = new PropertyPath("GetValue");
            //tb_U_Standard.tb_Msg.SetBinding(TextBox.TextProperty, binding4);
            //tb_U_Standard.btn_GetPos.Click += (s, e) =>
            //{
            //    //try
            //    //{
            //    //    tb_U_Standard.tb_Msg.Text = lineAix.Aix.GetCmdPos().ToString();
            //    //    tb_U_Standard.tb_Msg.Focus();
            //    //    Thread.Sleep(100);
            //    //    tb_U_Standard.btn_GetPos.Focus();

            //    //}
            //    //catch (Exception ex)
            //    //{

            //    //    MessageBox.Show(ex.Message);
            //    //}
            //};

            Dg_Msg.ItemsSource = CurTray.Points;

            SetFunsSelections();





        }

        private void Fun_ReCheckControlPoints()
        {
            if (CurTray.RowNums.GetValue == 1 && CurTray.ColNums.GetValue == 1)
            {
                tb_Control_1_X.IsEnabled = true;

                //img_ControlPoint_Tip.Source = null;
            }
            else if (CurTray.RowNums.GetValue == 1 && CurTray.ColNums.GetValue > 1)
            {
                tb_Control_1_X.IsEnabled = true;

                //img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_1.jpg", UriKind.Relative));
            }
            else if (CurTray.RowNums.GetValue > 1 && CurTray.ColNums.GetValue == 1)
            {
                tb_Control_1_X.IsEnabled = true;

                //img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_2.jpg", UriKind.Relative));
            }
            else if (CurTray.RowNums.GetValue > 1 && CurTray.ColNums.GetValue > 1)
            {
                tb_Control_1_X.IsEnabled = true;

                //img_ControlPoint_Tip.Source = new BitmapImage(new Uri("/Resources/Point_3.jpg", UriKind.Relative));
            }
            else
            {
                tb_Control_1_X.IsEnabled = false;

                //img_ControlPoint_Tip.Source = null;

            }
        }

        public void SetFunsSelections()
        {
            object model = CurTray;
            if (model == null)
                return;

            this.gd_1.Children.Clear();
            Type type = model.GetType();

            var memberInfos = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(s => s.IsDefined(typeof(FunctionSelectionRemarkAttribute), true) == true);
            int rowIndex = 0;
            //之前的启用禁用的设置
            foreach (var item in memberInfos)
            {
                if (item.MemberType == MemberTypes.Field)
                {
                    FieldInfo fieldInfo = item as FieldInfo;
                    object singleMsgSource = fieldInfo.GetValue(model);
                    FunctionSelectionRemarkAttribute paramRemarkAttribute = (FunctionSelectionRemarkAttribute)item.GetCustomAttributes(typeof(FunctionSelectionRemarkAttribute), true)[0];
                    //添加行
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    gd_1.RowDefinitions.Add(rowDefinition);
                    rowIndex = gd_1.RowDefinitions.IndexOf(rowDefinition);
                    TextBlock textBlock = new TextBlock();
                    string tempRemark = paramRemarkAttribute.GetRemark();
                    tempRemark = LanguageControlBase.LanguageSearchHelper.GetLanguageExplain(tempRemark);
                    //if (!tempRemark.Contains(':'))
                    //{
                    //    tempRemark = textBlock.Text + "：";
                    //}
                    textBlock.Text = tempRemark;
                    textBlock.FontSize = 15;
                    gd_1.Children.Add(textBlock);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(textBlock, rowIndex);
                    Grid.SetColumn(textBlock, 0);


                    //初始化控件
                    if (fieldInfo.FieldType.Name == typeof(ParamStructText<>).Name)
                    {
                        paramRemarkAttribute.SetControlIntial(bool.Parse(((ParamStructText<bool>)singleMsgSource).GetValue.ToString()));

                    }
                    else if (fieldInfo.FieldType == typeof(bool))
                    {
                        paramRemarkAttribute.SetControlIntial(bool.Parse(singleMsgSource.ToString()));
                    }
                    else
                    {
                        continue;
                    }
                    //添加设置控件
                    Control control = paramRemarkAttribute.GetControl();

                    control.Height = 25;
                    control.Width = 200;
                    control.FontSize = 15;
                    control.Margin = new Thickness(0, 5, 5, 5);
                    control.VerticalContentAlignment = VerticalAlignment.Bottom;
                    control.HorizontalAlignment = HorizontalAlignment.Left;
                    gd_1.Children.Add(control);
                    Grid.SetRow(control, rowIndex);
                    Grid.SetColumn(control, 2);

                    //设置事件
                    paramRemarkAttribute.actionUseTrigger += () =>
                    {
                        if (fieldInfo.FieldType.Name == typeof(ParamStructText<>).Name)
                        {
                            ParamStructText<bool> msgText = singleMsgSource as ParamStructText<bool>;
                            msgText.GetValue = true;
                        }
                        else
                        {
                            Type tt = (fieldInfo.GetValue(model)).GetType();

                            fieldInfo.SetValue(model, Convert.ChangeType(true, tt));

                        }


                    };

                    paramRemarkAttribute.actionLimitUseTrigger += () =>
                    {
                        if (fieldInfo.FieldType.Name == typeof(ParamStructText<>).Name)
                        {
                            ParamStructText<bool> msgText = singleMsgSource as ParamStructText<bool>;
                            msgText.GetValue = false;
                        }
                        else
                        {
                            Type tt = (fieldInfo.GetValue(model)).GetType();

                            fieldInfo.SetValue(model, Convert.ChangeType(false, tt));
                        }


                    };
                }



            }


            //OtherParam
            var propertyInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(s => !s.IsInitOnly && s.IsDefined(typeof(TextBoxRemarkAttribute), true));
            foreach (var vr in propertyInfos)
            {

                if (vr.FieldType.Name == typeof(ParamStructText<>).Name || vr.FieldType.Name == typeof(ParamObjectText<>).Name)
                {
                    object singleMsgSource = vr.GetValue(model);
                    //获取位置信息标识
                    TextBoxRemarkAttribute paramRemarkAttribute = (TextBoxRemarkAttribute)vr.GetCustomAttributes(typeof(TextBoxRemarkAttribute), true)[0];
                    //添加行
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    gd_1.RowDefinitions.Add(rowDefinition);
                    rowIndex = gd_1.RowDefinitions.IndexOf(rowDefinition);

                    //StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    //Grid.SetRow(stackPanel, rowIndex);
                    //Grid.SetColumn(stackPanel, 0);
                    //gd_1.Children.Add(stackPanel);

                    TextBlock textBlock = new TextBlock();
                    string tempRemark = paramRemarkAttribute.GetRemark();
                    tempRemark = LanguageControlBase.LanguageSearchHelper.GetLanguageExplain(tempRemark);
                    if (!tempRemark.Contains(':'))
                    {
                        tempRemark = "：" + tempRemark;
                    }
                    textBlock.Text = tempRemark;
                    textBlock.FontSize = 15;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(textBlock, rowIndex);
                    Grid.SetColumn(textBlock, 2);
                    gd_1.Children.Add(textBlock);

                    //添加输入框
                    TextBox textBox = new TextBox();
                    textBox.Height = 25;
                    textBox.Width = 100;
                    textBox.FontSize = 15;
                    textBox.Margin = new Thickness(0, 5, 5, 5);
                    textBox.VerticalContentAlignment = VerticalAlignment.Bottom;
                    gd_1.Children.Add(textBox);
                    Grid.SetRow(textBox, rowIndex);
                    Grid.SetColumn(textBox, 0);
                    //stackPanel.Children.Add(textBox);
                    //stackPanel.Children.Add(textBlock);
                    //绑定
                    Binding binding = new Binding();
                    binding.Source = singleMsgSource;
                    binding.Path = new PropertyPath("GetValue");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                    textBox.SetBinding(TextBox.TextProperty, binding);

                }
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
                uc_Boxs.Draw_Boxs(CurTray, lineAix);
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
            if (wnd_CheckLogin.IsLoginOK)
            {
                gd_RowColSettings.IsEnabled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurTray.RowNums.PropertyValueMustChanged += Fun_ReCheckControlPoints;
            CurTray.ColNums.PropertyValueMustChanged += Fun_ReCheckControlPoints;
            Fun_ReCheckControlPoints();
            uc_Boxs.Draw_Boxs(CurTray, lineAix);
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
                        lineAix.XY_MovAbs(CurTray.Points[0].X, CurTray.Points[0].Y);
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue = 0;
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
                        lineAix.XY_MovAbs(CurTray.Points[CurTray.Points.Count - 1].X, CurTray.Points[CurTray.Points.Count - 1].Y);
                        this.Dispatcher.Invoke(() =>
                        {
                            uc_Boxs.Index.GetValue = CurTray.Points.Count - 1;
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
                int i = uc_Boxs.Index.GetValue + 1;

                await Task.Run(() =>
                {
                    try
                    {
                        lineAix.Z_MovWait();

                        if (i < CurTray.Points.Count)
                        {
                            lineAix.XY_MovAbs(CurTray.Points[i].X, CurTray.Points[i].Y);

                        }
                        else
                        {
                            lineAix.XY_MovAbs(CurTray.Points[0].X, CurTray.Points[0].Y);
                            i = 0;
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

        private async void btn_Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i = uc_Boxs.Index.GetValue - 1;

                await Task.Run(() =>
                {
                    try
                    {
                        if (i >= 0)
                        {
                            lineAix.XY_MovAbs(CurTray.Points[i].X, CurTray.Points[i].Y);

                        }
                        else
                        {
                            lineAix.XY_MovAbs(CurTray.Points[CurTray.Points.Count - 1].X, CurTray.Points[CurTray.Points.Count - 1].Y);
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
        private async void btn_ZDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i = uc_Boxs.Index.GetValue ;

                await Task.Run(() =>
                {
                    try
                    {
                        if (i >= 0)
                        {
                            lineAix.Z_MovAbs(CurTray.Points[i].Z);

                        }
                        else
                        {
                            lineAix.Z_MovAbs(CurTray.Points[0].Z);
                            i = 0;

                        }


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

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    lineAix.Stop();
                    lineAix.StopXYZ();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btn_Clamp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i = uc_Boxs.Index.GetValue - 1;

                await Task.Run(() =>
                {
                    try
                    {
                        if (Gripper == null) return;
                        Gripper.Clamp_ON();                       
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

        private async void btn_ZUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                await Task.Run(() =>
                {
                    try
                    {
                        lineAix.Z_MovAbs(lineAix.PosWait.PosMsg_3.GetValue);
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

        private async void btn_Clamp_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                int i = uc_Boxs.Index.GetValue - 1;

                await Task.Run(() =>
                {
                    try
                    {
                        if (Gripper == null) return;
                        Gripper.Clamp_OFF();
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


 
    }
}
