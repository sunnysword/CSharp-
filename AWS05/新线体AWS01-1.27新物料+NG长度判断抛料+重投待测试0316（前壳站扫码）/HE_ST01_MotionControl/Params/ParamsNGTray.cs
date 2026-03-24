using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Params
{
    public class NGInfo
    {
        public string Info { get; set; }
        public int Count { get; set; }
    }
    public class ParamsNGTray : ParamsBase
    {
        public static readonly ParamsNGTray Cur = new ParamsNGTray();

        private ParamsNGTray() : base()
        {
            ClearNum += Clear;
            Read();

        }
        public static ParamsNGTray CreateInstance()
        {
            return Cur;
        }

        public Action<List<NGInfo>> FlushNGInfo;

        [SaveRemark]
        public List<NGInfo> NGInfoCollection = new List<NGInfo>();

        public void ClearIndex()
        {
            NGInfoCollection.Clear();
            this.Save();
            FlushNGInfo?.Invoke(NGInfoCollection);
        }
        public void AddNGInfoOnce(string ngInfo)
        {
            if (string.IsNullOrEmpty(ngInfo)) return;
            if (NGInfoCollection == null)
            {
                NGInfoCollection = new List<NGInfo>();
            }
            var item = NGInfoCollection.Where(s => s.Info == ngInfo).FirstOrDefault();
            if (item != null)
            {
                item.Count++;
            }
            else
            {
                NGInfoCollection.Add(new NGInfo
                {
                    Info = ngInfo,
                    Count = 1
                });
            }
            NGInfoCollection = (List<NGInfo>)(NGInfoCollection.OrderByDescending(s => s.Count).ToList());
            this.Save();
            FlushNGInfo?.Invoke(NGInfoCollection);
        }

        #region Top3 NG内容
        /// <summary>
        /// 设置计数
        /// </summary>
        public Action SetNGEvent;

        /// <summary>
        /// 清除计数
        /// </summary>
        public Action ClearNum;

        /// <summary>
        /// PCB扫码NG
        /// </summary>
        public string ScanPCBNg = "PCB扫码NG";

        ///// <summary>
        ///// PCB拍照定位NG
        ///// </summary>
        //public string PCBLocateNg = "PCB拍照定位NG";

        /// <summary>
        /// PCB放料前定位NG
        /// </summary>
        public string PCBBeforePlaceNg = "PCB放料前定位NG";

        /// <summary>
        /// PCB放料后定位NG
        /// </summary>
        public string PCBAfterPlaceNg = "PCB放料后定位NG";

        ///// <summary>
        ///// 外壳拍照定位NG
        ///// </summary>
        //public string ShellLocateNg = "外壳拍照定位NG";

        /// <summary>
        /// 外壳放料前定位NG
        /// </summary>
        public string ShellBeforePlaceNg = "外壳放料前定位NG";

        /// <summary>
        /// 外壳放料后定位NG
        /// </summary>
        public string ShellAfterPlaceNg = "外壳放料后定位NG";

        /// <summary>
        /// MES过站NG
        /// </summary>
        public string MesCheckNg = "MES过站NG";

        [SaveRemark]
        public int ScanPCBNgNum = 0;

        //[SaveRemark]
        //public int PCBLocateNgNum = 0;

        [SaveRemark]
        public int PCBBeforePlaceNgNum = 0;

        [SaveRemark]
        public int PCBAfterPlaceNgNum = 0;

        //[SaveRemark]
        //public int ShellLocateNgNum = 0;

        [SaveRemark]
        public int ShellBeforePlaceNgNum = 0;

        [SaveRemark]
        public int ShellAfterPlaceNgNum = 0;

        [SaveRemark]
        public int MesCheckNGsNum = 0;

        /// <summary>
        /// PCB扫码NG
        /// </summary>
        public void AddScanPCBNg()
        {
            ScanPCBNgNum++;
            Save();
            SetNGEvent?.Invoke();
        }

        /// <summary>
        /// PCB拍照定位NG
        /// </summary>
        /// 
        //public void AddPCBLocateNg()
        //{
        //    PCBLocateNgNum++;
        //    Save();
        //    SetNGEvent?.Invoke();
        //}

        /// <summary>
        /// PCB放料前定位NG
        /// </summary>
        public void AddPCBBeforePlaceNg()
        {
            PCBBeforePlaceNgNum++;
            Save();
            SetNGEvent?.Invoke();
        }

        /// <summary>
        /// PCB放料后定位NG
        /// </summary>
        public void AddPCBAfterPlaceNg()
        {
            PCBAfterPlaceNgNum++;
            Save();
            SetNGEvent?.Invoke();
        }

        /// <summary>
        /// 外壳拍照定位NG
        /// </summary>
        //public void AddShellLocateNg()
        //{
        //    ShellLocateNgNum++;
        //    Save();
        //    SetNGEvent?.Invoke();
        //}

        /// <summary>
        /// 外壳放料前定位NG
        /// </summary>
        public void AddShellBeforePlaceNg()
        {
            ShellBeforePlaceNgNum++;
            Save();
            SetNGEvent?.Invoke();
        }

        /// <summary>
        /// 外壳放料后定位NG
        /// </summary>
        public void AddShellAfterPlaceNg()
        {
            ShellAfterPlaceNgNum++;
            Save();
            SetNGEvent?.Invoke();
        }

        /// <summary>
        /// MES过站NG
        /// </summary>
        public void AddMesCheckNGNumNg()
        {
            MesCheckNGsNum++;
            Save();
            SetNGEvent?.Invoke();
        }
        public void Clear()
        {
            ScanPCBNgNum = 0;
            //PCBLocateNgNum = 0;
            PCBBeforePlaceNgNum = 0;
            PCBAfterPlaceNgNum = 0;
            //ShellLocateNgNum = 0;
            ShellBeforePlaceNgNum = 0;
            ShellAfterPlaceNgNum = 0;
            MesCheckNGsNum = 0;
            Save();
            SetNGEvent?.Invoke();
        }
        #endregion
    }
}
