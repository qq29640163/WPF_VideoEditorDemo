using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.FFmpegs.Filters
{
    public class MovieFilter : IFilter
    {
        public string ImageUrl { get; }

        public string FilterName { get => "Movie"; }

        public string OutputFlag { get; set; }
        public string InputFlag { get; set; }

        public MovieFilter(string url)
        {
            ImageUrl = CustomToEscape(url);
        }

        private string CustomToEscape(string str)
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\\': // 反斜杠
                        sb.Append("\\\\\\\\");
                        break;
                    case ':':
                        sb.Append("\\\\:");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public string GetFilter()
        {
            return (string.IsNullOrEmpty(InputFlag) ? "" : $"[{InputFlag}]") + $"movie={ ImageUrl }" + (OutputFlag == "" ? "" : $"[{OutputFlag}]");
        }
    }
}
