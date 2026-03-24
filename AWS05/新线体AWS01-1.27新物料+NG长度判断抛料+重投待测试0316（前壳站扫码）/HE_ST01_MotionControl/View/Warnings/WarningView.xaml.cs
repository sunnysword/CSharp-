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
using System.Windows.Media.Effects;

namespace Handler.View
{
    /// <summary>
    /// WarningView.xaml 的交互逻辑
    /// </summary>
    public partial class WarningView : Window
    {
        private WarningView()
        {

            InitializeComponent();

            this.Owner = Application.Current.MainWindow;

        }

        public string ReturnContent { get; private set; }

        bool _canClose = false;

        private string[] _buttonContents;

        public static List<WarningView> WarningViewList = new List<WarningView>();
        public WarningView(string message, params string[] buttonContents) : this()
        {
            WarningViewList.Add(this);
            _buttonContents = buttonContents;
            this.textBlockMessage.Text = message;
        }

        public async new Task<string> ShowDialog()
        {
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            foreach (var item in _buttonContents)
            {
                Button button = new Button
                {
                    Height = 30,
                    Width = 70,
                    Content = item,
                    Background = System.Windows.Media.Brushes.LightBlue,
                    Margin = new Thickness(3)
                };
                button.Click += (s, e) =>
                {
                    ReturnContent = button.Content.ToString();
                    this.Close();
                };
                stackPanel.Children.Add(button);
            }
            Grid.SetColumn(stackPanel, 2);
            Grid.SetRow(stackPanel, 2);
            this.grid.Children.Add(stackPanel);
            base.Show();
            while (true)
            {
                if (_canClose)
                    break;
                if (string.IsNullOrWhiteSpace(ReturnContent))
                {
                    await Task.Delay(100);
                }
                else
                {
                    break;
                }
            }
            return ReturnContent;
        }

        public static void CloseAllWindows()
        {
            foreach (var item in WarningViewList)
            {
                item._canClose = true;
                item?.Close();
            }
            for (int i = 0; i < WarningViewList.Count; i++)
            {
                WarningViewList[i] = null;
            }
            WarningViewList.Clear();
        }



        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BuzzerOnEventHandler?.Invoke();
            //if (BuzzerTime > 0)
            //{
            //    await Task.Delay((int)BuzzerTime);
            //    BuzzerOffEventHandler?.Invoke();
            //}
            //if (!_needCloseManual)
            //{
            //    this.DialogResult = true;

            //    this.Close();
            //}


        }



    }
}
