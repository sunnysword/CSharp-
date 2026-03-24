using AccustomeAttributedll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Handler.Motion;
using static Handler.Motion.DevLifeManager;

namespace Handler.View.DevManager
{
    /// <summary>
    /// Wnd_LifeShowManager.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_LifeShowManager : Window
    {
        public Wnd_LifeShowManager()
        {
            InitializeComponent();
        }

        void SetParam(DevLifeManager otherParamHelper)
        {
            //添加位置组件
            int RowIndex = 0;//一开始时为0
            Type type = otherParamHelper.GetType();
            var propertyInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(s => s.IsDefined(typeof(TextBoxRemarkAttribute), true));
            foreach (var vr in propertyInfos)
            {

                if (vr.FieldType.Name == typeof(DevLifeUseInfo<>).Name)
                {


                    object singleMsgSource = vr.GetValue(otherParamHelper);
                    //获取位置信息标识
                    TextBoxRemarkAttribute paramRemarkAttribute = (TextBoxRemarkAttribute)vr.GetCustomAttributes(typeof(TextBoxRemarkAttribute), true)[0];
                    //添加行
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = GridLength.Auto;
                    gd_Text_1.RowDefinitions.Add(rowDefinition);

                    RowIndex = gd_Text_1.RowDefinitions.IndexOf(rowDefinition);
                    TextBlock textBlock = new TextBlock();
                    string tempRemark = paramRemarkAttribute.GetRemark();
                    tempRemark = LanguageControlBase.LanguageSearchHelper.GetLanguageExplain(tempRemark);
                    //if (!tempRemark.Contains(':'))
                    //{
                    //    tempRemark = "：" + tempRemark;
                    //}
                    textBlock.Text = tempRemark;
                    textBlock.FontSize = 15;
                    gd_Text_1.Children.Add(textBlock);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(textBlock, RowIndex);
                    Grid.SetColumn(textBlock, 1);
                    //添加输入框
                    TextBox textBox_1 = new TextBox();

                    textBox_1.Margin = new Thickness(5);
                    textBox_1.VerticalContentAlignment = VerticalAlignment.Bottom;
                    gd_Text_1.Children.Add(textBox_1);
                    textBox_1.IsReadOnly = true;
                    Grid.SetRow(textBox_1, RowIndex);
                    Grid.SetColumn(textBox_1, 3);
                    //绑定
                    Binding binding_1 = new Binding();
                    binding_1.Source = singleMsgSource;
                    binding_1.Path = new PropertyPath("CurrentValue");
                    binding_1.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                    textBox_1.SetBinding(TextBox.TextProperty, binding_1);

                    TextBox textBox_2 = new TextBox();


                    textBox_2.Margin = new Thickness(5);
                    textBox_2.VerticalContentAlignment = VerticalAlignment.Bottom;
                    gd_Text_1.Children.Add(textBox_2);
                    textBox_2.IsReadOnly = true;
                    Grid.SetRow(textBox_2, RowIndex);
                    Grid.SetColumn(textBox_2, 5);
                    //绑定
                    Binding binding_2 = new Binding();
                    binding_2.Source = singleMsgSource;
                    binding_2.Path = new PropertyPath("MaxValue");
                    binding_2.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                    textBox_2.SetBinding(TextBox.TextProperty, binding_2);

                    //添加重置按钮
                    Button button = new Button();
                    button.Content = "重置";
                    button.Click += delegate
                    {
                        try
                        {
                            LanCustomControldll.Wnd_CheckLogin wnd_CheckLogin = new LanCustomControldll.Wnd_CheckLogin();
                            wnd_CheckLogin.Title = tempRemark + "  重置确认";


                            wnd_CheckLogin.SetCheckLogin(new
                                 Func<string, bool>((s) =>
                                 {
                                     if (s == "123123")
                                     {
                                         return true;
                                     }
                                     else
                                     {
                                         return false;
                                     }
                                 })
                            );
                            wnd_CheckLogin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            wnd_CheckLogin.ShowDialog();
                            if (wnd_CheckLogin.IsLoginOK)
                            {
                                (singleMsgSource as DevLifeManager.IRstValue)?.Fun_RstCurrentValue();
                                DevLifeManager.Cur.Save();
                            }

                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
                    };
                    button.Width = 80;
                    button.Margin = new Thickness(5);
                    gd_Text_1.Children.Add(button);
                    Grid.SetRow(button, RowIndex);
                    Grid.SetColumn(button, 7);
                }
            }
            RowDefinition rowDefinition2 = new RowDefinition();

            gd_Text_1.RowDefinitions.Add(rowDefinition2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetParam(Cur);
        }
    }
}
