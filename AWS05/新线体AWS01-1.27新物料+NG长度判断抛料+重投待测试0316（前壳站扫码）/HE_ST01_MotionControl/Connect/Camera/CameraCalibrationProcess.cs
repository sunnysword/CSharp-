using AbstractLineAixsdll;
using AixCommInfo;
using AM.Core.Extension;
using Handler.Motion.PalletTray;
using Handler.Process.Station.TrayLoad.Pallet;
using LanCustomControldllExtend.FourAxis;
using RM_dll2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.Camera
{
    public class CameraCalibrationProcess : AM.Core.Process.ProcessIPauseBaseExtend
    {
        public CameraCalibrationProcess(string name, MotionHelper motionHelper, bool IsCreateLog = false)
            : base(name, motionHelper, IsCreateLog)
        {
        }

        public LineAixsZCreateBase ThreeAxis { get; set; }

        public PalletCalib Calib { get; set; }

        public CameraHelper Camera { get; set; }


        protected override void ImplementRunAction()
        {
            Ref_Step("清除点位索引");
            TrayPointZU point = null;
            try
            {
                while (IsProcessRunning)
                {
                    WaitPause();
                    if (!IsProcessRunning) break;

                    switch (Step)
                    {
                        case "清除点位索引":
                            Calib.Clear();
                            Ref_Step("移动点位");
                            break;

                        case "移动点位":
                            point = Calib.GetTrayPointWithZU();
                            if (point != null)
                            {
                                WriteToUser($"移动到第{Calib.Index + 1}个点位");
                                Ref_Step("XY开始移动");
                            }
                            else
                            {
                                WriteToUser($"点位移动完成");
                                Ref_Step("退出");
                            }
                            break;

                        case "XY开始移动":
                            ThreeAxis.CancleOneSafeCheck();
                            ThreeAxis.LineMovAbs(point.X, point.Y);
                            Sleep(200);
                            if (ThreeAxis.Aix_1.CheckPos(point.X)&& ThreeAxis.Aix_2.CheckPos(point.Y))
                            {
                                Ref_Step("Z开始移动");
                            }
                            break;

                        case "Z开始移动":
                            ThreeAxis.CancleOneSafeCheck();
                            ThreeAxis.Aix_Z.MovAbs(point.Z);
                            Sleep(200);
                            if (ThreeAxis.Aix_Z.CheckPos(point.Z))
                            {
                                Ref_Step("通知相机到位");
                            }
                            break;

                        case "通知相机到位":
                            if (string.IsNullOrEmpty(Calib.CameraCalibCmdHeader.GetValue))
                            {
                                WriteErrToUser("相机标定失败，相机标定命令未设置，请在运控软件中设置");
                                break;
                            }
                            if(Camera.SendAndCheck(Calib.CameraCalibCmdHeader.GetValue,out _,out string error,
                                $"{Calib.Index + 1},{ThreeAxis.Aix_1.GetCmdPos()},{ThreeAxis.Aix_2.GetCmdPos()}", false)==true)
                            {
                                SleepLoopCheck(2000);
                                Calib.AddIndex();
                                Ref_Step("移动点位");
                            }
                            else
                            {
                                WriteErrToUser(error);
                            }
                            break;

                        case "退出":
                            IsProcessRunning = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
