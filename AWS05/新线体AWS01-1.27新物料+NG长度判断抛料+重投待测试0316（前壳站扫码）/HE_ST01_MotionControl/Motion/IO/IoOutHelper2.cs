using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion.IO
{
    public class IoOutHelper2 : IoOutHepler
    {
        public enum OutputStatus
        {
            ON,
            OFF,
        }
        public IoOutHelper2(IoControl helper, string _remark, params byte[] ByteParas) : base(helper, _remark, ByteParas)
        {
        }

        public IoOutHelper2(IoControl helper, string _remark, string _buttonRemark, params byte[] ByteParas) : base(helper, _remark, _buttonRemark, ByteParas)
        {
        }

        public IoOutHelper2(IoControl helper, string _remark, bool IsAddClick, params byte[] ByteParas) : base(helper, _remark, IsAddClick, ByteParas)
        {
        }

        public IoOutHelper2(IoControl helper, string _remark, string _buttonRemark, bool IsAddClick, params byte[] ByteParas) : base(helper, _remark, _buttonRemark, IsAddClick, ByteParas)
        {
        }

        public OutputStatus LockStatus { get; set; }
        public void LockOutputStatus()
        {
            if (this.In_Outbit())
            {
                LockStatus = OutputStatus.ON;
            }
            else
            {
                LockStatus = OutputStatus.OFF;
            }
        }
        public void ResetOutputStatus()
        {
            switch (LockStatus)
            {
                case OutputStatus.ON:
                    this.Out_ON();
                    break;
                case OutputStatus.OFF:
                    this.Out_OFF();
                    break;
                default:
                    break;
            }
        }

    }
}
