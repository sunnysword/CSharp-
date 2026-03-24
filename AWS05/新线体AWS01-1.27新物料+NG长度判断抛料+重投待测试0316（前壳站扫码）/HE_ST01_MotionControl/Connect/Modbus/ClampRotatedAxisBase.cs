using AixCommInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Modbus
{
    public abstract class ClampRotatedAxisBase : DH_ClampRotatedAxisDemo.DHClampRotatedAxisHelper, IProxyMov
    {
        public ClampRotatedAxisBase(string name, string portNameCfgName) : base(name, portNameCfgName)
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => System.IO.Path.Combine(Motion.ParamPath_Motion.SelectedDirPath, $"{this.GetType().Name}.ini"), () => this);
        }
        ISaveReaddll.ISaveRead SaveRead;

        public SingleAixPosAndSpeedMsgAndIMov CreatePosMov()
        {
            return new SingleAixPosAndSpeedMsgAndIMov(() => this);
        }

        [ISaveReaddll.SaveRemark]
        public override byte RotatedSpeed { get => base.RotatedSpeed; set => base.RotatedSpeed = value; }
        public void Save()
        {
            SaveRead.Save();
        }

        public void Read()
        {
            SaveRead.Read();
        }

        public void MovAbs(double dis, bool checkdone)
        {
            Rotated_MovAbs((short)dis, checkdone);
        }

        public void MovAbs(double dis, double speed, double acc, double dec, bool checkdone)
        {
            Rotated_MovAbs((short)dis, (byte)speed, checkdone);
        }

        public override bool In_ClampGrip()
        {
            return base.In_ClampGrip() /*&& Motion.Process.RunMode.RunModeManager.Cur.SelectedRunMode.GetWorkRunMode() == Process.RunMode.WorkRunMode.Empty*/;
        }



    }

    public static class ClampRotatedAxisBaseExtend
    {
        public static bool CheckRoatetedPos<T>(this T axis, Func<T, double> get, double dx = 1) where T : ClampRotatedAxisBase
        {
            return axis.CheckRoatetdAxisPos(get(axis), dx);
        }
    }
}
