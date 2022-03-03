using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone
{
    /// <summary>
    /// 报头实体类
    /// </summary>
    public class HeaderEntity
    {
        public static string ACCEPT = "Accept";
        public static string AUTHORIZATION = "Authorization";
        public static string CONTENTTYPE = "Content-Type";
        public static string USERAGENT = "User-Agent";
        public static string XMMECLIENTINFO = "X-Mme-Client-Info";

        private string _header = null;
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private string _value = null;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public HeaderEntity(string header, string value)
        {
            this.Header = header;
            this.Value = value;
        }
    }
}
