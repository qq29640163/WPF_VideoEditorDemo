using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.Http
{
    public static class HttpUtilityTools
    {
        public static string RequestUrlEncode(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            return System.Web.HttpUtility.UrlEncode(url);
        }

        public static string RequestUrlDecodee(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            return System.Web.HttpUtility.UrlDecode(url);
        }
    }
}
