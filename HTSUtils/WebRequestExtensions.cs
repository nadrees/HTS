using System;
using System.IO;
using System.Net;
using System.Text;

namespace HTSUtils
{
    public static class WebRequestExtensions
    {
        public static HttpWebRequest CreateWebRequest(String url)
        {
            var request = WebRequest.CreateHttp(url);
            request.CookieContainer = new CookieContainer();
            request.UserAgent = "Mozilla/5.0";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            request.Headers.Add("Pragma", "no-cache");

            return request;
        }
    }
}
