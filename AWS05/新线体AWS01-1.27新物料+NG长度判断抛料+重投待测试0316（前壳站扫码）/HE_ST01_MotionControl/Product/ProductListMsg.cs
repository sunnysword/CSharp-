using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    /// <summary>
    /// 产品信息站列表：
    /// </summary>
    public class ProductListMsg : LanguageControlBase.Wpf.BindableObject
    {
        /// <summary>
        /// 产品信息站列表：
        /// </summary>
        public static readonly ObservableCollection<ProductListMsg> ProductDataMsgsList = new ObservableCollection<ProductListMsg>();

        string _sn = "";
        string _operator = "";
        ProductResult _result = ProductResult.None;


        ProductErrMsg _productErrMsg = ProductErrMsg.None;
        string _startTime = "";
        string _endTime = "";
        public string SN
        {
            get { return _sn; }
            set
            {
                SetProperty(ref _sn, value);
            }
        }

        public string Operator
        {
            get { return _operator; }
            set
            {
                SetProperty(ref _operator, value);
            }
        }

        public ProductResult Result
        {
            get { return _result; }
            set
            {
                SetProperty(ref _result, value);
            }
        }

        public ProductErrMsg ProductErrMsg
        {
            get { return _productErrMsg; }
            set
            {
                SetProperty(ref _productErrMsg, value);
            }
        }

        public string StartTime
        {
            get { return _startTime; }
            set
            {
                SetProperty(ref _startTime, value);
            }
        }

        public string EndTime
        {
            get { return _endTime; }
            set
            {
                SetProperty(ref _endTime, value);
            }
        }



    }

    public enum ProductErrMsg
    {
        None = 0,
        Dirty = 1,
        CameraLocation = 2,
        GlueCheck = 3,
        CalibLens = 4,
        AA = 5,
        Defocus = 6

    }

    public enum ProductResult
    {
        None = 0,
        OK = 1,
        NG = 2
    }
}
