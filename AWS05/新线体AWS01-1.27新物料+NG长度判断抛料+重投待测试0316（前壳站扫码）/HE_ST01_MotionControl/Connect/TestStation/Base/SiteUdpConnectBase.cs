using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UdpWpfDemo3;
using Handler.Product;
using Handler.View;

namespace Handler.Connect
{
    /// <summary>
    /// 基础的站位通讯对象
    /// </summary>
    public abstract class SiteUdpConnectBase
    {

        public class CheckMTPCommand
        {
            public CheckMTPCommand(string command)
            {
                Command = command;
            }
            /// <summary>
            /// 命令
            /// </summary>
            public string Command { get; }

            Action<UdpWpfDemo3.MTP> CheckAction;

            public void SetCheckAction(Action<UdpWpfDemo3.MTP> action)
            {
                CheckAction = action;
            }

            public void ExecuteCheckAction(UdpWpfDemo3.MTP mTP)
            {

                CheckAction?.Invoke(mTP);
            }
        }

        public SiteUdpConnectBase(IConnectDLL.IConnectT<UdpWpfDemo3.MTP> uDPHELPer, string name, int index)
        {
            Connect = uDPHELPer;
            Name = name;
            Index = index;
            Connect.RecevieTActionEventHander += Connect_RecevieTActionEventHander;

            checkMTPCommandsList.Add(CheckFinish);
            checkMTPCommandsList.Add(CheckLimit);

            RegisterCheckCommand();
        }


        public int Index { get; set; }

        public string Name { get; }

        public readonly List<CheckMTPCommand> checkMTPCommandsList = new List<CheckMTPCommand>();

        /// <summary>
        /// 当前的测试项结果
        /// </summary>
        public List<ProductTestStationTestItem> TestItemList = new List<ProductTestStationTestItem>();
        /// <summary>
        /// 基础的通讯对象
        /// </summary>
        public IConnectDLL.IConnectT<UdpWpfDemo3.MTP> Connect { get; }

        /// <summary>
        /// 测试是否Ok
        /// </summary>
        public bool IsOk { get; private set; }

        /// <summary>
        /// 测试是否结束
        /// </summary>
        public bool IsFinished { get; private set; }

        public Action<string, Brush, bool> WriteToUserEventHandler;

        bool _islimit = false;


        public bool IsLimit
        {
            get { return _islimit; }
            set
            {
                if (value != _islimit)
                {
                    _islimit = value;
                    SendToLimit(_islimit);
                }
            }
        }


        readonly object ob_Lock = new object();



        #region 命令

        /// <summary>
        /// 测试是否结束
        /// </summary>
        CheckMTPCommand CheckFinish = new CheckMTPCommand("FINISH");

        /// <summary>
        /// 权限
        /// </summary>
        CheckMTPCommand CheckLimit = new CheckMTPCommand("LIMIT");

      
        #endregion

        void RegisterCheckCommand()
        {
            CheckFinish.SetCheckAction(s =>
            {
                WriteToUser($"{Name}开始响应：{s.command}");
                if (s.result.ToUpper() == "OK" || s.result.ToUpper() == "PASS")
                {
                    IsOk = true;
                }
                else
                {
                    IsOk = false;
                }
                if (s.text != null)
                {
                    string[] tempArray = s.text.Split(';');

                    string strNew = "";
                    for (int i = 0; i < tempArray.Length ; i++)
                    {
                        strNew += "[";
                        strNew += tempArray[i];

                        strNew += "]";
                    }
                    List<ProductTestStationTestItem> testItemMsgs = ProductTestStationTestItem.ChangeMsg(strNew);

                    foreach (var item in testItemMsgs)
                    {
                        TestItemList.Add(item);
                    }
                }
                IsFinished = true;
            });

            CheckLimit.SetCheckAction(s =>
            {
                WriteToUser($"{Name}开始响应：{s.command}");

                SendToLimit(IsLimit);
            });

          
        }

        private void Connect_RecevieTActionEventHander(MTP obj)
        {
            try
            {
                lock (ob_Lock)
                {
                    if (obj == null) return;
                    WriteToUser($"{Name}收到信息：{obj.command}, {obj.text}, {obj.result}");
                    foreach (var item in checkMTPCommandsList)
                    {
                        if (item.Command.ToUpper() == obj.command.ToUpper())
                        {
                            item.ExecuteCheckAction(obj);
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Motion.StaticInitial.Motion.WriteErrToUser($"{Name} 通讯处理异常：" + ex.Message);
            }

        }

        protected void WriteToUser(string str)
        {
            WriteToUserEventHandler?.Invoke(str, System.Windows.Media.Brushes.White, false);
        }
        protected void WriteToUser(string msg, Brush brush, bool IsClear)
        {
            WriteToUserEventHandler?.Invoke(msg, brush, IsClear);
        }

        /// <summary>
        /// 发送换型
        /// </summary>
        public void SendToChangeType()
        {
            MTP mc2Test = new MTP();
            mc2Test.command = "CHANGEMODEL";
            mc2Test.text = "MODEL=" + WorkItemManagerHelper.LoadedName;
            mc2Test.result = "";
            Connect.Send(mc2Test);
            WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
        }

        /// <summary>
        /// 发送开始
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="localStation"></param>
        /// <param name="socket"></param>
        public void SendToStart(string sn, string localStation, string socket, string other = null)
        {
            MTP mc2Test = new MTP();
            mc2Test.command = "START";

            mc2Test.text = $"SN={sn};PARA={localStation};";
            if (!string.IsNullOrWhiteSpace(other))
            {
                mc2Test.text += other;
            }
            mc2Test.result = null;
            IsFinished = false;
            IsOk = false;
            TestItemList.Clear();
            Connect.Send(mc2Test);
            WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
        }

        public void SendToNotTest(string sn, string localStation, string socket)
        {
            MTP mc2Test = new MTP();
            mc2Test.command = "NOTTEST";
            mc2Test.text = $"SN={sn};PARA={localStation};SOCKET={socket}";

            mc2Test.result = null;
            IsFinished = false;
            IsOk = false;
            TestItemList.Clear();



            Connect.Send(mc2Test);
            WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
        }

        public void SendToLimit(bool bl)
        {
            MTP mc2Test = new MTP();
            mc2Test.command = "LIMIT";
            if (bl == true)
            {
                mc2Test.text = "true";
            }
            else
            {
                mc2Test.text = "false";
            }

            Connect.Send(mc2Test);
            WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
        }
    }
}
