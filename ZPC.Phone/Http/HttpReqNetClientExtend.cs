using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace ZPC.Phone
{
    public class HttpReqNetClientExtend
    {
        public bool IsRequestSuccessed { get; set; }

        private int _dey = 0;
        protected string GenerateSessionIdCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this._dey;
            this._dey++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this._dey)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        public string RequestHttpData(string url, int timeOut, string requestMethod = "", string requestData = "")
        {
            IsRequestSuccessed = false;
            string result = "";
            HttpReqNetClient httpReq;
            if (string.IsNullOrEmpty(requestMethod))
                httpReq = new HttpReqNetClient(url);
            else
                httpReq = new HttpReqNetClient(url, requestMethod);
            httpReq.TimeOutSet(timeOut);

            httpReq.ContentType("application/json");
            HttpResponse response;
            if (string.IsNullOrEmpty(requestData))
                response = httpReq.GetHttpResponse();
            else
                response = httpReq.GetHttpResponse(requestData);
            if (response != null)
            {
                if (response.ResponseCode == HttpStatusCode.OK.ToString())
                {
                    result = response.ResponseBody;
                    IsRequestSuccessed = true;
                }
                else if (response.ResponseCode == HttpStatusCode.Unauthorized.ToString())
                {
                    Trace.TraceError(string.Format("Permission Denied: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                else if (response.ResponseCode == "422")
                {
                    Trace.TraceError(string.Format("Argument Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                else if (response.ResponseCode == "ConnectionClosed" || response.ResponseCode == "KeepAliveFailure" || response.ResponseCode == "ConnectFailure")
                {
                    result = "007";
                    Trace.TraceError(string.Format("ConnectionClosed Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));

                }
                else
                {
                    result = "001";
                    Trace.TraceError(string.Format("Unknown Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));

                }
                response.Close();
            }
            else
            {
                Trace.TraceError("Error: Response is NULL");
            }
            return result;
        }

        public byte[] RequestHttpByteData(string url, int timeOut, string requestMethod = "", string requestData = "")
        {
            IsRequestSuccessed = false;
            byte[] result = null;
            HttpReqNetClient httpReq;
            if (string.IsNullOrEmpty(requestMethod))
                httpReq = new HttpReqNetClient(url);
            else
                httpReq = new HttpReqNetClient(url, requestMethod);
            httpReq.TimeOutSet(timeOut);
            HttpResponse response;
            if (string.IsNullOrEmpty(requestData))
                response = httpReq.GetThumbnailResponse();
            else
                response = httpReq.GetThumbnailResponse(requestData);
            if (response != null)
            {
                if (response.ResponseCode == HttpStatusCode.OK.ToString())
                {
                    result = response.ResponseByte;
                    IsRequestSuccessed = true;
                }
                else if (response.ResponseCode == HttpStatusCode.Unauthorized.ToString())
                {
                    Trace.TraceError(string.Format("Permission Denied: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                else if (response.ResponseCode == "422")
                {
                    Trace.TraceError(string.Format("Argument Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                else
                {
                    Trace.TraceError(string.Format("Unknown Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                response.Close();
            }
            else
            {
                Trace.TraceInformation("Error: Response is NULL");
            }
            return result;
        }

        public string UpdateTracingData(string url, int timeOut, string requestMethod = "", string requestData = "")
        {
            IsRequestSuccessed = false;
            string result = "";
            HttpReqNetClient httpReq;
            if (string.IsNullOrEmpty(requestMethod))
                httpReq = new HttpReqNetClient(url);
            else
                httpReq = new HttpReqNetClient(url, requestMethod);
            httpReq.TimeOutSet(timeOut);
            httpReq.ContentType("application/json");
            HttpResponse response;
            if (string.IsNullOrEmpty(requestData))
                response = httpReq.GetHttpResponse();
            else
                response = httpReq.GetHttpResponse(requestData);
            if (response != null)
            {
                if (response.ResponseCode == HttpStatusCode.OK.ToString())
                {
                    result = response.ResponseBody;
                    IsRequestSuccessed = true;
                }
                else
                {
                    result = response.ResponseCode;
                    Trace.TraceInformation(string.Format("UpdateTracingData Error: Code:{0} === Message:{1}", response.ResponseCode, response.ResponseBody));
                }
                response.Close();
            }
            else
            {
                result = "001";
                Trace.TraceInformation("Error: Response is NULL");
            }
            return result;
        }

        public bool DownLoadData(string url, string savePath, int timeOut, string requestMethod = "", string requestData = "")
        {
            IsRequestSuccessed = false;
            bool result = false;
            HttpReqNetClient httpReq;
            if (string.IsNullOrEmpty(requestMethod))
                httpReq = new HttpReqNetClient(url);
            else
                httpReq = new HttpReqNetClient(url, requestMethod);
            httpReq.TimeOutSet(timeOut);
            HttpResponse response;
            if (string.IsNullOrEmpty(requestData))
                response = httpReq.GetDownloadResponse();
            else
                response = httpReq.GetDownloadResponse(requestData);
            if (response != null)
            {
                if (response.ResponseCode == HttpStatusCode.OK.ToString())
                {
                    string tmptarget = savePath;
                    int length = 10485760;// 1024 * 1024 * 10;
                    byte[] bytesInStream = new byte[length];
                    int len = 0;
                    int total = 0;
                    using (FileStream fs = new FileStream(tmptarget, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        while ((len = response.ResponseStream.Read(bytesInStream, 0, bytesInStream.Length)) > 0)
                        {
                            fs.Write(bytesInStream, 0, len);
                            total += len;
                        }
                    }
                    result = true;
                    IsRequestSuccessed = true;
                }
                else
                {
                    Trace.TraceError(string.Format("DownLoadData Unknown Error URL:{0} --- ResponseCode:{1}", url, response.ResponseCode));
                }
                response.Close();
            }
            else
            {
                Trace.TraceError(string.Format("Error: Response is NULL URL:{0}", url));
            }
            return result;
        }
    }
}
