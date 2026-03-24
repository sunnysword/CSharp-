using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Handler
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        public static bool isirLog = true;

        public static readonly string Cfg = "SysCfg";
        private ILog_dll.LogProcessHelper _log = new ILog_dll.LogProcessHelper("异常闪退");

        static App()
        {
            //创建默认的配置文件夹
            if (!System.IO.Directory.Exists(Cfg))
            {
                System.IO.Directory.CreateDirectory(Cfg);
            }
        }

        public App()
        {
            this.Startup += App_Startup;
            this.Startup += new StartupEventHandler(CheckLogin);
            this.Startup += new StartupEventHandler(StartProgram);

            AM.Communication.CommunicationCommon.WriteErrToUserEventHandler
                   += s => Motion.StaticInitial.Motion.WriteToUser(s, System.Windows.Media.Brushes.Red, false);
            AM.Communication.CommunicationCommon.GetCurrentModePathEventHandler += () => Handler.Motion.ParamPath_Motion.SelectedDirPath;

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;      //UI全局异常未捕获
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException; //Task线程未捕获异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; //多线程异常
        }
        ~App()
        {
            AppStartHelper.StopOtherPro();
        }

        /// <summary>
        /// UI全局异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            var content = "全局UI异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(e.Exception);
            MessageBox.Show(content);
            _log.Write("全局UI异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(e.Exception));
            _log.Write("全局UI异常无法处理基本信息:" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
            //e.Handled = true;
            // MessageBox.Show("全局UI异常无法处理:" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
        /// <summary>
        /// Task线程未捕获异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var content = "全局Task异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(e.Exception);
            MessageBox.Show(content);
            _log.Write("全局Task异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(e.Exception));
            _log.Write("全局Task异常无法处理基本信息:" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
        /// <summary>
        /// 多线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            if (e.ExceptionObject is System.Exception ex)
            {
                var content = "全局多线程异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex);
                MessageBox.Show(content);
                _log.Write("全局多线程异常无法处理详细信息:" + AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex));
                _log.Write($"全局多线程异常无法处理基本信息:{ex.Message}" + "\r\n" + ex.StackTrace);
                // MessageBox.Show($"全局异常无法处理:{ ex.Message}");
            }
        }

        private void CheckLogin(object sender, StartupEventArgs e)
        {
            if (isirLog)
            {
                IrisLoginHelper.MainWindow wnd = new IrisLoginHelper.MainWindow();
                bool IsLoginOK = true;
                wnd.LoginFailedEventHanlder += () =>
                {
                    IsLoginOK = false;
                    this.Shutdown(0);
                    Environment.Exit(0);
                };
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            else
            {
                if (App.isirLog)
                {
                    IrisLoginHelper.MainWindow wnd = new IrisLoginHelper.MainWindow();
                    bool IsLoginOK = true;
                    wnd.LoginFailedEventHanlder += () =>
                    {
                        IsLoginOK = false;
                        this.Shutdown(0);
                        Environment.Exit(0);
                    };
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wnd.ShowDialog();
                }
                else
                {
                    Wpf_LoginWnd.LoginMainWnd wnd = new Wpf_LoginWnd.LoginMainWnd();
                    bool IsLoginOK = true;
                    wnd.LoginFailedEventHanlder += () =>
                    {
                        IsLoginOK = false;
                        this.Shutdown(0);
                        Environment.Exit(0);
                    };
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wnd.ShowDialog();
                }

            }


          
            //if(IsLoginOK)
            //{
            //    Window window = new Window();
            //    window.Title = "工单信息输入";
            //    window.Height = 300;
            //    window.Width = 600;
            //    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //    MES_YF_dll.MES_WorkOrderSetting mES_WorkOrderSetting = new MES_YF_dll.MES_WorkOrderSetting();
            //    window.Content = mES_WorkOrderSetting;
            //    mES_WorkOrderSetting.MakeSureActionEventHandler += window.Close;
            //    window.ShowDialog();

            //}

        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //建立进城单例

            bool ret;
            mutex = new System.Threading.Mutex(true, "WPFDemoMN200", out ret);
            if (!ret)
            {

                MessageBox.Show("Program is open!", "Operation Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);

            }



            //Process.GetCurrentProcess().ProcessorAffinity= new IntPtr(Convert.ToInt32("1".PadLeft(Environment.ProcessorCount / 2, '1'), 2));//设置当前允许使用的处理器数


        }

        static List<string> processesWaitStart = new List<string>();


        void StartProgram(object sender, StartupEventArgs e)
        {
            AppStartHelper.StartProgram();
        }

    }
}
