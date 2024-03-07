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
        private readonly string CLASS_URL = "edu/community/search/searchStudentList.do";

        private DateTime LastLogin = new(0);
        private bool isLogin = false;
        public bool IsLogin
        {
            get
            {
                TimeSpan timeDiff = DateTime.Now - LastLogin;
                bool isvalid = timeDiff.TotalMinutes < 10;
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
            List<KeyValuePair<string, string>> LoginInfo = new()
            {
                new("userId", id),
                new("userPwd", pw),
            };

            var (CsrfHeader, CsrfToken) = await GetCsrfToken(LOGINFORM_URL);
            List<KeyValuePair<string, string>> headers = new()
            {
                new(CsrfHeader, CsrfToken)
            };

            var (StatusCode, Response) = await PostAsString(LOGIN_URL, null, LoginInfo, headers);
            if (StatusCode == HttpStatusCode.Forbidden)
            {
                ExceptionHandler.ErrorMessage("Invalid CSRF Token");
                return false;
            }

            JObject JsonResponse = JObject.Parse(Response);
            bool result = JsonResponse["status"]?.ToString() == "success";
            if (result)
                IsLogin = true;
            return result;
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

            if (result)
                IsLogin = false;

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
            List<KeyValuePair<string, string>> headers = new() { new("Accept", "*/*") };
            var (_, Response) = await GetAsString(HOME_URL, null, headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='inRoomEnd']");
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
            List<KeyValuePair<string, string>> headers = new() { new("Accept", "*/*") };
            var (_, Response) = await GetAsString(HOME_URL, null, headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='outRoomEnd']");
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
            List<KeyValuePair<string, string>> headers = new() { new("Accept", "*/*") };
            List<KeyValuePair<string, string>> body = new() { new("attdYrM", DateTime.Now.Year.ToString() + month.ToString("00")) };

            var (_, Response) = await PostAsString(ATTENDANCE_CONFIRM_URL, null, body, headers);

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(Response);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='confirmArea']/table[1]/tbody/tr[2]/td[2]/text()[2]");
            if (node == null)
            {
                ExceptionHandler.ErrorMessage("Error is occurred while getting expected payment.");
                return string.Empty;
            }

            string content = node.InnerText;
            Regex regex = new(@"\((.*)\)");
            Match match = regex.Match(content);
            string pay = match.Groups[1].Value.Trim();
            return pay;
        }

        public async Task<JObject> GetStudentInfo()
        {
            HtmlDocument htmlDoc = new();
            List<KeyValuePair<string, string>> headers = new() { new("Accept", "*/*") };
           
            var (_, Response) = await GetAsString(HOME_URL, null, headers);
            htmlDoc.LoadHtml(Response);
            string studentNumber = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='profile-area']/div[1]/a/div/span[1]").InnerText.Trim();
            string studentName = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='profile-area']/div[1]/a/div/span[2]/em").InnerText.Trim();
            string studentMileage = htmlDoc.DocumentNode.SelectSingleNode("//section[@class='main-page sec1']/div/div[2]/div[1]/header/dl[1]/dd").InnerText.Trim();
            string studentLevelPoint = htmlDoc.DocumentNode.SelectSingleNode("//section[@class='main-page sec1']/div/div[2]/div[1]/header/dl[2]/dd").InnerText.Trim();
            string studentLevel = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='my-level']/div/p/span").InnerText.Trim();
            studentLevel = Regex.Replace(studentLevel, @"\s+", " ");

            (_, Response) = await GetAsString(CLASS_URL, null, headers);
            htmlDoc.LoadHtml(Response);
            string studentLocationClass = htmlDoc.DocumentNode.SelectSingleNode("//article/div[1]/h3/span[2]").InnerText.Trim();
            string studentLocation = studentLocationClass[..2];
            string studentClass = studentLocationClass[2..];

            JObject result = new();
            result.Add("Number", studentNumber);
            result.Add("Name", studentName);
            result.Add("Mileage", studentMileage);
            result.Add("Level", studentLevel);
            result.Add("LevelPoint", studentLevelPoint);
            result.Add("Location", studentLocation);
            result.Add("Class", studentClass);
            return result;
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
