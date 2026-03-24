using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.Modbus
{
    public class ModbusTorqueCheck : ModbusHelper
    {
        public ModbusTorqueCheck(byte slaveId, SerialPort serialPort) : base(slaveId, serialPort)
        {
        }

        public bool GetCurrentValue(out double value, out string error)
        {
            value = 0.00d;
            error = string.Empty;
            try
            {
                var array = this.ReadHoldingRegisters(20, 2);
                value = this.ConvertToDouble(array);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

        }


        public bool GetMaxValue(out double value, out string error)
        {
            value = 0.00d;
            error = string.Empty;
            try
            {
                var array = this.ReadHoldingRegisters(22, 2);
                value = this.ConvertToDouble(array);
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
                this.WriteSingleRegister(10, 1);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

        }

        public override double ConvertToDouble(ushort[] values)
        {
            // sign: 符号位, exponent: 阶码, mantissa:尾数
            int us1, us2;
            us1 = values[1];
            us2 = values[0];

            // sign: 符号位, exponent: 阶码, mantissa:尾数
            int sign, exponent;
            float mantissa;//计算符号位
            sign = us1 / 32768;//去掉符号位
            int emCode = us1 % 32768;//计算阶码
            exponent = emCode / 128;//计算尾数
            mantissa = (float)(emCode % 128 * 65536 + us2) / 8388608;//代入公式
                                                                     //fValue = (-1) ^ S x 2 ^ (E - 127) x (1 + M)
            return Math.Round((float)Math.Pow(-1, sign) * (float)Math.Pow(2, exponent - 127) * (1 + mantissa), 3);
        }
    }
}
