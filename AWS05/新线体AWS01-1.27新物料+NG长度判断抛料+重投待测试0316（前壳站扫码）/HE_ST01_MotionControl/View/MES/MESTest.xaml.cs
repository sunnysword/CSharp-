using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Handler.MES;
using Handler.View;
using static DataAnalysis.JsonClass;

namespace Handler.View
{
    /// <summary>
    /// MESTest.xaml 的交互逻辑
    /// </summary>
    public partial class MESTest : Window
    {
        public MESTest()
        {

            InitializeComponent();
            myDataGrid.ItemsSource = MESHelper2.Cur.resList;
            txt_StartTimeInput.Text = Handler.Product.ProductInfo.GetMesCurrentTime();
            txt_EndTimeInput.Text = Handler.Product.ProductInfo.GetMesCurrentTime();

        }
        private void btn_CheckSN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_SN.Text))
            {
                MessageBox.Show("输入的SN不能为空！");
                return;
            }
            DataAnalysis.JsonClass.CheckSnJsonDataToMes checkSnJsonDataToMes = new DataAnalysis.JsonClass.CheckSnJsonDataToMes
            {
                Sn = txt_SN.Text,
                Model = WorkItemManagerHelper.LoadedName
            };
            string firstResult = MESHelper2.Cur.motionSendMsg.SendCheckSnMsgToMes(checkSnJsonDataToMes);

            var res = MESHelper2.Cur.MESResult(firstResult, "0x02", txt_SN.Text, out string err);
            if (res)
            {
                MessageBox.Show("CheckSN成功！");
            }
            else
            {
                MessageBox.Show($"CheckSN失败，原因：{err}");
            }
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (myDataGrid.SelectedItem is MESHelper2.MESResponse selectedItem)
            {
                MESHelper2.Cur.resList.Remove(selectedItem);
            }
        }

        private void btn_DataUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_SNInput.Text))
            {
                MessageBox.Show("输入的SN不能为空！");
                return;
            }
            var selectItem = txt_ResultInput.SelectedItem as ComboBoxItem;
            DataAnalysis.JsonClass.DataUpJsonDataToMes dataUpJsonDataToMes = new DataAnalysis.JsonClass.DataUpJsonDataToMes
            {
                Sn = txt_SNInput.Text,
                StartTime = txt_StartTimeInput.Text,
                Model = WorkItemManagerHelper.LoadedName,
                EndTime = txt_EndTimeInput.Text,
                UpEnabled = "1",
                TestResult = selectItem?.Content.ToString(),
                TestValue = new List<TestData> { },
                TestImage = new List<string> { "" },
                EndImage = new List<string> { "" }
            };
            DataAnalysis.JsonClass.TestData testData = new DataAnalysis.JsonClass.TestData
            {
                LowLimit = "",
                UpLimit = "",
                Result = "",
                TestItem = "",
                Value = ""

            };
            dataUpJsonDataToMes.TestValue.Add(testData);

            string firstResult = MESHelper2.Cur.motionSendMsg.SendDataUpMsgToMes(dataUpJsonDataToMes);

            var res = MESHelper2.Cur.MESResult(firstResult, "0x04", txt_SNInput.Text, out string err);
            if (res)
            {
                MessageBox.Show("DataUp成功！");
            }
            else
            {
                MessageBox.Show($"DataUp失败，原因：{err}");
            }
        }

        private void btn_Bind_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_BindSN.Text))
            {
                MessageBox.Show("输入的SN不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(txt_BindLensSN.Text))
            {
                MessageBox.Show("输入的LensSN不能为空！");
                return;
            }
            DataAnalysis.JsonClass.BindJsonDataToMes bindJsonDataToMes = new DataAnalysis.JsonClass.BindJsonDataToMes
            {
                Sn = txt_BindSN.Text,
                Model = WorkItemManagerHelper.LoadedName,
                LensCode = txt_BindLensSN.Text
            };
            string firstResult = MESHelper2.Cur.motionSendMsg.SendBindMsgToMes(bindJsonDataToMes);
            var res = MESHelper2.Cur.MESResult(firstResult, "0x03", txt_BindSN.Text, out string err); if (res)
            {
                MessageBox.Show("Bind成功！");
            }
            else
            {
                MessageBox.Show($"Bind失败，原因：{err}");
            }
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            MESHelper2.Cur.resList.Clear();
        }
    }
}
