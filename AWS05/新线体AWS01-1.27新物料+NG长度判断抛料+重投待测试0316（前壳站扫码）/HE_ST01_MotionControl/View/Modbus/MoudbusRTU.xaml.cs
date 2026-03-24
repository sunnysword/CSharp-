using Handler.Connect;
using Handler.Connect.Modbus;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace Handler.View.MoudbusRTU
{
    /// <summary>
    /// MoudbusRTU.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusRTU : Window
    {
        ModbusHelper Helper = null;

        private ModbusRTU()
        {
            InitializeComponent();
        }

        bool IsStop = false;

        public ModbusRTU(ModbusHelper invoker) : this()
        {
            this.Helper = invoker;
        }

        private void MoudbusRTU_Loaded(object sender, RoutedEventArgs e)
        {
            #region 串口设定的数据绑定
            //写入单个线圈 写入值到DO 功能码05
            //批量写入线圈 写多线圈寄存器 功能码15
            //写入单个寄存器 写入值到AO 功能码06
            //批量写入寄存器 写多个保持寄存器 功能码16

            //读取输出线圈 读取DO 功能码01
            //读取输入线圈 读取DI 功能码02
            //读取保持型寄存器  读取AO值  功能吗03
            //读取输入寄存器 读取AI 功能码04
            //
            //
            //
            //
            //
            //功能码
            string[] RTUFunctionCode = {
                "01 读取输出线圈 读取DO", "02 读取输入线圈 读取DI", "03 读取保持型寄存器  读取AO值", "04 读取输入寄存器 读取AI",
                "05 写入单个线圈 写入值到DO", "06 写入单个寄存器 写入值到AO", "0F 批量写入线圈 写多线圈寄存器 功能码15", "10 批量写入寄存器 写多个保持寄存器 功能码16" };
            comb_FunctionCode.ItemsSource = RTUFunctionCode;

            #endregion

        }

        private void MoudbusRTU_Closed(object sender, EventArgs e)
        {
            IsStop = true;
            //SDCOM.Save();
        }

        //将获取的信息展示在界面
        void ShowMsg(string str)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                tb_RichTextBox.AppendText(str + " ");
            }));
        }

        public void ShowMsg<T>(T[] values)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                tb_RichTextBox.Clear();
                tb_RichTextBox.AppendText(DateTime.Now.ToString() + "\r\n");
            }));
            foreach (var item in values)
            {
                ShowMsg(item.ToString());
            }
        }



        private void ExecuteFunction()
        {
            short[] temp = new short[10];
            double[] temp_value = new double[10];
            try
            {
                var content = comb_FunctionCode.Text;
                var strStartAddress = tb_ReadStartAdress.Text;
                var strLength = tb_ReadLength.Text;
                ushort startAddress = ushort.Parse(strStartAddress);
                ushort length = ushort.Parse(strLength);

                switch (content)
                {
                    //  string[] RTUFunctionCode = { 
                    // "01 读取输出线圈 读取DO", "02 读取输入线圈 读取DI", "03 读取保持型寄存器  读取AO值", "04 读取输入寄存器 读取AI",
                    case "01 读取输出线圈 读取DO"://读取单个线圈

                        var result = this.Helper.ReadCoils(startAddress, length);
                        ShowMsg(values: result);
                        break;

                    case "02 读取输入线圈 读取DI"://读取输入线圈/离散量线圈
                        ShowMsg(
                            values:
                            this.Helper.ReadInputs(startAddress, length)
                            );
                        break;

                    case "03 读取保持型寄存器  读取AO值"://读取保持寄存器
                        ShowMsg(
                           values:
                           this.Helper.ReadHoldingRegisters(startAddress, length)
                           );
                        break;
                    case "04 读取输入寄存器 读取AI"://读取输入寄存器
                        ShowMsg(
                            values:
                            this.Helper.ReadInputRegisters(startAddress, length)
                            );
                        break;
                    //"05 写入单个线圈 写入值到DO", "06 写入单个寄存器 写入值到AO", "0F 批量写入线圈 写多线圈寄存器 功能码15", "10 批量写入寄存器 写多个保持寄存器 功能码16" };

                    case "05 写入单个线圈 写入值到DO"://写单个线圈
                        if (this.tb_Date.Text == "1")
                        {
                            this.Helper.WriteSingleCoil(startAddress, true);
                        }
                        else
                        {
                            this.Helper.WriteSingleCoil(startAddress, false);
                        }
                        break;
                    case "06 写入单个寄存器 写入值到AO"://写单个输入线圈/离散量线圈
                        this.Helper.WriteSingleRegister(startAddress, ushort.Parse(this.tb_Date.Text));
                        break;
                    case "0F 批量写入线圈 写多线圈寄存器 功能码15"://写一组线圈
                        List<bool> list = new List<bool>();
                        if (this.tb_Date.Text.Contains(','))
                        {
                            var array = this.tb_Date.Text.Split(',');
                            foreach (var item in array)
                            {
                                if (item == "1" || item.ToUpper() == "TRUE")
                                {
                                    list.Add(true);
                                }
                                else
                                {
                                    list.Add(false);
                                }
                            }
                        }
                        this.Helper.WriteArrayCoil(startAddress, list.ToArray());
                        break;
                    case "10 批量写入寄存器 写多个保持寄存器 功能码16"://写一组保持寄存器
                        List<ushort> list2 = new List<ushort>();
                        if (this.tb_Date.Text.Contains(','))
                        {
                            var array = this.tb_Date.Text.Split(',');
                            foreach (var item in array)
                            {
                                list2.Add(ushort.Parse(item));

                            }
                        }
                        this.Helper.WriteArrayRegister(startAddress, list2.ToArray());
                        break;
                    default:
                        MessageBox.Show("请选择功能码");
                        break;
                }

                //1 3 5 6 10
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// SetMessage
        /// </summary>
        /// <param name="msg"></param>
        public void SetMsg(string msg)
        {
            this.Dispatcher.Invoke(new Action(() => { tb_RichTextBox.AppendText(msg); }));
        }

        private void Btn_Read_Write_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFunction();
        }

        public void AddButton(string content, Action buttonClick)
        {
            Button button = new Button
            {
                Content = content
            };
            button.Click += (s,e) =>
            {
                try
                {
                    buttonClick?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            this.stackPanel.Children.Add(button);
        }
    }
}
