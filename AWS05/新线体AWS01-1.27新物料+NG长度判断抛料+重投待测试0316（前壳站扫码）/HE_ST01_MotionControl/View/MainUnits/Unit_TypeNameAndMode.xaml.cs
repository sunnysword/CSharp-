using LanCustomControldll;
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
using Handler.Process.RunMode;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Unit_TypeNameAndMode.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_TypeNameAndMode : UserControl, IShow
    {
        public Unit_TypeNameAndMode()
        {
            InitializeComponent();
        }


        public void ShowUI()
        {
            tb_LoginUsers.Text = Wpf_LoginWnd.LoginHelper.Cur.CurUserMsg.Category.ToString() + " | " + Wpf_LoginWnd.LoginHelper.Cur.CurUserMsg.ID;
            tb_TypeName.Text = WorkItemManagerHelper.LoadedName;
            tb_Mode.Text =WorkModeManager.Cur.SelectWorkMode.Name;

            //tb_WorkOrder.Text = MES_YF_dll.MesProxyHelper.MO;
        }
    }
}
