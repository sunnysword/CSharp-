using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler
{
    public class AppStartHelper
    {
        private static string DirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QuickStart");

        private static string GetTargetPath(string currentPath)
        {
            try
            {
                if (System.IO.File.Exists(currentPath))
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(currentPath);
                    string targetPath = shortcut.TargetPath;
                    string targetDirectory = shortcut.WorkingDirectory;
                    return targetPath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        private static IEnumerable<string> GetFileTargetPathArray(IEnumerable<string> array)
        {
            foreach (var item in array)
            {
                yield return GetTargetPath(item);
            }
        }
        public static void StartProgram()
        {
            StopOtherPro();
            System.Threading.Thread.Sleep(1000);
            if (!System.IO.Directory.Exists(DirPath))
            {
                System.IO.Directory.CreateDirectory(DirPath);
                return;
            }
            string[] pFileName = System.IO.Directory.GetFiles(DirPath);
            foreach (var item in pFileName)
            {
                try
                {
                    var current = System.Diagnostics.Process.Start(item);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"打开软件失败{item}:{ex.Message}");

                }
            }
        }

        public static void StopOtherPro()
        {
            if (!System.IO.Directory.Exists(DirPath))
            {
                System.IO.Directory.CreateDirectory(DirPath);
                return;
            }
            string[] pFileName = System.IO.Directory.GetFiles(DirPath);
            var targetPathArray = GetFileTargetPathArray(pFileName);
            System.Diagnostics.Process[] processesRuned = System.Diagnostics.Process.GetProcesses();
            foreach (var item in processesRuned)
            {
                foreach (var item2 in targetPathArray)
                {
                    if (string.IsNullOrWhiteSpace(item2))
                        continue;
                    string str = System.IO.Path.GetFileNameWithoutExtension(item2);
                    if (str.Contains('.'))
                    {
                        str = str.Split('.')[0];
                    }

                    if (item.ProcessName == str)
                    {
                        try
                        {
                            try
                            {
                                item.Kill();
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show($"关闭软件失败{str}:{ex.Message}");
                            }


                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

        }
    }
}
