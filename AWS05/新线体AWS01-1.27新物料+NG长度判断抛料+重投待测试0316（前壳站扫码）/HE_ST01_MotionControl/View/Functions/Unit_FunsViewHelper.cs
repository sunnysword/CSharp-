using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Handler.View.MainUnits
{
    internal class UserControlMode
    {
        public UserControl UserControl { get; }
        public string Name { get; }
        public double MaxWidth { get; }

        public UserControlMode(UserControl control, Funs.FunsSelectionHelperbase function)
        {
            UserControl = control;
            Name = function.HeaderName;
            MaxWidth = UserControl.MaxWidth;
        }
    }
    class Unit_FunsViewHelper
    {
        public static ObservableCollection<UserControlMode> demoViewModels = new ObservableCollection<UserControlMode>();
        static Unit_FunsViewHelper()
        {

            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunCommon.Cur), Funs.FunCommon.Cur));
            //demoViewModels.Add(new UserControlMode(new Unit_SiteStationSetting(),Funs.FunSocketSetting.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunStationSetting.Cur), Funs.FunStationSetting.Cur));
            //demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunLaser.Cur), Funs.FunLaser.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunSelection.Cur), Funs.FunSelection.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunTimeSettings.Cur), Funs.FunTimeSettings.Cur));
            //demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.Fun夹爪Setting.Cur), Funs.Fun夹爪Setting.Cur));
            //demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunDeveiceLife.Cur), Funs.FunDeveiceLife.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunMES.Cur), Funs.FunMES.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunMESCheck.Cur), Funs.FunMESCheck.Cur));
            demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunCommunicator.Cur), Funs.FunCommunicator.Cur));
            //demoViewModels.Add(new UserControlMode(new Unit_FunsBaseSetting(Funs.FunScrewGun.Cur), Funs.FunScrewGun.Cur));
        }


    }
}
