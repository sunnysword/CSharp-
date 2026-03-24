using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Handler.Product;

namespace Handler.View
{
    /// <summary>
    /// Wnd_ProductMsg.xaml 的交互逻辑
    /// </summary>
    public partial class Wnd_ProductMsg : Window
    {
        public Wnd_ProductMsg()
        {
            InitializeComponent();
        }
        static readonly object ob_lock = new object();
        static Wnd_ProductMsg Cur = null;
        public static Wnd_ProductMsg CreateInstrance()
        {
            if (Cur == null)
            {
                lock (ob_lock)
                {
                    if (Cur == null)
                    {
                        Cur = new Wnd_ProductMsg();
                    }
                }
            }

            return Cur;

        }

        class ProductMsgBinding
        {
            Func<int> funcGetValue;

            public string Name { get; set; }
            public int Value => funcGetValue.Invoke();

            public ProductMsgBinding(string name, Func<int> funcGetValue)
            {
                this.Name = name;
                this.funcGetValue = funcGetValue;
            }


        }

        readonly ObservableCollection<ProductMsgBinding> productMsgBindings = new ObservableCollection<ProductMsgBinding>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ProductNumberManager.Cur.IsBindUI = true;
            //productMsgBindings.Add(new ProductMsgBinding("Dirty Test", () => ProductNumberManager.Cur.WhiteNgNums));
            //productMsgBindings.Add(new ProductMsgBinding("Camera Location", () => ProductNumberManager.Cur.CamLoactionNgNums));
            //productMsgBindings.Add(new ProductMsgBinding("Glue Check", () => ProductNumberManager.Cur.CamGlueCheckNgNums));
            //productMsgBindings.Add(new ProductMsgBinding("Calib Lens", () => ProductNumberManager.Cur.CalibLensNgNums));
            //productMsgBindings.Add(new ProductMsgBinding("AA", () => ProductNumberManager.Cur.AANgNums));
            ////productMsgBindings.Add(new ProductMsgBinding("Defocus", () => ProductNumberManager.Cur.CalibLensNgNums));

            lv_Ng.ItemsSource = productMsgBindings;

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //ProductNumberManager.Cur.IsBindUI = false;
            Cur = null;
        }
    }
}
