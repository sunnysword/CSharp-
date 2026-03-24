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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Handler.View.Pallet
{
    /// <summary>
    /// ButtonGroupView.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonGroupView : UserControl
    {
        public ButtonGroupView()
        {
            InitializeComponent();
        }

        public void AddButton(string content,Action butttonClickEvent)
        {
            Button button = new Button();
            button.Margin = new Thickness(5);
            button.Content = content;
            button.Click += (s,e)=>
            {
                try
                {
                    butttonClickEvent?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            this.stackPanel.Children.Add(button);
        }
        public void AddButton(string content, UserControl view)
        {
            Button button = new Button();
            button.MinWidth = 40;
            button.MinHeight = 20;
            button.Content = content;
            button.Margin = new Thickness(5);
            button.Click += (s, e) =>
            {
                try
                {
                    Window window = new Window();
                    window.WindowState = WindowState.Maximized;
                    window.Content = view;
                    window.Title = content.ToString();
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            this.stackPanel.Children.Add(button);
        }

    }
}
