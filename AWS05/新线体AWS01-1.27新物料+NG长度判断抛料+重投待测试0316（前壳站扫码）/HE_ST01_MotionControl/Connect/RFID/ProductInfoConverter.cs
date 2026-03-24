using Handler.Process;
using Handler.Process.Station;
using Handler.Product;
using Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handler.View;

namespace Handler.Connect.RFID
{
    internal class ProductInfoConverter
    {
        public static object lockCheck = new object();
        public static bool GetRFIDFromNumber(string content,out string rfidNumber,out string error)
        {
            lock (lockCheck)
            {
                rfidNumber = string.Empty;
                error = string.Empty;
                try
                {
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        error = "读取转盘治具RFID编号为空";
                        return false;
                    }
                    rfidNumber = content.Last().ToString();
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }
           
        }
        public static ProductInfo GetProductInfoClass(ProductInfo product, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var array = content.Split(';');
            if (array[0].ToUpper() == "NULL")
            {
                return null;
            }
            if (product == null)
            {
                product = new ProductInfo();
            }
            switch (array[0])
            {
                case "1":
                    product.Result = ProductInfo.ResultOK;
                    break;

                case "0":
                    product.Result = ProductInfo.ResultNG;
                    break;

                default:
                    throw new Exception("无法通过RFID内容确定产品信息");
            }

            if (array.Length > 1)
            {
                product.SN = array[1];
            }
            return product;
        }

        public static string GetStringFromProduct(ProductInfo product)
        {
            if (product == null) return "NULL";
            StringBuilder sb = new StringBuilder();
            sb.Append(WorkItemManagerHelper.LoadedName);//机种
            sb.Append(":");
            sb.Append("1");//载具编号（暂时自定义）
            sb.Append(":");
            sb.Append(product.SN);//产品sn
            sb.Append(":");
            sb.Append(product.Result);//产品结果
            sb.Append(":");
            sb.Append("null");//自定义内容
            return sb.ToString();
        }

    }
}
