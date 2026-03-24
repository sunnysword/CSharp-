using ISaveReaddll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handler.Product
{
    class ProductNumberManager : LanguageControlBase.Wpf.BindableObject
    {
        static string path_product => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductMsg.ini");
        public static readonly ProductNumberManager Cur = new ProductNumberManager();
        private ProductNumberManager()
        {
            SaveRead = new SaveReaddll.SaveReadHeler(() => path_product, () => this);
            try
            {
                Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取配置参数时出错:"+ex.Message);
            }
          
        } 

        SaveReaddll.SaveReadHeler SaveRead;

        #region NG产品信息

        [SaveRemark]
         int _OKNums =0;

        [SaveRemark]
         int _ngNums = 0;

        /// <summary>
        /// 总产量
        /// </summary>
        //[SaveRemark]
        public int Total => OKNums + NgNums;

        /// <summary>
        /// NG总数
        /// </summary>
        public  int NgNums 
        {
            get { return _ngNums; }
            set
            {
                    
                    _ngNums = value;
                    Save();
             
            }
        }




        /// <summary>
        /// OK总数
        /// </summary>
        public  int OKNums
        {
            get { return _OKNums; }
            set
            {

                    _OKNums = value;
                    Save();
             
            }
        }

   



        #endregion



        public  void Save()
        {
            try
            {
                SaveRead?.Save();
            }
            catch (Exception ex)
            {

                
            }
           
        }
        public void Read()
        {
            SaveRead.Read();
        }

        public void Clear()
        {

            _ngNums = 0;
            _OKNums = 0;
            
            Save();


        }



    }
}
