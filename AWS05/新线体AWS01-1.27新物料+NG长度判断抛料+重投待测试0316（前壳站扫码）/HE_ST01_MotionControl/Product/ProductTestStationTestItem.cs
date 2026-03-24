using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    /// <summary>
    /// 产品每项测试数据
    /// </summary>
    public class ProductTestStationTestItem
    {
        /// <summary>
        /// 测试项，解释其作用
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 测试项名字标识，英文
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前测试项结果：0失败，1成功
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 测试值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 测试项的下限值
        /// </summary>
        public string LowLimit { get; set; }

        /// <summary>
        /// 测试项的上限值
        /// </summary>
        public string UpLimit { get; set; }

        /// <summary>
        /// 标准值
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public string GetStdString()
        {
            return $"[message={this.Message}," +
                $"name={this.Name}," +
                $"result={this.Result}," +
                $"value={this.Value}," +
                $"lowerLimit={this.LowLimit}," +
                $"upperLimit={this.UpLimit}," +
                $"standard={this.Standard}]";

        }






        public static List<ProductTestStationTestItem> ChangeMsg(string msg)
        {
            List<ProductTestStationTestItem> testItemsList = new List<ProductTestStationTestItem>();
            if (msg == null) return testItemsList;

            Type type = typeof(ProductTestStationTestItem);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            void Calid(string strInput)
            {

                string tempMsg = strInput;
                int index_1 = tempMsg.IndexOf('[');
                int index_2 = tempMsg.IndexOf(']');
                if (index_1 == -1)
                {
                    return;
                }
                else
                {
                    if (index_2 == -1)
                    {
                        return;
                    }
                    else
                    {
                        if ((index_2 - index_1) != 1 && (index_2 - index_1) > 0)
                        {
                            string str = tempMsg.Substring(index_1, index_2 - index_1 + 1);

                            str = str.TrimStart('[');
                            str = str.TrimEnd(']');
                            if (!string.IsNullOrEmpty(str))
                            {
                                string[] strValue = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                ProductTestStationTestItem testItemMsg = new ProductTestStationTestItem();
                                bool IsCanAdd = false;

                                foreach (var item2 in strValue)
                                {
                                    if (item2.Contains('='))
                                    {

                                        foreach (var item in propertyInfos)
                                        {
                                            var value = item2.Split('=')[0].Trim().ToLower();
                                            if (value == item.Name.ToLower())
                                            {
                                                object obValue = Convert.ChangeType(item2.Split('=')[1].Trim(), item.PropertyType);
                                                item.SetValue(testItemMsg, obValue);
                                                if (!IsCanAdd) IsCanAdd = true;
                                            }

                                        }
                                    }

                                }
                                if (IsCanAdd)
                                {
                                    testItemsList.Add(testItemMsg);
                                }
                            }



                        }

                        Calid(strInput.Substring(index_2 + 1));


                    }
                }
            }
            try
            {
                Calid(msg);
            }
            catch
            {


            }



            return testItemsList;
        }



        public static List<T> ConvertByPropertyName<T>(List<ProductTestStationTestItem> testItemMsgs) where T : new()
        {
            List<T> Temp = new List<T>();
            if (testItemMsgs == null || testItemMsgs.Count == 0) return Temp;

            Type src_type = typeof(ProductTestStationTestItem);
            Type dst_Type = typeof(T);

            PropertyInfo[] propertyInfos_src = src_type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertyInfos_dst = dst_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);



            bool IsCanSet = false;
            Action<T, ProductTestStationTestItem> SetProEvent = (d, s) => { };

            foreach (var item_src in propertyInfos_src)
            {
                foreach (var item_dst in propertyInfos_dst)
                {
                    if (item_src.Name.ToLower() == item_dst.Name.ToLower())
                    {
                        if (item_src.PropertyType == item_dst.PropertyType)
                        {
                            IsCanSet = true;
                            SetProEvent += (d, s) =>
                            {
                                item_dst.SetValue(d, item_src.GetValue(s));
                            };

                            break;
                        }
                    }

                }

            }

            if (!IsCanSet) return Temp;

            foreach (var item in testItemMsgs)
            {
                T t = new T();
                SetProEvent(t, item);
                Temp.Add(t);
            }




            return Temp;

        }
    }
}
