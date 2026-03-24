using AccustomeAttributedll;
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
    public class PalletInfoOnePoint : PalletInfo
    {
        public PalletInfoOnePoint() : base(null)
        {

        }
        public PalletInfoOnePoint(Func<string> readPath) : base(readPath)
        {
            Read();
        }


        /// <summary>
        /// 行间距
        /// </summary>
        [SaveRemark]
        public readonly ParamStructText<double> RowSpacing = new ParamStructText<double>() { GetValue = 10.0 };

        /// <summary>
        /// 列间距
        /// </summary>
        [SaveRemark]
        public readonly ParamStructText<double> ColumnSpacing = new ParamStructText<double>() { GetValue = 4.0 };

        [SaveRemark]
        [FunctionSelectionRemark("XY互换", new string[] { "启用", "禁用" }, 0)]
        public bool IsXYChange = false;

        [SaveRemark]
        [FunctionSelectionRemark("X方向取反", new string[] { "启用", "禁用" }, 0)]
        public bool IsXReversal = false;

        [SaveRemark]
        [FunctionSelectionRemark("Y方向取反", new string[] { "启用", "禁用" }, 0)]
        public bool IsYReversal = false;
        /// <summary>
        /// 获取点位方式（true：视觉定位，false:固定点位）
        /// </summary>
        [SaveRemark]
        [FunctionSelectionRemark("获取点位方式", new string[] { "视觉定位", "固定点位" }, 0)]
        public bool IsPointByCamera = true;


        /// <summary>
        /// 计算托盘点位
        /// </summary>
        public override void Fun_CalcPalletPoints()
        {
            try
            {
                TrayPointZU point = new TrayPointZU();
                if (IsPointByCamera)
                {
                    point = this.Control_Point1;
                }
                else
                {
                    point = this.Control_Point2;
                }
                Points.Clear();

                if (RowNums.GetValue == 1 && ColNums.GetValue == 1)
                {
                    //只需要一个控制点即可
                    Points.Add(new TrayPointZU()
                    {
                        X = point.X,
                        Y = point.Y,
                        Z = point.Z,
                        U = point.U,
                    });
                    return;
                }
                TrayPointZU GetTrayPointByRowAndColumn(int currentRow, int currentColumn)
                {
                    var startPosX = point.X;
                    var startPosY = point.Y;
                    var rowSpacing = this.RowSpacing.GetValue;
                    var columnSpacing = this.ColumnSpacing.GetValue;
                    TrayPointZU TrayPointZU = new TrayPointZU()
                    {
                        Z = point.Z,
                        U = point.U,
                    };
                    if (!IsXYChange)
                    {
                        if (!IsXReversal)
                        {
                            TrayPointZU.X = startPosX + (currentColumn - 1) * columnSpacing;
                        }
                        else
                        {
                            TrayPointZU.X = startPosX - (currentColumn - 1) * columnSpacing;
                        }
                        if (!IsYReversal)
                        {
                            TrayPointZU.Y = startPosY + (currentRow - 1) * rowSpacing;
                        }
                        else
                        {
                            TrayPointZU.Y = startPosY - (currentRow - 1) * rowSpacing;
                        }
                    }
                    else
                    {
                        if (!IsXReversal)
                        {
                            TrayPointZU.X = startPosX + (currentRow - 1) * rowSpacing;
                        }
                        else
                        {
                            TrayPointZU.X = startPosX - (currentRow - 1) * rowSpacing;
                        }
                        if (!IsYReversal)
                        {
                            TrayPointZU.Y = startPosY + (currentColumn - 1) * columnSpacing;
                        }
                        else
                        {
                            TrayPointZU.Y = startPosY - (currentColumn - 1) * columnSpacing;
                        }
                    }

                    return TrayPointZU;
                }


                for (int i = 0; i < this.RowNums.GetValue; i++)
                {
                    for (int j = 0; j < this.ColNums.GetValue; j++)
                    {
                        Points.Add(GetTrayPointByRowAndColumn(i + 1, j + 1));
                    }
                }
            }
            finally
            {
                PallletChangeEvent?.Invoke();
            }


        }




    }
}
