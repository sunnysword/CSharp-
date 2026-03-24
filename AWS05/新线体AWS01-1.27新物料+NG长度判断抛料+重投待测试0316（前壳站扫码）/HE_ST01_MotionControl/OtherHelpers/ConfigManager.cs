using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HE_ST01_MotionControl.OtherHelpers
{
    public class ConfigManager
    {
        
        static ConfigManager()
        {
           
        }
        /// <summary>
        /// 获取配置文件中AppSetting的内容
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        private static string GetAppConfigSettingValue(string key)
        {
            string ret = ConfigurationManager.AppSettings[key];
            if (ret == null)
            {
                //log.Error(string.Format("{0} does not exist in config document!", key));
                throw new KeyNotFoundException();
            }

            return ret;
        }

        public static int GetInt32(string key)
        {
            int ret = 0;

            try
            {
                ret = Convert.ToInt32(GetAppConfigSettingValue(key));
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return 0;
            }

            return ret;
        }

        public static uint GetUint32(string key)
        {
            try
            {
                return Convert.ToUInt32(GetAppConfigSettingValue(key));
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                throw;
            }
        }

        public static double GetDouble(string key)
        {
            double ret = 0.0;

            try
            {
                ret = Convert.ToDouble(GetAppConfigSettingValue(key));
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return 0;
            }

            return ret;
        }

        public static string GetString(string key)
        {
            try
            {
                return GetAppConfigSettingValue(key);
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return "";
            }
        }

        //修改配置文档
        public static bool AddString(string applicationName, string key, string value)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            Configuration config;

            map.ExeConfigFilename = System.Environment.CurrentDirectory + $"\\{applicationName}.exe.config";
            config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            bool bolExist = false;
            foreach (string Item in config.AppSettings.Settings.AllKeys)
            {
                if (Item == key)
                {
                    bolExist = true;
                    break;
                }
            }
            if (bolExist)
            {
                config.AppSettings.Settings.Remove(key);
            }
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }

        public static bool AddConnectString(string applicationName, string name, string connectStr)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            Configuration config;

            map.ExeConfigFilename = System.Environment.CurrentDirectory + $"\\{applicationName}.exe.config";
            config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            if (config.ConnectionStrings.ConnectionStrings[name] != null)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(name);
            }
            ConnectionStringSettings mySettings = new ConnectionStringSettings();
            mySettings.ConnectionString = connectStr;
            mySettings.Name = name;
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
            return true;
        }
    }
}
