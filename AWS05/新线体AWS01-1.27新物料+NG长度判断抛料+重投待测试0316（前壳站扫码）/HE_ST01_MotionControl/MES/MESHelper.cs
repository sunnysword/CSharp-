using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Product;
using Newtonsoft.Json;

namespace Handler.MES
{
    public class MESHelper
    {
        public static readonly MESHelper Cur = new MESHelper();

        private MESHelper()
        {
            MESCommand.End = "#end";
            MESCommand.GetStationNameEventHandler += () => Funs.FunMES.Cur.MESStationName.GetValue;
            CommandLogin = new MESCommand("登录", "ff0x01", "LoginUp");
            CommandCheckSN = new MESCommand("CheckSn", "ff0x02", "CheckSn");
            CommandDataUpLoad = new MESCommand("数据上传", "ff0x04", "DataUp");
            CommandBind = new MESCommand("条码绑定", "ff0x03", "Bind");
            CommandAlarmUp = new MESCommand("故障收集", "ff0x14", "AlarmUp");
            CommandCheckDevice = new MESCommand("设备点检", "ff0x17", "CheckDevice");
            CommandGetDeviceStatus = new MESCommand("获取设备状态", "ff0x18", "GetDeviceStatus");
            MesConnect.RecevieMsgActionEventHander += Cur_getMasterMsgEventHandle;
            RM_dll2.AllErrInfoRecord.WriteErrToUserEvent += s => Task.Run(() =>
            {
                Fun_AlarmUp(s);
            });
            CheckCarrierdll.CarrierCheckManger.Cur.CheckFinishEventHandler += () =>
            {
                string command = $"ff0x16;Command:CheckCarrier;Station:{MESCommand.GetStationNameEventHandler?.Invoke()};Result=OK;#end";
                MesConnect.Send(command);
            };

            Motion.StaticInitial.Motion.actionClearErrEventHandler += () => Task.Run(() =>
            {
                DeviceStatus(3);
            });
            Motion.StaticInitial.Motion.actionStartEventHandler += () => Task.Run(() =>
            {
                DeviceStatus(0);
            });
            Motion.StaticInitial.Motion.actionPauseEventHandler += () => Task.Run(() =>
            {
                DeviceStatus(1);
            });
            Motion.StaticInitial.Motion.actionStopEventHandler += () => Task.Run(() =>
            {
                DeviceStatus(1);
            });
            Motion.StaticInitial.Motion.actionEmgEventHandler += () => Task.Run(() =>
            {
                DeviceStatus(2);
            });
        }

        /// <summary>
        /// 故障收集
        /// </summary>
        /// <param name="result">故障信息</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public void Fun_AlarmUp(string result)
        {
            if (!Funs.FunMES.Cur.IsDataUpErrorMsgs.GetValue)
            {
                return;
            }
            if (!Funs.FunSelection.Cur.IsUseMes.GetValue)
            {
                return;

            }

            try
            {
                // 使用正则表达式移除逗号、冒号和分号  
                var input = Regex.Replace(result, "[,:;]", "");
                input = Regex.Replace(input, @"\[.*?\]", string.Empty);
                //input.Replace("callName=", "");
                input.Replace("=", "");
                var pattern = @"[\u3001\u3002\uff0c\uff1a\uff1b\uff0e]"; // 中文逗号、句号、冒号、分号、中文句号  
                var output = Regex.Replace(input, pattern, "");
                var error = string.Empty;
                StringBuilder sb = new StringBuilder(128);
                //sb.Append("Data:");
                sb.Append($"{{\"ErrMsg\":\"{output}\",}}");
                //sb.Append($"StartTime={GetTimeStamp(DateTime.Now.ToString())}");
                //CommandAlarmUp.SetData(sb.ToString());
                CommandAlarmUp.SetDataWithCommand(sb.ToString());
                MESResponse(CommandAlarmUp, out error);
            }
            catch (Exception ex)
            {
                this.WriteToUser("DeviceStatus函数异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                //return false;
            }
        }
        public void DeviceStatus(int result)
        {

            if (!Funs.FunMES.Cur.IsDataUpErrorMsgs.GetValue)
            {
                return;
            }
            if (!Funs.FunSelection.Cur.IsUseMes.GetValue)
            {
                return;

            }
            try
            {
                var error = string.Empty;
                StringBuilder sb = new StringBuilder(128);
                //sb.Append("Data:");
                sb.Append($"{{\"Status\":\"{result}\"}}");
                //sb.Append($"StartTime={DateTime.Now.ToString()};");
                //CommandGetDeviceStatus.SetData(sb.ToString());
                CommandGetDeviceStatus.SetDataWithCommand(sb.ToString());
                MESResponse(CommandGetDeviceStatus, out error, false);
            }
            catch (Exception ex)
            {
                this.WriteToUser("DeviceStatus函数异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                //return false;
            }
        }

        public static MESHelper CreateInstance()
        {
            return Cur;
        }

        public IConnectDLL.IConnect MesConnect => (IConnectDLL.IConnect)Connect.ConnectFactory.tcpServerMes;

        private MESCommand CommandLogin;    //登录
        private MESCommand CommandCheckSN; //Check SN
        private MESCommand CommandDataUpLoad; //上传数据
        private MESCommand CommandBind; //绑定数据
        MESCommand CommandAlarmUp;//故障收集
        MESCommand CommandCheckDevice;//设备点检
        MESCommand CommandGetDeviceStatus;//获取设备状态

        private object _lockSend = new object();
        private ILog_dll.LogProcessHelper _mesLogger = new ILog_dll.LogProcessHelper("MES运控日志");

        public Action<string> WriteToUserEventHandler;

        public void WriteToUser(string msg) => _mesLogger.Write(msg);

        private bool MESResponse(MESCommand command, out string returnMessage, bool wait = true)
        {
            lock (_lockSend)
            {
                try
                {
                    if (MesConnect.IsConnected == false)
                    {
                        returnMessage = "MES通讯未连接，请检查通讯状态";
                        return false;
                    }
                    returnMessage = "";
                    string content = command.ToString();
                    WriteToUser("准备向MES发送信息:" + content);
                    if (MESCommand.MESCommandDict.ContainsKey(command.Header))
                    {
                        MESCommand.MESCommandDict[command.Header].Result = new MESResult();
                    }
                    MesConnect.Send(command.ToString());
                    WriteToUser("向MES发送信息完成:" + content);
                    if (!wait)
                    {
                        WriteToUser("此命令不需要等待接收MES回复信息");
                        return true;
                    }
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Restart();
                    while (true)
                    {
                        if (MESCommand.MESCommandDict[command.Header].Result.Result == true)
                        {
                            return true;
                        }
                        else if (MESCommand.MESCommandDict[command.Header].Result.Result == false)
                        {
                            returnMessage = MESCommand.MESCommandDict[command.Header].Result.Msg;
                            return false;
                        }
                        else if (timer.ElapsedMilliseconds > 100000)
                        {
                            returnMessage = "MES回复超时";
                            return false;
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    returnMessage = "向MES发送消息时异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex);
                    return false;
                }
            }
        }

        public void GetMesResult(string message)
        {
            MESResult result = new MESResult();
            string error = String.Empty;
            string header = String.Empty;
            try
            {
                string[] strArray = message.Split(';');
                header = strArray[0];
                result.Header = header;

                if (MESCommand.MESCommandDict.ContainsKey(strArray[0]) == false)
                {
                    error = "MES解析发现未注册该命令头:" + strArray[0] + "," + message;
                    WriteToUser(error);
                    throw new Exception(error);
                }
                else if (strArray.Length < 2)
                {
                    error = "收到MES消息解析后格式长度不正确：" + message;
                    WriteToUser(error);
                    result.Msg = error;
                    result.Result = false;
                }
                else
                {
                    //string data = strArray[2];
                    //string[] dataArray = strArray[2].Split(new string[] { "Data:" }, options: StringSplitOptions.None)[1].Split(new char[] { ',' }, options: StringSplitOptions.RemoveEmptyEntries);
                    var indexData = 2;
                    if (strArray[2].Contains("Data")) { indexData = 2; }
                    else if (strArray[3].Contains("Data")) { indexData = 3; }
                    string data = strArray[indexData];
                    string[] dataArray = strArray[indexData].Split(new string[] { "Data:" }, options: StringSplitOptions.None)[1].Split(new char[] { ',' }, options: StringSplitOptions.RemoveEmptyEntries);
                    string other = string.Empty;
                    foreach (var item in dataArray)
                    {
                        //if (item.Contains("Result="))
                        //{
                        //    result.Result = item.Split('=')[1].ToUpper() == "OK" ? true : false;
                        //}
                        if (item.Contains("Result"))
                        {
                            var items = item.Split(new string[] { "Result" }, StringSplitOptions.None);
                            result.Result = item.Split(new string[] { "Result" }, StringSplitOptions.None)[1].Trim().ToUpper().Contains("OK") ? true : false;
                        }
                        else if (item.Contains("Msg="))
                        {
                            result.Msg = item.Split('=')[1] + ":";
                        }
                        else
                        {
                            other += item + ",";


                        }
                    }
                    result.Msg += other;
                }

                MESCommand.MESCommandDict[header].Result = result;
            }
            catch (Exception ex)
            {
                WriteToUser($"运控解析MES信息:{message}出现异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                throw;
            }
            finally
            {
            }
        }

        public class ResponseData
        {
            public string Command { get; set; }
            public string Station { get; set; }
            public DataContent Data { get; set; }
            public string EndIdentifier { get; set; }

            public class DataContent
            {
                public string Sn { get; set; }
                public string Result { get; set; }
                public string Msg { get; set; }
            }
        }

        public MESResult GetMesResultForNew(string message)
        {
            try
            {

                MESResult result = new MESResult();

                string error = String.Empty;
                string header = String.Empty;
                short level = 0;
                if (message.Contains("0x01"))
                {
                    result.Header = "0x01";
                    var ret = MESCommand.dataRecieve.GetLoginResult(message, ref level, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                }
                else if (message.Contains("0x02"))
                {
                    result.Header = "0x02";
                    string sn = "";
                    var ret = MESCommand.dataRecieve.GetCheckSnResult(message, out sn, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                    result.Extra = sn;
                }
                else if (message.Contains("0x04"))
                {
                    result.Header = "0x04";
                    string sn = "";
                    //var ret = MESCommand.dataRecieve.GetDataUpResult(message, out sn, out error);
                    //if (error != "")
                    //{
                    //    WriteToUser(error);
                    //    //throw new Exception(error);
                    //}
                    var resultRsp = JsonConvert.DeserializeObject<ResponseData>(message);
                    result.Result = resultRsp?.Data?.Result == "OK";// ret;todo目前艾薇视自己代码返回NG
                    result.Extra = resultRsp?.Data?.Sn;
                }
                else if (message.Contains("0x03"))
                {
                    result.Header = "0x03";
                    short ngCode = 0;
                    string sn = "";
                    var ret = MESCommand.dataRecieve.GetBindResult(message,ref sn, ref ngCode, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                }
                else if (message.Contains("0x14"))
                {
                    result.Header = "0x14";

                    var ret = MESCommand.dataRecieve.GetAlarmUpResult(message, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                }
                else if (message.Contains("0x17"))
                {
                    result.Header = "0x17";

                    var ret = MESCommand.dataRecieve.GetPointInspectionResult(message, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                }
                else if (message.Contains("0x18"))
                {
                    result.Header = "0x18";

                    var ret = MESCommand.dataRecieve.GetUpStatusResult(message, out error);
                    if (error != "")
                    {
                        WriteToUser(error);
                        throw new Exception(error);
                    }
                    result.Result = ret;
                }
                return result;
            }
            catch (Exception ex)
            {
                WriteToUser($"运控解析MES信息:{message}出现异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                throw;
            }
            finally
            {
            }
        }

        private void Cur_getMasterMsgEventHandle(string obj)
        {
            this.WriteToUser("收到MES消息:" + obj);
            string cmd = obj.Split(';')[0];
            this.WriteToUser("收到MES消息解析命令头为:" + cmd);

            try
            {
                string commandHeader = "";
                var splits = cmd.Split(':', ',');
                if (splits.Length >= 2)
                {
                    commandHeader = splits[1].Replace("\"", "");
                }
                if (commandHeader != "")
                {
                    var hasDict = MESCommand.MESCommandDict.FirstOrDefault(o => o.Key.Contains(commandHeader));
                    if (hasDict.Key != "")
                    {
                        hasDict.Value.Result = GetMesResultForNew(obj);
                        return;
                    }
                }

                return;
                switch (cmd)
                {
                    case "ff0x05":    //换型
                        if (Motion.StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                        {
                            this.WriteToUser("响应MES换型命令");
                            try
                            {
                                var modelName = obj.Split(new string[] { "Model=" }, StringSplitOptions.None)[1].Split(';')[0];
                                var modeArray = View.WorkItemManagerHelper.workIterm.ItermMessagesCollection;
                                for (int i = 0; i < modeArray.Count; i++)
                                {
                                    if (modeArray[i].Name == modelName)
                                    {
                                        View.WorkItemManagerHelper.workIterm.ModifyCurrentPath(modeArray[i]);
                                        MesConnect.Send($"ff0x05;Command:Remodel;Station:{MESCommand.Station};Data:Result=OK;#end");
                                        this.WriteToUser($"ff0x05;Command:Remodel;Station:{MESCommand.Station};Data:Result=OK;#end");
                                        return;
                                    }
                                }

                                Motion.StaticInitial.Motion.WriteErrToUser("MES换型失败，未找到对应型号:" + modelName + "," +
                                    "MES原始命令:" + obj);
                                MesConnect.Send($"ff0x05;Command:Remodel;Data:Model=NG;#end");
                                this.WriteToUser($"ff0x05;Command:Remodel;Station:{View.WorkItemManagerHelper.LoadedName};#end");
                            }
                            catch (Exception ex)
                            {
                                Motion.StaticInitial.Motion.WriteErrToUser("MES换型时发生异常：" + ex.Message);
                            }
                        }
                        else
                        {
                            Motion.StaticInitial.Motion.WriteErrToUser("当前处于自动流程，无法响应MES换型命令");
                        }

                        break;
                    case "ff0x06":    //清料
                        this.WriteToUser("响应MES清料命令");
                        MesConnect.Send($"ff0x06;Command:Clean;#end");

                        break;
                    case "ff0x07":    //开始
                        if (Motion.StaticInitial.Motion.CurOrder.IsResOK == false)
                        {
                            this.WriteToUser("设备没有复位，无法响应MES启动命令");
                            Handler.Motion.StaticInitial.Motion.WriteErrToUser("设备没有复位，无法响应MES启动命令");
                            MesConnect.Send($"ff0x07;Command:Start;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=NG;#end");
                        }
                        else
                        {
                            if (Motion.StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                            {
                                this.WriteToUser("响应MES开始命令");
                                Motion.StaticInitial.Motion.CommitStartRunOrder();
                                MesConnect.Send($"ff0x07;Command:Start;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=OK;#end");
                            }
                            else
                            {
                                this.WriteToUser("设备已经启动，不需要响应MES启动命令");
                                MesConnect.Send($"ff0x07;Command:Start;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=NG;#end");
                            }
                        }


                        break;
                    case "ff0x08":    //停止
                        this.WriteToUser("响应MES停止命令");
                        Handler.Motion.StaticInitial.Motion.WriteToUser("响应MES停止命令");
                        Motion.StaticInitial.Motion.CommitStopOrder();
                        MesConnect.Send($"ff0x08;Command:Stop;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=OK;#end");

                        break;
                    case "ff0x09":    //复位
                        if (Motion.StaticInitial.Motion.CurOrder.IsAutoRunning == false)
                        {
                            this.WriteToUser("响应MES复位命令");
                            Motion.StaticInitial.Motion.CommitResOrder();
                        }
                        else if (Motion.StaticInitial.Motion.CurOrder.IsResing == true)
                        {
                            //Motion.StaticInitial.Motion.WriteErrToUser("MES复位失败，当前处于复位流程，无法执行MES复位命令");
                            //MesConnect.Send($"ff0x09;Command:Reset;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=NG;#end");
                        }
                        else
                        {
                            Motion.StaticInitial.Motion.WriteErrToUser("MES复位失败，当前处于自动流程，无法执行MES复位命令");
                            MesConnect.Send($"ff0x09;Command:Reset;Station:{MESCommand.GetStationNameEventHandler()};Data:Result=NG;#end");
                        }

                        break;

                    case "ff0x16":
                        CheckCarrierdll.WndCarrierCheckCreateView wndCarrierCheckCreateView = new CheckCarrierdll.WndCarrierCheckCreateView
                        {
                            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                        };
                        wndCarrierCheckCreateView.ShowDialog();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Motion.StaticInitial.Motion.WriteErrToUser("收到MES消息处理MES命令时发生异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                WriteToUser("收到MES消息处理MES命令时发生异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
            }
        }

        public bool? CheckLogin(string userName, string password, out string error)
        {
            error = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder(128);
                //sb.Append("Data:");
                sb.Append($"{{\"user\":\"{userName}\",\"password\":\"{password}\"}}");
                CommandLogin.SetDataWithCommand(sb.ToString());
                return MESResponse(CommandLogin, out error);
            }
            catch (Exception ex)
            {
                this.WriteToUser("CheckLogin函数异常" + ex.Message + ex.StackTrace);
                return false;
            }
        }

        public bool? CheckSN(string sn, out string error)
        {
            error = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder(128);
                //sb.Append("Station:");
                //sb.Append($"ST01;");
                //sb.Append("Data:");
                sb.Append($"{{\"Sn\":\"{sn}\",\"Model\":\"{View.WorkItemManagerHelper.LoadedName}\"}}");
                CommandCheckSN.SetDataWithCommand(sb.ToString());
                return MESResponse(CommandCheckSN, out error);
            }
            catch (Exception ex)
            {
                error = "运控软件执行异常，" + ex.Message;
                this.WriteToUser("CheckSN函数异常" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                return false;
            }
        }

        //todo这里json不对
        public bool? DataUp(ProductInfo product, out string error)
        {
            error = string.Empty;
            try
            {
                product.SetCurrentTimeToEndTime();
                var startTime = GetTimeStamp(product.StartTime);
                var endTime = GetTimeStamp(product.EndTime);
                StringBuilder sb = new StringBuilder(128);
                //sb.Append("Data:");
                sb.Append($"{{\"Sn\":\"{product.SN}\",\"TestResult\":\"{(product.Result == Product.ProductInfo.ResultOK ? "OK" : "NG")}\",\"Model\":\"{View.WorkItemManagerHelper.LoadedName}\",");
                sb.Append($"\"StartTime\":\"{startTime}\",\"EndTime\":\"{endTime}\",");
                var testItmes = this.GetTestValueItems(product);
                sb.Append($"{testItmes}}}");

                CommandDataUpLoad.SetDataWithCommand(sb.ToString());
                return MESResponse(CommandDataUpLoad, out error);
            }
            catch (Exception ex)
            {
                error = "运控软件执行异常，" + ex.Message;
                this.WriteToUser("DataUp函数异常:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                return false;
            }
        }
        //todo这里格式不对
        public bool? Bind(string sn, string pcbSN, out string error)
        {
            error = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder(128);
                sb.Append($"{{\"Sn\":\"{sn}\",\"QrCode\":{sn},\"PcbaCode1\":\"{pcbSN}\",\"Model\":\"{View.WorkItemManagerHelper.LoadedName}\"}}");

                CommandBind.SetDataWithCommand(sb.ToString());
                return MESResponse(CommandBind, out error);
            }
            catch (Exception ex)
            {
                error = "运控软件执行异常，" + ex.Message;
                this.WriteToUser("MES 条码绑定Bind函数异常" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                return false;
            }
        }

        public string GetTestValueItems(ProductInfo product)
        {
            StringBuilder sb = new StringBuilder(1024);
            sb.Append($"\"TestValue\":[],");
            foreach (var item in product.GetAllTestItems())
            {
                string resultStr = String.Empty;
                if (item.Result == Handler.Product.ProductInfo.ResultNG
                      || item.Result == "0" || item.Result.ToLower() == "ng"
                          || item.Result.ToLower() == "fail")
                {
                    resultStr = "NG";
                }
                else
                {
                    resultStr = "OK";
                }
                //old
                //sb.Append($"{item.Name}@{item.LowLimit}@{item.UpLimit}@{item.Value}@{resultStr}|");
                sb.Append($"{item.Name}@{item.LowLimit}{item.Unit}@{item.UpLimit}{item.Unit}@{item.Value}{item.Unit}@{resultStr}|");

            }
            if (product.GetAllTestItems().Count != 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }
            //sb.Append($"|锁附方案名@@@{Funs.FunScrewGun.Cur.ProgramNo.GetValue}@OK|");
            //sb.Append($"PCBSN@@@{product.PCBSN}@OK|");
            //sb.Append($"半成品测试方案名@@@{Funs.FunSelection.Cur.testPrpogramName.GetValue}@OK|");
            //视觉参数
            //sb.Append($"前壳相机检测@{product.Cam1BlobLimit}@@{product.Cam1BlobValue}@OK|");
            //sb.Append($"PCB相机取料@{product.Cam3PCBLimit}@@{product.Cam3PCBValue}@OK|");
            //sb.Append($"PCB相机检测@{product.Cam3PCBLimit}@@{product.Cam4PCBValue}@OK|");
            //sb.Append($"锁附定位@{product.Cam4ScrewLimit}@@{product.Cam4ScrewValue}@OK|");
            //sb.Append($"RFID治具@@@{product.RFID}@OK|");
            //sb.Append($"镭雕功率@64@66@{Funs.FunLaser.Cur.LaserCleanPower.GetValue.ToString()}@OK|");
            //sb.Append($"镭雕方案名@@@{Funs.FunLaser.Cur.LaserCodeModuleName.GetValue}@OK|");
            //sb.Append($"镭雕SN@@@{product.SN}@OK|");
            //sb.Append($"批头计数@@{Funs.FunDeveiceLife.Cur.StationBitsMaxCount.GetValue.ToString()}@{Funs.FunDeveiceLife.Cur.StationBitsUsageCurrenCount}@OK|");
            //sb.Append($"探针计数@@{Funs.FunDeveiceLife.Cur.StationProbeMaxCount.GetValue.ToString()}@{Funs.FunDeveiceLife.Cur.StationProbeUsageCurrenCount}@OK|");
            // sb.Append($"前壳料盘@@{
            // .Process_HolderLoad.TrayBox.FloorNums.GetValue.ToString()}@{PalletManger.Process_HolderLoad.TrayBox.Index}@OK|");
            // sb.Append($"PCB料盘@@{PalletManger.Process_PCBLoad.TrayBox.FloorNums.GetValue.ToString()}@{PalletManger.Process_PCBLoad.TrayBox.Index}@OK");
            //sb.Append($"PCB料盘@@@{PalletManger.Process_PCBLoad.TrayBox.FloorNums.GetValue.ToString()}|");
            //sb.Append(";");
            sb.Append($"\"TestImage\":[{product.TestPicturePath}],");
            sb.Append($"\"ResultImage\":[{product.ResultPicturePath}],");
            sb.Append($"\"Enabled\":\"1\",");
            //sb.Append($"\"Enabled\":\"1\",}}，");
            return sb.ToString();

            #region old
            //StringBuilder sb = new StringBuilder(1024);
            //foreach (var item in product.GetAllTestItems())
            //{
            //    string resultStr = String.Empty;
            //    if (item.Result == Handler.Product.ProductInfo.ResultNG
            //          || item.Result == "0" || item.Result.ToLower() == "ng"
            //              || item.Result.ToLower() == "fail")
            //    {
            //        resultStr = "NG";
            //    }
            //    else
            //    {
            //        resultStr = "OK";
            //    }
            //    sb.Append($"{item.Name}@{item.LowLimit}@{item.UpLimit}@{item.Value}@{resultStr}|");
            //}
            //if (product.GetAllTestItems().Count != 0)
            //{
            //    sb = sb.Remove(sb.Length - 1, 1);
            //}
            //sb.Append(";");
            //sb.Append($"LaserPower={Funs.FunLaser.Cur.LaserCleanPower.GetValue}%;");
            //sb.Append($"TestImage={product.TestPicturePath};");
            //sb.Append($"ResultImage={product.ResultPicturePath};");
            //sb.Append($"Enabled=1;");

            //return sb.ToString();
            #endregion

        }

        public string GetTimeStamp(string date)
        {
            DateTime dt = Convert.ToDateTime(date);
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long timeStamp = (long)(dt - startTime).TotalMilliseconds;
            return timeStamp.ToString();
        }
    }
}