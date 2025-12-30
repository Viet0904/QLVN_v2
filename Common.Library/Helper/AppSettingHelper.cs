using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace Common.Library.Helper
{
    public class AppSettingHelper
    {
        public static string GetDefault(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetAsUrl(string key)
        {
            var url = ConfigurationManager.AppSettings[key];

            if (url != null)
            {
                url = url.Trim();
                url = Regex.Replace(url, @"\/+$", "/");

                if (url[url.Length - 1] != '/')
                {
                    url += '/';
                }
            }

            return url;
        }
    }
}
