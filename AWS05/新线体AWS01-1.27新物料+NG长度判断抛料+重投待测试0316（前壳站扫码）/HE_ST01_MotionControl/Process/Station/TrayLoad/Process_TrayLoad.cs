using AbstractSingleAixdll;
using AM.Core.IO;
using Handler.Motion.IO;
using Handler.Motion;
using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PALLET_DEMO;
using PALLET_DEMO.TrayBox;
using Handler.Motion.Axis;
using AM.Tools;
using AixRegisterInfodll;
using AM.Core.Extension;
using AixCommInfo;

namespace Handler.Process.Station
{
    public enum ENUM_Load_State
    {
        /// <summary>
        /// 机械手不可以来取料
        /// </summary>
        NotReady,

        /// <summary>
        /// 机械手可以来取料
        /// </summary>
        Ready,

        /// <summary>
        /// 全部取料完成
        /// </summary>
        AllTakeOver
    }

    public class Process_TrayLoad : RM_dll2.Process.ProcessIPauseBase
    {
        public Process_TrayLoad(string name) : base(name, StaticInitial.Motion, true)
        {
        }

        //public static bool isEmptyRun = true;
        public static bool isOnlyTray = false;//todo  2025-03-16
        public static bool isAllow = false;

        public bool IsWarning { get; set; }
        public string WarningMsg { get; set; }

        public void ClearWarning()
        {
            IsWarning = false;
            StaticIOHelper.蜂鸣器.Out_OFF();
        }

        public void WriteWarningToUser(string msg)
        {
            WarningMsg = msg;
            IsWarning = true;
            this.WriteToUser(msg, System.Windows.Media.Brushes.Yellow, true);
        }


        public Func<bool> CylinderSafe;
        /// <summary>
        /// 按钮灯是否停止闪烁
        /// </summary>
        public bool IsButtonLightStop { get; set; }
        /// <summary>
        /// 等待位，人工更换料盘时轴上升的位置
        /// </summary>
        public SingleAixPosAndSpeedMsgAndIMov PosWait;
        /// <summary>
        /// 料仓移栽气缸
        /// </summary>
        public Cylinder CylinderTrayVehical;
        /// <summary>
        /// 检测料仓有无
        /// </summary>
        public IoInHelper In_CurrentFloorCheckTray1;
        /// <summary>
        /// 检测料仓有无
        /// </summary>
        public IoInHelper In_CurrentFloorCheckTray2;
        /// <summary>
        /// 当前层是否有料盘检测
        /// </summary>
        public IoInHelper In_CheckTrayExist;
        /// <summary>
        /// 料仓有无检测1
        /// </summary>
        public IoInHelper In_TrayBoxCheck1;
        /// <summary>
        /// 料仓有无检测2
        /// </summary>
        public IoInHelper In_TrayBoxCheck2;
        /// <summary>
        /// 料盘抽出后移栽到位检测1
        /// </summary>
        public IoInHelper In_TransplantTrayOutCheck1;
        /// <summary>
        /// 料盘抽出后移栽到位检测2
        /// </summary>
        public IoInHelper In_TransplantTrayOutCheck2;
        /// <summ
        /// 料仓对象
        /// </summary>
        public PALLET_DEMO.TrayBox.StorageBoxInfo TrayBox;
        /// <summary>
        /// 按钮灯输出
        /// </summary>
        public IoOutHepler Out_ButtonLight;
        /// <summary>
        /// 换料按钮输入
        /// </summary>
        public IoInHelper In_Button;
        public IoInHelper In_Door;
        public IoOutHepler Out_Door;
        public Func<bool> IsCheckDoorInputFunc;
        public Action FlushTrayAction;

        /// <summary>
        /// 当前的料盘层的序号(从0开始)
        /// </summary>
        public int Index => TrayBox.Index;
        /// <summary>
        /// 当前使用料盘层
        /// </summary>
        PALLET_DEMO.TrayBox.StorageFloorInfo CurrentFloor => TrayBox.StorageFloors[Index];
        /// <summary>
        /// 当前上料位状态
        /// </summary>
        public ENUM_Load_State Load_state;

        public override void Stop()
        {
            base.Stop();

            TrayBox.Save();
            this.ClearWarning();
            Out_ButtonLight.Out_OFF();
            Load_state = ENUM_Load_State.NotReady;
        }

        public void ButtonLightFlicker()
        {
            IsButtonLightStop = false;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Restart();
            while (!IsButtonLightStop && this.IsProcessRunning)
            {
                Out_ButtonLight.Out_ON();
                if (timer.ElapsedMilliseconds <= 10 * 1000)
                {
                    StaticIOHelper.蜂鸣器.Out_ON();
                }
                Thread.Sleep(500);
                if (this.IsButtonLightStop) return;
                Out_ButtonLight.Out_OFF();
                StaticIOHelper.蜂鸣器.Out_OFF();
                Thread.Sleep(500);
            }
            Out_ButtonLight.Out_ON();
            StaticIOHelper.蜂鸣器.Out_OFF();
        }

        public override void WriteErrToUser(string msg)
        {
            msg = $"{Name}:{msg}";
            base.WriteErrToUser(msg);
        }

        protected override void ImplementRunAction()
        {
            string error = string.Empty;
            Load_state = ENUM_Load_State.NotReady;
            Ref_Step("判断当前工位是否存在料仓");
            bool needMoveToTheLastOne = false;
            this.ClearWarning();
            IoCheck inputButton = new IoCheck(() => In_Button.In());

            try
            {
                while (true)
                {
                    WaitPause();
                    if (!IsProcessRunning)
                    {
                        break;
                    }
                    if (this.IsWarning)
                    {
                        if (inputButton.RisingCheck())
                        {
                            if (!IsButtonLightStop)
                            {
                                isAllow = true;
                                Out_Door?.Out_OFF();
                                IsButtonLightStop = true;
                                Sleep(500);
                                Out_ButtonLight.Out_ON();
                                StaticIOHelper.蜂鸣器.Out_OFF();
                            }
                            else
                            {
                                Out_Door?.Out_ON();
                                if (IsCheckDoorInputFunc())
                                {
                                    Sleep(500);
                                    if (In_Door.In() == false)
                                    {
                                        WriteErrToUser($"上料盘后门锁未关好，请关好门后重新按下换料完成按钮");
                                        continue;
                                    }
                                }
                                IsButtonLightStop = true;
                                Out_ButtonLight.Out_OFF();
                                this.ClearWarning();
                                TrayBox.Rst();
                                isAllow = false;
                            }
                        }
                        else
                        {
                            Sleep(100);
                            continue;
                        }
                    }

                    switch (Step)
                    {
                        //case "判断料仓料盘位置是否溢出":
                        //    int Count = 0;
                        //    while (Count<7)
                        //    {
                        //        TrayBox.aix.MovAbs(TrayBox.StorageFloors[Count].Pos);
                        //        if (!TrayBox.aix.CheckPos(TrayBox.StorageFloors[Count].Pos, 0.01))
                        //        {
                        //            WriteErrToUser("检查料盘位置时,轴未运动到位！");
                        //            break;
                        //        }
                        //        if (In_TrayBoxCheck1.In() || In_TrayBoxCheck2.In())
                        //        {
                        //            WriteErrToUser($"当前{Name}托盘位置有偏差，可能导致撞机，请调整当前层托盘位置");
                        //            break;
                        //        }
                        //        else
                        //        {
                        //            Count++;
                        //        }

                        //    }
                        //    Ref_Step("判断当前工位是否存在料仓");
                        //    break;

                        case "判断当前工位是否存在料仓":
                            //if (!In_CurrentFloorCheckTray1.In())
                            //{
                            //    WriteErrToUser($"{Name}:未检测到整个料仓，料仓有无检测1无信号！{this.GetInputInfo(In_CurrentFloorCheckTray1)}");
                            //    break;
                            //}
                            //if (!In_CurrentFloorCheckTray2.In())
                            //{
                            //    WriteErrToUser($"{Name}:未检测到整个料仓，料仓有无检测2无信号！{this.GetInputInfo(In_CurrentFloorCheckTray2)}");
                            //    break;
                            //}
                            Ref_Step("判断料仓状态");
                            break;

                        case "判断料仓状态":
                            //料仓里的料盘都使用完了[Index大于]
                            WriteToUser($"当前Index:{Index}");
                            if (Index >= TrayBox.StorageFloors.Count - 1)
                            {
                                Ref_Step("准备重置料仓");
                            }
                            else
                            {
                                switch (CurrentFloor.FloorState)
                                {
                                    case StorageFloorState.Using:
                                        Ref_Step("上料区记忆有料盘检测是否有料盘");
                                        break;

                                    case StorageFloorState.WaitUse:
                                    case StorageFloorState.HaveUsed:
                                    case StorageFloorState.Empty:
                                    case StorageFloorState.Limit:
                                        Ref_Step("判断上料区是否有料盘");
                                        break;
                                }
                            }
                            break;

                        case "上料区记忆有料盘检测是否有料盘":
                            if (In_TransplantTrayOutCheck1.In() || In_TransplantTrayOutCheck2.In())
                            {
                                if (!CylinderTrayVehical.OriginPos(out error))
                                {
                                    WriteErrToUser(error);
                                    break;
                                }

                                Ref_Step("通知可以取料");
                            }
                            else
                            {
                                ErrorDialog("当前记忆上料区存在料盘，但传感器未检测到料盘",
                                    ("从料仓重新拉取", () =>
                                    {
                                        CurrentFloor.FloorState = StorageFloorState.HaveUsed;
                                        WriteToUser("，检查下一层");
                                        TrayBox.AddIndex();             //弹框人工确认更好一些。
                                        Ref_Step("判断料仓状态");
                                    }
                                ),
                                    ("重新检测", null
                                )
                                    );
                            }
                            break;

                        case "判断上料区是否有料盘":
                            if (In_TransplantTrayOutCheck1.In() || In_TransplantTrayOutCheck2.In())
                            {
                                WriteErrToUser("当前记忆上料区不应该存在料盘，请将料盘取走");
                                break;
                            }
                            Ref_Step("判断当前料仓状态");
                            break;

                        case "判断当前料仓状态":
                            switch (CurrentFloor.FloorState)
                            {
                                case StorageFloorState.Using:
                                    Ref_Step("上料区记忆有料盘检测是否有料盘");
                                    break;

                                case StorageFloorState.WaitUse:
                                    Ref_Step("移动到当前层");
                                    break;

                                case StorageFloorState.HaveUsed:
                                case StorageFloorState.Empty:
                                case StorageFloorState.Limit:
                                    Ref_Step("准备料仓料架移动一层");
                                    break;
                            }
                            break;

                        case "移动到当前层":
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            TrayBox.aix.MovAbs(TrayBox.StorageFloors[Index].Pos, true, true);
                            if (TrayBox.aix.CheckPos(TrayBox.StorageFloors[Index].Pos, 0.01))
                            {
                                Ref_Step("判断当前层是否有料盘");
                            }
                            break;

                        case "判断当前层是否有料盘":
                            if (!In_CheckTrayExist.In())
                            {
                                WriteToUser("当前层未检测到料盘");
                                TrayBox.setCurrentFloorState(StorageFloorState.Empty);
                                Ref_Step("判断当前料仓状态");
                                break;
                            }
                            else
                            {
                                WriteToUser("当前层检测到料盘");
                                Ref_Step("将料盘抽出");
                            }
                            break;

                        case "将料盘抽出":
                            if (In_TrayBoxCheck1.In() || In_TrayBoxCheck2.In())
                            {
                                WriteErrToUser("当前托盘位置有偏差，可能导致撞机，请调整当前层托盘位置");
                                break;
                            }
                            if (!CylinderTrayVehical.OriginPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            var pos = TrayBox.StorageFloors[Index].Pos - TrayBox.FloorStep.GetValue / 2;
                            TrayBox.aix.MovAbs(pos, true, true);
                            if (TrayBox.aix.CheckPos(pos, 0.01))
                            {
                                Ref_Step("气缸伸出");
                            }
                            break;

                        case "气缸伸出":
                            if (!CylinderTrayVehical.WorkPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            Sleep(500);
                            Ref_Step("气缸伸出后Z轴移动");
                            break;

                        case "气缸伸出后Z轴移动":
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            pos = TrayBox.StorageFloors[Index].Pos;
                            TrayBox.aix.MovAbs(pos, true, true);
                            if (TrayBox.aix.CheckPos(pos, 0.01))
                            {
                                Ref_Step("Z轴移动后气缸缩回");
                            }
                            break;

                        case "Z轴移动后气缸缩回":
                            if (!CylinderTrayVehical.OriginPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            TrayBox.setCurrentFloorState(StorageFloorState.Using);
                            TrayBox.Save();
                            FlushTrayAction?.Invoke();
                            Ref_Step("判断料盘是否到位");
                            break;

                        case "判断料盘是否到位":
                            if (!In_TransplantTrayOutCheck1.In())
                            {
                                WriteErrToUser("料盘抽出后，料盘有无检测1无信号，请检查料盘是否抽出。" + this.GetInputInfo(In_TransplantTrayOutCheck1));
                                break;
                            }
                            if (!In_TransplantTrayOutCheck2.In())
                            {
                                WriteErrToUser("料盘抽出后，料盘有无检测2无信号，请检查料盘是否抽出。" + this.GetInputInfo(In_TransplantTrayOutCheck1));
                                break;
                            }
                            Ref_Step("通知可以取料");
                            break;

                        case "通知可以取料":
                            Load_state = ENUM_Load_State.Ready;
                            TrayBox.Save();
                            if (Index == TrayBox.StorageFloors.Count - 2)
                            {
                                WriteToUser("由于当前料盘为倒数第二层，通知人员提前更换料仓料盘");
                                Ref_Step("通知人员提前更换料仓料盘");
                                break;
                            }
                            else
                            {
                                needMoveToTheLastOne = false;
                            }
                            Ref_Step("等待取料全部完成");
                            break;

                        case "通知人员提前更换料仓料盘":
                            if (!WaitAxisPoint(PosWait))
                            {
                                break;
                            }
                            inputButton.Reset();
                            Out_ButtonLight.Out_ON();
                            WriteWarningToUser("料仓料盘已经使用完，请及时更换料盘");
                            //弹窗
                            //Task.Run(() =>
                            //{

                            //    Handler.App.Current.Dispatcher.Invoke(() =>
                            //    {
                            //        var result = new Handler.View.WarningView("料仓料盘已经使用完，请及时更换料盘",
                            //             new string[] { "确定" }).ShowDialog();
                            //    });
                            //});
                            Task.Run(() =>
                            {
                                ButtonLightFlicker();
                            });
                            needMoveToTheLastOne = true;
                            Ref_Step("等待取料全部完成");
                            break;

                        case "等待取料全部完成":
                            if (Load_state == ENUM_Load_State.AllTakeOver)
                            {
                                Ref_Step("料盘上所有点位已取完");
                            }
                            //else if (isOnlyTray)
                            //{
                            //    WriteToUser("当前状态只有料仓流程，模拟取完");
                            //    Thread.Sleep(3000);
                            //    Ref_Step("料盘上所有点位已取完");
                            //}
                            else
                            {
                                Sleep(1000);
                            }
                            break;

                        case "料盘上所有点位已取完":
                            if (needMoveToTheLastOne)
                            {
                                WriteToUser("由于当前料盘为倒数第二层，所以放入倒数第一层");
                                //TrayBox.AddIndex();
                            }
                            else
                            {
                                WriteToUser("由于当前料盘不是倒数第二层，所以放入正常层");
                            }
                            Ref_Step("放回托盘前Z轴移动");
                            break;

                        case "放回托盘前Z轴移动":
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            if (needMoveToTheLastOne)
                            {
                                TrayBox.aix.MovAbs(TrayBox.StorageFloors[TrayBox.StorageFloors.Count - 1].Pos, true, true);
                                if (!TrayBox.aix.CheckPos(TrayBox.StorageFloors[TrayBox.StorageFloors.Count - 1].Pos, 0.01))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                TrayBox.aix.MovAbs(TrayBox.StorageFloors[Index].Pos, true, true);
                                if (!TrayBox.aix.CheckPos(TrayBox.StorageFloors[Index].Pos, 0.01))
                                {
                                    break;
                                }
                            }
                            Ref_Step("放回托盘前判断当前层是否有料盘防止撞料盘");
                            break;

                        case "放回托盘前判断当前层是否有料盘防止撞料盘":
                            if ((In_CheckTrayExist.In()))
                            {
                                WriteErrToUser("托盘准备放回料仓前检测到料仓当前层有托盘，请将当前层托盘取走");
                                break;
                            }
                            ReportMsg("料仓当前层检测无料盘，可以送回空料盘");
                            Ref_Step("放回托盘");
                            break;

                        case "放回托盘":
                            if (!CylinderTrayVehical.WorkPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            Sleep(500);
                            Ref_Step("放回托盘后判断托盘位置");
                            break;

                        case "放回托盘后判断托盘位置":
                            Ref_Step("放回托盘后Z轴移动一小段距离");
                            break;

                        case "放回托盘后Z轴移动一小段距离":
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            if (needMoveToTheLastOne)
                            {
                                pos = TrayBox.StorageFloors[TrayBox.StorageFloors.Count - 1].Pos - TrayBox.FloorStep.GetValue / 2;
                                TrayBox.aix.MovAbs(pos, true, true);
                                if (!TrayBox.aix.CheckPos(pos, 0.01))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                pos = TrayBox.StorageFloors[Index].Pos - TrayBox.FloorStep.GetValue / 2;
                                TrayBox.aix.MovAbs(pos, true, true);
                                if (!TrayBox.aix.CheckPos(pos, 0.01))
                                {
                                    break;
                                }
                            }
                            Ref_Step("放回托盘后气缸缩回");
                            break;

                        case "放回托盘后气缸缩回":
                            if (!CylinderTrayVehical.OriginPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            if (In_TrayBoxCheck1.In() || In_TrayBoxCheck2.In())
                            {
                                WriteErrToUser("托盘放回后位置有偏差，请调整当前层托盘位置");
                                break;
                            }

                            if (needMoveToTheLastOne)
                            {

                            }
                            else
                            {
                                TrayBox.setCurrentFloorState(StorageFloorState.HaveUsed);
                            }
                            Ref_Step("准备料仓料架移动一层");
                            break;

                        case "准备料仓料架移动一层":
                            if (!CylinderSafe())
                            {
                                WriteErrToUser("料盘移栽气缸不在安全位置");
                                break;
                            }
                            if (Index == TrayBox.StorageFloors.Count - 2)
                            {
                                WriteToUser($"料仓已运行到最后一层，Index = {Index}");
                                TrayBox.AddIndex();
                            }
                            else
                            {
                                if (needMoveToTheLastOne)
                                {
                                    needMoveToTheLastOne = false;
                                }
                                else
                                {
                                    TrayBox.AddIndex();
                                }
                                TrayBox.aix.MovAbs(TrayBox.StorageFloors[Index].Pos, true, true);
                                if (!TrayBox.aix.CheckPos(TrayBox.StorageFloors[Index].Pos, 0.01))
                                {
                                    break;
                                }
                            }
                            Ref_Step("判断料仓状态");
                            break;

                        case "准备重置料仓":
                            if (!CylinderTrayVehical.OriginPos(out error))
                            {
                                WriteErrToUser(error);
                                break;
                            }
                            if (WaitAxisPoint(PosWait))//料盘使用完移动到等待点
                            {
                                Ref_Step("通知人工更换料盘");
                            }
                            break;

                        case "通知人工更换料盘":
                            TrayBox.Rst();
                            WriteErrToUser($"{Name}:料仓里料盘都已经使用完，请更换料仓里料盘");
                            Ref_Step("判断当前工位是否存在料仓");
                            break;
                    }
                    Sleep(100);
                }
            }
            catch (Exception ex)
            {
                WriteErrToUser($"{Name}:出现异常，{ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex)}");
                WriteErrToUser($"{Name}:出现异常，{ExceptionHelper.GetInnerExceptionMessage(ex)}");
            }
        }

        public bool WaitAxisPoint(SingleAixPosAndSpeedMsgAndIMov pos)
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

        public string GetInputInfo(params Iodll.IoInHelper[] ios)
        {
            string info = string.Empty;
            foreach (var item in ios)
            {
                info += $"(IO点位名称:{item.Remark},IO模块名称:{item.ioHelper.Name},索引:{string.Join(",", item.InBytes)}";
            }
            return info;
        }

        public void ErrorDialog(string content, params (string, Action)[] valueTurpes)
        {
            Task<string> result = null;
            content = $"{Name}:{content}";
            App.Current.Dispatcher.Invoke(() =>
            {
                WriteErrToUser(content);
                List<string> list = new List<string>();
                result = new View.WarningView(content,
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
