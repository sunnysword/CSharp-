using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Handler.Connect;
using Handler.Product;

namespace Handler.Views.HomePage
{
    /// <summary>
    /// Unit_ResultList.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_ResultList : UserControl, INotifyPropertyChanged
    {
        public Unit_ResultList()
        {
            InitializeComponent();
            this.DataContext=this;
            Init();
            //this.stackPanel1.DataContext = this;
            this.stackPanel2.DataContext = this;
        }

        ObservableCollection<UdpItemData>[] SiteTestDatas = new ObservableCollection<UdpItemData>[2];
        private string _totalResult1;
        public string TotalResult1
        {
            get
            {
                return _totalResult1;
            }
            set
            {
                if (_totalResult1 != value)
                {
                    _totalResult1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalResult1)));
                }
            }

        }
        private string _totalResult2;
        public string TotalResult2
        {
            get
            {
                return _totalResult2;
            }
            set
            {
                if (_totalResult2 != value)
                {
                    _totalResult2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalResult2)));
                }
            }

        }

        private void Init()
        {
            for (int i = 0; i < SiteTestDatas.Length; i++)
            {
                SiteTestDatas[i] = new ObservableCollection<UdpItemData>();
            }
            //lv_Sit1Station.ItemsSource = SiteTestDatas[0];
            lv_Sit2Station.ItemsSource = SiteTestDatas[1];
        }

        public void Clear()
        {
            for (int i = 0; i < SiteTestDatas.Length; i++)
            {
                SiteTestDatas[i]?.Clear();
            }
        }

        public void FlushData(int siteIndex, string totalResult, List<UdpItemData> udpItemDatas)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (siteIndex == 0)
                {
                    TotalResult1 = totalResult;
                }
                else
                {
                    TotalResult2 = totalResult;
                }
                var itemList = SiteTestDatas[siteIndex];
                itemList.Clear();
                if (itemList == null) return;
                if (udpItemDatas == null) return;
                if (udpItemDatas.Count == 0) return;
                foreach (var item in udpItemDatas)
                {
                    itemList.Add(item);
                }
              
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
