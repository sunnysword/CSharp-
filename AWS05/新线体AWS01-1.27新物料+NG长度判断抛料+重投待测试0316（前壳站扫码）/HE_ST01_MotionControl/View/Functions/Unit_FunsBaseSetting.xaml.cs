using AccustomeAttributedll;
using AixCommInfo;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Handler.Funs;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_FunsBaseSetting.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_FunsBaseSetting : UserControl
    {
        public Unit_FunsBaseSetting(Funs.FunsSelectionHelperbase model)
        {
            InitializeComponent();
            FunctionInstance = model;
        }

        public Funs.FunsSelectionHelperbase FunctionInstance { get; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetFunsSelections(FunctionInstance);
        }

        public void SetFunsSelections(Funs.FunsSelectionHelperbase model)
        {
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
                    switch (vr.Name)
                    {
                        case "ForqueWorkDownLimit":
                        case "ForqueDownDownLimit":
                        case "AngelWorkDownLimit":
                        case "AngelDownDownLimit":
                            textBox.IsReadOnly = true;
                            break;
                    }
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
    }
}
