using Handler.Connect;
using Handler.Connect.Modbus;
using Handler.Modbus;
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

namespace Handler.View.Gripper
{
    /// <summary>
    /// ViewPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class ViewGripper : UserControl
    {
        ModbusGripper_Hsl Helper = null;

        public ViewGripper()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 夹爪连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGripperConn_Click(object sender, RoutedEventArgs e)
        {
            this.Helper = null;

            //if (cmbGripperSel.Text == "前壳夹爪") this.Helper = ConnectFactory.FrontShellGripper_Hsl;
            //if (cmbGripperSel.Text == "PCB夹爪") this.Helper = ConnectFactory.PCBGripper_Hsl;


            if (this.Helper == null)
            {
                MessageBox.Show("夹爪连接失败！");
            }
            else
            {
                MessageBox.Show("夹爪连接成功！");
                lblInitResult.Content = Get_GripperInitState();
            }
        }
        /// <summary>
        /// 夹爪初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGripperInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startAddress = "256";
                ushort value = 1;

                this.Helper.WriteSingleRegister(startAddress, value);
                for(int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    lblInitResult.Content = Get_GripperInitState();
                    if (lblInitResult.Content.ToString() == "初始化成功") break;
                }
                if (lblInitResult.Content.ToString() == "初始化成功")
                {
                    MessageBox.Show("夹爪初始化成功！");
                }
                else
                {
                    MessageBox.Show("夹爪初始化失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 获取夹爪初始化状态
        /// </summary>
        /// <returns></returns>
        private string Get_GripperInitState()
        {
            try
            {
                string startAddress = "512";
                short value;
                string state = "";

                value = this.Helper.ReadSingleRegister(startAddress);

                if (value == 0) state = "未初始化";
                if (value == 1) state = "初始化成功";

                return state;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取夹爪初始化状态失败！错误信息：" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// 夹爪夹持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGripperClamp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startAddress = "259";
                ushort value = ushort.Parse(txtClampDistance.Text);
                string state = "";
                ushort result = 999;

                this.Helper.WriteSingleRegister(startAddress, value);
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    state = Get_GripperClampState();
                    if (state != "运动中") break;
                }
                result = Get_GripperClampResult();
                CurrentDistance.Text = this.Helper.ReadSingleRegisterU("514").ToString();//读取当前夹爪位置信息
                MessageBox.Show("夹爪夹持完成！夹持状态：" + state + " ； 夹持距离：" + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 夹爪张开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGripperOpenClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Helper.WriteSingleRegister("259", (short)1000);
                for (int i = 0; i < 300; i++)
                {
                    Thread.Sleep(10);
                    if (this.Helper.ReadSingleRegister("513") != 0) break;
                }
                CurrentDistance.Text = this.Helper.ReadSingleRegisterU("514").ToString();//读取当前夹爪位置信息
                if (this.Helper.ReadSingleRegisterU("514") >= (short)1000)
                    MessageBox.Show("夹爪松开成功！");
                else
                    MessageBox.Show("夹爪松开失败！");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private string Get_GripperClampState()
        {
            try
            {
                string startAddress = "513";
                short value;
                string state = "";

                value = this.Helper.ReadSingleRegister(startAddress);
                if (value == 0) state = "运动中";
                if (value == 1) state = "到达位置";
                if (value == 2) state = "夹住物体";
                if (value == 3) state = "物体掉落";

                return state;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取夹爪夹持状态失败！错误信息：" + ex.Message);
                return "";
            }
        }

        private ushort Get_GripperClampResult()
        {
            try
            {
                string startAddress = "514";
                ushort value;

                value = this.Helper.ReadSingleRegisterU(startAddress);

                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取夹爪夹持位置失败！错误信息：" + ex.Message);
                return 999;
            }
        }
        /// <summary>
        /// 夹爪旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGripperRotate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startAddress = "261";
                short value = short.Parse(txtRotationAngle.Text);
                string state = "";
                short result = 999;

                this.Helper.WriteSingleRegister(startAddress, value);
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    state = Get_GripperRotateState();
                    if (state != "运动中") break;
                }
                result = Get_GripperRotateResult();
                CurrentAngle.Text = this.Helper.ReadSingleRegisterU("520").ToString();
                MessageBox.Show("夹爪旋转完成！旋转状态：" + state + " ； 旋转角度：" + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string Get_GripperRotateState()
        {
            try
            {
                string startAddress = "523";
                short value;
                string state = "";

                value = this.Helper.ReadSingleRegister(startAddress);
                if (value == 0) state = "运动中";
                if (value == 1) state = "到达位置";
                if (value == 2) state = "堵转";

                return state;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取夹爪旋转状态失败！错误信息：" + ex.Message);
                return "";
            }
        }

        private short Get_GripperRotateResult()
        {
            try
            {
                string startAddress = "520";
                short value;

                value = this.Helper.ReadSingleRegister(startAddress);

                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取夹爪旋转角度失败！错误信息：" + ex.Message);
                return 999;
            }
        }
    }
}
