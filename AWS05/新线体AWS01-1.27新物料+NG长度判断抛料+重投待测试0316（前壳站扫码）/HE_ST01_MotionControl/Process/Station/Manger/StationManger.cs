#define ST01  // 手动定义编译条件
using Handler.Connect.Camera;
using Handler.Motion.Axis;
using Handler.Motion.IO;
using Handler.Connect;
using Handler.Process.Station.TrayLoad.Pallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HE_ST01_MotionControl.OtherHelpers;
using Handler.Process.Station.TrayLoad;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;
using HE_ST01_MotionControl.Motion.Axis.AxisGroup;
using HE_ST01_MotionControl.Connect.Modbus;


namespace Handler.Process.Station
{
    public class StationManger
    {
        public static readonly StationManger Cur = new StationManger();

        private StationManger()
        {
            //工位列表
        }

        public void Register()//主程序调用
        {

            Process_前壳Tray抓取.Cur.Camera = CameraManager.CameraFrontHolder;
            Process_前壳Tray抓取.Cur.Out_CameraLight = StaticIOHelper.外壳站相机光源;
            Process_前壳Tray抓取.Cur.Pallet = PalletManger.Pallet前壳;
            Process_前壳Tray抓取.Cur.ScanPcbShellPallet = PalletManger.ScanPcbShell;
            Process_前壳Tray抓取.Cur.Gripper = ClampRotatedAxisShell.Cur;
            Process_前壳Tray抓取.Cur.ClampProductCheckSensor = StaticIOHelper.外壳电夹爪下降取料过压检测;
            Process_前壳Tray抓取.Cur.TrayProcess = PalletManger.Process_前壳;
            Process_前壳Tray抓取.Cur.PosPlace = ThreeAxis前壳上料.Cur.PosPlace;
            Process_前壳Tray抓取.Cur.安全位置 = ThreeAxis前壳上料.Cur.PosWait;
            Process_前壳Tray抓取.Cur.是否开启前壳夹爪旋转或夹持状态检测 = () => Funs.FunSelection.Cur.FrontHolderCheckProduct.GetValue;

            Process_PCBTray抓取.Cur.Camera = CameraManager.CameraPCB;
            Process_PCBTray抓取.Cur.Out_CameraLight = StaticIOHelper.PCB站相机光源;
            Process_PCBTray抓取.Cur.Pallet = PalletManger.PalletPCB;
            Process_PCBTray抓取.Cur.NGPallet = PalletManger.PalletPCBNG;
            Process_PCBTray抓取.Cur.ScanPallet = PalletManger.ScanPCB;
            Process_PCBTray抓取.Cur.Gripper = ClampRotatedAxisPCB.Cur;
            Process_PCBTray抓取.Cur.ClampProductCheckSensor = StaticIOHelper.PCB电夹爪下降取料过压检测;
            Process_PCBTray抓取.Cur.TrayProcess = PalletManger.Process_PCB;
            Process_PCBTray抓取.Cur.PosPlace = ThreeAxisPCB上料.Cur.PosPlace;
            Process_PCBTray抓取.Cur.安全位置 = ThreeAxisPCB上料.Cur.PosWait;
            Process_PCBTray抓取.Cur.是否开启PCB夹爪旋转或夹持状态检测 = () => Funs.FunSelection.Cur.PCBCheckProduct.GetValue;
        }

        /// <summary>
        /// 光管电机数量，注意：只有需要运动控制的光管电机才可以纳入统计，如果是手动光管是不参与计数的
        /// </summary>
        public const int LightPiPeMototCount = 0;

        public const int RotateAxisStationCount = 6;


        public static StationManger CreateInstance() => Cur;
        /// <summary>
        /// 测试工位集合
        /// </summary>
        public readonly List<RM_dll2.Process.ProcessBase> TestStationList = new List<RM_dll2.Process.ProcessBase>();



    }
}
