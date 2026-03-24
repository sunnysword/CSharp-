using Handler.Motion;
using IConnectDLL;
using RM_dll2.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.Camera
{
    public class CameraCmd
    {
        public bool IsPass
        {
            get
            {
                if(Result==Result_OK) return true;
                else return false;
            }
        }
        public const string Result_OK = "1";
        public const string Result_NG = "0";
        //public const string Result_OK = "OK";
        //public const string Result_NG = "NG";
        public bool IsTrigger { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }


        public string FullMessage { get; set; }
    }
    public class CameraHelper
    {
        public CameraHelper(string name, IConnect connect, Func<bool> isConnectEventHander)
        {
            this.Connect = connect;
            this.IsConnectEventHander = isConnectEventHander;
            this.Connect.RecevieMsgActionEventHander += Connect_RecevieMsgActionEventHander;
            this.Name = name;
        }



        public string Name { get; set; }
        public IConnect Connect { get; set; }

        public static bool IsStop { get; set; }

        internal Dictionary<string, CameraCmd> CommandDict { get; private set; } = new Dictionary<string, CameraCmd>();
        protected Func<bool> IsConnectEventHander;
        private object _lockCheck = new object();

        public Action<string> WriteToUserEventHandler;

        public void WriteToUser(string obj)
        {
            WriteToUserEventHandler?.Invoke(obj);
        }
        private void Connect_RecevieMsgActionEventHander(string obj)
        {
            char split = ',';

            string TryGetValue(string[] array, int index)
            {
                if (index < 0) return string.Empty;
                if (array.Length >= index + 1)
                {
                    return array[index];
                }
                else
                {
                    return string.Empty;
                }
            }
            string TryGetValue2(string[] array)
            {
                string content = string.Empty;
                if (array.Length >= 3)
                {
                    for (int i = 1; i < array.Length - 1; i++)
                    {
                        content += array[i];
                        content += split;
                    }
                    return content;
                }
                else
                {
                    return string.Empty;
                }
            }
            try
            {
                WriteToUser($"收到{Name}发来消息:{obj}");
                if (obj.Contains(split))
                {
                    var array = obj.Split(split);
                    var result = TryGetValue(array, array.Length - 1);
                    var command = TryGetValue(array, 0);
                    var message = TryGetValue2(array);

                    switch (command)
                    {
                        default:
                            if (CommandDict.ContainsKey(command))
                            {
                                CommandDict[command].Result = result;
                                CommandDict[command].Message = message;
                                CommandDict[command].IsTrigger = true;
                                CommandDict[command].FullMessage = obj;

                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"处理{Name}发来消息异常:" + ex.ToString();
                var messageFull = $"处理{Name}发来消息异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex);
                WriteToUser(messageFull);
                WriteToUser(message);
                StaticInitial.Motion.WriteErrToUser(message);
            }

        }

        public bool? SendAndCheck(string command, out CameraCmd cameraMsg, out string error,
            string extraMsg = null
            , bool needWait = true,
            ProcessBase process = null)
        {
            cameraMsg = null;
            error = string.Empty;
            try
            {
                lock (_lockCheck)
                {
                    if (process != null)
                    {
                        this.WriteToUserEventHandler += process.WriteToUser;
                    }
                    IsStop = false;
                   
                    if (!CommandDict.ContainsKey(command))
                    {
                        CommandDict.Add(command, new CameraCmd());
                    }
                    CommandDict[command].IsTrigger = false;

                    if (IsConnectEventHander?.Invoke() == false)
                    {
                        error = $"{Name}未连接，请检查";
                        return false;
                    }
                    var msg = command;
                    if (!string.IsNullOrWhiteSpace(extraMsg))
                    {
                        msg += $",{extraMsg}";
                    }
                    WriteToUser($"向{Name}发送消息:{msg}");
                    this.Connect.Send(msg);

                    if (!needWait) return true;

                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Restart();
                    while (true)
                    {
                        if (IsConnectEventHander?.Invoke() == false)
                        {
                            error = $"{Name}未连接，请检查";
                            return false;
                        }
                        if (IsStop)
                        {
                            return null;
                        }
                        if (this.CommandDict[command].IsTrigger == true)
                        {
                            cameraMsg = this.CommandDict[command];
                            return true;
                        }
                        else
                        {
                            if (timer.ElapsedMilliseconds >= 30 * 1000)
                            {
                                error = $"{Name}:发送指令{command}后等待回复超时";
                                return false;
                            }
                        }
                        System.Threading.Thread.Sleep(5);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                process?.WriteToUser("视觉指令发送过程中异常：" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                error = "视觉指令发送过程中异常:" + ex.Message;
                return false;
            }
            finally
            {
                if (process != null)
                {
                    this.WriteToUserEventHandler -= process.WriteToUser;
                }
            }

        }
    }
}
