using IConnectDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect
{
    public enum LaserCmdType
    {
        Initialize,
        Data,
        MarkStart,
    }
    public class LaserHelper
    {
       
        public LaserHelper(string name,IConnectDLL.IConnect connect)
        {
            _log = new ILog_dll.LogProcessHelper(name);
            this.Connect = connect;
            this.Connect.RecevieMsgActionEventHander += Connect_RecevieMsgActionEventHander;
            CommandDict.Add(LaserCmdType.Initialize, (false,string.Empty));
            CommandDict.Add(LaserCmdType.Data, (false,string.Empty));
            CommandDict.Add(LaserCmdType.MarkStart, (false,string.Empty));
        }

        private object _lockExecute = new object();

        public const char CMD_HEAD = (char)0x02;
        public const char CMD_END = (char)0x03;

        public Action<string> WriteToUserEventHandler;

        private ILog_dll.LogProcessHelper _log;
        public bool IsStop { get; set; }   
        public IConnect Connect { get; set; }

        public Func<bool> IsConnectEventHandler;

        public Dictionary<LaserCmdType, (bool?, string)> CommandDict = new Dictionary<LaserCmdType, (bool?, string)>();
        private void Connect_RecevieMsgActionEventHander(string obj)
        {
            WriteToUser("收到消息:" + obj);
            obj = obj.Trim(CMD_HEAD, CMD_END);
            WriteToUser("解析后消息:" + obj);
            switch (obj)
            {
                case "$Initialize_OK":
                    CommandDict[LaserCmdType.Initialize] = (true, string.Empty);
                    break;
                case "$Initialize_FALSE":
                    CommandDict[LaserCmdType.Initialize] = (false, string.Empty);
                    break;

                case "$Receive_OK":
                    CommandDict[LaserCmdType.Data] = (true, string.Empty);
                    break;

                case "$Receive_ERROR":
                    CommandDict[LaserCmdType.Data] = (false, string.Empty);
                    break;

                case "$MarkStart_OK":
                    CommandDict[LaserCmdType.MarkStart] = (true, string.Empty);
                    break;

                case "$MarkStart_ERROR":
                    CommandDict[LaserCmdType.MarkStart] = (false, string.Empty);
                    break;
                default:
                    break;
            }

        }

      

        public void Send(string msg)
        {
            char chHead = CMD_HEAD;
            char chTail = CMD_END;
            string strSend; //发送字符串
            string strCmd; //指令字符串
            strCmd = msg;
            strSend = string.Format("{0}{1}{2}", chHead, strCmd, chTail);
            this.Connect.Send(strSend);
        }

        public void WriteToUser(string msg)
        {
            _log.Write(msg);
            WriteToUserEventHandler?.Invoke(msg);
        }

        public bool? Execute(LaserCmdType cmdType,string message,out string error)
        {
            error = string.Empty;
            IsStop = false;
            if (IsConnectEventHandler?.Invoke() == false)
            {
                error = "镭雕激光通讯未连接";
                return false;
            }
            lock (_lockExecute)
            {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                string msg = string.Empty;
                string cmd = string.Empty;
                switch (cmdType)
                {
                    case LaserCmdType.Initialize:
                        cmd = "$Initialize_";
                        msg = $"{cmd}{message}";
                        break;
                    case LaserCmdType.Data:
                        cmd = "$Data_";
                        msg = $"{cmd}{message}"; break;
                    case LaserCmdType.MarkStart:
                        cmd = "$MarkStart_";
                        msg = $"{cmd}{message}"; break;
                    default:
                        break;
                }
                CommandDict[cmdType] = (null, string.Empty);
                this.Send(msg);
                timer.Restart();
                while (true)
                {
                    if (IsStop) { return null; }
                    if (CommandDict[cmdType].Item1==true)
                    {
                        return true;
                    }
                    else if (CommandDict[cmdType].Item1 == false)
                    {
                        error = "镭雕响应失败";
                        return false;
                    }
                    if (timer.ElapsedMilliseconds > 10 * 1000)
                    {
                        error = "镭雕响应超时";
                        return false;
                    }
                    System.Threading.Thread.Sleep(5);
                }
            }
           
        }
    }
}
