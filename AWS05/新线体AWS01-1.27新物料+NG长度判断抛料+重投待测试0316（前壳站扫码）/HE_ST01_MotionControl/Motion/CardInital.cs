using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion
{
    internal class CardInital
    {
        public static event Action<string> ReportInfoProcessBarEvent;
        public static CardInitialClosedll.CardInitialCloseHelper card = new ZMotionHelper.ZMotionCard(
            HE_ST01_MotionControl.Properties.Settings.Default.Motion_IP);
        private static void Card_ReportCardInitializeProcess(string msg)
        {
            ReportInfoProcessBarEvent?.Invoke(msg);
        }

        static CardInital()
        {
            AM.Global.GlobalCfg.G_CardType = AM.Global.GlobalCfg.CardType.ZMotion;
        }

        static void CylinderInit()
        {
            AM.Core.IO.IOBuild.CylinderBuilder.InitCylinder(typeof(Motion.IO.StaticIOHelper));
        }
        public static void Initial()
        {
            AM.Core.IO.IOBuilder.Init();
            AM.Core.IO.IOBuilder.IOSetValue(typeof(Motion.IO.StaticIOHelper));
            CylinderInit();
           

        }

        public static void CardInit()
        {
            //正运动
            if (HE_ST01_MotionControl.Properties.Settings.Default.NeedInitial)
            {
                card.ReportCardInitializeProcess += Card_ReportCardInitializeProcess;
                card.Initial();
            }
        }
    }
}
