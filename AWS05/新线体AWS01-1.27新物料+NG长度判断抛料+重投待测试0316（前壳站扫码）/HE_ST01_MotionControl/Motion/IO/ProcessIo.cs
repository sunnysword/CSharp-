using AM.Core.Extension;
using AM.Core.IO;
using Handler.Motion.Axis;
using RM_dll2.IoCylinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.IO
{
    class ProcessIo
    {

        static ProcessIo()
        {
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampCy, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampFrontBackCy, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(150);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaGripRotate, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(50);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampFrontBackCy, CylinderActionType.OUT);
            //PlasmaGripLens1.Add(50);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.OUT);


            //PlasmaGripLens2.Add(StaticIOHelper.CylinderPlasmaClampCy, CylinderActionType.OUT);
            //PlasmaGripLens2.Add(400);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.BACK);
            //PlasmaGripLens2.Add(StaticIOHelper.CylinderPlasmaGripRotate, CylinderActionType.OUT);
            //PlasmaGripLens2.Add(50);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.OUT);



            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.BACK);
            //PlasmaPutLens_1.Add(StaticIOHelper.CylinderPlasmaClampFrontBackCy, CylinderActionType.BACK);

            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.BACK);
            //PlasmaPutLens.Add(StaticIOHelper.CylinderPlasmaGripRotate, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.OUT);
            //PlasmaPutLens.Add(StaticIOHelper.CylinderPlasmaClampCy, CylinderActionType.BACK);
            //PlasmaGripLens1.Add(StaticIOHelper.CylinderPlasmaClampUpDownCy, CylinderActionType.BACK);
            //PlasmaPutLens.Add(StaticIOHelper.CylinderPlasmaClampFrontBackCy, CylinderActionType.BACK);



        }

        public static bool IsTest { get; set; }

        public static CylinderActionGroup PlasmaGripLens1 = new CylinderActionGroup();
        public static CylinderActionGroup PlasmaGripLens2 = new CylinderActionGroup();
        public static CylinderActionGroup PlasmaPutLens_1 = new CylinderActionGroup();
        public static CylinderActionGroup PlasmaPutLens = new CylinderActionGroup();
    }
}
