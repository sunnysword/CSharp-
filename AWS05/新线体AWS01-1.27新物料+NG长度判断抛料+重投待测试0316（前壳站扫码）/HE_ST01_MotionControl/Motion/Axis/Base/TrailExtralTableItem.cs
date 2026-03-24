using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Handler.Motion.Axis
{
    public class TrailExtralTableItem
    {

        public TrailExtralTableItem(UserControl control, string name)
        {
            userControl = control;
            Name = name;
        }
        public string Name { get; }
        public UserControl userControl { get; }
    }
}
