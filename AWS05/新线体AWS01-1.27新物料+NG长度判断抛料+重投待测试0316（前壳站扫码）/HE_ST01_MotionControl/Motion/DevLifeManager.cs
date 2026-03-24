using AccustomeAttributedll;
using AixCommInfo;
using ISaveReaddll;
using SaveReaddll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion
{
    /// <summary>
    /// 设备寿命管理
    /// </summary>
    public class DevLifeManager
    {
        public static readonly DevLifeManager Cur = new DevLifeManager();
        private DevLifeManager()
        {
            saveRead = new SaveReadHeler(() => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.Cfg, "DevLifeManager.ini"), () => this);

            ISaveReaddll.SaveReadManagerHelper.RemoveISaveRead(saveRead);
            Read();
        }
        SaveReadHeler saveRead;
        public interface IRstValue
        {
            void Fun_RstCurrentValue();
        }



        public class DevLifeUseInfo<T> : BindableObject, IRstValue where T : struct
        {
            [SaveRemark]
            T _maxValue;

            [SaveRemark]
            T _curValue;

            /// <summary>
            /// 最大值
            /// </summary>
            public T MaxValue
            {
                get { return _maxValue; }
                set
                {
                    SetProperty(ref _maxValue, value);
                }
            }

            /// <summary>
            /// 当前已经使用的值
            /// </summary>
            public T CurrentValue
            {
                get { return _curValue; }
                set
                {
                    SetProperty(ref _curValue, value);
                }
            }

            /// <summary>
            /// 重置当前值
            /// </summary>
            public void Fun_RstCurrentValue()
            {
                CurrentValue = default(T);

            }

            public bool IsOverMaxValue()
            {
                return System.Collections.Generic.Comparer<T>.Default.Compare(_curValue, _maxValue) >= 0;
            }

        }


        [SaveRemark]
        [TextBoxRemark("探针使用次数")]
        /// <summary>
        /// 探针最大使用使用次数
        /// </summary>
        public readonly DevLifeUseInfo<int> PinMaxCount = new DevLifeUseInfo<int>() { CurrentValue = 0 ,MaxValue=10000};

        [SaveRemark]
        [TextBoxRemark("Plasma使用时间(h)")]
        /// <summary>
        /// Plasma使用时间(h)
        /// </summary>
        public readonly DevLifeUseInfo<double> PlasmaMaxTime = new DevLifeUseInfo<double>() { CurrentValue = 0,MaxValue=30000 };



        readonly object ob_lock = new object();

        public void Save()
        {
            lock (ob_lock)
            {
                saveRead.Save();
            }
          
        }


        public void Read()
        {
            lock (ob_lock)
            {
                saveRead.Read();
            }
          
        }



    }
}
