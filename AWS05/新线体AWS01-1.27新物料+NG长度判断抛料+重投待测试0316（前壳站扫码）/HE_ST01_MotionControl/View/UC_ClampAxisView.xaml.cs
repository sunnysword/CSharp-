using AccustomeAttributedll;
using AixCommInfo;
using Handler.Modbus;
using LanCustomControldll;
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

namespace DS_SorterAndOven.View.MainUnits
{
    /// <summary>
    /// UC_ClampAxisView.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ClampAxisView : UserControl, IShow
    {
        public UC_ClampAxisView()
        {
            InitializeComponent();
            IUserLimitdll.PreControlBindingHelper.SetLitmitBinding(this, IUserLimitdll.UserLevelModes.Operation);

        }

        class RotatedAxisPosInfo
        {

            public string Name { get; set; }

            public SingleAixPosAndSpeedMsgAndIMov PosValue { get; set; }

            public ParamStructText<double> PosMsg
            {
                get
                {
                    if (PosValue == null) return null;
                    return PosValue.PosMsg;
                }
            }
        }

        ClampRotatedAxisBase ClampRotatedAxis;
        List<RotatedAxisPosInfo> RotatedAxisPos = new List<RotatedAxisPosInfo>();

        DH_ClampRotatedAxisDemo.UC_ClampRotatedView C_ClampRotatedView;
        private void GetRotatedAxisPosInfoList()
        {
            if (ClampRotatedAxis == null) return;
            itemsControlRotatedPos.ItemsSource = null;
            RotatedAxisPos.Clear();
            Type type = ClampRotatedAxis.GetType();
            var fileds = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(s => s.IsDefined(typeof(TextBoxRemarkAttribute), false));
            foreach (var item in fileds)
            {
                if (item.FieldType != typeof(SingleAixPosAndSpeedMsgAndIMov))
                {
                    continue;
                }
                var txt = item.GetCustomAttribute(typeof(TextBoxRemarkAttribute)) as TextBoxRemarkAttribute;
                if (txt == null) continue;

                RotatedAxisPosInfo temp = new RotatedAxisPosInfo();
                temp.Name = txt.GetRemark();
                temp.PosValue = item.GetValue(ClampRotatedAxis) as SingleAixPosAndSpeedMsgAndIMov;
                if (temp.PosValue == null) continue;
                RotatedAxisPos.Add(temp);
            }

            itemsControlRotatedPos.ItemsSource = RotatedAxisPos;

        }
        public void SetAxisInfo(ClampRotatedAxisBase clampRotatedAxis)
        {
            ClampRotatedAxis = clampRotatedAxis;

            clampRotatedAxis?.Read();
            GetRotatedAxisPosInfoList();
            C_ClampRotatedView = new DH_ClampRotatedAxisDemo.UC_ClampRotatedView(clampRotatedAxis);
            cc_ClampDebug.Content = C_ClampRotatedView;
            tb_RotatedSpeed.Text = clampRotatedAxis.RotatedSpeed.ToString();
        }

        public void ShowUI()
        {
            if (this.IsVisible == false) return;
            C_ClampRotatedView?.ShowUI();
            if (ClampRotatedAxis.GetAixRunState())
            {
                itemsControlRotatedPos.IsEnabled = false;
            }
            else
            {
                itemsControlRotatedPos.IsEnabled = true;

            }
        }

        private void btnClampRotatetdMovPos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button)
                {
                    var btn = sender as Button;
                    if (btn.Tag == null) return;
                    (btn.Tag as RotatedAxisPosInfo)?.PosValue.Mov(false);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                double speed = 0;
                if (double.TryParse(tb_RotatedSpeed.Text, out speed))
                {
                    ClampRotatedAxis.RotatedSpeed = (byte)speed;
                    tb_RotatedSpeed.Text = ClampRotatedAxis.RotatedSpeed.ToString();
                }
                else
                {
                    MessageBox.Show("旋转轴速度输入正确!速度只能是整数");
                    return;
                }
                ClampRotatedAxis?.Save();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tb_RotatedSpeed.Text = ClampRotatedAxis.RotatedSpeed.ToString();
        }
    }
}
