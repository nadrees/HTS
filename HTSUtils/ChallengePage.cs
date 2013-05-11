using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace HTSUtils
{
    public class ChallengePage
    {
        private const String loginUrl = @"https://www.hackthissite.org/user/login";

        private readonly Cookie phpSessId;
        private readonly String url;

        private String submitAction;

        public ChallengePage(String url, String username, String password)
        {
            phpSessId = GetPHPSESSIDCookie();
            LogIn(username, password);

            this.url = url;
        }

        public HttpWebRequest CreateAuthenticatedWebRequest(String url)
        {
            var request = WebRequestExtensions.CreateWebRequest(url);
            request.CookieContainer.Add(phpSessId);

            return request;
        }

        private static readonly Regex submitActionRegex = new Regex("<form name=\"submitform\" action=\"(.+?)\".*?>", RegexOptions.Compiled);
        public String GetChallengePage()
        {
            var request = WebRequestExtensions.CreateWebRequest(url);
            request.CookieContainer.Add(phpSessId);

            String result = String.Empty;
            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
                LogAndOpenPage(result, "challengePage.html");

                var match = submitActionRegex.Match(result);
                if (!match.Success)
                    throw new Exception("This challenge cannot be auto-submitted");

                submitAction = String.Format("https://www.hackthissite.org{0}", match.Groups[1].Value);
            }

            Console.WriteLine("Retrieved challenge page. Timer starts now");

            return result;
        }

        public void SubmitAnswer(String answer)
        {
            Console.WriteLine("Submitting \"{0}\" as answer", answer);

            String body = String.Format("solution={0}", HttpUtility.UrlEncode(answer));

            var request = WebRequestExtensions.CreateWebRequest(submitAction);
            request.Method = "POST";
            request.CookieContainer.Add(phpSessId);
            request.Referer = submitAction;

            byte[] bytes = UTF8Encoding.UTF8.GetBytes(body);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(body);
            }

            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                String responseHtml = reader.ReadToEnd();
                LogAndOpenPage(responseHtml, "result.html");
            }
        }

        private void LogIn(string username, string password)
        {
            Console.WriteLine("Logging in");

            String body = String.Format("username={0}&password={1}", username, password);

            var request = WebRequestExtensions.CreateWebRequest(loginUrl);
            request.Method = "POST";
            request.CookieContainer.Add(phpSessId);
            request.Referer = loginUrl;

            byte[] bytes = UTF8Encoding.UTF8.GetBytes(body);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(body);
            }

            var response = request.GetResponse();
            if (response.ResponseUri.ToString() == loginUrl)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    LogAndOpenPage(reader.ReadToEnd(), "error.html");
                }
                throw new Exception("Username or password was incorrect");
            }

            Console.Write("Logged in");
        }

        private Cookie GetPHPSESSIDCookie()
        {
            // visit login site once to get PHPSESSID cookie
            var request = WebRequestExtensions.CreateWebRequest(loginUrl);
            var response = (HttpWebResponse)request.GetResponse();

            return response.Cookies["PHPSESSID"];
        }

        private void LogAndOpenPage(String html, String filename)
        {
            File.WriteAllText(filename, html);

            String fullPath = Path.GetFullPath(filename);

            Process.Start(String.Format("file://{0}", fullPath));
        }
    }
}
