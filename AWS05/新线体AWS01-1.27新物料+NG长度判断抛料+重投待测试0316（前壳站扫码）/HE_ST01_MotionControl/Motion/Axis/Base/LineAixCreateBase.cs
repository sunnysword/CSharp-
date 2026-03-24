using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis
{
    /// <summary>
    /// xy 轴插补的抽象基类
    /// </summary>
    public abstract class LineAixCreateBase:AbstractLineAixsdll.AbstractLineAixs,ISaveReaddll.ISaveRead
    {
        public LineAixCreateBase(AbstractSingleAixdll.AbstractSingleAix singleAix1,
            AbstractSingleAixdll.AbstractSingleAix singleAix2) :base(new ZMotionHelper.LineAixs.ZMotionLineAixsFactory(),singleAix1,singleAix2)
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => ParamPath_Motion.CurFileFullPath, () => this);

        }


        SaveReaddll.SaveReadHeler SaveRead;



        public void CheckPos(double x, double y)
        {
            if (!Aix_1.CheckPos(x, 1))
            {
                throw new Exception($"X轴目标位置实际位置检测异常,当前位置={Aix_1.GetEncPos()},目标位置={x}");
            }
            if (!Aix_2.CheckPos(y, 1))
            {
                throw new Exception($"y轴目标位置实际位置检测异常,当前位置={Aix_2.GetEncPos()},目标位置={y}");
            }

        }

        public void Read()
        {
            SaveRead.Read();
        }

        public void Save()
        {
            SaveRead.Save();
        }
    }
}
