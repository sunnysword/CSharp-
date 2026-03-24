using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Core.Process
{
    public enum SignalType
    {
        ON,
        OFF,
    }
    public static class ProcessExtension
    {
        public static bool? CheckSignalLoop(
            this RM_dll2.Process.ProcessIPauseBase processBase, 
            Iodll.IoInHelper input, 
            SignalType signalType, 
            int waitTime = 30000,
            bool needReportAlarm=true,
            bool needLoopCheck=true)
        {
            if (input == null) throw new ArgumentNullException(nameof(input), "CheckSignalLoop:传入的input信号不能为空");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Restart();
            while (true)
            {
                if (processBase.IsProcessRunning == false)
                {
                    timer.Stop();
                    return null;
                }
                bool result = false;
                result = signalType == SignalType.ON ? input.In() : !input.In();
                if (result)
                {
                    timer.Stop();
                    return true;
                }
                else
                {
                    if (timer.ElapsedMilliseconds >= waitTime)
                    {
                        if (needReportAlarm)
                        {
                            var signalStr = signalType == SignalType.ON ? "ON" : "OFF";
                            if (!processBase.IsErr)
                            {
                               // processBase.WriteErrToUser($"等待信号超时，需要信号为{signalStr}状态，信号具体信息:" +
                               //$"(IO点位名称:{input.Remark},IO点所属IO卡:{input.ioHelper.Name},IO点索引:{string.Join(",", input.InBytes)})");
                                return true;
                            }
                        }
                        else
                        {
                        }

                        if (!needLoopCheck)
                        {
                            return false;
                        }
                        else
                        {
                            timer.Restart();
                        }

                    }
                }
                System.Threading.Thread.Sleep(5);
            }
        }

        public static bool? CheckSignalLoop(this RM_dll2.Process.ProcessIPauseBase processBase, Func<bool> func, string errorMsg, int waitTime = 30000, bool needReportAlarm = true,
            bool needLoopCheck = true)
        {
            if (func == null) throw new ArgumentNullException(nameof(func), "CheckSignalLoop:传入的func信号不能为空");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Restart();
            while (true)
            {
                if (processBase.IsProcessRunning == false)
                {
                    timer.Stop();
                    timer = null;
                    return null;
                }
                bool result = false;
                result = func();
                if (result)
                {
                    timer.Stop();
                    return true;
                }
                else
                {
                    if (timer.ElapsedMilliseconds >= waitTime)
                    {
                        if (needReportAlarm)
                        {
                            if (!processBase.IsErr)
                            {
                                processBase.WriteErrToUser($"{errorMsg}");
                            }
                        }
                        else
                        {
                        }

                        if (!needLoopCheck)
                        {
                            return false;
                        }
                        else
                        {
                            timer.Restart();
                        }
                    }
                }
                System.Threading.Thread.Sleep(5);
            }
        }
    }
}
