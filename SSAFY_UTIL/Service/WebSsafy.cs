using SSAFY_UTIL.Service.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace SSAFY_UTIL.Service
{
    public class WebSsafy : NetworkingBase
    {
        private static readonly string MAIN_URL = "https://edu.ssafy.com/";
        public readonly string HOME_URL = "edu/main/index.do";
        public readonly string LOGINFORM_URL = "comm/login/SecurityLoginForm.do";

        private readonly string LOGIN_URL = "comm/login/SecurityLoginCheck.do";
        private readonly string LOGOUT_URL = "comm/login/systemLogout.do";
        private readonly string CHECKIN_URL = "edu/mycampus/attendance/attendanceClassCheckIn.do";
        private readonly string CHECKOUT_URL = "edu/mycampus/attendance/attendanceClassCheckOut.do";
        private readonly string ATTENDANCE_URL = "edu/mycampus/attendance/attendanceClassList.do";
        private readonly string ATTENDANCE_CONFIRM_URL = "edu/mycampus/attendance/attendanceConfirm.do";

        private DateTime LastLogin = new(0);
        private bool isLogin = false;
        public bool IsLogin
        {
            get
            {
                TimeSpan timeDiff = DateTime.Now - LastLogin;
                bool isvalid = timeDiff.TotalMinutes > 10;
                if (!isvalid)
                {
                    isLogin = false;
                    return false;
                }
                return isLogin & isvalid;
            }
            set
            {
                isLogin = value;
                LastLogin = DateTime.Now;
            }
        }
        
        private WebSsafy() : base(new Uri(MAIN_URL), new List<KeyValuePair<string, string>> { 
            new("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"),
            new("Accept", "application/json"),
        }) { }
        private static readonly Lazy<WebSsafy> _instance = new Lazy<WebSsafy>(() => new WebSsafy());
        public static WebSsafy Instance { get { return _instance.Value; } }

        public async Task<bool> Login(string id, string pw)
        {
            JObject LoginInfo = new();
            LoginInfo.Add("userId", id);
            LoginInfo.Add("userPwd", pw);

            var (CsrfHeader, CsrfToken) = await GetCsrfToken(LOGINFORM_URL);
            List<KeyValuePair<string, string>> headers = new()
            {
                new(CsrfHeader, CsrfToken)
            };

            var (StatusCode, Response) = await PostAsString(LOGIN_URL, null, LoginInfo.ToString(), headers);
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                ExceptionHandler.ErrorMessage("Invalid CSRF Token");
                return false;
            }

            JObject JsonResponse = JObject.Parse(Response);
            return JsonResponse["status"]?.ToString() == "success";
        }

        public async Task<bool> Logout()
        {
            var (CsrfHeader, CsrfToken) = await GetCsrfToken(HOME_URL);
            List<KeyValuePair<string, string>> headers = new()
            {
                new(CsrfHeader, CsrfToken)
            };

            bool result = false;
            var (StatusCode, _) = await PostAsString(LOGOUT_URL, null, null, headers);
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                ExceptionHandler.ErrorMessage("Invalid CSRF Token");
                result = false;
            }
            else if (StatusCode == HttpStatusCode.Redirect)
                result = true;
            return result;
        }

        public async Task<bool> CheckIn()
        {
            var (CsrfHeader, CsrfToken) = await GetCsrfToken(HOME_URL);
            List<KeyValuePair<string, string>> headers = new()
            {
                new(CsrfHeader, CsrfToken)
            };

            var (StatusCode, Response) = await PostAsString(CHECKIN_URL, null, null, headers);
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                ExceptionHandler.ErrorMessage("Invalid CSRF Token");
                return false;
            }

            JObject JsonResponse = JObject.Parse(Response);
            if (JsonResponse["message"] == null)
            {
                ExceptionHandler.ErrorMessage(JsonResponse["result"]?["message"]?.ToString() ?? "Error");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckOut()
        {
            var (CsrfHeader, CsrfToken) = await GetCsrfToken(HOME_URL);
            List<KeyValuePair<string, string>> headers = new()
            {
                new(CsrfHeader, CsrfToken)
            };

            var (StatusCode, Response) = await PostAsString(CHECKOUT_URL, null, null, headers);
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                ExceptionHandler.ErrorMessage("Invalid CSRF Token");
                return false;
            }

            JObject JsonResponse = JObject.Parse(Response);
            if (JsonResponse["message"] == null)
            {
                ExceptionHandler.ErrorMessage(JsonResponse["result"]?["message"]?.ToString() ?? "Error");
                return false;
            }
            return true;
        }

        public async Task<(string, string)> CheckInTime()
        {
            List<KeyValuePair<string, string>> headers = new()
            {
                new("Accept", "*/*")
            };
            var (_, Response) = await GetAsString(HOME_URL, null, headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'inRoomEnd')]");
            if (node == null)
            {
                return (string.Empty, string.Empty);
            }

            string content = node.InnerText;
            Regex regex = new Regex(@"(?<Time>\d\d:\d\d)\s*(?<Message>[\w\s]+)");
            Match match = regex.Match(content);
            string time = match.Groups["Time"].Value;
            string message = match.Groups["Message"].Value.Trim();

            return (time, message);
        }

        public async Task<(string, string)> CheckOutTime()
        {
            List<KeyValuePair<string, string>> headers = new()
            {
                new("Accept", "*/*")
            };
            var (_, Response) = await GetAsString(HOME_URL, null, headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'outRoomEnd')]");
            if (node == null)
            {
                return (string.Empty, string.Empty);
            }

            string content = node.InnerText;
            Regex regex = new Regex(@"(?<Time>\d\d:\d\d)\s*(?<Message>[\w\s]+)");
            Match match = regex.Match(content);
            string time = match.Groups["Time"].Value;
            string message = match.Groups["Message"].Value.Trim();

            return (time, message);
        }

        public async Task<string> GetExpectedPay(int month)
        {
            List<KeyValuePair<string, string>> headers = new()
            {
                new("Accept", "*/*")
            };
            JObject body = new();
            body.Add("attdYrM", DateTime.Now.Year.ToString() + month.ToString("00"));
            var (_, Response) = await PostAsString(ATTENDANCE_CONFIRM_URL, null, body.ToString(), headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'confirmArea')]/table[1]/tbody/tr[2]/td[2]/text()[2]");
            if (node == null)
            {
                ExceptionHandler.ErrorMessage("Error is occurred while getting expected payment.");
                return string.Empty;
            }

            string content = node.InnerText;
            Regex regex = new Regex(@"\((.*)\)");
            Match match = regex.Match(content);
            string pay = match.Groups[1].Value.Trim();
            return pay;
        }

        private async Task<(string, string)> GetCsrfToken(string Url)
        {
            List<KeyValuePair<string, string>> headers = new()
            {
                new("Accept", "*/*")
            };
            var (_, content) = await GetAsString(Url, null, headers);
            
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(content);

            string header = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='_csrf_header']").Attributes["content"].Value;
            string token = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='_csrf']").Attributes["content"].Value;
            return (header, token);
        }
    }
}
