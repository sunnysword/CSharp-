using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.MES
{
    public class MESResult
    {
        public const string RESULT_PASS = "PASS";
        public const string RESULT_FAIL = "FAIL";
        public string Header  { get; set; }
        public string Command { get; set; }
        public bool? Result { get; set; } = null;
        public string Msg { get; set; }
        public object Extra { get; set; }
    }
}
