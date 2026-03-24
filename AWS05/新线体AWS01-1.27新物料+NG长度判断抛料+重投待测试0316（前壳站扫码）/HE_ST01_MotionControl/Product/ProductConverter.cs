using AviewMotionUI.ProductionStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    internal class ProductConverter
    {
        public static List<ProductTestStationTestItem> ChangeMsg(string msg)    //用于白板数据转换
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
                                string[] strValue = str.Split(',');
                                ProductTestStationTestItem testItemMsg = new ProductTestStationTestItem();
                                bool IsCanAdd = false;

                                foreach (var item2 in strValue)
                                {
                                    if (item2.Contains('='))
                                    {

                                        foreach (var item in propertyInfos)
                                        {
                                            if (item2.Split('=')[0].ToLower() == item.Name.ToLower())
                                            {
                                                object obValue = Convert.ChangeType(item2.Split('=')[1], item.PropertyType);
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

        //TotalResult:aaResult_2,ocDiffX_0.00,ocDiffY_0.00,xAngle_0.00,yAngle_0.00,sfrC_0.00,sfrUl_0.00,sfrUr_0.00,sfrLl_0.00,sfrLr_0.00,default_save_path_
        public static List<ProductTestStationTestItem> ChangeMsg2(string msg)   //用于AA数据转换
        {
            List<ProductTestStationTestItem> testItemsList = new List<ProductTestStationTestItem>();
            if (msg == null) return testItemsList;
            //
            // aaResult_2,ocDiffX_0.00,ocDiffY_0.00,xAngle_0.01,yAngle_0.04,uv_after_sfr1_l_52.74,uv_after_sfr1_u_52.20,uv_after_sfr4_r_50.49,uv_after_sfr4_d_46.13,uv_after_sfr3_l_31.56,uv_after_sfr3_d_37.50,uv_after_sfr2_r_49.47,uv_after_sfr2_u_40.70,uv_after_sfr1_l_42.78,uv_after_sfr1_u_40.93
            string[] items = msg.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var allInfos in items)
            {
                string[] allInfo = allInfos.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (allInfo.Length > 1)
                {
                    string[] data = allInfo[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < data.Length; i++)
                    {
                        var item = data[i].Trim();
                        if (item.Contains("="))
                        {
                            var splitArray = item.Split('=');
                            if (splitArray.Length == 2)
                            {
                                if (!string.IsNullOrEmpty(splitArray[0]))
                                {
                                    testItemsList.Add(new ProductTestStationTestItem()
                                    {
                                        Name = splitArray[0].Trim(),
                                        Value = splitArray[1].Trim(),
                                    });
                                }
                            }
                        }
                    }
                }
            }


            return testItemsList;
        }

        public static IList<StationResultInfo> GetProductStationResultItems(Product.ProductInfo product)    //用于生产统计转换
        {
            if (product == null) return null;

            IList<StationResultInfo> listStation = new List<StationResultInfo>();
            try
            {
                bool isNG = false;
                foreach (var item in product.ProductTestStationList)
                {
                    StationResultInfo plasmaResultInfo = new StationResultInfo();
                    plasmaResultInfo.Name = item.TestStationName;
                    plasmaResultInfo.NGItems = new List<string>();
                    if (item.IsUse == true)
                    {
                        if (isNG)
                        {
                            plasmaResultInfo.Result = StationResult.NoTest;
                        }
                        else
                        {
                            if (item.Result == ProductInfo.ResultOK)
                            {
                                plasmaResultInfo.Result = StationResult.OK;
                            }
                            else
                            {
                                isNG = true;
                                plasmaResultInfo.Result = StationResult.NG;
                                plasmaResultInfo.NGItems = new List<string>() { item.FailedMsg };
                            }
                        }
                    }
                    else
                    {
                        plasmaResultInfo.Result = StationResult.Limited;
                    }

                    listStation.Add(plasmaResultInfo);

                }
            }
            catch (Exception)
            {
            }
            return listStation;
        }


        private static object _lockAdd = new object();
        public static void AddTestItemsOnce(ProductTestStation station, ProductTestStationTestItem testItems)
        {
            if (station == null)
                return;
            if (testItems == null)
                return;
            lock (_lockAdd)
            {
                station.StationTestItems.Add(new ProductTestStationTestItem
                {
                    Name = testItems.Name,
                    Message = testItems.Message,
                    Value = testItems.Value,
                    LowLimit = testItems.LowLimit,
                    UpLimit = testItems.UpLimit,
                    Standard = testItems.Standard,
                    Result = testItems.Result,
                });
            }
        }

        //public static List<BaoLongLine2_MES.TestItem> GetMesTestItems(Product.ProductInfo product)    //用于Mes转换
        //{
        //    if (product == null) return null;

        //    List<BaoLongLine2_MES.TestItem> listStation = new List<BaoLongLine2_MES.TestItem>();
        //    try
        //    {
        //        foreach (var item2 in product.ProductTestStationList)
        //        {
        //            foreach (var item in item2.StationTestItems)
        //            {
        //                listStation.Add(new BaoLongLine2_MES.TestItem
        //                {
        //                    Name = item.Name,
        //                    Value = item.Value,
        //                    Result = item.Result,
        //                    LowerLimit = item.LowerLimit,
        //                    UpperLimit = item.UpperLimit,
        //                    Message = item.Message,
        //                    Standard = item.Standard
        //                });
        //            }

        //        }

        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return listStation;
        //}
    }
}
