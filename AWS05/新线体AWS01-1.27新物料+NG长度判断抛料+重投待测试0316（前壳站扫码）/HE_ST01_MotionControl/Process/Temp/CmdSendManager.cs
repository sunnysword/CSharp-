using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Process.Station
{
    /// <summary>
    /// 命令发送管理器
    /// </summary>
    class CmdSendManager
    {
        /// <summary>
        /// 指令响应对象
        /// </summary>
        public class CmdResponse
        {
            protected internal CmdResponse(CmdSendManager cmdSendManager)
            {
                CmdSendManager = cmdSendManager;
            }

            /// <summary>
            /// 属于哪一个指令管理器
            /// </summary>
            public CmdSendManager CmdSendManager { get; }

            /// <summary>
            /// 是否响应命令标志
            /// </summary>
            public bool IsResponseCmd { get; private set; }

            /// <summary>
            /// 检测是否需要暂停
            /// </summary>
            /// <returns></returns>
            public bool Fun_CheckPause()
            {
                return CmdSendManager.IsRequestPause;
            }

            /// <summary>
            /// 响应指令
            /// </summary>
            public void Fun_ResponseCmd()
            {
                IsResponseCmd = true;
            }

            /// <summary>
            /// 重置响应标志
            /// </summary>
            public void Fun_ResetResponseCmd()
            {
                IsResponseCmd = false;
            }

        }

        public CmdSendManager(string name)
        {
            Name = name;
        }

        public string Name { get; }

        readonly object ob_lock = new object();
        /// <summary>
        /// 是否需要暂停标志
        /// </summary>
        public bool IsRequestPause { get;private set; }

        /// <summary>
        /// 响应对象的集合
        /// </summary>
        readonly List<CmdResponse> responsesList = new List<CmdResponse>();

        /// <summary>
        ///向响应对象 发送暂停指令
        /// </summary>
        public void SendPause()
        {
            ResetResponseFlag();
            IsRequestPause = true;
        }

        /// <summary>
        /// 向响应对象发送 取消暂停指令
        /// </summary>
        public void SendCanclePause()
        {
            IsRequestPause = false;
           
        }

        /// <summary>
        /// 重置响应对象的响应标志
        /// </summary>
        public void ResetResponseFlag()
        {
            foreach (var item in responsesList)
            {
                item.Fun_ResetResponseCmd();
            }
        }

        /// <summary>
        /// 检测是否响应
        /// </summary>
        /// <returns></returns>
        public bool CheckResponse()
        {
            foreach (var item in responsesList)
            {
                if(!item.IsResponseCmd)
                {
                    return false;
                }
            }
            return true;
        }

        public CmdResponse CreateCmdResponse()
        {
            lock (ob_lock)
            {
                CmdResponse cmd = new CmdResponse(this);
                responsesList.Add(cmd);
                return cmd;
            }
        }

    }
}
