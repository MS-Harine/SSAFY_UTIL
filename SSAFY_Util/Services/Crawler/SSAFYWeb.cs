using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SSAFY_Util.Services.Crawler
{
    public class SSAFYWeb : APIBase
    {
        private const string BASE_URL = "https://edu.ssafy.com";
        private const string MAIN_URL = "https://edu.ssafy.com/edu/main/index.do";
        private bool isLogin = false;
        private DateTime? lastLogin;
        private string? id;
        private string? pw;

        public SSAFYWeb() { }

        public bool Login(string? id, string? pw)
        {
            if (id == null || pw == null) return false;

            ChromeDriver _driver = GetDriver();
            _driver.Navigate().GoToUrl(BASE_URL);

            IWebElement userId = _driver.FindElement(By.Id("userId"));
            IWebElement userPassword = _driver.FindElement(By.Id("userPwd"));
            IWebElement loginBtn = _driver.FindElement(By.CssSelector("div.field-set.log-in > div.form-btn > a"));

            userId.SendKeys(id);
            userPassword.SendKeys(pw);
            loginBtn.Click();

            try
            {
                _driver.FindElement(By.ClassName("profile-area"));
                isLogin = true;
                this.id = id;
                this.pw = pw;
                lastLogin = DateTime.Now;
            }
            catch (NoSuchElementException)
            {
                isLogin = false;
            }

            if (isLogin) ClosePopup();
            else QuitDriver();

            return isLogin;
        }

        public bool CheckLogin()
        {
            if (!isLogin)
                return Login(id, pw);

            if ((lastLogin - DateTime.Now)?.TotalMinutes > 10)
            {
                Logout();
                return Login(id, pw);
            }

            return true;
        }

        public string? GetCheckTime(bool isCheckIn)
        {
            if (!isLogin)
                return null;

            ChromeDriver _driver = GetDriver();
            CheckLogin();
            _driver.Navigate().GoToUrl(MAIN_URL);

            string? time;
            string tag = isCheckIn ? "inRoomEnd" : "outRoomEnd";
            try
            {
                IWebElement checkInTime = _driver.FindElement(By.ClassName(tag));
                time = checkInTime.FindElement(By.ClassName("t1")).Text.Trim();
            }
            catch
            {
                time = null;
            }

            return time;
        }

        public bool CheckAttendance(bool isCheckIn)
        {
            ChromeDriver _driver = GetDriver();
            CheckLogin();

            bool result;
            string tag = isCheckIn ? "checkIn" : "checkOut";
            try
            {
                IWebElement checkInBtn = _driver.FindElement(By.Id(tag));
                ClosePopup();
                checkInBtn.Click();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool Logout()
        {
            ChromeDriver _driver = GetDriver();

            string logoutScript = "fnLogoutProc()";
            _driver.ExecuteScript(logoutScript);
            QuitDriver();

            return true;
        }

        private void ClosePopup()
        {
            ChromeDriver _driver = GetDriver();

            try
            {
                IWebElement popup = _driver.FindElement(By.ClassName("pop-event-close"));
                popup.Click();
            }
            catch (NoSuchElementException) { }
        }
    }
}