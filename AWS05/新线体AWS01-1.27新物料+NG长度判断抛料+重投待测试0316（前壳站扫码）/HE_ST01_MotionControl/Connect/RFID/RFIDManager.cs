using Handler.Process;
using Handler.Process.Station;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkItemHelper2;

namespace Handler.Connect.RFID
{
    internal class RFIDManager
    {
        //public static readonly RFIDHelper RFIDLoadUpDown = new RFIDHelper((ConnectFactory.RFID1 as AM.Communication.Entity.RFIDEntity).EntityInfo);
        //public static readonly RFIDHelper RFIDRotateAxis = new RFIDHelper((ConnectFactory.RFID2 as AM.Communication.Entity.RFIDEntity).EntityInfo);
        //public static readonly RFIDHelper RFIDUnloadUpDown = new RFIDHelper((ConnectFactory.RFID3 as AM.Communication.Entity.RFIDEntity).EntityInfo);
        //public static readonly RFIDHelper RFIDRotateAxis = new RFIDHelper((ConnectFactory.RFID4 as AM.Communication.Entity.RFIDEntity).EntityInfo);

        // public static readonly RFIDHelper_AYNETTEK RFIDLoadUpDown = new RFIDHelper_AYNETTEK(1, "上料RFID");
        public static readonly RFIDHelper_AYNETTEK Shell_RFID = new RFIDHelper_AYNETTEK(1, "外壳RFID", () => { return Funs.FunCommunicator.Cur.前壳RFID.GetValue; });// "外壳RFID");
        public static readonly RFIDHelper_AYNETTEK PCB_RFID = new RFIDHelper_AYNETTEK(1, "PCBRFID", () => { return Funs.FunCommunicator.Cur.PCBRFID.GetValue; });

        public static void Initial()
        {

        }
    }
}
