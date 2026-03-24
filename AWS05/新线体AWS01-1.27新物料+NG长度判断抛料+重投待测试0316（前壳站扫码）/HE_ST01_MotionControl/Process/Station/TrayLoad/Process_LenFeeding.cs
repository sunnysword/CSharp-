using AM.Core.IO;
using Handler.Process.RunMode;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station;
using Handler.Process;
using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HE_ST01_MotionControl.Process.Station.TrayLoad
{
    public class Process_LenFeeding : StationBase
    {
        public static readonly Process_LenFeeding Cur = new Process_LenFeeding();
        private Process_LenFeeding() : base("镜头皮带送料流程")
        {
        }
        public IoOutHepler 取镜头入料皮带电机 { get; set; }
        public IoOutHepler 取镜头到位皮带电机 { get; set; }//正转
        public IoOutHepler 取镜头到位皮带电机反转 { get; set; }
        public IoOutHepler 取镜头到位皮带电机减速 { get; set; }
        public IoOutHepler 允许镜头流线1放料 { get; set; }


        public Cylinder 取镜头进料皮带阻挡气缸 { get; set; }
        public Cylinder 取镜头到位皮带顶升气缸 { get; set; }
        //TODO public Cylinder 取镜头到位平台升降气缸 { get; set; }//这里也可以func保证平台气缸准备完成
        public Func<bool> 镜头平台准备;
        public Action<bool> 镜头平台有料;

        public IoInHelper 镜头流线1放料完成 { get; set; }
        public IoInHelper 镜头进料皮带入料光电 { get; set; }
        public IoInHelper 镜头进料皮带到位光电 { get; set; }

        public IoInHelper 镜头到位皮带入料光电 { get; set; }
        public IoInHelper 镜头到位皮带到位光电 { get; set; }
        public IoInHelper 镜头到位皮带减速光电 { get; set; }
        public int StepTimeout = 5;//单位秒


        public IAllowPlace PlaceStation { get; set; }//这个是干什么用的？

        public bool CanSkipStep { get; set; }
        public DateTime timeout = DateTime.Now;

        protected override void ImplementRunAction()
        {

            IsReady = false;
            string[] CamValue = new string[] { "", "", "", "" };

            string error = string.Empty;

            Ref_Step("检测镜头平台准备");
            if (!WaitIO(取镜头进料皮带阻挡气缸, CylinderActionType.WorkPos))
            {

            }
            取镜头入料皮带电机.Out_OFF();
            取镜头到位皮带电机.Out_OFF();
            取镜头到位皮带电机反转.Out_OFF();
            允许镜头流线1放料.Out_OFF();
            取镜头到位皮带电机减速.Out_OFF();
            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }
                switch (Step)
                {
                    case "检测镜头平台准备":
                        {
                            if (!镜头平台准备())
                            {
                                Sleep(1000);
                                break;
                            }

                            if (CanSkipStep)
                            {
                                SleepLoopCheck(10000);
                                break;
                            }

                            if (!WaitIO(取镜头到位皮带顶升气缸, CylinderActionType.OriginPos))
                            {
                                break;
                            }

                            // 判断模式是否为清料模式 
                            // 判断进料和到位光电都无料
                            // 入料和到位皮带正转，允许上一工位放料
                            if (Process_AutoRun.Cur.IsClearMode)
                            {
                                WriteToUser($"1----清料模式,不允许再供料", Brushes.Yellow, false);
                                Sleep(1000);

                                break;

                            }

                            if (镜头进料皮带入料光电.In() || 镜头进料皮带到位光电.In() || 镜头到位皮带入料光电.In() || 镜头到位皮带到位光电.In() || 镜头到位皮带减速光电.In())
                            {
                                WriteToUser($"当前镜头供料皮带检测到有载具，不能供料,请清空", Brushes.Red, false);
                                Sleep(1000);

                                break;
                            }
                            取镜头入料皮带电机.Out_ON();
                            取镜头到位皮带电机.Out_ON();
                            取镜头到位皮带电机反转.Out_OFF();
                            允许镜头流线1放料.Out_ON();
                            if (!WaitIO(取镜头进料皮带阻挡气缸, CylinderActionType.OriginPos))
                            {
                                break;
                            }
                            Ref_Step("等待放料完成");
                            timeout = DateTime.Now;
                            break;
                        }
                    case "等待放料完成":
                        {
                            if (CurrentWorkMode.Mode == WorkMode.TestRun)
                            {
                                WriteToUser("空跑模式，跳过镜头喂料料盘流入", Brushes.Yellow, false);
                                Thread.Sleep(3000);
                                Ref_Step("镜头平台料准备");
                                break;
                            }
                            if (镜头到位皮带减速光电.In())//无论哪一步减速光电有信号都要将镜头到位皮带减速
                            {
                                取镜头到位皮带电机减速.Out_ON();
                            }
                            if (镜头到位皮带到位光电.In())
                            {
                                取镜头到位皮带电机.Out_OFF();
                                取镜头到位皮带电机减速.Out_OFF();
                            }
                            if (镜头流线1放料完成.In())
                            {
                                if (!WaitIO(取镜头进料皮带阻挡气缸, CylinderActionType.WorkPos))
                                {
                                    break;
                                }
                                允许镜头流线1放料.Out_OFF();
                            }

                            // 检测放料完成信号，如果此时遇到减速光电则切换到减速
                            if (镜头流线1放料完成.In() && 镜头进料皮带入料光电.In() && 镜头进料皮带到位光电.In())
                            {
                                WriteToUser("检测到镜头入料OK", Brushes.White, false);
                                取镜头入料皮带电机.Out_OFF();
                                timeout = DateTime.Now;
                                Ref_Step("等待到位完成");
                                break;
                            }
                            if ((DateTime.Now - timeout).TotalSeconds > StepTimeout)
                            {
                                ErrorDialog("等待镜头入料光电信号或放料完成信号超时！请人工处理后重试",
                                  ("忽略", () =>
                                  {
                                      timeout = DateTime.Now;
                                      Ref_Step("等待到位完成");
                                  }
                                ),
                                  ("处理完毕", () =>
                                  {
                                      timeout = DateTime.Now;
                                  }
                                )
                               );
                            }
                            break;

                        }
                    case "等待到位完成":
                        if (镜头到位皮带减速光电.In())//无论哪一步减速光电有信号都要将镜头到位皮带减速
                        {
                            取镜头到位皮带电机减速.Out_ON();
                        }
                        if (镜头到位皮带到位光电.In())
                        {
                            取镜头到位皮带电机.Out_OFF();
                            取镜头到位皮带电机减速.Out_OFF();
                        }
                        if (镜头流线1放料完成.In())
                        {
                            if (!WaitIO(取镜头进料皮带阻挡气缸, CylinderActionType.WorkPos))
                            {
                                break;
                            }
                            允许镜头流线1放料.Out_OFF();
                        }

                        if (镜头到位皮带入料光电.In() && 镜头到位皮带到位光电.In() && 镜头到位皮带减速光电.In())
                        {
                            WriteToUser("检测到镜头到位OK", Brushes.White, false);
                            Ref_Step("镜头平台料准备");
                            break;
                        }

                        if ((DateTime.Now - timeout).TotalSeconds > StepTimeout)
                        {
                            ErrorDialog("等待镜头到位光电信号超时！请人工处理后重试",
                              ("忽略", () =>
                              {

                                  Ref_Step("镜头平台料准备");
                              }
                            ),
                              ("处理完毕", () =>
                              {
                                  timeout = DateTime.Now;
                              }
                            )
                           );
                        }
                        break;

                    case "镜头平台料准备":
                        镜头平台有料(true);
                        取镜头入料皮带电机.Out_OFF();
                        取镜头到位皮带电机.Out_OFF();
                        取镜头到位皮带电机反转.Out_OFF();
                        允许镜头流线1放料.Out_OFF();
                        取镜头到位皮带电机减速.Out_OFF();
                        if (!WaitIO(取镜头到位皮带顶升气缸, CylinderActionType.WorkPos))
                        {

                            break;
                        }

                        Ref_Step("检测镜头平台准备");
                        break;

                    default:
                        throw new Exception($"{Name}:当前步骤不存在，当前步骤：{Step}");

                }

                Sleep(10);
            }


        }


    }
}
