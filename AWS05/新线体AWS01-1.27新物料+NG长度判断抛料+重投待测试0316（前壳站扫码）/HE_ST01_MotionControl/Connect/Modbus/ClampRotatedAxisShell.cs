using AccustomeAttributedll;
using AixCommInfo;
using Handler.Modbus;
using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.Connect.Modbus
{

    public class ClampRotatedAxisShell : ClampRotatedAxisBase
    {
        public static readonly ClampRotatedAxisShell Cur = new ClampRotatedAxisShell();
        public ClampRotatedAxisShell() : base("前壳夹爪", "Clamp_Grip.ini")
        {
            前壳夹爪Tray取料旋转 = CreatePosMov();
            前壳夹爪Tray放料旋转 = CreatePosMov();
            前壳夹爪扫码旋转 = CreatePosMov();
            前壳NG放料旋转 = CreatePosMov();
           
        }

        public static ClampRotatedAxisShell CreateInstrance()
        {
            return Cur;
        }

        [TextBoxRemark("前壳夹爪Tray取料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov 前壳夹爪Tray取料旋转;

        [TextBoxRemark("前壳夹爪Tray放料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov 前壳夹爪Tray放料旋转;


        [TextBoxRemark("前壳夹爪扫码旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov 前壳夹爪扫码旋转;

        [TextBoxRemark("前壳NG放料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov 前壳NG放料旋转;

       
    }

    public class ClampRotatedAxisPCB : ClampRotatedAxisBase
    {
        public static readonly ClampRotatedAxisPCB Cur = new ClampRotatedAxisPCB();
        public ClampRotatedAxisPCB() : base("PCB夹爪", "ClampPCB_Grip.ini")
        {
            PCB夹爪Tray取料旋转 = CreatePosMov();
            PCB夹爪Tray放料旋转 = CreatePosMov();
            PCB夹爪扫码旋转 = CreatePosMov();
            PCBNG放料旋转 = CreatePosMov();

        }

        public static ClampRotatedAxisPCB CreateInstrance()
        {
            return Cur;
        }

        [TextBoxRemark("PCB夹爪Tray取料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov PCB夹爪Tray取料旋转;

        [TextBoxRemark("PCB夹爪Tray放料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov PCB夹爪Tray放料旋转;


        [TextBoxRemark("PCB夹爪扫码旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov PCB夹爪扫码旋转;

        [TextBoxRemark("PCBNG放料旋转")]
        [SaveRemark]
        public readonly SingleAixPosAndSpeedMsgAndIMov PCBNG放料旋转;


    }
}
