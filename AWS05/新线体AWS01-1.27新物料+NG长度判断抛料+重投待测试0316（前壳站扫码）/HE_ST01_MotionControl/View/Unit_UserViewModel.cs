using LanCustomControldll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Handler.View
{
    /// <summary>
    /// 用来管理（Motion界面里的）用户界面，当需要向Motion界面里的界面新增一个时，需要创建该对象一个
    /// 实例，并将需要增加的用户界面传递给它
    /// </summary>
    class Unit_UserViewModel
    {

        /// <summary>
        /// 管理所有用户界面对象的集合
        /// </summary>
        public static ObservableCollection<Unit_UserViewModel> userControlsListHaved = new ObservableCollection<Unit_UserViewModel>();


        /// <summary>
        /// 当前展示的用户界面
        /// </summary>
        public static Unit_UserViewModel userControl_CurShow { get; private set; }

        /// <summary>
        /// 切换界面的按钮
        /// </summary>
        public LanguageControlBase.Wpf.LanButton button { get; set; }

        /// <summary>
        /// 传递的界面
        /// </summary>
        public UserControl userControl { get; set; }


        static event Action actionResetButtonBackground;//用来重置所有按键的状态
        static event Action ationShowEventHandler;//用来展示相应界面的状态信息
        public static event Action actionButtonDownEventHandler;//按钮按下后所触发的事件

        public Unit_UserViewModel(string _Name, UserControl userControl_)
        {

            userControl = userControl_;
            //
            LanguageControlBase.Wpf.LanButton button_ = new LanguageControlBase.Wpf.LanButton();
            button_.LanContent = _Name;
            button_.Height = 40;
            button_.Width = 128;
            button_.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            button_.Click += (s, r) =>
            {
                Button tempButton = s as Button;
                if (tempButton.Background == Brushes.Green) return;
                //Fun_ResetAllButtonBackground();
                userControl_CurShow?.ResetButtonBackground();
                SetShow();
                actionButtonDownEventHandler?.Invoke();
            };

            button = button_;
            //

            actionResetButtonBackground += ResetButtonBackground;
           
            userControlsListHaved.Add(this);

        }
        public Unit_UserViewModel(string _Name, UserControl userControl_, IShow show)
        {

            userControl = userControl_;
            //
            LanguageControlBase.Wpf.LanButton button_ = new LanguageControlBase.Wpf.LanButton();
            button_.LanContent = _Name;
            button_.Height = 40;
            button_.Width = 128;
            button_.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            button_.Click += (s, r) =>
            {
                Button tempButton = s as Button;
                if (tempButton.Background == Brushes.Green) return;
                //Fun_ResetAllButtonBackground();
                userControl_CurShow?.ResetButtonBackground();
                SetShow();
                actionButtonDownEventHandler?.Invoke();
            };

            button = button_;
            //

            actionResetButtonBackground += ResetButtonBackground;
            if (show != null)
            {
                ationShowEventHandler += show.ShowUI;
            }
            userControlsListHaved.Add(this);

        }

        public Unit_UserViewModel(string _Name, UserControl userControl_, Action show)
        {

            userControl = userControl_;
            //
            LanguageControlBase.Wpf.LanButton button_ = new LanguageControlBase.Wpf.LanButton();
            button_.LanContent = _Name;
            button_.Height = 40;
            button_.Width = 128;
            button_.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            button_.Click += (s, r) =>
            {
                Button tempButton = s as Button;
                if (tempButton.Background == Brushes.Green) return;
                //Fun_ResetAllButtonBackground();
                userControl_CurShow?.ResetButtonBackground();
                SetShow();
                actionButtonDownEventHandler?.Invoke();
            };

            button = button_;
            //

            actionResetButtonBackground += ResetButtonBackground;
            if (show != null)
            {
                ationShowEventHandler += show;
            }
            userControlsListHaved.Add(this);

        }

        public void ResetButtonBackground()
        {
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD));
        }


        public void SetShow()
        {
            button.Background = Brushes.Green;
            userControl_CurShow = this;
        }

        public static void Fun_ResetAllButtonBackground()
        {
            actionResetButtonBackground?.Invoke();
        }

        public static void Fun_AllShow()
        {
            ationShowEventHandler?.Invoke();
        }

    }
}
