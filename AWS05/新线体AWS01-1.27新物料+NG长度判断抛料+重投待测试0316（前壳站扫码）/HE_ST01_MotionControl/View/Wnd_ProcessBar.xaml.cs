using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Handler.View
{
    /// <summary>
    /// Wnd_ProcessBar.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_ProcessBar : Window
    {
        public Wnd_ProcessBar()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }

        public static Wnd_ProcessBar Cur { get; private set; }
        static readonly object ob_lock = new object();
        static bool IsCanClose = false;

        public static Wnd_ProcessBar CreateInstrance()
        {
            if (Cur == null)
            {
                lock (ob_lock)
                {
                    if (Cur == null)
                    {
                        Cur = new Wnd_ProcessBar();
                    }
                }
            }

            return Cur;
        }

        public static void SingleShow()
        {
            Window window = CreateInstrance();
            window.Owner = App.Current.MainWindow;
            //window.Topmost = true;
            window.Show();
            window.Focus();

        }
        public static void SingleClose()
        {
            if (Cur != null)
            {
                IsCanClose = true;
                Cur?.Close();
            }

        }

        public void WriteProcessLog(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (pBar_1.Value >= pBar_1.Maximum)
                {
                    pBar_1.Value = 0;
                }

                pBar_1.Value++;

                rtb_log.AppendText(str + "\n");
                rtb_log.ScrollToEnd();
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Cur = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsCanClose)
            {
                IsCanClose = false;
                e.Cancel = true;
                return;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsCanClose = false;
        }
    }
}
