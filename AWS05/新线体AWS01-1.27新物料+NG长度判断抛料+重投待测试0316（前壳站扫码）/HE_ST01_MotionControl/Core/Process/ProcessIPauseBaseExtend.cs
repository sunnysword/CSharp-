using AM.Core.Extension;
using AM.Core.IO;
//using Handler.Core.Extension;
using Handler.Motion.Axis;
using LanCustomControldllExtend.FourAxis;
using Handler.Process.RunMode;
using Handler.Product;
using RM_dll2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.Modbus;
using System.Threading;

namespace AM.Core.Process
{
    public class ProcessIPauseBaseExtend : RM_dll2.Process.ProcessIPauseBase
    {
        protected readonly CheckActionHelper CheckAction = new CheckActionHelper();
        public static StationSelectFunsBase CurrentWorkMode => WorkModeManager.Cur.SelectWorkMode;

        public ProcessIPauseBaseExtend(string name, MotionHelper motionHelper, bool IsCreateLog = false) : base(name, motionHelper, IsCreateLog)
        {
            this.ProcessStartAsyncEvent += () => NGProductCount = 0;
        }

        public string RunTimeCirculate { get; protected set; }


        /// <summary>
        /// NG产品计数，用于连续不良报警统计，可能会被清零，不能用来作为工位的NG计数
        /// </summary>
        public int NGProductCount { get; set; } = 0;

        public static Func<int> GetMaxNGProductCountEventHandler;

        protected override void ImplementRunAction()
        {
            throw new NotImplementedException();
        }

        public override void WriteErrToUser(string msg)
        {
            msg = $"{Name}:{msg}";
            base.WriteErrToUser(msg);
        }

        public void ClearNGProductCount() => this.NGProductCount = 0;

        public void AddAndCheckNGProductCount()
        {
            if (Handler.Funs.FunCommon.Cur.ErrContinueNumIsUse.GetValue == false)
            {
                return;
            }
            ++this.NGProductCount;
            if (GetMaxNGProductCountEventHandler != null)
            {
                if (this.NGProductCount >= GetMaxNGProductCountEventHandler())
                {

                    this.WriteErrToUser($"连续不良报警，连续不良数:{this.NGProductCount},工序名称:{this.Name}");
                    this.NGProductCount = 0;
                }
            }
        }

        public void RunAndCheckProcess(RM_dll2.Process.ProcessBase process)
        {
            var runArgs = process.Run();
            if (runArgs.IsExitOK == false)
            {
                throw new Exception($"{process.Name}异常，异常原因:{runArgs.ErrInfo},异常步骤:{runArgs.TrackStep}");
            }
        }


        public bool ParallelExecute(params Func<bool>[] items)
        {
            List<Task<bool>> list = new List<Task<bool>>();
            foreach (var item in items)
            {
                list.Add(Task.Run(() =>
                {
                    Task<bool> task = Task.Run(item);
                    return task;
                }));
            }
            Task.WaitAll(list.ToArray());
            return list.Where(s => s.Result == false).Count() == 0;
        }

        public bool WaitIO(AM.Core.IO.Cylinder cylinder, AM.Core.IO.CylinderActionType actionType)
        {
            if (cylinder == null) return true;
            string error = string.Empty;
            bool result = true;
            switch (actionType)
            {
                case CylinderActionType.WorkPos:
                    result = cylinder.WorkPos(out error);
                    break;
                case CylinderActionType.OriginPos:
                    result = cylinder.OriginPos(out error);
                    break;
                default:
                    break;
            }
            if (!result)
            {
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
            }
            return result;
        }

        /// <summary>
        /// 等待夹爪夹持到位
        /// </summary>
        /// <param name="gripper">夹爪对象</param>
        /// <param name="distance">夹持距离</param>
        /// <returns>在指定时间内夹爪夹持到指定位置，返回true，否则返回false</returns>
        public bool WaitGripperClamp(ModbusGripper_Hsl gripper, short distance)
        {
        
            if (gripper == null) return false;
            string error = "夹爪夹持未到位";
            bool result = false;

            gripper.WriteSingleRegister("259", distance);
            for (int i = 0; i < 300; i++)
            {
                Thread.Sleep(10);
                if (gripper.ReadSingleRegister("513") != 0) break;
            }
            if (gripper.ReadSingleRegisterU("514") >= distance) result = true; else result = false;

            if (!result)
            {
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
            }
            return result;
        }

        /// <summary>
        /// 判断夹爪是否夹持到物体
        /// </summary>
        /// <param name="gripper">夹爪对象</param>
        /// <param name="distance">夹持距离</param>
        /// <returns>夹爪夹持到物体，返回true，否则返回false</returns>
        public bool CheckGripperClamp(ModbusGripper_Hsl gripper, short distance)
        {
            if (gripper == null) return false;
            bool result = false;

            //if (gripper.ReadSingleRegisterU("514") > distance && gripper.ReadSingleRegisterU("513") == 2) result = true; else result = false;
            if (gripper.ReadSingleRegisterU("513") == 2)
                 result = true; 
            else
                result = false;

            return result;
        }

        /// <summary>
        /// 等待夹爪旋转到位
        /// </summary>
        /// <param name="gripper">夹爪对象</param>
        /// <param name="Angle">旋转角度</param>
        /// <returns>在指定时间内夹爪旋转到指定角度，返回true，否则返回false</returns>
        public bool WaitGripperRotate(ModbusGripper_Hsl gripper, short Angle)
        {
            if (gripper == null) return false;//原来是Ture
            string error = "夹爪旋转未到位";
            bool result = true;

            gripper.WriteSingleRegister("261", Angle);
            for (int i = 0; i < 300; i++)
            {
                Thread.Sleep(10);
                if (gripper.ReadSingleRegister("523") != 0) break;
            }
            //if (gripper.ReadSingleRegister("520") == Angle) result = true; else result = false;
            if (/*gripper.ReadSingleRegister("520") == Angle &&*/ gripper.ReadSingleRegister("523") == 1) result = true; else result = false;

            if (!result)
            {
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
            }
            return result;
        }

        /// <summary>
        /// 等待IO信号
        /// </summary>
        /// <param name="io">输入点对象</param>
        /// <param name="msMaxTime">最大等待时间(ms单位)，小于等于0表示不等待，只检测一次IO信号，根据IO信号判断方法返回值；若大于0，则若超出此设定时间仍未感应到信号，返回false</param>
        /// <returns>在指定时间内感应到信号，返回true，否则返回false</returns>
        public bool WaitIO(Iodll.IoInHelper io, int msMaxTime = 0)
        {
            if (msMaxTime <= 0)
            {
                if (io.In()) return true;
                else
                {
                    if (this.IsProcessRunning == false) return false;
                    this.WriteErrToUser("感应输入信号超时:" + GetInputInfo(io));
                    return false;
                }
            }
            else
            {
                var time = 0;
                while (!io.In())
                {
                    if (!IsProcessRunning) return false;
                    Sleep(10);
                    ++time;
                    if (time > msMaxTime / 10.0)
                    {
                        if (this.IsProcessRunning == false) return false;
                        this.WriteErrToUser("感应输入信号超时:" + GetInputInfo(io));
                        return false;
                    }
                }
            }

            return true;
        }


        public bool WaitAxisPoint(FourAxisPosMsgILineZMov pos, AxisMoveType moveType = AxisMoveType.AxisXYZ)
        {
            if (pos.MovWithCheck(out string error, moveType))
            {
                return true;
            }
            else
            {
                System.Threading.Thread.Sleep(10);
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
                return false;
            }
        }


        public bool WaitAxisPoint(AixCommInfo.ThreeAixsPosMsgILineZMov pos, AxisMoveType moveType = AxisMoveType.AxisXYZ)
        {
            if (pos.MovWithCheck(out string error, moveType))
            {
                return true;
            }
            else
            {
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
                return false;
            }
        }

        public bool WaitAxisPoint(AixCommInfo.SingleAixPosAndSpeedMsgAndIMov pos)
        {
            if (pos.MovWithCheck(out string error))
            {
                return true;
            }
            else
            {
                if (this.IsProcessRunning == false) return false;
                this.WriteErrToUser($"{this.Name}:{error}");
                return false;
            }
        }
        /// <summary>
        /// 延时，并循环检查条件
        /// </summary>
        /// <param name="time"></param>
        public void SleepLoopCheck(int time)
        {
            int i = 0;
            while (this.IsProcessRunning)
            {
                if (i >= time)
                {
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                    i += 100;
                }
            }
        }

        public string GetInputInfo(params Iodll.IoInHelper[] ios)
        {
            string info = string.Empty;
            foreach (var item in ios)
            {
                info += $"(IO点位名称:{item.Remark},IO模块名称:{item.ioHelper.Name},索引:{string.Join(",", item.InBytes)}";
            }
            return info;
        }

        public string GetOutputInfo(params Iodll.IoOutHepler[] ios)
        {
            string info = string.Empty;
            foreach (var item in ios)
            {
                info += $"(IO点位名称:{item.Remark},io点所属io卡:{item.ioHelper.Name},io点索引:{string.Join(",", item.OutBytes)}";
            }
            return info;
        }

        public string ErrorDialog(string content, params string[] buttonContents)
        {
            Task<string> result = null;
            Handler.App.Current.Dispatcher.Invoke(() =>
            {
                WriteErrToUser(content);
                result = new Handler.View.WarningView(content,
                    buttonContents).ShowDialog();
            });
            var resultContent = result?.Result;
            return resultContent;
        }

        public void ErrorDialog(string content, params (string, Action)[] valueTurpes)
        {
            Task<string> result = null;
            content = $"{Name}:{content}";
            Handler.App.Current.Dispatcher.Invoke(() =>
            {
                WriteErrToUser(content);
                List<string> list = new List<string>();
                result = new Handler.View.WarningView(content,
                    valueTurpes.Select(s => s.Item1).ToArray()).ShowDialog();
            });
            var resultContent = result?.Result;
            var array = valueTurpes.Where(s => s.Item1 == resultContent);
            if (array.Count() > 0)
            {
                array.First().Item2?.Invoke();
            }
        }

    }
}
