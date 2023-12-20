using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DouImp
{
    public class AppConfig
    {
        private static string _rootPath;

        static AppConfig()
        {
            _rootPath = ConfigurationManager.AppSettings["RootPath"].ToString();
        }

        /// <summary>
        /// 檔案存放跟目錄
        /// </summary>
        public static string RootPath
        {
            get { return _rootPath; }
        }
    }
}