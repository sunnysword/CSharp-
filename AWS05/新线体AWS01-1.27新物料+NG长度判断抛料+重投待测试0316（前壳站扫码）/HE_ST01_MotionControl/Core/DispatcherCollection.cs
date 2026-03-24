using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace HE_ST01_MotionControl.Core
{
    public class DispatcherCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher = Application.Current.Dispatcher;
        private readonly object _syncLock = new object();

        public DispatcherCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, _syncLock);
        }
        public T SafeFirstOrDefault(Func<T, bool> predicate)
        {
            lock (_syncLock)
            {
                //直接枚举底层 Items
                return this.Items.FirstOrDefault(predicate);
            }
        }


        public List<T> SafeWhereToList(Func<T, bool> predicate)
        {
            lock (_syncLock)
            {
                //在锁内执行过滤并立刻物化为 List
                return this.Items.Where(predicate).ToList();
            }
        }
        protected override void InsertItem(int index, T item)
        {
            Action insert = () =>
            {
                lock (_syncLock)
                {
                    base.InsertItem(index, item);
                }
            };

            if (_dispatcher.CheckAccess())
                insert();
            else
                _dispatcher.Invoke(insert);
        }

        protected override void SetItem(int index, T item)
        {
            Action set = () =>
            {
                lock (_syncLock)
                {
                    base.SetItem(index, item);
                }
            };

            if (_dispatcher.CheckAccess())
                set();
            else
                _dispatcher.Invoke(set);
        }

        protected override void RemoveItem(int index)
        {
            Action remove = () =>
            {
                lock (_syncLock)
                {
                    base.RemoveItem(index);
                }
            };

            if (_dispatcher.CheckAccess())
                remove();
            else
                _dispatcher.Invoke(remove);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            Action move = () =>
            {
                lock (_syncLock)
                {
                    base.MoveItem(oldIndex, newIndex);
                }
            };

            if (_dispatcher.CheckAccess())
                move();
            else
                _dispatcher.Invoke(move);
        }

        protected override void ClearItems()
        {
            Action clear = () =>
            {
                lock (_syncLock)
                {
                    base.ClearItems();
                }
            };

            if (_dispatcher.CheckAccess())
                clear();
            else
                _dispatcher.Invoke(clear);
        }
    }
}
