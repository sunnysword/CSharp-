using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Handler.Product;

namespace Handler.Product
{
    public class UdpItemData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public string Unit { get; set; }

        public Brush bdBrush { get; set; } = Brushes.White;


        public static List<UdpItemData> GetUdpItemsFromProductTestStationTestItem(List<ProductTestStationTestItem> productTestStationTestItems)
        {
            if (productTestStationTestItems == null) return null;
            List<UdpItemData> udpItemDatas = new List<UdpItemData>();
            foreach (var item in productTestStationTestItems)
            {
                UdpItemData udpItemData = new UdpItemData();
                udpItemData.Result = item.Result;
                udpItemData.Name = $"{item.Name}[{item.LowLimit}~{item.UpLimit}]";
                //if (item.Name == "高度结果1" || item.Name == "高度结果2")
                //{
                //    continue;
                //}
                udpItemData.Value = item.Value;
                udpItemData.Unit = item.Unit;
                switch (udpItemData.Result.ToUpper())
                {
                    case "1":
                    case "OK":
                    case "PASS":
                        udpItemData.bdBrush = Brushes.LightGreen;
                        break;

                    case "0":
                    case "NG":
                    case "FAIL":
                        udpItemData.bdBrush = Brushes.Red;
                        break;

                    default:
                        udpItemData.bdBrush = Brushes.Gray;
                        break;
                }
                udpItemDatas.Add(udpItemData);
            }

            return udpItemDatas;
        }


        public static List<UdpItemData> GetUdpItemsFromProductTestStationTestItem2(List<ProductTestStationTestItem> productTestStationTestItems)
        {
            if (productTestStationTestItems == null) return null;
            List<UdpItemData> udpItemDatas = new List<UdpItemData>();
            foreach (var item in productTestStationTestItems)
            {
                UdpItemData udpItemData = new UdpItemData();
                udpItemData.Result = item.Result;
                udpItemData.Name = $"{item.Message}[{item.LowLimit}~{item.UpLimit}]";
                udpItemData.Value = item.Value;
                udpItemData.Unit = item.Unit;
                switch (udpItemData.Result.ToUpper())
                {
                    case "1":
                    case "OK":
                    case "PASS":
                        udpItemData.bdBrush = Brushes.LightGreen;
                        break;

                    case "0":
                    case "NG":
                    case "FAIL":
                        udpItemData.bdBrush = Brushes.Red;
                        break;

                    default:
                        udpItemData.bdBrush = Brushes.Gray;
                        break;
                }
                udpItemDatas.Add(udpItemData);
            }

            return udpItemDatas;
        }

    }
}
