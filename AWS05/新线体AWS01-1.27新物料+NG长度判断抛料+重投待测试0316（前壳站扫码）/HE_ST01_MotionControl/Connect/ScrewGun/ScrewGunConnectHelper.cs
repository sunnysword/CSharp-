using Handler.Funs;
using Handler.Motion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HaoEn_G.Connect
{
    public class ScrewGunConnectHelper
    {
        public TCPHelper3.ConnectClientHelper Connect;

        public ScrewGunConnectHelper(IConnectDLL.IConnect ScrewConnection)
        {
            Connect = (TCPHelper3.ConnectClientHelper)ScrewConnection;
            Connect.ClientConnectedEventHander += Initial;
            //订阅收到螺丝枪数据时的事件
            Connect.RecevieMsgActionEventHander += ParseStr;
        }
        public Action<string> WriteToUserEventHandler;


        public void WriteToUser(string msg)
        {
            WriteToUserEventHandler?.Invoke(msg);
        }

        /// <summary>
        /// 最终扭矩
        /// </summary>
        public double Final_torque = 0;

        /// <summary>
        /// 扭矩上限值
        /// </summary>
        public double Final_Up_torque = 0;
        /// <summary>
        /// 扭矩下限值
        /// </summary>
        public double Final_Down_torque = 0;

        /// <summary>
        /// 最终旋转角度
        /// </summary>
        public double Final_angle = 0;

        /// <summary>
        /// 角度上限值
        /// </summary>
        public double Final_Up_angle = 0;

        /// <summary>
        /// 角度下限值
        /// </summary>
        public double Final_Down_angle = 0;

        /// <summary>
        /// 当前程序号
        /// </summary>
        public string ProjectNums = "001";
        public bool IsRecevied { get; private set; }

        /// <summary>
        /// 初始化后触发的事件
        /// </summary>
        public Action AfterInitialEventHandle;

        private void ParseStr(string msg)
        {
            WriteToUser("收到螺丝枪消息:" + msg);
            msg = msg.Trim();
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            string str_Final_angle = string.Empty;
            string str_Up_angle = string.Empty; // 角度上限
            string str_Down_angle = string.Empty; //下限
            string str_Final_torque = string.Empty;
            string str_Up_torque = string.Empty; // 扭矩上限
            string str_Down_torque = string.Empty; //下限


            try
            {
                if (msg.Contains("30230"))
                {
                    int start_index = msg.IndexOf("30230");
                    str_Final_torque = msg.Substring(start_index, 29);
                    str_Final_torque = str_Final_torque.Substring(str_Final_torque.Length - 9, 9);
                   
                    int indexOfE = str_Final_torque.ToLower().IndexOf("e");
                    Final_torque = double.Parse(str_Final_torque.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Final_torque.Substring(indexOfE + 1, str_Final_torque.Length - indexOfE - 1)));
                    //Final_torque = double.Parse(str_Final_torque.Substring(0, 5)) * Math.Pow(0.1, int.Parse(str_Final_torque.Substring(7, 2)));


                    //Motion.StaticInitial.Motion.WriteToUser($"Final_torque: {Final_torque} cNM");
                }
                if (msg.Contains("30231"))
                {
                    int start_index = msg.IndexOf("30231");
                    str_Final_angle = msg.Substring(start_index, 29);
                    str_Final_angle = str_Final_angle.Substring(str_Final_angle.Length - 9, 9);


                    int indexOfE = str_Final_angle.ToLower().IndexOf("e");

                    Final_angle = double.Parse(str_Final_angle.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Final_angle.Substring(indexOfE + 1, str_Final_angle.Length - indexOfE - 1)));

                    //Final_angle = double.Parse(str_Final_angle.Substring(0, 5)) * Math.Pow(10, int.Parse(str_Final_angle.Substring(7, 2)));
                    //Motion.StaticInitial.Motion.WriteToUser($"Final_angle: {Final_angle}  °");
                }
                //程序号
                if (msg.Contains("30216"))
                {
                    //int start_index = msg.IndexOf("30216");
                    //var nums = msg.Substring(start_index, 13);

                    string input = msg;
                    string keyword = "30216";
                    int index = input.IndexOf(keyword);
                    string result = input.Substring(index + keyword.Length + 12, 3);
                    ProjectNums = result;

                    WriteToUser("当前程序号："+result);
                }


                //30112（TargetTorque（目标扭矩））
                //30256（MinTorqueLimit（扭力下限））
                //30255（MaxTorqueLimit（扭力上限））
                //30258（MinAngleLimit（角度下限））
                //30257（MaxAngleLimit（角度上限））
                if (msg.Contains("30256"))
                {
                    int start_index = msg.IndexOf("30256");
                    str_Down_torque = msg.Substring(start_index, 29);
                    str_Down_torque = str_Down_torque.Substring(str_Down_torque.Length - 9, 9);
                    int indexOfE = str_Down_torque.ToLower().IndexOf("e");

                    Final_Down_torque = double.Parse(str_Down_torque.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Down_torque.Substring(indexOfE + 1, str_Down_torque.Length - indexOfE - 1)));
                }

                if (msg.Contains("30255"))
                {
                    int start_index = msg.IndexOf("30255");
                    str_Up_torque = msg.Substring(start_index, 29);
                    str_Up_torque = str_Up_torque.Substring(str_Up_torque.Length - 9, 9);
                    int indexOfE = str_Up_torque.ToLower().IndexOf("e");

                    Final_Up_torque = double.Parse(str_Up_torque.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Up_torque.Substring(indexOfE + 1, str_Up_torque.Length - indexOfE - 1)));
                }

                if (msg.Contains("30258"))
                {
                    int start_index = msg.IndexOf("30258");
                    str_Down_angle = msg.Substring(start_index, 29);
                    str_Down_angle = str_Down_angle.Substring(str_Down_angle.Length - 9, 9);
                    int indexOfE = str_Down_angle.ToLower().IndexOf("e");

                    Final_Down_angle = double.Parse(str_Down_angle.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Down_angle.Substring(indexOfE + 1, str_Down_angle.Length - indexOfE - 1)));
                }

                if (msg.Contains("30257"))
                {
                    int start_index = msg.IndexOf("30257");
                    str_Up_angle = msg.Substring(start_index, 29);
                    str_Up_angle = str_Up_angle.Substring(str_Up_angle.Length - 9, 9);
                    int indexOfE = str_Up_angle.ToLower().IndexOf("e");

                    Final_Up_angle = double.Parse(str_Up_angle.Substring(0, indexOfE)) * Math.Pow(10, double.Parse(str_Up_angle.Substring(indexOfE + 1, str_Up_angle.Length - indexOfE - 1)));
                }

                WriteToUser("扭矩下限：" + Final_Down_torque + " kgf.cm");
                FunScrewGun.Cur.ForqueWorkDownLimit.GetValue = Final_Down_torque;
                WriteToUser("扭矩上限：" + Final_Up_torque + " kgf.cm");
                FunScrewGun.Cur.ForqueDownDownLimit.GetValue = Final_Up_torque;
                WriteToUser("最终转矩：" + Final_torque + " kgf.cm");


                WriteToUser("角度上限：" + Final_Up_angle + " °");
                FunScrewGun.Cur.AngelDownDownLimit.GetValue = Final_Up_angle;
                WriteToUser("角度下限：" + Final_Down_angle + " °");
                FunScrewGun.Cur.AngelWorkDownLimit.GetValue = Final_Down_angle;
                WriteToUser("最终角度：" + Final_angle + " °");
                //LogManger.LogScrewGun.Write("最终转矩：" + Final_torque + " cNM");
                //LogManger.LogScrewGun.Write("最终角度：" + Final_angle + " °");
            }
            catch (Exception ex)
            {
                WriteToUser("解析螺丝枪信息出错:" + ex.Message);
                //Motion.StaticInitial.MotionHelper.WriteToUser($"解析螺丝枪信息{msg}出错:  {ex.Message}", Brushes.Red, false);
                //LogManger.LogScrewGun.Write($"解析螺丝枪信息{msg}出错:  {ex.Message}");
            }
            finally
            {
                IsRecevied = true;
            }
        }

        /// <summary>
        /// 更改螺丝枪程序
        /// </summary>
        /// 
        public void ChangeProgram()
        {
            for (int j = 0; j < 3; j++)
            {
                Initial();
                System.Threading.Thread.Sleep(500);
                int ProgramNo_ = FunScrewGun.Cur.ProgramNo.GetValue;
                string command;
                string commandTemp = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20";
                string strTemp = string.Empty;

                int temp = 0;
                byte[] ints = new byte[3];
                do
                {
                    ints[temp++] = (byte)(ProgramNo_ % 10);
                    ProgramNo_ /= 10;
                }
                while (ProgramNo_ != 0);

                for (int i = ints.Length - 1; i >= 0; i--)
                {
                    strTemp += Convert.ToString(ints[i] + 48, 16).PadLeft(2, '0').PadLeft(3, ' ');
                }

                command = commandTemp + strTemp + " 00";

                Connect.Send(HexStringToBytes(command));

                StaticInitial.Motion.WriteToUser("更换螺丝枪程序号指令：" + command);
                System.Threading.Thread.Sleep(500);
            }

            //switch (ProgramNo)
            //{
            //    case 1:
            //        command = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 31 00";
            //        connect.Send(HexStringToBytes(command));
            //        break;
            //    case 2:
            //        command = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 32 00";
            //        connect.Send(HexStringToBytes(command));
            //        break;

            //        /*
            //         * 30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 32 00
            //         * 30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 ||30 30 32|| 00
            //         * 若设置了其他螺丝枪程序，可在此处增加，程序号为上一行标注的三个16进制
            //         */
            //}
        }

        /// <summary>
        /// 切换点检模式
        /// </summary>
        public void ChangeProgram2()
        {
            for (int j = 0; j < 3;j++)
            {
                Initial();
                System.Threading.Thread.Sleep(500);
                int ProgramNo_ = FunScrewGun.Cur.ProgramNo2.GetValue;
                string command;
                string commandTemp = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20";
                string strTemp = string.Empty;

                int temp = 0;
                byte[] ints = new byte[3];
                do
                {
                    ints[temp++] = (byte)(ProgramNo_ % 10);
                    ProgramNo_ /= 10;
                }
                while (ProgramNo_ != 0);

                for (int i = ints.Length - 1; i >= 0; i--)
                {
                    strTemp += Convert.ToString(ints[i] + 48, 16).PadLeft(2, '0').PadLeft(3, ' ');
                }

                command = commandTemp + strTemp + " 00";

                Connect.Send(HexStringToBytes(command));
                StaticInitial.Motion.WriteToUser("更换螺丝枪程序号指令：" + command);
                System.Threading.Thread.Sleep(500);
            }

        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Initial()
        {
            CreateLink();
            Thread.Sleep(100);
            GetData();
            Thread.Sleep(100);
            GetLimitUp_Dowm();

            AfterInitialEventHandle?.Invoke();
        }

        /// <summary>
        /// 建立连接后，发送GetData()后，螺丝枪会自动发送结果。但是有时候不会发，最好每次重新发送GetData()
        /// </summary>
        public void Send()
        {
            Final_torque = 0;
            Final_angle = 0;
            IsRecevied = false;
            Thread.Sleep(100);
            GetLimitUp_Dowm();
            Initial();

        }

        /// <summary>
        /// 建立并保持连接
        /// </summary>
        public void CreateLink()
        {
            string command = "30 30 32 30 30 30 30 31 30 30 36 30 20 20 20 20 20 20 20 20 00";
            Connect.Send(HexStringToBytes(command));
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            string command = "30 30 32 30 30 30 30 33 30 30 31 30 20 20 20 20 20 20 20 20 00";
            Connect.Send(HexStringToBytes(command));
        }

        /// <summary>
        /// 获取每次的螺丝抢当前上下限值
        /// </summary>
        public void GetLimitUp_Dowm()
        {
            string command = "30 30 33 36 30 30 30 36 30 30 31 30 20 20 20 20 20 20 20 20 32 35 30 31 30 30 31 30 37 30 30 30 32 33 30 31";
            if (ProjectNums == "001")
            {
                command = "30 30 33 36 30 30 30 36 30 30 31 30 20 20 20 20 20 20 20 20 32 35 30 31 30 30 31 30 37 30 30 30 31 33 30 31";
            }
            else if (ProjectNums == "002")
            {
                command = "30 30 33 36 30 30 30 36 30 30 31 30 20 20 20 20 20 20 20 20 32 35 30 31 30 30 31 30 37 30 30 30 32 33 30 31";
            }
            else if (ProjectNums == "003")
            {
                command = "30 30 33 36 30 30 30 36 30 30 31 30 20 20 20 20 20 20 20 20 32 35 30 31 30 30 31 30 37 30 30 30 33 33 30 31";
            }
            Connect.Send(HexStringToBytes(command));
        }

        /// <summary>
        /// 获取数据，初始化时发送一次即可，后续会自动传输数据
        /// </summary>
        public void GetData()
        {
            string command = "30 30 36 30 30 30 30 38 30 30 31 30 20 20 20 20 20 20 20 20 31 32 30 31 30 30 31 33 31 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 31 00";
            Connect.Send(HexStringToBytes(command));
        }

        ///// <summary>
        ///// 螺丝枪程序号
        ///// </summary>
        //public int ProgramNo;

        ///// <summary>
        ///// 更改螺丝枪程序
        ///// </summary>
        //public void ChangeProgram()
        //{
        //    string command;

        //    switch (ProgramNo)
        //    {
        //        case 1:
        //            command = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 31 00";
        //            connect.Send(HexStringToBytes(command));
        //            break;

        //        case 2:
        //            command = "30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 32 00";
        //            connect.Send(HexStringToBytes(command));
        //            break;

        //            /*
        //             * 30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 30 30 32 00
        //             * 30 30 32 33 30 30 31 38 30 30 31 30 20 20 20 20 20 20 20 20 ||30 30 32|| 00
        //             * 若设置了其他螺丝枪程序，可在此处增加，程序号为上一行标注的三个16进制
        //             */
        //    }
        //}

        /// <summary>
        /// 字符串转换为16进制字符数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] HexStringToBytes(string str)
        {
            string[] str_temp = str.Trim().Split(' ');
            byte[] bytes = new byte[str_temp.Length];
            for (int i = 0; i < str_temp.Length; i++)
            {
                bytes[i] = Convert.ToByte(str_temp[i], 16);
            }
            return bytes;
        }
    }
}