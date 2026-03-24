using AixCommInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.Axis
{
    /// <summary>
    /// 插补创建的基类
    /// </summary>
    public abstract class LineAixZCreateBase : AbstractLineAixsdll.LineAixsZCreateBase, ISaveReaddll.ISaveRead
    {
        public LineAixZCreateBase(AbstractSingleAixdll.AbstractSingleAix singleAix1,
            AbstractSingleAixdll.AbstractSingleAix singleAix2, AbstractSingleAixdll.AbstractSingleAix singleAix3
            , string keyName = null) :
            base(singleAix1, singleAix2, singleAix3, new ZMotionHelper.LineAixs.ZMotionLineAixsFactory())
        {
            if (string.IsNullOrEmpty(keyName))
            {
                SaveRead = new SaveReaddll.SaveReadHeler(() => ParamPath_Motion.CurFileFullPath, () => this);
            }
            else
            {
                SaveRead = new SaveReaddll.SaveReadHeler(() => ParamPath_Motion.CurFileFullPath, () => this, section: keyName);
            }
        }
        SaveReaddll.SaveReadHeler SaveRead;



        public void CheckPos(double x, double y)
        {
            if (!Aix_1.CheckPos(x, 3))
            {
                throw new Exception($"X轴目标位置实际位置检测异常,当前位置={Aix_1.GetEncPos()},目标位置={x}");
            }
            if (!Aix_2.CheckPos(y, 3))
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
