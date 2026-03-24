using Handler.Params;
using Handler.Process.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    /// <summary>
    /// 产品信息
    /// </summary>
    public class ProductInfo : LanguageControlBase.Wpf.BindableObject
    {
        public ProductInfo()
        {
            Result = ProductInfo.ResultOK;
            StartTime = GetCurrentTime();
            ProductTestStationList.Add(new ProductTestStation()
            {
                TestStationName = "Default",
                Result = Product.ProductInfo.ResultOK,
                IsUse = true
            });
        }

        public string TestPicturePath = string.Empty;
        public string ResultPicturePath = string.Empty;

        /// <summary>
        /// 测试结果OK
        /// </summary>
        public const string ResultOK = "1";
        /// <summary>
        /// 测试结果NG
        /// </summary>
        public const string ResultNG = "0";
        /// <summary>
        /// 是否是前一站NG产品
        /// </summary>
        public bool IsLastStationNG { get; set; }

        /// <summary>
        /// 当前产品测试所在治具 A/B/C/D
        /// </summary>
        public string PARA { get; set; }

        public string PCBSN { get; set; }

        string _sn = "";
        string _sn2 = "";
        string _sn3 = "";
        string _result = "";
        string _rfid = "";
        string _failedMsg = "";
        string _startTime = "";
        string _endTime = "";
        DateTime _startTimeForCT;
        DateTime _endTimeForCT;
        EnumeProductState _productState = EnumeProductState.None;


        public bool IsPass
        {
            get
            {
                if (this.Result == ProductInfo.ResultOK
                    || this.Result.ToUpper() == "OK"
                    || this.Result.ToUpper() == "PASS"
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string _Cam1BlobValue = "";
        /// <summary>
        /// 前壳blob值
        /// </summary>
        public string Cam1BlobValue
        {
            get { return _Cam1BlobValue; }
            set
            {
                SetProperty(ref _Cam1BlobValue, value);
            }
        }

        public string _Cam1BlobLimit = "";
        /// <summary>
        /// 前壳blob下限
        /// </summary>
        public string Cam1BlobLimit
        {
            get { return _Cam1BlobLimit; }
            set
            {
                SetProperty(ref _Cam1BlobLimit, value);
            }
        }

        public string _Cam3PCBValue = "";
        /// <summary>
        /// PCB匹配值
        /// </summary>
        public string Cam3PCBValue
        {
            get { return _Cam3PCBValue; }
            set
            {
                SetProperty(ref _Cam3PCBValue, value);
            }
        }
        public string _Cam3PCBLimit = "";
        /// <summary>
        /// PCB匹配下限
        /// </summary>
        public string Cam3PCBLimit
        {
            get { return _Cam3PCBLimit; }
            set
            {
                SetProperty(ref _Cam3PCBLimit, value);
            }
        }


        public string _Cam4PCBValue = "";
        /// <summary>
        /// PCB检测匹配值
        /// </summary>
        public string Cam4PCBValue
        {
            get { return _Cam4PCBValue; }
            set
            {
                SetProperty(ref _Cam4PCBValue, value);
            }
        }
        public string _Cam4PCBLimit = "";
        /// <summary>
        /// PCB检测匹配下限
        /// </summary>
        public string Cam4PCBLimit
        {
            get { return _Cam4PCBLimit; }
            set
            {
                SetProperty(ref _Cam4PCBLimit, value);
            }
        }




        public string _Cam4ScrewValue = "";
        /// <summary>
        /// 锁附匹配值
        /// </summary>
        public string Cam4ScrewValue
        {
            get { return _Cam4ScrewValue; }
            set
            {
                SetProperty(ref _Cam4ScrewValue, value);
            }
        }
        public string _Cam4ScrewLimit = "";
        /// <summary>
        /// 锁附匹配下限
        /// </summary>
        public string Cam4ScrewLimit
        {
            get { return _Cam4ScrewLimit; }
            set
            {
                SetProperty(ref _Cam4ScrewLimit, value);
            }
        }



        /// <summary>
        /// 产品条码
        /// </summary>
        public string SN
        {
            get { return _sn; }
            set
            {
                SetProperty(ref _sn, value);
            }
        }


        /// <summary>
        /// 当前产品测试结果
        /// </summary>
        public string Result
        {
            get { return _result; }
            set
            {
                SetProperty(ref _result, value);
            }
        }

        public string RFID
        {
            get { return _rfid; }
            set
            {
                SetProperty(ref _rfid, value);
            }
        }


        /// <summary>
        /// 失败的原因
        /// </summary>
        public string FailedMsg
        {
            get { return _failedMsg; }
            set
            {
                SetProperty(ref _failedMsg, value);
            }
        }


        /// <summary>
        /// 当前产品的测试项，及其相关数据
        /// </summary>
        public List<ProductTestStation> ProductTestStationList { get; set; } = new List<ProductTestStation>();


        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                SetProperty(ref _startTime, value);
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime
        {
            get { return _endTime; }
            set
            {
                SetProperty(ref _endTime, value);
            }
        }

        public DateTime StartTimeForCT
        {
            get { return _startTimeForCT; }
            set
            {
                SetProperty(ref _startTimeForCT, value);
            }
        }

        public DateTime EndTimeForCT
        {
            get { return _endTimeForCT; }
            set
            {
                SetProperty(ref _endTimeForCT, value);
            }
        }

        /// <summary>
        /// 当前产品的状态
        /// </summary>
        public EnumeProductState ProductState
        {
            get { return _productState; }
            set
            {
                SetProperty(ref _productState, value);
            }
        }


        public void Clear()
        {
            SN = "";
            Result = "";
            FailedMsg = "";
            //productTestStations?.Clear();

        }

        public static ProductInfo GetInitialProduct() =>
            new ProductInfo
            {
                Result = ProductInfo.ResultOK,
            };

        public void SetProductNG(string failMsg)
        {
            this.Result = ProductInfo.ResultNG;
            this.FailedMsg = failMsg;
            if (failMsg == "MES过站失败")
            {
                ParamsNGTray.Cur.AddMesCheckNGNumNg();
            }
        }

        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString();
        }

        public void SetCurrentTimeToStartTime()
        {
            StartTime = GetCurrentTime();
        }

        public void SetCurrentTimeToEndTime()
        {
            EndTime = GetCurrentTime();
        }
        protected static string GetTimeStamp(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            long timeStamp = dto.ToUnixTimeSeconds();
            return timeStamp.ToString();
        }
        public static string GetMesCurrentTime()
        {
            DateTime dtNow = DateTime.Now;
            string ts = GetTimeStamp(dtNow);
            return ts;
        }

        /// <summary>
        /// 将当前的工作状态转移到下一位
        /// </summary>
        public void ChangeToNextState()
        {
            ProductState++;
        }

        public List<ProductTestStationTestItem> GetAllTestItems()
        {
            List<ProductTestStationTestItem> items = new List<ProductTestStationTestItem>();

            foreach (var item in this.ProductTestStationList)
            {
                foreach (var item2 in item.StationTestItems)
                {
                    items.Add(item2);

                }
            }
            //StaticInitial.Motion.WriteToUser($"获取产品所有测试项,一共{items.Count}项");
            return items;
        }

        public ProductTestStation GetProductTestStationByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            for (int i = 0; i < ProductTestStationList.Count; i++)
            {
                if (ProductTestStationList[i].TestStationName == name)
                {
                    return ProductTestStationList[i];
                }
            }
            var station = new ProductTestStation()
            {
                TestStationName = name,
                Result = Product.ProductInfo.ResultOK,
                IsUse = true
            };
            ProductTestStationList.Add(station);
            return station;
        }
        private object _lockAdd = new object();

        public void AddTestItemByTestStationName(string name, ProductTestStationTestItem item)
        {
            lock (_lockAdd)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    this.ProductTestStationList.Where(
                       s => s.TestStationName == "Default").First()
                       .StationTestItems.Add(item);
                }
                var count = this.ProductTestStationList.Where(s => s.TestStationName == name);
                if (count.Count() > 0)
                {
                    count.First().StationTestItems.Add(item);
                }
                else
                {
                    this.ProductTestStationList.Add(new ProductTestStation
                    {
                        TestStationName = name,
                        StationTestItems = new List<ProductTestStationTestItem>() { item }
                    });
                }
            }

        }

        public ProductTestStationTestItem GetTestItemByName(string stationName, string testItemName)
        {
            return this.ProductTestStationList.Where(s => s.TestStationName == stationName)
                .FirstOrDefault()?.StationTestItems
                .Where(s => s.Name == testItemName)
                .FirstOrDefault();

        }

        public ProductTestStation GetTestStationByName(string stationName)
        {
            return this.ProductTestStationList.Where(s => s.TestStationName == stationName)
                .First();
        }

        public string GetResultByCustomFormat()
        {
            return this.Result == ProductInfo.ResultOK ? "OK" : "NG";
        }
    }



    public enum EnumeProductState
    {
        Clear = -1,
        None = 0,

        WaitPlasma,//等待Plasma
        //PlasmaIng,//Plsam进行中，
        WaitDispenser,//等待点胶

        //DispenserIng,//点胶进行中

        WaitAA,//等待AA
        //AAIng,//AA进行中

        WaitUnLoad,//等待下料
        Unknow_1,
        Unknow_2,
        Unknow_3,
        Unknow_4,
        Unknow_5,
        Unknow_6,


    }
}
