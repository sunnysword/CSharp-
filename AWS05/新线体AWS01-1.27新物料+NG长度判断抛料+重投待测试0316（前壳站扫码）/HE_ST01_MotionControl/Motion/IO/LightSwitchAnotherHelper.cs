using AixRegisterInfodll;
using RM_dll2.Light;
using RM_dll2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HE_ST01_MotionControl.Process.Station.TrayLoad.Pallet;

namespace HE_ST01_MotionControl.Motion.IO
{
    internal class LightSwitchAnotherHelper : LightSwitchBase
    {
        private IoCheck ioCheck_IsErr;
        private bool _suppressInterlock = false;
        public LightSwitchAnotherHelper(MotionHelper motionHelper) : base(motionHelper)
        {
            RedLight.PreON += () =>
            {
                if (!_suppressInterlock)
                {
                    Green_OFF();
                    YellowOFF();

                }
            };
            GreenLight.PreON += () =>
            {
                if (!_suppressInterlock)
                {
                    Red_OFF();
                    YellowOFF();

                }
            };
            YellowLight.PreON += () =>
            {
                if (!_suppressInterlock)
                {
                    Green_OFF();
                    Red_OFF();

                }
            };
            ioCheck_IsErr = new IoCheck(() => Motion.CurOrder.IsErr);
        }


        public override void ScanLightState()
        {
            if (Motion != null && Motion.CurOrder != null)
            {
                if (ioCheck_IsErr.RisingCheck())
                {
                    Buzzer_ON();
                }

                if (ioCheck_IsErr.FallingCheck())
                {
                    Buzzer_OFF();
                    Red_OFF();
                }

                if (Motion.CurOrder.IsErr)
                {
                    YellowOFF();
                    Green_OFF();
                    Red_ON();
                }
                else if (Motion.CurOrder.IsResing)
                {
                    Yellow_ON();
                }
                else if (Motion.CurOrder.IsAutoRunning)
                {

                    if (Motion.CurOrder.IsPause)
                    {
                        _suppressInterlock = true;
                        Red_OFF();
                        Yellow_ON();
                        Green_OFF();
                        _suppressInterlock = false;

                    }
                    else
                    {
                        Red_OFF();
                        if (YellowLight.IsON)
                        {
                            YellowOFF();
                        }
                        Green_ON();
                        if (PalletManger.Process_前壳.IsWarning || PalletManger.Process_PCB.IsWarning)
                        {
                            Sleep(200);
                            Green_OFF();
                            Sleep(200);
                        }

                    }
                }
                else
                {
                    Yellow_ON();
                }
            }
        }
    }
}
