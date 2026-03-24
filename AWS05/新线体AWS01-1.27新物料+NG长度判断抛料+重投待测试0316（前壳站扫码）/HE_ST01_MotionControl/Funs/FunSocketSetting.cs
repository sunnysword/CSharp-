using AixCommInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;


namespace Handler.Funs
{
    /// <summary>
    /// 测试站设置
    /// </summary>
    class FunSocketSetting : FunsSelectionHelperbase
    {
        public static readonly FunSocketSetting Cur = new FunSocketSetting();
        private FunSocketSetting() : base("治具设置")
        {
            this.SaveRead.ReadFinished += () =>
            {
                ConfirmArray(ref SocketIsUseArray, Handler.Process.Station.StationManger.RotateAxisStationCount);
                ConfirmArray(ref StationSensorIsUseArray, Handler.Process.Station.StationManger.RotateAxisStationCount);
            };
            Read();
            ConfirmArray(ref SocketIsUseArray, Handler.Process.Station.StationManger.RotateAxisStationCount);
            ConfirmArray(ref StationSensorIsUseArray, Handler.Process.Station.StationManger.RotateAxisStationCount);
        }

        public Action<int, bool> SocketLimitStatusChangedEventHandler;

        public static FunSocketSetting CreateInstance()
        {
            return Cur;
        }
        /// <summary>
        /// 治具是否启用列表
        /// </summary>
        [ISaveReaddll.SaveRemark]
        public ParamStructText<bool>[] SocketIsUseArray = new ParamStructText<bool>[Handler.Process.Station.StationManger.RotateAxisStationCount];
        /// <summary>
        /// 工位产品检测有无启用禁用
        /// </summary>
        [ISaveReaddll.SaveRemark]
        public ParamStructText<bool>[] StationSensorIsUseArray = new ParamStructText<bool>[Handler.Process.Station.StationManger.RotateAxisStationCount];


        /// <summary>
        /// 换型号时重新读写一次
        /// </summary>
        /// <returns></returns>
        public void ChangProgramSocket()
        {
            Read();
        }

        public bool CheckSocketAllLimit()
        {
            for (int i = 0; i < SocketIsUseArray.Length; i++)
            {
                if (SocketIsUseArray[i].GetValue)
                    return false;
            }
            return true;
        }



        void ConfirmArray<T>(ref T[] array, int length) where T : new()
        {
            List<T> list = array.ToList();
            if (array.Length != length)
            {
                if (array.Length > length)
                {
                    for (int i = 0; i < array.Length - length; i++)
                    {
                        list.RemoveAt(array.Length - i - 1);
                    }
                }
                else
                {

                    for (int i = 0; i < length - array.Length; i++)
                    {
                        T item = new T();
                        list.Add(item);
                    }
                }

            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    list[i] = new T();
                }
            }
            array = list.ToArray();
        }

    }
}
