using Handler.Motion.PalletTray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.Station.TrayLoad.Pallet
{
    public class TrayPointZ:TrayPoint
    {
       
    }

    public class TrayPointZU : TrayPointZ
    {

        [ISaveReaddll.SaveRemark]
        double _x = 0;
        [ISaveReaddll.SaveRemark]
        double _y = 0;
        [ISaveReaddll.SaveRemark]
        int _id = 0;

        bool _isLimit = false;


        TrayPointStatus _pointStatus = TrayPointStatus.WaitUse;

        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                SetProperty(ref _x, value);
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                SetProperty(ref _y, value);
            }
        }


        public int Id
        {

            get
            {
                return _id;
            }

            set { SetProperty(ref _id, value); }
        }


        /// <summary>
        /// 点的状态
        /// </summary>
        public TrayPointStatus PointStatus
        {
            get { return _pointStatus; }

            set
            {
                SetProperty(ref _pointStatus, value);
            }
        }


        /// <summary>
        /// 是否禁用
        /// 如果禁用则跳过该点
        /// </summary>
        public bool IsLimit
        {
            get { return _isLimit; }
            set
            {
                SetProperty(ref _isLimit, value);
            }
        }

        [ISaveReaddll.SaveRemark]
        double _z = 0;

        public double Z
        {
            get
            {
                return _z;
            }

            set
            {
                SetProperty(ref _z, value);
            }
        }


        [ISaveReaddll.SaveRemark] 
        double _u = 0;

        public double U
        {
            get
            {
                return _u;
            }

            set
            {
                SetProperty(ref _u, value);
            }
        }
    }
}
