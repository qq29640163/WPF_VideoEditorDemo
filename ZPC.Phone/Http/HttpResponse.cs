using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZPC.Phone
{
    public class HttpResponse
    {
        public HttpResponse(System.Net.HttpWebResponse webResponse, string responseCode, string responseBody, byte[] responseByte = null, Stream responseStream = null)
        {
            WebResponse = webResponse;
            ResponseCode = responseCode;
            ResponseBody = responseBody;
            ResponseByte = responseByte;
            ResponseStream = responseStream;
        }

        public string ResponseCode { get; set; }
        public string ResponseBody { get; set; }
        public byte[] ResponseByte { get; set; }
        public Stream ResponseStream { get; set; }

        public string ResponseDateTime { get; set; }

        public System.Net.HttpWebResponse WebResponse { get; set; }

        public void Close()
        {
            if (WebResponse != null)
            {
                WebResponse.Close();
                WebResponse.Dispose();
            }
        }
    }
}
