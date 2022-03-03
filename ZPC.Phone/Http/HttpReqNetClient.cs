using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ZPC.Phone
{
    public class HttpReqNetClient
    {
        private HttpWebRequest request = null;
        private int rangeFrom = -1;
        private int rangeTo = -1;
        private DateTime requestdate = DateTime.MinValue;

        #region 构造方法

        public HttpReqNetClient(string url)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }

        public HttpReqNetClient(string url, string method)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
        }

        public HttpReqNetClient(string url, string method, string host)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.Host = host;
        }

        public HttpReqNetClient(string url, string method, string host, string accept)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.Host = host;
            request.Accept = accept;
        }

        public HttpReqNetClient(string url, string method, string host, string accept, string contentType)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.Host = host;
            request.Accept = accept;
            request.ContentType = contentType;
        }

        #endregion

        #region 添加/移除头部

        public void AddHead(string header, string value)
        {
            if (request != null)
            {
                if (header == "Date")
                {
                    requestdate = Convert.ToDateTime(value);
                }
                else if (header == "Range")
                {
                    string pattern = "([0-9]+)";
                    MatchCollection mc = Regex.Matches(value, pattern);
                    if (mc.Count >= 2)
                    {
                        rangeFrom = Convert.ToInt32(mc[0].Value);
                        rangeTo = Convert.ToInt32(mc[1].Value);
                    }
                }
                else
                {
                    request.Headers.Add(header, value);
                }
            }
        }

        public void RemoveHead(string header)
        {
            request.Headers.Remove(header);
        }


        public void AddHead(List<HeaderEntity> headerList)
        {
            if (request != null)
            {
                if (headerList != null && headerList.Count > 0)
                {
                    foreach (var itemHeader in headerList)
                    {
                        if (itemHeader.Header == "Date")
                        {
                            requestdate = Convert.ToDateTime(itemHeader.Value);
                            continue;
                        }
                        if (itemHeader.Header == "Range")
                        {
                            string pattern = "([0-9]+)";
                            MatchCollection mc = Regex.Matches(itemHeader.Value, pattern);
                            if (mc.Count >= 2)
                            {
                                rangeFrom = Convert.ToInt32(mc[0].Value);
                                rangeTo = Convert.ToInt32(mc[1].Value);
                            }
                            continue;
                        }
                        this.AddHead(itemHeader.Header, itemHeader.Value);
                    }
                }
            }
        }

        #endregion

        #region TimeOut 设置

        public void TimeOutSet(int timeOut)
        {
            if (request != null)
            {
                request.Timeout = timeOut;
            }
        }

        #endregion

        #region useragent设置

        public void UserAgentSet(string userAgent)
        {
            if (request != null)
            {
                request.UserAgent = userAgent;
            }
        }

        #endregion

        #region method 设置

        public void MethodSet(string method)
        {
            if (request != null)
            {
                request.Method = method;
            }
        }

        #endregion

        #region host 设置

        public void HostSet(string host)
        {
            if (request != null)
            {
                request.Host = host;
            }
        }

        #endregion

        #region accept 设置

        public void AcceptSet(string accept)
        {
            if (request != null)
            {
                request.Accept = accept;
            }
        }

        #endregion

        #region contentType 设置

        public void ContentType(string contentType)
        {
            if (request != null)
            {
                request.ContentType = contentType;
            }
        }

        #endregion

        #region contentLenth 设置

        public void ContentLength(long contentLength)
        {
            if (request != null)
            {
                request.ContentLength = contentLength;
            }
        }

        #endregion

        #region range 设置

        public void RangeSet(long from, long to)
        {
            if (request != null)
            {
                request.AddRange(from, to);
            }
        }

        #endregion

        #region accept 设置

        public void KeepAliveSet(bool keepAlive)
        {
            if (request != null)
            {
                request.KeepAlive = keepAlive;
            }
        }

        #endregion

        #region postdata 设置

        public void AddPostData(string requestData)
        {
            if (request != null)
            {
                try
                {
                    SecurityProtocolTypes();
                    byte[] postDataByte = null;
                    if (!string.IsNullOrEmpty(requestData))
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        postDataByte = encoding.GetBytes(requestData);
                        request.ContentLength = postDataByte.Length;
                    }
                    if (postDataByte != null && postDataByte.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postDataByte, 0, postDataByte.Length);
                        }
                    }


                }
                catch (Exception)
                {

                }
            }
        }

        #endregion

        #region 发送请求

        public HttpResponse GetHttpResponse(string postData = "")
        {
            HttpResponse result = null;
            byte[] postDataByte = null;
            if (request != null)
            {
                SecurityProtocolTypes();
                try
                {
                    if (!string.IsNullOrEmpty(postData))
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        postDataByte = encoding.GetBytes(postData);
                        request.ContentLength = postDataByte.Length;
                    }
                    if (postDataByte != null && postDataByte.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postDataByte, 0, postDataByte.Length);
                        }
                    }
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    string responseCode = response.StatusCode.ToString();
                    string coder = response.CharacterSet;
                    Encoding responseEncoding = Encoding.UTF8;
                    if (!string.IsNullOrEmpty(coder))
                    {
                        responseEncoding = Encoding.GetEncoding(coder);
                    }
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent ||
                        response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.PartialContent)
                    {
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, responseEncoding))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                                WebHeaderCollection webHeaders = response.Headers;
                                if (webHeaders != null && webHeaders.Count > 0)
                                {
                                    result.ResponseDateTime = webHeaders.Get("Date");
                                }
                            }
                        }
                    }
                    if (response != null)
                        response.Close();
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        result = new HttpResponse(null, webEx.Status.ToString(), null);
                    }
                    else
                    {
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        string responseCode = response.StatusCode.ToString();
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                        if (response != null)
                            response.Close();
                    }
                }
                catch (Exception e)
                {
                    //Trace.TraceInformation("HttpResponse error:" + e.Message);
                }
            }
            return result;
        }

        public HttpResponse GetHttpResponse(byte[] postData)
        {
            HttpResponse result = null;
            if (request != null)
            {
                SecurityProtocolTypes();
                try
                {
                    if (postData != null && postData.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postData, 0, postData.Length);
                        }
                    }
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    string responseCode = response.StatusCode.ToString();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent ||
                        response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.PartialContent)
                    {

                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        result = new HttpResponse(null, webEx.Status.ToString(), null);
                    }
                    else
                    {
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        string responseCode = response.StatusCode.ToString();
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Trace.TraceInformation("HttpResponse error:" + e.Message);
                }
            }
            return result;
        }


        public HttpResponse GetHttpResponseHead(string headerName, string postData = "")
        {
            HttpResponse result = null;
            byte[] postDataByte = null;
            if (request != null)
            {
                SecurityProtocolTypes();
                try
                {
                    if (!string.IsNullOrEmpty(postData))
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        postDataByte = encoding.GetBytes(postData);
                        request.ContentLength = postDataByte.Length;
                    }
                    if (postDataByte != null && postDataByte.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postDataByte, 0, postDataByte.Length);
                        }
                    }
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    string head = response.GetResponseHeader(headerName);
                    string responseCode = response.StatusCode.ToString();
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent ||
                        response.StatusCode == HttpStatusCode.Accepted)
                    {
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, head);
                            }
                        }
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        result = new HttpResponse(null, webEx.Status.ToString(), null);
                    }
                    else
                    {
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        string responseCode = response.StatusCode.ToString();
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Trace.TraceInformation("GetHttpResponseHead error:" + e.Message);
                }
            }
            return result;
        }

        public HttpResponse GetThumbnailResponse(string postData = "")
        {
            HttpResponse result = null;
            byte[] postDataByte = null;
            if (request != null)
            {
                SecurityProtocolTypes();
                try
                {
                    if (!string.IsNullOrEmpty(postData))
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        postDataByte = encoding.GetBytes(postData);
                        request.ContentLength = postDataByte.Length;
                    }
                    if (postDataByte != null && postDataByte.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postDataByte, 0, postDataByte.Length);
                        }
                    }
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    string responseCode = response.StatusCode.ToString();
                    string coder = response.CharacterSet;
                    Encoding responseEncoding = Encoding.UTF8;
                    if (!string.IsNullOrEmpty(coder))
                    {
                        responseEncoding = Encoding.GetEncoding(coder);
                    }
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent ||
                        response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.PartialContent)
                    {
                        using (Stream sr = response.GetResponseStream())
                        {
                            byte[] bytesInStream = new byte[1024 * 1024];
                            int len = 0;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                while ((len = sr.Read(bytesInStream, 0, bytesInStream.Length)) > 0)
                                {
                                    ms.Write(bytesInStream, 0, len);
                                }
                                byte[] bytes = ms.ToArray();
                                result = new HttpResponse(response, responseCode, "", bytes);
                            }
                        }
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        result = new HttpResponse(null, webEx.Status.ToString(), null);
                    }
                    else
                    {
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        string responseCode = response.StatusCode.ToString();
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Trace.TraceInformation("HttpResponse error:" + e.Message);
                }
            }
            return result;
        }

        public HttpResponse GetDownloadResponse(string postData = "")
        {
            HttpResponse result = null;
            byte[] postDataByte = null;
            if (request != null)
            {
                SecurityProtocolTypes();
                try
                {
                    if (!string.IsNullOrEmpty(postData))
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        postDataByte = encoding.GetBytes(postData);
                        request.ContentLength = postDataByte.Length;
                    }
                    if (postDataByte != null && postDataByte.Length > 0)
                    {
                        using (Stream newStream = request.GetRequestStream())
                        {
                            newStream.Write(postDataByte, 0, postDataByte.Length);
                        }
                    }
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    string responseCode = response.StatusCode.ToString();
                    string coder = response.CharacterSet;
                    Encoding responseEncoding = Encoding.UTF8;
                    if (!string.IsNullOrEmpty(coder))
                    {
                        responseEncoding = Encoding.GetEncoding(coder);
                    }
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent ||
                        response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.PartialContent)
                    {
                        result = new HttpResponse(response, responseCode, "", null, response.GetResponseStream());
                    }
                }
                catch (WebException webEx)
                {
                    if (webEx.Response == null)
                    {
                        result = new HttpResponse(null, webEx.Status.ToString(), null);
                    }
                    else
                    {
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        string responseCode = response.StatusCode.ToString();
                        using (Stream sr = response.GetResponseStream())
                        {
                            using (StreamReader respStreamReader = new StreamReader(sr, Encoding.UTF8))
                            {
                                result = new HttpResponse(response, responseCode, respStreamReader.ReadToEnd());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Trace.TraceInformation("HttpResponse error:" + e.Message);
                }
            }
            return result;
        }

        private void SecurityProtocolTypes()
        {
            OperatingSystem os = Environment.OSVersion;
            if (os.Version.Major == 6) //判断是否是vista
            {
                if (os.Version.Minor == 0)
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                }
            }
        }

        #endregion
    }
}
