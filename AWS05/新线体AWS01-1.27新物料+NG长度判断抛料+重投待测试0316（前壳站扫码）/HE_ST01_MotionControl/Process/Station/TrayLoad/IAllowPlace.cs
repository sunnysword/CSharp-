using Handler.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Process.Station.TrayLoad
{
    public interface IAllowPlace
    {
        bool Allow { get; set; }

        bool AllowFinish { get; set; }

        ProductInfo CurrentProduct { get; set; }

    }
}
