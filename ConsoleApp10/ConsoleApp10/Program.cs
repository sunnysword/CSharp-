using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp10
{
    internal class Program
    {
        public static bool MyProperty { get; set; } = true;

        static void Main(string[] args)
        {
            //string path = @"C:\Users\Administrator\Desktop\测试.rff";
            ////string CopyPath = @"C:\Users\Administrator\Desktop\新测试.rff";
            //string str = "";
            //using (StreamReader sr = new StreamReader(path, Encoding.Default))
            //{
            //    str = sr.ReadToEnd();
            //    string[] lines1 = str.Split(new char[] { '[', ']' });
            //    string[] lines2 = lines1[1].Split(new char[] { '{', '}' });
            //    string[] lines3 = lines2[1].Split(new char[] { ',', '"' });
            //    string Url = lines3[13];
            //    Console.WriteLine($"下载地址为：{Url}");

            //}
            //File.Copy(path, CopyPath);

            //string path = $"..//SaveParam/{DateTime.Now.ToString("yyyy-MM-dd")}";

            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            //byte[] buffer=Encoding.ASCII.GetBytes(str);
            //using (FileStream fs=new FileStream(CopyPath, FileMode.Append,FileAccess.Write))
            //{
            //    fs.Write(buffer,0,buffer.Length);
            //}

            //Console.WriteLine("文件创建成功！");
            MyProperty = false;
            //MyProperty = true;
            Console.WriteLine($"当前布尔值为：{Convert.ToInt32(MyProperty)}");
            Console.ReadKey();


        }
    }
}
