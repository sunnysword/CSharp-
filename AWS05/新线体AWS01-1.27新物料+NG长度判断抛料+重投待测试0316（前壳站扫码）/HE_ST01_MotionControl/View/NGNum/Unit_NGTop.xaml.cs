using Handler.Params;
using LanCustomControldll;
using System;
using System.Collections.Generic;
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

namespace Handler.View.NGNum
{
    /// <summary>
    /// Unit_NGTop.xaml 的交互逻辑
    /// </summary>
    public partial class Unit_NGTop : UserControl, IShow
    {
        public Unit_NGTop()
        {
            InitializeComponent();
            ParamsNGTray.Cur.SetNGEvent += Set;
            


        }

        public static object _lock = new object();

        public Dictionary<string, int> TopNumber = new Dictionary<string, int>();

        public Dictionary<int, string> NameNumber = new Dictionary<int, string>();

        public List<int> Maxlist = new List<int>();

        public void Set()
        {
            lock (_lock)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        TopNumber.Clear();
                        NameNumber.Clear();
                        Maxlist.Clear();

                        TopNumber.Add(ParamsNGTray.Cur.ScanPCBNg, ParamsNGTray.Cur.ScanPCBNgNum);
                       // TopNumber.Add(ParamsNGTray.Cur.PCBLocateNg, ParamsNGTray.Cur.PCBLocateNgNum);
                        TopNumber.Add(ParamsNGTray.Cur.PCBBeforePlaceNg, ParamsNGTray.Cur.PCBBeforePlaceNgNum);
                        TopNumber.Add(ParamsNGTray.Cur.PCBAfterPlaceNg, ParamsNGTray.Cur.PCBAfterPlaceNgNum);
                        //TopNumber.Add(ParamsNGTray.Cur.ShellLocateNg, ParamsNGTray.Cur.ShellLocateNgNum);
                        TopNumber.Add(ParamsNGTray.Cur.ShellBeforePlaceNg, ParamsNGTray.Cur.ShellBeforePlaceNgNum);
                        TopNumber.Add(ParamsNGTray.Cur.ShellAfterPlaceNg, ParamsNGTray.Cur.ShellAfterPlaceNgNum);
                        TopNumber.Add(ParamsNGTray.Cur.MesCheckNg, ParamsNGTray.Cur.MesCheckNGsNum);

                        NameNumber.Add(0, ParamsNGTray.Cur.ScanPCBNg);
                        //NameNumber.Add(1, ParamsNGTray.Cur.PCBLocateNg); //前几项
                        //NameNumber.Add(1, ParamsNGTray.Cur.PCBBeforePlaceNg);
                        //NameNumber.Add(2, ParamsNGTray.Cur.PCBAfterPlaceNg);
                        ////NameNumber.Add(4, ParamsNGTray.Cur.ShellLocateNg);
                        //NameNumber.Add(3, ParamsNGTray.Cur.ShellBeforePlaceNg);
                        //NameNumber.Add(4, ParamsNGTray.Cur.ShellAfterPlaceNg);
                        NameNumber.Add(1, ParamsNGTray.Cur.MesCheckNg);

                        Maxlist.Add(ParamsNGTray.Cur.ScanPCBNgNum);
                        //Maxlist.Add(ParamsNGTray.Cur.PCBLocateNgNum);
                        //Maxlist.Add(ParamsNGTray.Cur.PCBBeforePlaceNgNum);
                        //Maxlist.Add(ParamsNGTray.Cur.PCBAfterPlaceNgNum);
                        ////Maxlist.Add(ParamsNGTray.Cur.ShellLocateNgNum);
                        //Maxlist.Add(ParamsNGTray.Cur.ShellBeforePlaceNgNum);
                        //Maxlist.Add(ParamsNGTray.Cur.ShellAfterPlaceNgNum);
                        Maxlist.Add(ParamsNGTray.Cur.MesCheckNGsNum);


                        int[] array = Maxlist.ToArray();
                        int[] sortedIndexArray = array.Select((r, i) => new { Value = r, Index = i }) //升序排列，返回索引顺序
                                    .OrderByDescending(t => t.Value)
                                    .Select(p => p.Index)
                                    .ToArray();

                        tb_TOP1.Text = NameNumber[sortedIndexArray[0]];
                        tb_Top1Num.Text = TopNumber[NameNumber[sortedIndexArray[0]]].ToString();
                        tb_TOP2.Text = NameNumber[sortedIndexArray[1]];
                        tb_Top2Num.Text = TopNumber[NameNumber[sortedIndexArray[1]]].ToString();
                        //tb_TOP3.Text = NameNumber[sortedIndexArray[2]];
                        //tb_Top3Num.Text = TopNumber[NameNumber[sortedIndexArray[2]]].ToString();
                        //tb_TOP4.Text = NameNumber[sortedIndexArray[3]];
                        //tb_Top4Num.Text = TopNumber[NameNumber[sortedIndexArray[3]]].ToString();
                        //tb_TOP5.Text = NameNumber[sortedIndexArray[4]];
                        //tb_Top5Num.Text = TopNumber[NameNumber[sortedIndexArray[4]]].ToString();
                        //tb_TOP6.Text = NameNumber[sortedIndexArray[5]];
                        //tb_Top6Num.Text = TopNumber[NameNumber[sortedIndexArray[5]]].ToString();
                        //tb_TOP7.Text = NameNumber[sortedIndexArray[6]];
                        //tb_Top7Num.Text = TopNumber[NameNumber[sortedIndexArray[6]]].ToString();
                        //tb_TOP8.Text = NameNumber[sortedIndexArray[7]];
                        //tb_Top8Num.Text = TopNumber[NameNumber[sortedIndexArray[7]]].ToString();


                    }
                    catch (Exception)
                    {

                    }

                });
            }
        }


        public void ShowUI()
        {
        }


    }
}
