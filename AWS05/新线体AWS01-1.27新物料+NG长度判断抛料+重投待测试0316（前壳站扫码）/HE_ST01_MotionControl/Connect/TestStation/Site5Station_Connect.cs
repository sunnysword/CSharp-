 
using Handler.Motion.IO;
using Handler.Process.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdpWpfDemo3;

namespace Handler.Connect
{
    /// <summary>
    /// 工位2通讯
    /// </summary>
    public class Site5Station_Connect : SiteUdpConnectBase
    {
        public static readonly Site5Station_Connect Cur = new Site5Station_Connect();
        private Site5Station_Connect() : base(ConnectFactory.Connect_Test,"工位五测试软件", 4)
        {
            checkMTPCommandsList.Add(CheckReload);
            checkMTPCommandsList.Add(CheckCurrent);
            RegisterCheckCommand();

        }

        /// <summary>
        /// 重新上顶
        /// </summary>
        CheckMTPCommand CheckReload = new CheckMTPCommand("RELOAD");
        CheckMTPCommand CheckCurrent = new CheckMTPCommand("CURRENT");

        void RegisterCheckCommand()
        {
            CheckCurrent.SetCheckAction(s =>
            {
                try
                {
                    WriteToUser($"{Name}开始响应：{s.command}");

                    MTP mc2Test = new MTP();
                    mc2Test.command = s.command;
                    StringBuilder stringBuilder = new StringBuilder();
                    ConnectFactory.serialPortPower.CheckCurrentAndGetVoltage(Power3Demo.ChannelIndex.First,
                        out string current, out string vol);
                    current = (double.Parse(current) * 1000).ToString();
                    vol = (double.Parse(vol) / 1000.00d).ToString();
                    stringBuilder.Append($"{current}:{vol};");
                    ConnectFactory.serialPortPower.CheckCurrentAndGetVoltage(Power3Demo.ChannelIndex.Second,
                        out current, out vol);
                    current = (double.Parse(current) * 1000).ToString();
                    vol = (double.Parse(vol) / 1000.00d).ToString();
                    stringBuilder.Append($"{current}:{vol};");
                    ConnectFactory.serialPortPower.CheckCurrentAndGetVoltage(Power3Demo.ChannelIndex.Third,
                        out current, out vol);
                    current = (double.Parse(current) * 1000).ToString();
                    vol = (double.Parse(vol) / 1000.00d).ToString();
                    stringBuilder.Append($"{current}:{vol};");
                    mc2Test.result = "OK";
                    mc2Test.text = stringBuilder.ToString();
                    Connect.Send(mc2Test);
                    WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
                }
                catch (Exception ex)
                {
                    Handler.Motion.StaticInitial.Motion.WriteErrToUser("收到测试获取电流命令后处理异常:"+ex.Message);
                }
              
            });

            CheckReload.SetCheckAction(s =>
            {
                WriteToUser($"{Name}开始响应：{s.command}");

                //MTP mc2Test = new MTP();
                //mc2Test.command = s.command;
                //PowerHelper.White_Power_OFF();
                //StaticIOHelper.Cy_IlluminateTheLiftingCylinder.OriginPos(out _);
                //System.Threading.Thread.Sleep(500);
                //StaticIOHelper.Cy_IlluminateTheLiftingCylinder.WorkPos(out _);
                //PowerHelper.White_Power_ON();
                //PowerHelper.White_WaitON_Finished();
                //System.Threading.Thread.Sleep(500);
                //mc2Test.result = "OK";
                //Connect.Send(mc2Test);
                //WriteToUser($"{Name}发送：{mc2Test.command}, {mc2Test.text}, {mc2Test.result}");
            });
        }

    }
}
