using Handler.Connect;
using Handler.Connect.Modbus;
using Handler.Connect.RFID;
using Handler.Funs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using USR_RfidAynettek;

namespace Handler.View.RFID
{
    /// <summary>
    /// ViewPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class ViewRFID : UserControl
    {
        RFIDHelper_AYNETTEK Helper = null;
        ConnectClientHelper helperTCP = null;

        public ViewRFID()
        {
            InitializeComponent();
        }

        private void btnRFIDConn_Click(object sender, RoutedEventArgs e)
        {
            this.Helper = null;

            if (cmbRFIDSel.Text == "外壳RFID")
            {
                if (FunCommunicator.Cur.ShellTCPRfid.GetValue)
                {
                    this.helperTCP = ConnectFactory.RFID_Shell;
                }
                else
                {
                    this.Helper = RFIDManager.Shell_RFID;
                }
            }
            if (cmbRFIDSel.Text == "PCBRFID")
            {
                if (FunCommunicator.Cur.PCBTCPRfid.GetValue)
                {
                    this.helperTCP = ConnectFactory.RFID_PCB;
                }
                else
                {
                    this.Helper = RFIDManager.PCB_RFID;
                }

            }

            if (this.Helper == null)
            {
                MessageBox.Show("RFID连接失败！");
            }
            else
            {
                MessageBox.Show("RFID连接成功！");
            }
        }

        private void btnReadUID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                string error = string.Empty;
                if (Helper == null)
                {
                    MessageBox.Show("TCP-RFID未实现该功能！");
                    return;
                }
                if (this.Helper.ReadRFIDUID(out data, out error))
                {
                    this.txtUIDInfo.Text = data;
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReadRFIDNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                string error = string.Empty;

                RFIDModel model = Helper != null ? RFIDDataHelper.GetRfid(this.Helper, out error) : RFIDDataHelperTCP.GetRfid(this.helperTCP, out error);
                if (model != null)
                {
                    this.txtRFIDNo.Text = model.CarrierID;
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnWriteRFIDNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = txtRFIDNo.Text.Trim();
                string error = string.Empty;

                if (Helper != null ? RFIDDataHelper.WriteNewCarrier(this.Helper, data, out error) : RFIDDataHelperTCP.WriteNewCarrier(this.helperTCP, data, out error))
                {
                    MessageBox.Show("治具编号写入成功！");
                }
                else
                {
                    MessageBox.Show(error);
                }
                //if (this.Helper.WriteRFIDNumber(data, out error))
                //{
                //    MessageBox.Show("治具编号写入成功！");
                //}
                //else
                //{
                //    MessageBox.Show(error);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReadAllRFID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                string error = string.Empty;

                RFIDModel model = Helper != null ? RFIDDataHelper.GetRfid(this.Helper, out error) : RFIDDataHelperTCP.GetRfid(this.helperTCP, out error);
                if (model != null)
                {
                    this.txtAllRFID.Text = model.ProductID + ":" + model.CarrierID + ":" + model.Barcode + ":" + model.Result + ":" + Convert.ToString(model.isFront ? "1" : "0") + ":" + model.StationState;

                }
                else
                {
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnWriteAllRFID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = txtAllRFID.Text.Trim();
                string error = string.Empty;

                if (Helper != null ? RFIDDataHelper.WriteAllRFID(this.Helper, data, out error) : RFIDDataHelperTCP.WriteAllRFID(this.helperTCP, data, out error))
                {
                    MessageBox.Show("治具编号写入成功！");
                }
                else
                {
                    MessageBox.Show(error);
                }
                //if (this.Helper.WriteRFIDNumber(data, out error))
                //{
                //    MessageBox.Show("治具编号写入成功！");
                //}
                //else
                //{
                //    MessageBox.Show(error);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnReadProductInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                string error = string.Empty;

                RFIDModel model = Helper != null ? RFIDDataHelper.GetRfid(this.Helper, out error) : RFIDDataHelperTCP.GetRfid(this.helperTCP, out error);
                if (model != null)
                {
                    this.txtProductInfo.Text = model.ProductID;
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnWriteProductInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = txtProductInfo.Text.Trim();
                string error = string.Empty;

                if (Helper!=null? RFIDDataHelper.WriteProduct(this.Helper, data, out error) : RFIDDataHelperTCP.WriteProduct(this.helperTCP, data, out error))
                {
                    MessageBox.Show("产品信息写入成功！");
                }
                else
                {
                    MessageBox.Show(error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
