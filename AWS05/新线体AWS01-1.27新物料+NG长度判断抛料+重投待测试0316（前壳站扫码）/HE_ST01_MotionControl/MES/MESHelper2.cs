using DataAnalysis;
using Handler.Core;
using Handler.Funs;
using Handler.Motion;
using Handler.Product;
using Handler.View;
using HE_ST01_MotionControl.Core;
using IConnectDLL.MethodCall;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Threading;
using static DataAnalysis.JsonClass;

namespace Handler.MES
{
    class MESHelper2
    {
        public class MESResponse
        {
            public MESResponse(string _command, bool _result, string _sn, string _errMsg)
            {
                Command = _command;
                Result = _result;
                SN = _sn;
                ErrMsg = _errMsg;
            }
            public MESResponse(string _command, bool _result, string _errMsg)
            {
                Command = _command;
                Result = _result;
                ErrMsg = _errMsg;
            }
            public string Command { get; set; }
            public bool Result { get; set; }
            public string SN { get; set; }
            public string ErrMsg { get; set; }
        }
        public static readonly MESHelper2 Cur = new MESHelper2();
        private MESHelper2()
        {
            MesConnect.RecevieMsgActionEventHander += GetMesMsg;
            resList = new DispatcherCollection<MESResponse> { };
            //BindingOperations.EnableCollectionSynchronization(resList, _lock);

        }
        private readonly object _lock = new object();

        public static MESHelper2 CreateInstance()
        {
            return Cur;
        }

        public IConnectDLL.IConnect MesConnect => Handler.Connect.ConnectFactory.tcpServerMes;

        private ILog_dll.LogProcessHelper _mesLogger = new ILog_dll.LogProcessHelper("MES运控日志");

        public Action<string> WriteToUserEventHandler;
        private readonly object _lockSend = new object();
        public void WriteToUser(string msg) => _mesLogger.Write(msg);


        public DispatcherCollection<MESResponse> resList;

        public DataAnalysis.MotionSendMsg motionSendMsg = new DataAnalysis.MotionSendMsg(FunMES.Cur.MESStationName.GetValue);
        public DataAnalysis.MotionReciveMsg motionReciveMsg = new DataAnalysis.MotionReciveMsg(FunMES.Cur.MESStationName.GetValue);
        private void GetMesMsg(string obj)
        {
            //resListCheck.Clear();
            this.WriteToUser("收到MES消息:" + obj);
            string command = motionReciveMsg.GetCommand(obj);
            this.WriteToUser("收到MES消息解析命令头为:" + command);
            WriteToUserEventHandler?.Invoke(obj);
            bool result;
            string errMsg;
            string sn;
            try
            {
                switch (command)
                {
                    case "0x02":    //checksn
                        result = motionReciveMsg.GetCheckSnResult(obj, out sn, out errMsg);
                        //此处应该还要out一个sn
                        //resListCheck.Add(new MESResponse(command, result, errMsg));
                        resList.Add(new MESResponse(command, result, sn, errMsg));
                        break;
                    case "0x03":
                        string SN = "";
                        short ngCode = 0;
                        result = motionReciveMsg.GetBindResult(obj, ref SN, ref ngCode, out errMsg);
                        resList.Add(new MESResponse(command, result, SN, ngCode + "; " + errMsg));
                        break;
                    case "0x04":    //checksn
                        result = motionReciveMsg.GetDataUpResult(obj, out sn, out errMsg);
                        //此处应该还要out一个sn
                        //resListCheck.Add(new MESResponse(command, result, errMsg));
                        resList.Add(new MESResponse(command, result, sn, errMsg));
                        break;
                    case "0x05":    //换型
                        var resRe = mesReciveMsg.ReModel_AnalysisMesReciveMsg(obj, out errMsg);

                        if (resRe != null && ((BackCommonJsonDataToMes)resRe.Data).Result == "OK")
                        {
                            StaticInitial.Motion.WriteToUser($"通知Mes转换到型号{WorkItemManagerHelper.LoadedName}成功");
                            MesReModel = true;
                        }
                        else
                        {
                            StaticInitial.Motion.WriteErrToUser($"通知Mes转换到型号{WorkItemManagerHelper.LoadedName}失败，请查看Mes型号");
                            MesReModel = false;
                        }
                        //if (StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                        //{
                        //    this.WriteToUser("响应MES换型命令");
                        //    try
                        //    {
                        //        var modelName = motionReciveMsg.GetNewModel(obj, out errMsg);
                        //        var modeArray = WorkItemManagerHelper.workIterm.ItermMessagesCollection;
                        //        for (int i = 0; i < modeArray.Count; i++)
                        //        {
                        //            if (modeArray[i].Name == modelName)
                        //            {
                        //                WorkItemManagerHelper.workIterm.ModifyCurrentPath(modeArray[i]);
                        //                DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes0 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                        //                {
                        //                    Msg = "OK",
                        //                    Result = "OK"
                        //                };
                        //                string firstResult0 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes0);

                        //                MesConnect.Send(firstResult0);
                        //                return;
                        //            }
                        //        }

                        //        DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes1 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                        //        {
                        //            Msg = "NG",
                        //            Result = "NG"
                        //        };
                        //        string firstResult1 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes1);

                        //        MesConnect.Send(firstResult1);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        StaticInitial.Motion.WriteErrToUser("MES换型时发生异常：" + ex.Message);
                        //    }
                        //}
                        //else
                        //{
                        //    StaticInitial.Motion.WriteErrToUser("当前处于自动流程，无法响应MES换型命令");
                        //    DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                        //    {
                        //        Msg = "当前处于自动流程，无法响应MES换型命令",
                        //        Result = "NG"
                        //    };
                        //    string firstResult = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes);

                        //    MesConnect.Send(firstResult);
                        //}

                        break;
                    case "0x06":    //清料
                        if (motionReciveMsg.GetInstructionResult(obj, out errMsg))
                        {
                            this.WriteToUser("响应MES清料命令");
                            //待完善清料方法；

                            DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes2 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                            {
                                Msg = "响应MES清料命令",
                                Result = "OK"
                            };
                            string firstResult2 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes2);
                            MesConnect.Send(firstResult2);
                        }
                        break;
                    case "0x07":    //开始
                        if (motionReciveMsg.GetInstructionResult(obj, out errMsg))
                        {
                            if (StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                            {
                                this.WriteToUser("响应MES开始命令");
                                StaticInitial.Motion.CommitStartRunOrder();
                            }
                            DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes3 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                            {
                                Msg = "响应MES开始命令",
                                Result = "OK"
                            };
                            string firstResult3 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes3);

                            MesConnect.Send(firstResult3);
                        }
                        break;
                    case "0x08":    //停止
                        if (motionReciveMsg.GetInstructionResult(obj, out errMsg))
                        {
                            if (StaticInitial.Motion.CurOrder.IsAutoRunning == true)
                            {
                                this.WriteToUser("响应MES停止命令");
                                StaticInitial.Motion.CommitStopOrder();
                            }
                            DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes4 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                            {
                                Msg = "响应MES停止命令",
                                Result = "OK"
                            };
                            string firstResult4 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes4);

                            MesConnect.Send(firstResult4);
                        }
                        break;
                    case "0x09":    //复位
                        if (motionReciveMsg.GetInstructionResult(obj, out errMsg))
                        {
                            if (StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                            {
                                this.WriteToUser("响应MES复位命令");
                                StaticInitial.Motion.CommitResOrder();
                                DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes5 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                                {
                                    Msg = "响应MES复位命令",
                                    Result = "OK"
                                };
                                string firstResult5 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes5);

                                MesConnect.Send(firstResult5);
                            }
                            else
                            {
                                StaticInitial.Motion.WriteErrToUser("MES复位失败，当前处于自动流程，无法执行MES复位命令");
                                DataAnalysis.JsonClass.BackCommonJsonDataToMes backCommonJsonDataToMes5 = new DataAnalysis.JsonClass.BackCommonJsonDataToMes
                                {
                                    Msg = "MES复位失败，当前处于自动流程，无法执行MES复位命令",
                                    Result = "NG"
                                };
                                string firstResult5 = motionSendMsg.BackReModelMsgToMes(backCommonJsonDataToMes5);

                                MesConnect.Send(firstResult5);
                            }
                        }

                        break;

                    default:
                        throw new Exception("没有此命令");
                }
            }
            catch (Exception ex)
            {
                StaticInitial.Motion.WriteErrToUser("收到MES消息处理MES命令时发生异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                WriteToUser("收到MES消息处理MES命令时发生异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
            }

        }
        public bool CheckSN(string _sN, out string err)
        {
            DataAnalysis.JsonClass.CheckSnJsonDataToMes checkSnJsonDataToMes = new DataAnalysis.JsonClass.CheckSnJsonDataToMes
            {
                Sn = _sN,
                Model = WorkItemManagerHelper.LoadedName
            };
            string firstResult = motionSendMsg.SendCheckSnMsgToMes(checkSnJsonDataToMes);
            return MESResult(firstResult, "0x02", _sN, out err);
        }
        public bool Bind(string _sN, string _lenSN, out string err)
        {

            DataAnalysis.JsonClass.BindJsonDataToMes bindJsonDataToMes = new DataAnalysis.JsonClass.BindJsonDataToMes
            {
                Sn = _sN,
                Model = WorkItemManagerHelper.LoadedName,
                LensCode = _lenSN
            };
            string firstResult = motionSendMsg.SendBindMsgToMes(bindJsonDataToMes);
            return MESResult(firstResult, "0x03", _sN, out err);
        }
        DataAnalysis.MesReciveMsg mesReciveMsg = new DataAnalysis.MesReciveMsg();
        DataAnalysis.MesSendMsg mesSendMsg = new DataAnalysis.MesSendMsg();
        private bool MesReModel = false;
        public async Task<bool> SwitchModel()
        {
            MesReModel = false;
            ReModelJsonDataToMotion_And_GetLaserCarvingCodeJsonDataToMes reModelJsonDataToMotion = new ReModelJsonDataToMotion_And_GetLaserCarvingCodeJsonDataToMes
            {
                Model = WorkItemManagerHelper.LoadedName
            };


            var str = mesSendMsg.SendRemodelMsgToMotion(FunMES.Cur.MESStationName.GetValue, reModelJsonDataToMotion);

            StaticInitial.Motion.WriteToUser($"通知Mes转换到型号{WorkItemManagerHelper.LoadedName}");
            if (!MesConnect.IsConnected)
            {
                StaticInitial.Motion.WriteErrToUser("MES通讯未连接，请检查通讯状态");
                return false;
            }
            lock (_lockSend)
            {
                MesConnect.Send(str);
            }
            await Task.Delay(5000);
            if (MesReModel)
            {
                return true;
            }
            else
            {
                StaticInitial.Motion.WriteErrToUser("通知Mes换型等待回复超时，请检查Mes型号");
                return false;
            }
        }
        public bool DataUp(ProductInfo product, bool isUpEnabled, out string err)
        {
            var vr = product.GetAllTestItems();
            DataAnalysis.JsonClass.DataUpJsonDataToMes dataUpJsonDataToMes = new DataAnalysis.JsonClass.DataUpJsonDataToMes
            {
                Sn = product.SN,
                StartTime = product.StartTime,
                Model = WorkItemManagerHelper.LoadedName,
                EndTime = product.EndTime,
                UpEnabled = isUpEnabled ? "1" : "0",
                TestResult = product.Result == "1" ? "OK" : "NG",
                TestValue = new List<TestData> { },
                TestImage = new List<string> { "" },
                EndImage = new List<string> { "" }

            };
            for (int i = 0; i < vr.Count; i++)
            {
                DataAnalysis.JsonClass.TestData testData = new DataAnalysis.JsonClass.TestData
                {
                    LowLimit = vr[i].LowLimit,
                    UpLimit = vr[i].UpLimit,
                    Result = vr[i].Result,
                    TestItem = vr[i].Name,
                    Value = vr[i].Value

                };
                dataUpJsonDataToMes.TestValue.Add(testData);
            }
            string firstResult = motionSendMsg.SendDataUpMsgToMes(dataUpJsonDataToMes);

            return MESResult(firstResult, "0x04", product.SN, out err);
        }
        //private string sn;
        public bool MESResult(string _firstResult, string _command, string _sn, out string _err)
        {
            _err = string.Empty;
            bool? result = null;
            string tempErr = string.Empty;

            if (!MesConnect.IsConnected)
            {
                _err = "MES通讯未连接，请检查通讯状态";
                return false;
            }
            try
            {
                MesConnect.Send(_firstResult);

                var frame = new DispatcherFrame();
                var startTime = DateTime.Now;

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };

                timer.Tick += (s, e) =>
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > 10000)
                    {
                        tempErr = "MES回复超时";
                        WriteToUser(tempErr);
                        timer.Stop();
                        frame.Continue = false;
                        return;
                    }

                    var matched = resList.SafeFirstOrDefault(item => item.SN == _sn && item.Command == _command);
                    if (matched != null)
                    {

                        result = matched.Result;
                        tempErr = matched.ErrMsg;
                        timer.Stop();
                        frame.Continue = false;


                    }
                };

                timer.Start();
                Dispatcher.PushFrame(frame); // 启动消息泵，避免界面卡死

                _err = tempErr;
                return result == true;
            }
            catch (Exception ex)
            {
                _err = "向MES发送消息时异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex);
                WriteToUser(_err);
                return false;
            }
        }

        public virtual void RemoveFailItem(DispatcherCollection<MESResponse> mESResponses, string SN, string command)
        {
            var items = mESResponses.SafeWhereToList(item => item.SN == SN && item.Command == command && item.Result == false);
            if (items.Count() > 0)
            {
                //自动删除resList失败的数据
                foreach (var item in items)
                {
                    mESResponses.Remove(item);
                }
            }
        }
    }
}
