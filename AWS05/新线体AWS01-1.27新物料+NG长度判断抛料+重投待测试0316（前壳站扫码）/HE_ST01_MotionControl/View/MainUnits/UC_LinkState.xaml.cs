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
using AixCommInfo;
using Handler.Motion.Process.Station;
using Handler.Process.Station;
using Handler.Core.Converter;


namespace Handler.View.MainUnits
{
    /// <summary>
    /// UC_LinkState.xaml 的交互逻辑
    /// </summary>
    public partial class UC_LinkState : UserControl
    {
        public UC_LinkState()
        {
            InitializeComponent();
        }

        public void SetViewBinding()
        {
            SetBinding(this.tb_IsUseRFID, Funs.FunSelection.Cur.IsUseRFID);
            SetBinding(this.tb_MesIsUse, Funs.FunSelection.Cur.IsUseMes);
            SetBinding(this.tb_1, Funs.FunSelection.Cur.前壳是否启用扫码);
            SetBinding(this.tb_2, Funs.FunSelection.Cur.PCB是否启用扫码);
            SetBinding(this.tb_3, Funs.FunSelection.Cur.FrontHolderCheckProduct);
            SetBinding(this.tb_4, Funs.FunSelection.Cur.PCBCheckProduct);
            SetBinding(this.tb_5, Funs.FunCommon.Cur.BackConnect);
            SetBinding(this.tb_6, Funs.FunStationSetting.Cur.是否启用前壳工位);
            SetBinding(this.tb_7, Funs.FunStationSetting.Cur.是否启用pcb工位);
            SetBinding(this.tb_8, Funs.FunSelection.Cur.IsUseSafeDoor);

        }

        private void SetBinding(TextBlock textBlock,ParamStructText<bool> param)
        {
            Binding binding = new Binding();
            binding.Source = param;
            binding.Path = new PropertyPath("GetValue");
            binding.Converter = new BoolTextConverter();
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            Binding binding2 = new Binding();
            binding2.Source = param;
            binding2.Path = new PropertyPath("GetValue");
            binding2.Converter = new BoolColorConverter();
            textBlock.SetBinding(TextBlock.BackgroundProperty, binding2);
        }

        private void SetState(TextBlock textBlock,bool result)
        {
            if (result)
            {
                textBlock.Text = "启用中";
                textBlock.Background = Brushes.Green;
            }
            else
            {
                textBlock.Text = "禁用中";
                textBlock.Background = Brushes.Red;
            }
        }

        public void ShowUI()
        {
            //this.tb_ScrewGunNo.Text = Funs.FunScrewGun.Cur.ProgramNo.GetValue.ToString();

            //tb_Current1.Text = PowerHelper.Cur.current_value_A;
            //tb_Current2.Text = PowerHelper.Cur.current_value_B;
        }
    }
}
