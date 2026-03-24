using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Motion
{
    public static class ParamPath_Motion
    {
        //参数路径的配置文件，保存着Model下当前选择的目录
        static string readPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParamPathSetting.ini");
        /// <summary>
        /// 文件夹
        /// </summary>
        public static string SelectedDirName { get; private set; } = "Default";
        public static string SelectedDirFullName { get; private set; } = "Default";


        static string FileName = "MotionParam.ini";
        /// <summary>
        /// 模板的根文件夹
        /// </summary>
        public static string ModelRoot => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model");

        /// <summary>
        /// 当前的文件夹
        /// </summary>
        public static string SelectedDirPath => System.IO.Path.Combine(ModelRoot, SelectedDirName);
        public static string SelectedDirFullPath => System.IO.Path.Combine(SelectedDirPath, SelectedDirFullName);

        /// <summary>
        /// 当前保存的具体文件，用于参数
        /// </summary>
        public static string CurFileFullPath => System.IO.Path.Combine(SelectedDirPath, FileName);
        static ParamPath_Motion()
        {

            CreateFilePath();

        }
        /// <summary>
        /// 文件夹为模板
        /// </summary>
        static void CreateFilePath()
        {
            string strDefaultMode = "defaultMode";
            string strFullDefaultMode = "defaultMode";
            if (System.IO.File.Exists(path: readPath))
            {
                string[] temp = System.IO.File.ReadLines(readPath).ToArray();
                if (temp.Length >= 2)
                {
                    SelectedDirName = temp[0];
                    SelectedDirFullName = temp[1];
                    if (System.IO.Directory.Exists(SelectedDirFullPath))
                    {
                        if (!System.IO.File.Exists(CurFileFullPath))
                        {
                            using (System.IO.File.Create(CurFileFullPath)) { }
                            ;
                        }
                        return;
                    }
                }
            }

            SelectedDirName = strDefaultMode;
            SelectedDirFullName = strFullDefaultMode;
            if (!System.IO.Directory.Exists(SelectedDirFullPath))
            {
                System.IO.Directory.CreateDirectory(SelectedDirFullPath);

            }
            string filePath = Path.Combine(SelectedDirFullPath, "ccc.json");
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");  //写入空内容
            }

            if (!System.IO.File.Exists(CurFileFullPath))
            {
                using (System.IO.File.Create(CurFileFullPath)) { }

            }
            SaveSelectedItem();
        }

        static void SaveSelectedItem()
        {
            System.IO.File.WriteAllLines(readPath, new string[] { SelectedDirName, SelectedDirFullName });
        }
        public static void ResetLoadFold(string str)
        {
            SelectedDirName = System.IO.Path.GetFileName(str);
            //SaveSelectedItem();
        }
        public static void ResetLoadFoldFull(string str)
        {
            SelectedDirFullName = System.IO.Path.GetFileName(str);
            SaveSelectedItem();
        }
    }
}