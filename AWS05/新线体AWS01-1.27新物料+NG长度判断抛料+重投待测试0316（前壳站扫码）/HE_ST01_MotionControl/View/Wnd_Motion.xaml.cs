using AccustomeAttributedll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThreadInfoRecorddll;
using Handler.Motion;
using Handler.Motion.Axis;

namespace Handler.View
{
    /// <summary>
    /// Wnd_Motion.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_Motion : Window
    {
        static Wnd_Motion Cur = null;
        static readonly object ob_lock = new object();
        private Wnd_Motion()
        {
            InitializeComponent();
         

        }

        bool IsExit = false;

        public static Wnd_Motion CreateInstrance()
        {
            if (Cur == null)
            {
                lock (ob_lock)
                {
                    if (Cur == null)
                    {
                        Cur = new Wnd_Motion();
                    }
                }
            }

            return Cur;
        }

        public static void SingleShow()
        {
            Wnd_Motion wnd_Motio = CreateInstrance();
            wnd_Motio.WindowState = WindowState.Normal;
            wnd_Motio.Show();
            wnd_Motio.Focus();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MotionWndViewModelHelper motionWindowViewModel = MotionWndViewModelHelper.CreateInstrance();

            //绑定内容控件
            Binding binding = new Binding("UserControlCurShow") { Source = motionWindowViewModel };
            Motion_Content.SetBinding(ContentProperty, binding);

            //创建按钮列表
           
            foreach (var vr in motionWindowViewModel.unit_UserViewModelsList)
            {
                Motion_SwitchBtns.Children.Add(vr.button);

            }
            this.Closed += delegate
            {
                Motion_SwitchBtns.Children.Clear();
            };


            motionWindowViewModel.IsCanShow = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MotionWndViewModelHelper motionWindowViewModel = MotionWndViewModelHelper.CreateInstrance();
            motionWindowViewModel.IsCanShow = false;
            Cur = null;
            IsExit = true;

        }

       
       
        RM_dll2.MotionHelper Motion => StaticInitial.Motion;

        void ShowUI()
        {
            //Thread.CurrentThread.Name = $"{Thread.CurrentThread.ManagedThreadId} {this.GetType().Name}_ShowUI";
            //ThreadInfoRecord.WriteInfo();
            //MotionWndViewModelHelper motionWindowViewModel = MotionWndViewModelHelper.CreateInstrance();
            //while (true)
            //{

            //    if (IsExit)
            //    {
            //        return;
            //    }


            //    this.Dispatcher.Invoke(()=>
            //    {

            //        motionWindowViewModel.ShowUI();


            //    });


            //    Thread.Sleep(100);
            //}

            //motionWindowViewModel.ShowUI();

        }

    }
}
