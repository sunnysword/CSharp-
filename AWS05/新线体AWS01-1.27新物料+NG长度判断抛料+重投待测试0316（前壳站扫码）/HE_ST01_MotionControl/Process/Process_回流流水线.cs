using Handler.Connect;
using Handler.Motion.IO;
using Handler.Process.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Process
{
    internal class Process_回流流水线 : WorkStationBase
    {
        public static readonly Process_回流流水线 Cur = new Process_回流流水线();
        public int maxPoint = 3;

        public List<料位Model> 料位 = new List<料位Model>();

        public bool 回流料准备信号 = false;
        private Process_回流流水线() : base("回流流线流程")
        {

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
                {

                    while (true)
                    {
                        if (Process_流线流程.Cur.公共料位正在运行())
                        {
                            WriteToUser("回流公共料位正在运行...");
                            continue;
                        }
                        break;
                    }

                    if (StaticIOHelper.回流移栽气缸.OriginPos(out string error))
                    {
                        StaticIOHelper.回流移栽皮带线正反转.Out_OFF();
                        StaticIOHelper.回流移栽皮带线启停.Out_ON();
                        StaticIOHelper.后段回流线启停.Out_ON();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                    StaticIOHelper.回流移栽皮带线正反转.Out_OFF();

                    StaticIOHelper.后段回流线启停.Out_OFF();
                    Thread.Sleep(500);
                    if (StaticIOHelper.回流移栽气缸.WorkPos(out string error))
                    {
                        回流料准备信号 = true;
                        return true;
                    }
                    else
                        return false;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In();
                },
                流出后料到位检测 = () =>
                {
                    return !StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In();
                },
                出料皮带启动 = () =>
                {

                    return true;
                },
                出料皮带停止 = () =>
                {

                    return true;
                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In();

                },
                //首站 = true,
                料位名称 = "移栽上料盘位",
            });//移栽回流
            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_ON();
                    StaticIOHelper.后段回流线启停.Out_ON();
                    return true;
                },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_OFF();
                    StaticIOHelper.后段回流线启停.Out_OFF();

                    return true;
                },
                出料皮带启动 = () =>
                {
                    while (true)
                    {
                        if (Process_流线流程.Cur.公共料位正在运行())
                        {
                            WriteToUser("回流公共料位正在运行...");
                            continue;
                        }
                        break;
                    }

                    if (StaticIOHelper.回流移栽气缸.OriginPos(out string error))
                    {
                        Thread.Sleep(400);
                        StaticIOHelper.回流移栽皮带线正反转.Out_OFF();
                        StaticIOHelper.回流移栽皮带线启停.Out_ON();
                        StaticIOHelper.后段回流线启停.Out_ON();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.回流移栽皮带线启停.Out_OFF();
                    StaticIOHelper.后段回流线启停.Out_OFF();
                    Thread.Sleep(500);
                    if (StaticIOHelper.回流移栽气缸.WorkPos(out string error))
                    {
                        回流料准备信号 = true;
                        return true;
                    }
                    else
                        return false;
                },
                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.回流线后段缓存位流进检测.In() && !StaticIOHelper.回流线后段缓存位流出检测.In()
                    && !StaticIOHelper.回流线前段缓存位流进检测.In() && !StaticIOHelper.回流线前段缓存位流出检测.In();

                },
                //////////////////
                流出后料到位检测 = () =>
                {
                    return (StaticIOHelper.前回流移栽载具流出光电检测.In() && !StaticIOHelper.前回流移栽载具流进光电检测.In())
                   && !StaticIOHelper.回流线后段缓存位流进检测.In() && !StaticIOHelper.回流线后段缓存位流出检测.In();

                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.回流线后段缓存位流进检测.In() && !StaticIOHelper.回流线后段缓存位流出检测.In();

                },
                料位名称 = "回流后端皮带",
            });//回流前段皮带

            料位.Add(new 料位Model()
            {
                进料皮带启动 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_ON();
                    return true;
                },
                进料皮带停止 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_OFF();

                    return true;
                },

                出料皮带启动 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_ON();
                    StaticIOHelper.后段回流线启停.Out_ON();
                    return true;
                },
                出料皮带停止 = () =>
                {
                    StaticIOHelper.前段回流线启停.Out_OFF();
                    StaticIOHelper.后段回流线启停.Out_OFF();

                    return true;
                },

                流进后料到位检测 = () =>
                {
                    return StaticIOHelper.回流线前段缓存位流进检测.In() && !StaticIOHelper.回流线前段缓存位流出检测.In();


                },
                流出后料到位检测 = () =>
                {
                    return StaticIOHelper.回流线后段缓存位流进检测.In() && !StaticIOHelper.回流线后段缓存位流出检测.In()
                    && !StaticIOHelper.回流线前段缓存位流进检测.In() && !StaticIOHelper.回流线前段缓存位流出检测.In();
                },
                静态到位检测 = () =>
                {
                    return StaticIOHelper.回流线前段缓存位流进检测.In() && !StaticIOHelper.回流线前段缓存位流出检测.In();

                },
                料位名称 = "回流前端皮带",
                首站 = true,
            });//回流后段皮带
            //&& 
        }


        public void 初始化料位状态()
        {

            //皮带主流程

            for (int i = 0; i < maxPoint; i++)
            {
                if (料位[i].静态到位检测())
                {
                    料位[i].料位准备就绪 = 料位状态.料位可放行;
                }
                else
                {
                    料位[i].料位准备就绪 = 料位状态.空料位;
                }

                料位[i].前站要料信号 = false;
            }
            料位[0].当站顶升气缸 = null;
            料位[1].当站顶升气缸 = StaticIOHelper.回流线后段阻挡气缸;
            料位[2].当站顶升气缸 = StaticIOHelper.回流线前段阻挡气缸;

            料位[0].后序工站顶升气缸 = null;
            料位[1].后序工站顶升气缸 = null;
            料位[2].后序工站顶升气缸 = StaticIOHelper.回流线后段阻挡气缸; ;


            料位[0].前序工站顶升气缸 = null;
            料位[1].前序工站顶升气缸 = StaticIOHelper.回流线前段阻挡气缸;
            料位[2].前序工站顶升气缸 = null;


        }
        public void FreshCurrentState()
        {

        }
        bool 模拟皮带终端要料信号 = true;
        protected override void Action()
        {

        }

        public bool 公共料位正在运行()
        {
            return (料位[0].料位准备就绪 == 料位状态.正在放行 && 料位[1].料位准备就绪 == 料位状态.正在放行);
        }

        string error = "";
        protected override void ImplementRunAction()
        {

            初始化料位状态();
            WriteToUser("开始回流流程");
            while (true)
            {
                WaitPause();
                if (!IsProcessRunning)
                {
                    break;
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
                            料位[i].前站要料信号 = Process_流线流程.Cur.需要回流料 ? Process_流线流程.Cur.载具要料信号 : 模拟皮带终端要料信号;
                            if (Process_流线流程.Cur.载具要料信号)
                            {
                                Process_流线流程.Cur.载具要料信号 = false;
                            }
                        }
                        else
                        {
                            料位[i].前站要料信号 = 料位[i - 1].前站要料信号;//空料可以传递要料信号
                        }
                        if (料位[i].首站)
                        {
                            //首站由于没有其他节点供料，则一直等待直至到有料
                            if (Handler.Funs.FunSelection.Cur.是否和ST02通信.GetValue)
                            {
                                if(!ConnectFactory.后站回料准备信号.In())
                                {
                                    continue;
                                }
                                ConnectFactory.当站回流要料信号.Out_ON();
                                ConnectFactory.当站回流完成信号.Out_OFF();

                            }
                            if (料位[i].放料到当前节点流程 != null && !料位[i].放料到当前节点流程.IsCompleted)
                            {

                            }
                            else
                            {
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
                                        lock (料位[index])
                                        {
                                            料位[index].料位准备就绪 = 料位状态.料位可放行;
                                            if(料位[index].首站)
                                            {
                                                ConnectFactory.当站回流要料信号.Out_OFF();
                                            }
                                        }
                                    }

                                });

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
                                料位[i].前站要料信号 = 模拟皮带终端要料信号;

                            }
                            else
                            {
                                if (料位[i - 1].料位准备就绪 == 料位状态.空料位)
                                {
                                    料位[i].前站要料信号 = 料位[i - 1].前站要料信号;
                                }
                            }
                        }

                        if (料位[i].前站要料信号)
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

                                        if (index != 0) 料位[index - 1].料位准备就绪 = 料位状态.料位可放行;
                                    }

                                }



                            });

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

                            model.出料皮带停止();
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

                        if (Handler.Funs.FunSelection.Cur.是否和ST02通信.GetValue && model.料位名称 == "回流前端皮带")
                        {
                            if (ConnectFactory.后站回料准备信号.In())
                            {
                                ConnectFactory.当站回流要料信号.Out_OFF();
                            }
                        }
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
                            if (Handler.Funs.FunSelection.Cur.是否和ST02通信.GetValue && model.料位名称 == "回流前端皮带")
                            {
                                if (!ConnectFactory.后站回料完成信号.In())
                                {
                                    break;
                                }
                                ConnectFactory.当站回流要料信号.Out_OFF();
                            }
                            ConnectFactory.当站回流完成信号.Out_ON();
                            model.进料皮带停止();
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
