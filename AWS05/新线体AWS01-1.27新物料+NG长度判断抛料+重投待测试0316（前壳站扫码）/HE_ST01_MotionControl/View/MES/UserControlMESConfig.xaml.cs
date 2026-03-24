using Handler.Connect.Camera;
using Handler.MES;
using Handler.Product;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalTestMachine.Views
{
    /// <summary>
    /// UserControlMESConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UserControlMESConfig : UserControl
    {
        public UserControlMESConfig()
        {
            InitializeComponent();

        }


        private void buttonSNCheck_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBoxSN.Text))
            {
                MessageBox.Show("请先输入产品条码");
                return;
            }
            if (MESHelper.Cur.CheckSN(this.textBoxSN.Text, out string error) == true)
            {
                MessageBox.Show("SN检查成功");
            }
            else
            {
                MessageBox.Show("SN检查失败" + error);
            }

        }

        private ProductInfo GetTestDate(bool result)
        {
            ProductInfo product = new ProductInfo();
            product.Result = result ? ProductInfo.ResultOK : ProductInfo.ResultNG;
            product.SN = this.textBoxSN.Text;
            product.SN = this.textBoxSN.Text;
            product.StartTime = DateTime.Now.ToString();
            product.EndTime = DateTime.Now.ToString();
            //sb.Append($"{item.Name}@{item.LowLimit}@{item.UpLimit}@{item.Value}@{resultStr}|");
            //
            product.AddTestItemByTestStationName("锁附", new ProductTestStationTestItem
            {
                Name = "螺丝1扭力值",
                UpLimit = "2.2",
                LowLimit = "1.8",
                Unit = "kgf.cm",
                Message = "",
                Value = "1.99",
                Result = "OK"
            }) ;

            product.AddTestItemByTestStationName("半成品测试", new ProductTestStationTestItem
            {
                Name = "电流",
                UpLimit = "130",
                Unit = "mA",
                LowLimit = "90",
                Message = "",
                Value = "111",
                Result = "OK"
            });
            return product;
        }

        private void btnMesUpload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBoxSN.Text))
            {
                MessageBox.Show("请先输入产品条码");
                return;
            }
            string data = string.Empty;
            if (MESHelper.Cur.DataUp(GetTestDate(true), out string error) == true)
            {
                MessageBox.Show("数据上传成功");
            }
            else
            {
                MessageBox.Show("数据上传失败" + error);
            }

        }

        private void btnMesUploadNG_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBoxSN.Text))
            {
                MessageBox.Show("请先输入产品条码");
                return;
            }
            string data = string.Empty;
            if (MESHelper.Cur.DataUp(GetTestDate(false), out string error) == true)
            {
                MessageBox.Show("数据上传成功");
            }
            else
            {
                MessageBox.Show("数据上传失败" + error);
            }

        }
 

        //public CameraHelper Camera { get; set; }

        private void btnMesBind_Click(object sender, RoutedEventArgs e)
        {
            //Task.Run(()=>
            //{
            //    CameraCmd cameraCmd = new CameraCmd();
            //    Camera = CameraManager.CameraFrontHolder;
            //    string ERROR = "";
            //    var cameraResult = Camera.SendAndCheck("cam1", out cameraCmd, out ERROR,
            //    "", process: null);
            //    char s = ';' ;
            //    var array = cameraCmd.FullMessage.Split(s);
            //    switch (array[1].ToUpper())
            //    {
            //        case "CAM1":
            //            var s1 = array[6];
            //            var s2 = array[7];
            //            break;
            //    }
            //    MessageBox.Show($"获取到的相机信息：{cameraCmd.Result},{cameraCmd.Message},{cameraCmd.FullMessage}");


            //});

        }
    }
}
