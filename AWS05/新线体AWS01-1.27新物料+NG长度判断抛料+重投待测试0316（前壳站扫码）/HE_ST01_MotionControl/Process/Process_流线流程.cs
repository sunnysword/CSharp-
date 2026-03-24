using AM.Core.IO;
using Handler.Connect;
using Handler.Motion.IO;
using Handler.Process.RunMode;
using Handler.Process.Station;
using HE_ST01_MotionControl.Process;
using IConnectDLL.MethodCall;
using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.Process.Station
{
    public enum 料位状态
    {
        料位就绪,
        料位可放行,
        正在放行,
        空料位,
    }

    public class 料位Model
    {
        public string Step要料 = "";
        public string Step放料 = "";
        public 料位状态 料位准备就绪 { get; set; } = 料位状态.空料位;
        public Cylinder 当站顶升气缸 { get; set; }
        public Cylinder 后序工站顶升气缸 { get; set; }

        public Cylinder 前序工站顶升气缸 { get; set; }
        public Func<bool> 进料皮带启动 { get; set; }
        public Func<bool> 进料皮带停止 { get; set; }

        public Func<bool> 出料皮带启动 { get; set; }
        public Func<bool> 出料皮带停止 { get; set; }
        public Func<bool> 流进后料到位检测 { get; set; }//此时当站有料，前一站无料

        public Func<bool> 流出后料到位检测 { get; set; }//此时当站无料，后一站有料
        public Func<bool> 静态到位检测 { get; set; }

        public bool 前站要料信号 { get; set; }

        public Task<bool> 放料到当前节点流程 { get; set; }
        public bool 首站 { get; set; }

        public string 料位名称 { get; set; }
    }

    //注意，正向是要料流程，回流是放料流程
    public class Process_流线流程 : WorkStationBase
    {
        public static readonly Process_流线流程 Cur = new Process_流线流程();
        public int maxPoint = 5;

        public List<料位Model> 料位 = new List<料位Model>();

        private Process_流线流程() : base("总流线流程")
        {
            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_ON();
                    return true;
                },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_OFF();
                    return true;
                },

                出料皮带启动 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_ON();
                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_OFF();
                    return true;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.PCB缓存位载具流进检测.In() && !StaticIOHelper.PCB缓存位载具流出检测.In()
                    && !StaticIOHelper.PCB上料位载具流出检测.In() && !StaticIOHelper.PCB上料位载具流进检测.In() && !StaticIOHelper.输送线后段皮带线载具流出检测.In();

                },
                流出后料到位检测 = () =>
                {
                    return !StaticIOHelper.PCB缓存位载具流进检测.In() && !StaticIOHelper.PCB缓存位载具流出检测.In();

                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.PCB缓存位载具流进检测.In() && !StaticIOHelper.PCB缓存位载具流出检测.In()
                    && !StaticIOHelper.PCB上料位载具流出检测.In();

                },
                前站要料信号 = 模拟皮带终端要料信号,
                料位名称 = "PCB预备位",
            });//pcb预备

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
          {
              StaticIOHelper.后段输送线启停.Out_ON();
              StaticIOHelper.前段输送线启停.Out_ON();
              return true;
          },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_OFF();
                    StaticIOHelper.前段输送线启停.Out_OFF();
                    return true;
                },
                出料皮带启动 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_ON();
                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.后段输送线启停.Out_OFF();
                    return true;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.PCB上料位载具流出检测.In() && !StaticIOHelper.PCB上料位载具流进检测.In()
                    && !StaticIOHelper.输送线后段皮带线载具流入检测.In() && !StaticIOHelper.外壳缓存位载具流进检测.In() && !StaticIOHelper.外壳缓存位载具流出检测.In();

                },
                流出后料到位检测 = () =>
                {
                    return StaticIOHelper.PCB缓存位载具流进检测.In() && !StaticIOHelper.PCB缓存位载具流出检测.In()
                   && !StaticIOHelper.PCB上料位载具流出检测.In() && !StaticIOHelper.PCB上料位载具流进检测.In() && !StaticIOHelper.输送线后段皮带线载具流出检测.In();
                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.PCB上料位载具流出检测.In() && !StaticIOHelper.PCB上料位载具流进检测.In()
                    && !StaticIOHelper.输送线后段皮带线载具流入检测.In();

                },
                料位名称 = "PCB上料位",
            });//pcb

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
         {

             StaticIOHelper.前段输送线启停.Out_ON();
             return true;
         },
                进料皮带停止 = () =>
                {

                    StaticIOHelper.前段输送线启停.Out_OFF();
                    return true;
                },
                出料皮带启动 = () =>
                {
                    StaticIOHelper.前段输送线启停.Out_ON();
                    StaticIOHelper.后段输送线启停.Out_ON();
                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.前段输送线启停.Out_OFF();
                    StaticIOHelper.后段输送线启停.Out_OFF();
                    return true;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.外壳缓存位载具流进检测.In() && !StaticIOHelper.外壳缓存位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流出检测.In() && !StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.外壳上料位载具流出检测.In();

                },
                流出后料到位检测 = () =>
                {
                    return !StaticIOHelper.PCB上料位载具流出检测.In() && StaticIOHelper.PCB上料位载具流进检测.In()
                    && !StaticIOHelper.输送线后段皮带线载具流入检测.In() && !StaticIOHelper.外壳缓存位载具流进检测.In() && !StaticIOHelper.外壳缓存位载具流出检测.In();

                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.外壳缓存位载具流进检测.In() && !StaticIOHelper.外壳缓存位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流出检测.In();

                },
                料位名称 = "前壳预备位",
            });//前壳预备

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
          {
              while (true)
              {
                  if (Process_回流流水线.Cur.公共料位正在运行())
                  {
                      WriteToUser("公共料位正在占用...");
                      continue;
                  }
                  break;
              }
              Thread.Sleep(200);

              lock (StaticIOHelper.回流移栽皮带线启停)
              {
                  StaticIOHelper.回流移栽皮带线正反转.Out_ON();
                  StaticIOHelper.回流移栽皮带线启停.Out_ON();
              }

              StaticIOHelper.前段输送线启停.Out_ON();
              return true;
          },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                    StaticIOHelper.回流移栽皮带线正反转.Out_OFF();

                    StaticIOHelper.前段输送线启停.Out_OFF();
                    return true;
                },

                出料皮带启动 = () =>
                {
                    StaticIOHelper.前段输送线启停.Out_ON();

                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.前段输送线启停.Out_OFF();

                    return true;
                },

                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.外壳上料位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流入检测.In() && !StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In();

                },
                流出后料到位检测 = () =>
                {
                    return StaticIOHelper.外壳缓存位载具流进检测.In() && !StaticIOHelper.外壳缓存位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流出检测.In() && !StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.外壳上料位载具流出检测.In();

                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.外壳上料位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流入检测.In();

                },
                料位名称 = "前壳上料备位",
            });//前壳

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
         {
             while (true)
             {
                 if (Process_回流流水线.Cur.公共料位正在运行())
                 {
                     WriteToUser("公共料位正在占用...");
                     continue;
                 }
                 break;
             }
             Thread.Sleep(200);

             StaticIOHelper.回流移栽皮带线正反转.Out_ON();
             StaticIOHelper.回流移栽皮带线启停.Out_ON();

             return true;
         },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                    StaticIOHelper.回流移栽皮带线正反转.Out_OFF();


                    return true;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.前回流移栽载具流出光电检测.In() || StaticIOHelper.前回流移栽载具流进光电检测.In();
                },
                流出后料到位检测 = () =>
                {
                    return StaticIOHelper.外壳上料位载具流进检测.In() && !StaticIOHelper.外壳上料位载具流出检测.In()
                    && !StaticIOHelper.输送线前段皮带线载具流入检测.In() && !StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In();

                },
                出料皮带启动 = () =>
                {
                    while (true)
                    {
                        if (Process_回流流水线.Cur.公共料位正在运行())
                        {
                            WriteToUser("公共料位正在占用...");
                            continue;
                        }
                        break;
                    }
                    Thread.Sleep(200);
                    StaticIOHelper.回流移栽皮带线正反转.Out_ON();
                    StaticIOHelper.回流移栽皮带线启停.Out_ON();
                    StaticIOHelper.前段输送线启停.Out_ON();
                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                    StaticIOHelper.回流移栽皮带线正反转.Out_OFF();

                    StaticIOHelper.前段输送线启停.Out_OFF();
                    return true;
                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.前回流移栽载具流出光电检测.In() || StaticIOHelper.前回流移栽载具流进光电检测.In();

                },
                首站 = true,
                料位名称 = "移栽上料盘位",
            });//移栽进

            //&& 
        }

        public bool 公共料位正在运行()
        {
            return (料位[3].料位准备就绪 == 料位状态.正在放行 && 料位[4].料位准备就绪 == 料位状态.正在放行) || 料位[4].料位准备就绪 == 料位状态.料位可放行;
        }


        public void 初始化料位状态()
        {

            //皮带主流程

            for (int i = 0; i < maxPoint; i++)
            {
                if (料位[i].静态到位检测())
                {
                    料位[i].料位准备就绪 = 料位状态.料位就绪;
                }
                else
                {
                    料位[i].料位准备就绪 = 料位状态.空料位;
                }

                料位[i].前站要料信号 = false;
            }
            料位[0].当站顶升气缸 = StaticIOHelper.PCB缓存位阻挡气缸;
            料位[1].当站顶升气缸 = StaticIOHelper.PCB上料位阻挡气缸;
            料位[2].当站顶升气缸 = StaticIOHelper.外壳缓存位阻挡气缸;
            料位[3].当站顶升气缸 = StaticIOHelper.外壳上料位阻挡气缸;
            料位[4].当站顶升气缸 = null;

            料位[0].后序工站顶升气缸 = null;
            料位[1].后序工站顶升气缸 = StaticIOHelper.PCB缓存位阻挡气缸;
            料位[2].后序工站顶升气缸 = StaticIOHelper.PCB上料位阻挡气缸;
            料位[3].后序工站顶升气缸 = StaticIOHelper.外壳缓存位阻挡气缸;
            料位[4].后序工站顶升气缸 = StaticIOHelper.外壳上料位阻挡气缸;

            料位[0].前序工站顶升气缸 = StaticIOHelper.PCB上料位阻挡气缸;
            料位[1].前序工站顶升气缸 = StaticIOHelper.外壳缓存位阻挡气缸;
            料位[2].前序工站顶升气缸 = StaticIOHelper.外壳上料位阻挡气缸;
            料位[3].前序工站顶升气缸 = null;
            料位[4].前序工站顶升气缸 = null;

        }
        public void FreshCurrentState()
        {

        }
        public bool 模拟皮带终端要料信号 = true;
        public bool 需要回流料 = true;
        public bool 载具要料信号 = false;
        protected override void Action()
        {

        }

        string error = "";
        protected override void ImplementRunAction()
        {

            初始化料位状态();
            WriteToUser("开始流水线流程");
            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }
                if (Process_回流流水线.Cur.回流料准备信号 && !需要回流料)
                {
                    WriteToUser("已检测到回流道上有载具，不再接收模拟载具信号，如需要中途再添加新载具，请在界面上复位回流料载具");
                    需要回流料 = true;
                }
                
                //从后往前依次要料,要料后间隔一个才能要料
                for (int i = 0; i < maxPoint; i++)
                {
                    料位状态 current = 料位状态.空料位;
                    lock (料位[i])
                    {
                        current = 料位[i].料位准备就绪;
                    }


                    if (current == 料位状态.空料位)
                    {
                        if (i == 0)
                        {
                            if (Funs.FunSelection.Cur.是否和ST02通信.GetValue)
                            {
                                料位[i].前站要料信号 = ConnectFactory.后站要料信号.In();
                                if (!料位[i].前站要料信号)
                                {
                                    ConnectFactory.当站完成信号.Out_OFF();
                                    ConnectFactory.当站料准备信号.Out_OFF();
                                }
                            }
                            else
                            {
                                料位[i].前站要料信号 = 模拟皮带终端要料信号;
                            }



                        }
                        else
                        {
                            料位[i].前站要料信号 = 料位[i - 1].前站要料信号;//空料可以传递要料信号
                        }
                        if (料位[i].首站)
                        {
                            //首站由于没有其他节点供料，则一直等待直至到有料
                            if (料位[i].放料到当前节点流程 != null && !料位[i].放料到当前节点流程.IsCompleted)
                            {

                            }
                            else if (!需要回流料)
                            {
                                Thread.Sleep(500);
                                int index = i;
                                lock (料位[index])
                                {
                                    料位[i].料位准备就绪 = 料位状态.正在放行;
                                }

                                料位[index].放料到当前节点流程 = Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        if (SubTask要料(料位[index]))
                                        {

                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    finally
                                    {
                                        if (index == maxPoint - 1 && !StaticIOHelper.回流移栽气缸.GetWorkPosState())
                                        {
                                            while (true)
                                            {
                                                if (Process_回流流水线.Cur.公共料位正在运行())
                                                {
                                                    WriteToUser("公共料位正在占用...");
                                                    continue;
                                                }
                                                Thread.Sleep(200);
                                                break;
                                                
                                            }
                                        }

                                        lock (料位[index])
                                        {
                                            料位[index].料位准备就绪 = 料位状态.料位可放行;
                                        }
                                    }

                                });

                            }
                            else
                            {
                                if (Process_回流流水线.Cur.回流料准备信号)
                                {
                                    载具要料信号 = false;
                                    Process_回流流水线.Cur.回流料准备信号 = false;
                                    Thread.Sleep(200);
                                    料位[i].料位准备就绪 = 料位状态.料位可放行;
                                }
                                else
                                {
                                    载具要料信号 = true;
                                }
                            }

                        }
                    }
                    else if (current == 料位状态.正在放行)
                    {

                    }
                    else if (current == 料位状态.料位就绪)//等待其他工站
                    {

                    }
                    else if (current == 料位状态.料位可放行 && (i == 0 || 料位[i - 1].料位准备就绪 == 料位状态.空料位))
                    {
                        lock (料位[i])
                        {
                            if (i == 0)
                            {
                                if (Funs.FunSelection.Cur.是否和ST02通信.GetValue)
                                {
                                    if (!料位[i].前站要料信号)
                                    {
                                        ConnectFactory.当站完成信号.Out_OFF();
                                        
                                    }
                                    
                                    ConnectFactory.当站料准备信号.Out_ON();
                                }
                                else
                                {
                                    料位[i].前站要料信号 = 模拟皮带终端要料信号;
                                }

                            }
                            else
                            {
                                if (料位[i - 1].料位准备就绪 == 料位状态.空料位)
                                {
                                    料位[i].前站要料信号 = 料位[i - 1].前站要料信号;
                                }
                            }
                        }

                        if (((i != 0 || !Funs.FunSelection.Cur.是否和ST02通信.GetValue) && 料位[i].前站要料信号) || (i == 0 && Funs.FunSelection.Cur.是否和ST02通信.GetValue && ConnectFactory.后站要料信号.In()))
                        {
                            if (料位[i].放料到当前节点流程 != null && !料位[i].放料到当前节点流程.IsCompleted) continue;//正在执行的节点不可执行
                            int index = i;
                            lock (料位[index])
                            {
                                料位[index].前站要料信号 = false;
                                料位[i].料位准备就绪 = 料位状态.正在放行;

                                if (index != 0) 料位[index - 1].料位准备就绪 = 料位状态.正在放行;
                            }

                            料位[i].放料到当前节点流程 = Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    if (index == 0 && Funs.FunSelection.Cur.是否和ST02通信.GetValue)
                                    {
                                        ConnectFactory.当站完成信号.Out_OFF();
                                        ConnectFactory.当站料准备信号.Out_OFF();
                                    }
                                    if (SubTask放料(料位[index]))
                                    {

                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                finally
                                {
                                    lock (料位[index])
                                    {

                                        料位[index].料位准备就绪 = 料位状态.空料位;

                                        if (index != 0) 料位[index - 1].料位准备就绪 = 料位状态.料位就绪;

                                        if (index == 0 && Funs.FunSelection.Cur.是否和ST02通信.GetValue)
                                        {
                                            ConnectFactory.当站料准备信号.Out_OFF();
                                            ConnectFactory.当站完成信号.Out_ON();

                                        }
                                    }

                                }



                            });

                        }
                        else if (i == 0 && Funs.FunSelection.Cur.是否和ST02通信.GetValue && 料位[i].前站要料信号)
                        {
                            ConnectFactory.当站料准备信号.Out_ON();
                        }
                    }

                }

            }

        }

        protected bool SubTask放料(料位Model model)
        {
            Ref_Step放料(model, "开始放料");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }
                switch (model.Step放料)
                {
                    case "开始放料":
                        if (!(model.当站顶升气缸?.WorkPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }
                        if (!(model.后序工站顶升气缸?.WorkPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }
                        model.出料皮带启动();
                        if (!(model.当站顶升气缸?.OriginPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }

                        timer.Restart();
                        Ref_Step放料(model, "放料后等待");
                        break;
                    case "放料后等待":
                        model.出料皮带启动();
                        if (model.流出后料到位检测())
                        {
                            if (!(model.当站顶升气缸?.WorkPos(out error) ?? true))
                            {
                                WriteErrToUser(error);
                            }
                            if (!(model.后序工站顶升气缸?.WorkPos(out error) ?? true))
                            {
                                WriteErrToUser(error);
                            }
                            //model.出料皮带停止();
                            Ref_Step放料(model, "退出");
                        }
                        else
                        {
                            if (timer.ElapsedMilliseconds >= 10000)
                            {
                                error = $"等待流水线放料超时，放料位置：{model.料位名称}";
                                WriteErrToUser(error);
                                Ref_Step("放料后等待");

                            }
                        }
                        break;
                    case "退出":
                        return true;
                    case "NG退出":
                        return false;
                }
            }
            return true;
        }

        public void Ref_Step要料(料位Model model, string tmp)
        {
            model.Step要料 = tmp;
        }

        public void Ref_Step放料(料位Model model, string tmp)
        {
            model.Step放料 = tmp;
        }
        protected bool SubTask要料(料位Model model)//要料没有超时
        {
            Ref_Step要料(model, "开始要料");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
                }
                switch (model.Step要料)
                {
                    case "开始要料":
                        if (!(model.当站顶升气缸?.WorkPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }
                        if (!(model.前序工站顶升气缸?.WorkPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }
                        model.进料皮带启动();
                        if (!(model.前序工站顶升气缸?.OriginPos(out error) ?? true))
                        {
                            WriteErrToUser(error);
                        }

                        timer.Restart();
                        Ref_Step要料(model, "要料后等待");
                        break;
                    case "要料后等待":
                        if (model.流进后料到位检测())
                        {
                            if (!(model.当站顶升气缸?.WorkPos(out error) ?? true))
                            {
                                WriteErrToUser(error);
                            }
                            if (!(model.前序工站顶升气缸?.WorkPos(out error) ?? true))
                            {
                                WriteErrToUser(error);
                            }
                            // model.进料皮带停止();
                            Ref_Step要料(model, "退出");
                        }
                        else
                        {
                            //if (timer.ElapsedMilliseconds >= 10000)
                            //{
                            //    error = $"等待流水线要料超时，要料位置：{model.料位名称}";
                            //    WriteErrToUser(error);
                            //    Ref_Step("NG退出");
                            //}
                        }
                        break;
                    case "退出":
                        return true;
                    case "NG退出":
                        return false;
                }
            }
            return true;
        }
    }



}
