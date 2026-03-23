using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp10
{
    internal class Test1
    {
        public static void Print1() 
        {
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine($"我是Print1:{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff")}");
            
        }
        public static void Print2()
        {
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine($"我是Print2:{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff")}");
            
        }
    }
}
