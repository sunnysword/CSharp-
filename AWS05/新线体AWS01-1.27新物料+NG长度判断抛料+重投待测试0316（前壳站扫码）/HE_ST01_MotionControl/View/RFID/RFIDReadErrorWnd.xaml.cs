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

namespace Handler.View.RFID
{
    /// <summary>
    /// RFIDReadErrorWnd.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDReadErrorWnd : Window
    {
        public RFIDReadErrorWnd()
        {
            InitializeComponent();
        }

        public string Btn_NoCarrier { get; set; } = "此站没有治具";
        public string Btn_UseManualInput { get; set; } = "手动输入RFID内容";
        public string Btn_ReadOnceMore { get; set; } = "重新读取一次";
        /// <summary>
        /// 手动输入内容
        /// </summary>
        public string ManualInputContent { get; set; }

        public string BtnClickContent;

        public string ShowAndReturn(string processName,string errorInfo)
        {
           
                BtnClickContent = string.Empty;
                ManualInputContent = string.Empty;
                this.Title = $"{processName}:RFID读取失败";
                this.textBlock_Header.Text= $"{processName}RFID读取失败，失败原因:{errorInfo}";
                this.Owner = App.Current.MainWindow;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.ShowDialog();
                ManualInputContent = this.textBox_ManualInput.Text;
                return BtnClickContent;
            
        }

        public void ShowAndReturn(string processName, string errorInfo, Action btn1Action, Action btn2Action, Action btn3Action)
        {
            var str = this.ShowAndReturn(processName,errorInfo);
            if (str == this.Btn_NoCarrier)
                btn1Action?.Invoke();
            if (str == this.Btn_UseManualInput)
                btn2Action?.Invoke();
            if (str == this.Btn_ReadOnceMore)
                btn3Action?.Invoke();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            BtnClickContent = button.Content.ToString();

            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
