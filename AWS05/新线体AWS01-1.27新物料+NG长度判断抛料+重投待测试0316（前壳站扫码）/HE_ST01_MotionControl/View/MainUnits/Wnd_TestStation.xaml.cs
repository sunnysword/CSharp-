using System;
using System.Collections.Generic;
using System.Linq;
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
 
using Handler.Process.Station;

namespace Handler.View.MainUnits
{
    /// <summary>
    /// Wnd_TestStation.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_TestStation : Window
    {
        static readonly object ob_lock = new object();
        static Wnd_TestStation Cur = null;
        private Wnd_TestStation()
        {
            InitializeComponent();
            Init();
        }

        public static Wnd_TestStation CreateInstrance()
        {
            if (Cur == null)
            {
                lock (ob_lock)
                {
                    if (Cur == null)
                    {
                        Cur = new Wnd_TestStation();
                    }
                }
            }

            return Cur;
        }

        public static void SingleShow()
        {
            Window window = CreateInstrance();
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowState = WindowState.Normal;
            window.Show();

        }

        void Init()
        {
            void SetTextBlockProperty(TextBlock textBlock)
            {
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.Margin = new Thickness(5);
            }
            void SearchAllButtonsAndSetButtonEnable(bool isEnable, DependencyObject element)
            {
                //遍历逻辑树
                if (!(element is DependencyObject))
                    return;
                foreach (var item in LogicalTreeHelper.GetChildren(element))
                {
                    if (item is Button button)
                    {
                        button.IsEnabled = isEnable;
                    }
                    SearchAllButtonsAndSetButtonEnable(isEnable, item as DependencyObject);
                }


            }
           
        }

        bool IsExit = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Cur = null;
            IsExit = true;
        }
      

      

    }
}
