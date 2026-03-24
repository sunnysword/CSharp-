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


namespace Handler.View.DevManager
{
    /// <summary>
    /// Wnd_DevLifeSetting.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_DevLifeSetting : Window
    {
        public Wnd_DevLifeSetting()
        {
            InitializeComponent();
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(this, IUserLimitdll.UserLevelModes.ModifyAndOperation);
        }
        void SetParam(DevLifeManager otherParamHelper)
        {
            //添加位置组件
            int RowIndex = 0;//一开始时为0
            Type type = otherParamHelper.GetType();
            var propertyInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(s => s.IsDefined(typeof(TextBoxRemarkAttribute), true));
            foreach (var vr in propertyInfos)
            {

                if (vr.FieldType.Name == typeof(DevLifeManager.DevLifeUseInfo<>).Name)
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
                    if (!tempRemark.Contains(':'))
                    {
                        tempRemark = "：" + tempRemark;
                    }
                    textBlock.Text = tempRemark;
                    textBlock.FontSize = 15;
                    gd_Text_1.Children.Add(textBlock);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(textBlock, RowIndex);
                    Grid.SetColumn(textBlock, 1);
                    //添加输入框
                    TextBox textBox = new TextBox();
                    textBox.Height = 25;
                    textBox.Width = 100;
                    textBox.FontSize = 15;
                    textBox.Margin = new Thickness(0, 5, 5, 5);
                    textBox.VerticalContentAlignment = VerticalAlignment.Bottom;
                    gd_Text_1.Children.Add(textBox);
                    Grid.SetRow(textBox, RowIndex);
                    Grid.SetColumn(textBox, 0);
                    //绑定
                    Binding binding = new Binding();
                    binding.Source = singleMsgSource;
                    binding.Path = new PropertyPath("MaxValue");
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                    textBox.SetBinding(TextBox.TextProperty, binding);

                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetParam(DevLifeManager.Cur);
            btn_SaveParam.SetSaveClick(delegate
            {
                DevLifeManager.Cur.Save();
            });
        }
    }
}
