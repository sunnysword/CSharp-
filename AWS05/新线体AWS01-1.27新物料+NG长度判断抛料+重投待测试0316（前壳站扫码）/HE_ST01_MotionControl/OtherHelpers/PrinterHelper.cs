using AM.Core.IO;
using Handler.Connect.Camera;
using Handler.Core.Process;
using Handler.Motion.Axis;
using Handler.Motion.IO;
using Handler.Motion.PalletTray;
using Handler.Process.Station.TrayLoad.Pallet;
using Handler.Process.Station.TrayLoad;
using Handler.Process.Station;
using Handler.Process;
using Handler.Product;
using Handler.Process.RunMode;
using Iodll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Handler.Motion;
using System.Xml.Linq;

namespace HE_ST01_MotionControl.OtherHelpers
{

    public class PrinterHelper : AM.Core.Process.ProcessIPauseBaseExtend
    {
        private BarTender.Application btAPP;
        private BarTender.Format btFormat;
        private string templeteFilePath;
        private string printDeviceName;

        public static IoInHelper 打印数据准备 { get; set; }
        public static IoInHelper 打印结束 { get; set; }
        public static IoOutHepler 打印开始 { get; set; }
        public PrinterHelper(string name, string templeteFilePath/*格式模板路径*/, string printDeviceName/*打印机名称，从打印机属性复制*/) : base(name, StaticInitial.Motion, true)
        {
            btAPP = new BarTender.Application();
            this.templeteFilePath = templeteFilePath;
            this.printDeviceName = printDeviceName;

        }
        public async Task<bool> Print(PrintParameter printParameter, int timeout = 10/*超时时间设定，单位s*/)
        {
            //等待dataready信号
            DateTime now = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - now).TotalSeconds > timeout)
                {

                    ErrorDialog("超时未收到打印设备准备信号！", "确认");
                    return false;
                }

                if (打印数据准备 == null)
                {
                    throw new Exception("未定义打印数据准备IO,请在外部先初始化！");
                }

                var state = 打印数据准备.In();
                if (state) break;
                await Task.Delay(500);
            }
            Sleep(10);
            //发送数据
            btFormat = btAPP.Formats.Open(templeteFilePath, false, printDeviceName);
            btFormat.PrintSetup.IdenticalCopiesOfLabel = 1;//打印份数
            btFormat.PrintSetup.NumberSerializedLabels = 1;//序列标签数
            if (printParameter.BarCode1 != "")
            {
                btFormat.SetNamedSubStringValue("BarCode1", printParameter.BarCode1);
            }
            if (printParameter.BarCode2 != "")
            {
                btFormat.SetNamedSubStringValue("BarCode2", printParameter.BarCode2);
            }
            if (printParameter.BarCode3 != "")
            {
                btFormat.SetNamedSubStringValue("BarCode3", printParameter.BarCode3);
            }
            if (printParameter.BarCode4 != "")
            {
                btFormat.SetNamedSubStringValue("BarCode4", printParameter.BarCode4);
            }
            if (printParameter.BarCode5 != "")
            {
                btFormat.SetNamedSubStringValue("BarCode5", printParameter.BarCode5);
            }
            btFormat.PrintOut();//第二个false设置打印时是否跳出打印属性
            btFormat.Close(BarTender.BtSaveOptions.btSaveChanges);
            //开始打印
            if (打印开始 == null)
            {
                throw new Exception("未定义打印开始IO,请在外部先初始化！");
            }

            打印开始.Out_ON();

            //等待打印结束

            now = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - now).TotalSeconds > timeout)
                {

                    ErrorDialog("超时未收到打印结束信号！", "确认");
                    return false;
                }

                if (打印结束 == null)
                {
                    throw new Exception("未定义打印数据准备IO,请在外部先初始化！");
                }

                var state = 打印结束.In();
                if (state) break;
                await Task.Delay(500);
            }
            return true;
        }

        public bool IsReadyForPrint()
        {
            if (打印数据准备 == null)
            {
                throw new Exception("未定义打印数据准备IO,请在外部先初始化！");
            }

            return 打印数据准备.In();
        }
        //注意用完关闭，不然new一个PrinterHelper就会再后台出一个bartender软件
        public void Close()
        {
            btAPP.Quit();
        }

    }

    public class PrintParameter
    {

        public string BarCode1 { get; set; } = "";
        public string BarCode2 { get; set; } = "";
        public string BarCode3 { get; set; } = "";
        public string BarCode4 { get; set; } = "";
        public string BarCode5 { get; set; } = "";


        public PrintParameter() { }
    }
}
