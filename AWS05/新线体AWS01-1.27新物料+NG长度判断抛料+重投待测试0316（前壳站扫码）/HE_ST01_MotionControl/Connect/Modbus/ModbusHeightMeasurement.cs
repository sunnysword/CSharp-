using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.Modbus
{
    public class ModbusHeightMeasurement : ModbusHelper
    {
        public ModbusHeightMeasurement(byte slaveId, SerialPort serialPort) : base(slaveId, serialPort)
        {
        }

        public bool GetCurrentHeight(out double height, out string error)
        {
            height = 0.00d;
            error = string.Empty;
            try
            {
                var array = this.ReadHoldingRegisters(0, 2);
                height = this.ConvertToDouble(array) / 1000.0d;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

        }

        public bool Zero(out string error)
        {
            error = string.Empty;
            try
            {
                this.WriteSingleRegister(20, 2);
                return true;
            }
            catch (Exception ex)
            {
                error = AM.Tools.ExceptionHelper.GetInnerExceptionMessageAndStackTrace(ex);
                return false;
            }

        }

        public override double ConvertToDouble(ushort[] values)
        {
            //前面是2，3，后面是0，1
            //var values = new ushort[] { value2, value1 };

            var testValue = values[1];
            var testValue2 = values[0];

            var hex1 = Convert.ToString(testValue, 16).PadLeft(4, '0');
            var hex2 = Convert.ToString(testValue2, 16).PadLeft(4, '0');

            var hex = $"{hex2}{hex1}";
            var dec = long.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier);

            var bin = Convert.ToString(dec, 2).PadLeft(32, '0');
            var valueRet = 0.00;
            if (bin[0] == '1')
            {
                valueRet = (dec - 0xffffffff - 1);
            }
            else
            {
                valueRet = dec;
            }
            //valueRet = valueRet * NumberOfPoint;
            return valueRet;
        }
    }
}
