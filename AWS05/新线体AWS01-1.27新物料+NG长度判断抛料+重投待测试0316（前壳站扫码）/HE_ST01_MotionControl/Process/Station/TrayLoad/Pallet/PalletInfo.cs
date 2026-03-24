using AixCommInfo;
using Handler.Process.Station.TrayLoad.Pallet;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Handler.Motion.PalletTray
{
    /// <summary>
    /// 料盘信息的表示类
    /// </summary>
    public class PalletInfo : BindableObject
    {

        public PalletInfo(Func<string> readPath)
        {
            SaveRead = new SaveReaddll.SaveReadHeler(readPath, () => this);

            Read();
            CheckDims();
            Fun_CalcPalletPoints();
            SaveRead.ReadFinished += SaveRead_ReadFinished;
            this.readPath = readPath;
        }

        private void SaveRead_ReadFinished()
        {
            CheckDims();
            Fun_CalcPalletPoints();
        }


        Func<string> readPath;

        SaveReaddll.SaveReadHeler SaveRead;

        /// <summary>
        /// 行数
        /// </summary>
        [SaveRemark]
        public ParamStructText<ushort> RowNums = new ParamStructText<ushort>() { GetValue = 10 };

        /// <summary>
        /// 列数
        /// </summary>
        [SaveRemark]
        public ParamStructText<ushort> ColNums = new ParamStructText<ushort>() { GetValue = 4 };

        ///// <summary>
        ///// Z标准坐标值
        ///// </summary>
        //[SaveRemark]
        //public ParamStructText<double> Z_Standard = new ParamStructText<double>() { GetValue = 4 };

        ///// <summary>
        ///// U标准坐标值
        ///// </summary>
        //[SaveRemark]
        //public ParamStructText<double> U_Standard = new ParamStructText<double>() { GetValue = 4 };

        /// <summary>
        /// 控制点1
        /// </summary>
        [SaveRemark]
        public TrayPointZU Control_Point1 = new TrayPointZU();

        /// <summary>
        /// 控制点2
        /// </summary>
        [SaveRemark]
        public TrayPointZU Control_Point2 = new TrayPointZU();

        /// <summary>
        /// 控制点3
        /// </summary>
        [SaveRemark]
        public TrayPointZU Control_Point3 = new TrayPointZU();

        /// <summary>
        /// 控制点4
        /// </summary>
        [SaveRemark]
        public TrayPointZU Control_Point4 = new TrayPointZU();

        /// <summary>
        /// 当前的取料点
        /// </summary>
        [SaveRemark]
        public int Index { get; set; }

        /// <summary>
        /// 点总数
        /// </summary>
        public int Count => Points.Count;

        /// <summary>
        /// 剩余，待取的点数
        /// </summary>
        public int LeftNums => Count - Index;

        /// <summary>
        /// 料盘上的点集
        /// </summary>      
        public readonly ObservableCollection<TrayPointZU> Points = new ObservableCollection<TrayPointZU>();


        /// <summary>
        /// 重置盘的状态，重新开始
        /// </summary>
        public void ResetPalletStatus()
        {
            foreach (var item in Points)
            {
                item.PointStatus = TrayPointStatus.WaitUse;

            }

        }


        public void Read()
        {
            SaveRead?.Read();
        }

        public void Save()
        {
            SaveRead?.Save();
        }



        /// <summary>
        /// 当重新计算盘型时触发的事件
        /// </summary>
        public Action PallletChangeEvent;

        /// <summary>
        /// 计算托盘点位
        /// </summary>
        public virtual void Fun_CalcPalletPoints()
        {
            //try
            //{
            //    Points.Clear();

            //    if (RowNums.GetValue == 1 && ColNums.GetValue == 1)
            //    {
            //        //只需要一个控制点即可
            //        Points.Add(new TrayPointZU() { X = Control_Point1.X, Y = Control_Point1.Y,
            //        Z= Control_Point1.Z,U=Control_Point1.U,
            //        });
            //        return;
            //    }

            //    double GetStep(double pt1, double pt2, int dims)
            //    {
            //        dims = dims - 1;
            //        return Math.Round((pt2 - pt1) / dims, 3, MidpointRounding.AwayFromZero);
            //    }

            //    TrayPointZU[] TwoPoints(TrayPointZU pt1, TrayPointZU pt2, int dims)
            //    {
            //        //需要两个控制点
            //        double x_step = GetStep(pt1.X, pt2.X, dims);
            //        double y_step = GetStep(pt1.Y, pt2.Y, dims);

            //        TrayPointZU[] tempPosArray = new TrayPointZU[dims];

            //        //设置起始点
            //        TrayPointZU startPos = new TrayPointZU();
            //        startPos.X = pt1.X;
            //        startPos.Y = pt1.Y;

            //        TrayPointZU endPos = new TrayPointZU();
            //        endPos.X = pt2.X;
            //        endPos.Y = pt2.Y;
            //        tempPosArray[0] = startPos;
            //        tempPosArray[tempPosArray.Length - 1] = endPos;

            //        // 设置中间点
            //        for (int i = 1; i < tempPosArray.Length - 1; i++)
            //        {
            //            TrayPointZU temp = new TrayPointZU();
            //            temp.X = startPos.X + i * x_step;
            //            temp.Y = startPos.Y + i * y_step;
            //            tempPosArray[i] = temp;

            //        }


            //        return tempPosArray;
            //    }

            //    void AddToList(TrayPointZU[] twoAixsPosMsgs)
            //    {
            //        foreach (var item in twoAixsPosMsgs)
            //        {
            //            item.Id = Points.Count;
            //            Points.Add(item);
            //        }
            //    }
            //    //1 行多列
            //    if (RowNums.GetValue == 1 && ColNums.GetValue > 1)
            //    {

            //        AddToList(TwoPoints(Control_Point1, Control_Point2, ColNums.GetValue));
            //        return;
            //    }

            //    //多行 1列
            //    if (RowNums.GetValue > 1 && ColNums.GetValue == 1)
            //    {

            //        AddToList(TwoPoints(Control_Point1, Control_Point3, RowNums.GetValue));
            //        return;
            //    }

            //    //多行多列
            //    if (RowNums.GetValue > 1 && ColNums.GetValue > 1)
            //    {

            //        TrayPointZU[] posMsgs_1 = TwoPoints(Control_Point1, Control_Point3, RowNums.GetValue);
            //        TrayPointZU[] posMsgs_2 = TwoPoints(Control_Point2, Control_Point4, RowNums.GetValue);

            //        for (int i = 0; i < posMsgs_1.Length; i++)
            //        {
            //            AddToList(TwoPoints(posMsgs_1[i], posMsgs_2[i], ColNums.GetValue));
            //        }


            //    }

            //}
            //finally
            //{
            //    PallletChangeEvent?.Invoke();
            //}


        }

        void CheckDims()
        {
            if (RowNums.GetValue < 1) RowNums.GetValue = 1;
            if (ColNums.GetValue < 1) ColNums.GetValue = 1;

        }

        public TrayPointZU GetFirstTrayPointWithZU()
        {
            TrayPointZU point = new TrayPointZU()
            {
                X = Points[0].X,
                Y = Points[0].Y,
                Z = Points[0].Z,
                U = Points[0].U,
            };
            return point;
        }


        public TrayPointZU GetTrayPointWithZU()
        {
            if (Index < Points.Count)
            {
                TrayPointZU point = new TrayPointZU()
                {
                    X = Points[Index].X,
                    Y = Points[Index].Y,
                    Z = Points[Index].Z,
                    U = Points[Index].U,
                };
                return point;
            }
            return null;
        }

        public TrayPointZU GetTrayPointWithZUFirstOffset(ThreeAixsPosMsgILineZMov start)
        {
            var firstPos = GetFirstTrayPointWithZU();

            if (Index < Points.Count)
            {

                TrayPointZU point = new TrayPointZU()
                {
                    X = start.PosMsg_1.GetValue - firstPos.X + Points[Index].X,
                    Y = start.PosMsg_2.GetValue - firstPos.Y + Points[Index].Y,
                    Z = Points[Index].Z,
                    U = Points[Index].U,
                };
                return point;
            }
            return null;
        }
        public void AddIndex()
        {
            Index++;
            SaveRead.Save();
        }

        public void Clear()
        {
            Index = 0;
            SaveRead.Save();

        }

        public void EndIndex()
        {
            Index = Points.Count-1;
            SaveRead.Save();
        }











    }
}
