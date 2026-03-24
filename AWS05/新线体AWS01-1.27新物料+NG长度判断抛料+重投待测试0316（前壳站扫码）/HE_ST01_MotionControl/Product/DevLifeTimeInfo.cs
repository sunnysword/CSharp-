using AixCommInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Product
{
    class DevLifeTimeInfo 
    {
       public static readonly DevLifeTimeInfo Cur = new DevLifeTimeInfo();

        static string path_product => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DevLifeTimeInfo.ini");
        private DevLifeTimeInfo()
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => path_product, () => this);
            Read();
        }
       
        SaveReaddll.SaveReadHeler SaveRead;

        [ISaveReaddll.SaveRemark]
        public readonly ParamStructText<int> PinMaxCount = new ParamStructText<int>() { GetValue=1000};

        [ISaveReaddll.SaveRemark]
        public readonly ParamStructText<int> PlasmaMaxCount = new ParamStructText<int>() { GetValue = 5000 };

        [ISaveReaddll.SaveRemark]
        public readonly ParamStructText<int> Pin_AACount = new ParamStructText<int>();

        [ISaveReaddll.SaveRemark]
        public readonly ParamStructText<int> Pin_WhiteCount = new ParamStructText<int>();

        [ISaveReaddll.SaveRemark]
        public readonly ParamStructText<int> Plasma_UseCount = new ParamStructText<int>();


        public void Save()
        {
            SaveRead?.Save();

        }


        public void Read()
        {
            SaveRead.Read();
        }



        public static void Add_AAPinCount()
        {
            App.Current.Dispatcher.Invoke(()=>
            {
                Cur.Pin_AACount.GetValue++;
            });

            
        }


        public static void Add_WhitePinCount()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Cur.Pin_WhiteCount.GetValue++;
            });
        }


        public static void Add_UsePlasmaCount()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Cur.Plasma_UseCount.GetValue++;
            });


        }

    }
}
