using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handler
{
    /// <summary>
    /// 窗体起始位置设置帮助类
    /// </summary>
    public class WndStartPositionHelper
    {
        public WndStartPositionHelper()
        {
            FileName = "MainWndStartPositionCfg.ini";
        }

        public WndStartPositionHelper(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; }//"MainWndStartPositionCfg.ini"
        string pathWndCfg => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
        readonly string Wnd_Section = "Window";
        public double Wnd_left { get; private set; } = 0;
        public double Wnd_Top { get; private set; } = 0;
        public double Wnd_Width { get; private set; } = 900;
        public double Wnd_Height { get; private set; } = 800;
        bool IsCloseWritePosition = false;
        bool IsUseCfgPosition = true;//是否使用配置文件

        void Ini_Write(string Key, string _value)
        {
            INIAPI.IniAPI.INIWriteValue(pathWndCfg, Wnd_Section, Key, _value);
        }
        string Ini_Read(string Key)
        {
            return INIAPI.IniAPI.INIGetStringValue(pathWndCfg, Wnd_Section, Key, "0");
        }

        void ReadWndCfg()
        {
            if (!System.IO.File.Exists(pathWndCfg))
            {
                using (System.IO.File.Create(pathWndCfg)) { };

                Fun_WritePositionToCfg();
                //Ini_Write("IsCloseWritePosition", IsCloseWritePosition.ToString());
                Ini_Write("IsUseCfgPosition", IsUseCfgPosition.ToString());
                IsCloseWritePosition = true;
                IsUseCfgPosition = true;
            }
            else
            {
                Fun_ReadWndPositionFromCfg();

                //IsCloseWritePosition = bool.Parse(Ini_Read("IsCloseWritePosition"));
            }

        }

        void Fun_WritePositionToCfg()
        {
            try
            {
                Ini_Write("Wnd_left", Wnd_left.ToString());
                Ini_Write("Wnd_Top", Wnd_Top.ToString());
                Ini_Write("Wnd_Width", Wnd_Width.ToString());
                Ini_Write("Wnd_Height", Wnd_Height.ToString());
            }
            catch
            {


            }

        }


        void Fun_ReadWndPositionFromCfg()
        {
            Wnd_left = double.Parse(Ini_Read("Wnd_left"));
            Wnd_Top = double.Parse(Ini_Read("Wnd_Top"));
            Wnd_Width = double.Parse(Ini_Read("Wnd_Width"));
            Wnd_Height = double.Parse(Ini_Read("Wnd_Height"));
            IsUseCfgPosition = bool.Parse(Ini_Read("IsUseCfgPosition"));
        }

        public void SetWndStartPosition(Window window)
        {
            if (window == null) return;
            ReadWndCfg();
            if (!IsUseCfgPosition) return;
            window.Left = this.Wnd_left;
            window.Top = this.Wnd_Top;
            window.Width = this.Wnd_Width;
            window.Height = this.Wnd_Height;

        }

        public void SaveWndCurrentPosition(Window window)
        {
            if (IsCloseWritePosition)
            {
                Wnd_Top = window.Top;
                Wnd_left = window.Left;
                Wnd_Width = window.Width;
                Wnd_Height = window.Height;
                Fun_WritePositionToCfg();
            }
        }


    }
}
