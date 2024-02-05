using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SSAFY_Util
{
    internal class WebService
    {
        protected ChromeDriverService? _driverService = null;
        protected ChromeOptions? _options = null;
        protected ChromeDriver? _driver = null;

        private readonly string BASE_URL = "https://edu.ssafy.com";
        private readonly string MAIN_URL = "https://edu.ssafy.com/edu/main/index.do";
        private bool isLogin = false;

        public WebService()
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }

        public bool Login(string id, string pw)
        {
            if (_driver == null)
                RunDriver();

            if (isLogin)
                return true;

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            _driver.Navigate().GoToUrl(BASE_URL);

            IWebElement userId = _driver.FindElement(By.Id("userId"));
            IWebElement userPassword = _driver.FindElement(By.Id("userPwd"));
            IWebElement loginBtn = _driver.FindElement(By.CssSelector("div.field-set.log-in > div.form-btn > a"));

            userId.SendKeys(id);
            userPassword.SendKeys(pw);
            loginBtn.Click();
            Thread.Sleep(1000);

            try
            {
                _driver.FindElement(By.ClassName("profile-area"));
                isLogin = true;
                ClosePopup();
            }
            catch (NoSuchElementException) {
                isLogin = false;
                CloseDriver();
                return false;
            }

            return true;
        }

        public string? GetCheckInTime()
        {
            if (_driver == null || !isLogin)
                return null;

            _driver.Navigate().GoToUrl(MAIN_URL);
            string? time;
            try
            {
                IWebElement checkInTime = _driver.FindElement(By.ClassName("inRoomEnd"));
                time = checkInTime.FindElement(By.TagName("span")).Text.Trim();
            }
            catch
            {
                return null;
            }

            return time;
        }

        public string? GetCheckOutTime()
        {
            if (_driver == null || !isLogin)
                return null;

            _driver.Navigate().GoToUrl(MAIN_URL);
            string? time;
            try
            {
                IWebElement checkInTime = _driver.FindElement(By.ClassName("outRoomEnd"));
                time = checkInTime.FindElement(By.TagName("span")).Text.Trim();
            }
            catch
            {
                return null;
            }

            return time;
        }

        public string? CheckIn(string id, string pw)
        {
            if (_driver == null)
                RunDriver();

            bool loginResult = isLogin ? true : Login(id, pw);
            string? result = loginResult ? RoomCheckIn() : null;
            Logout();
            isLogin = false;
            CloseDriver();

            return result;
        }

        public string? CheckOut(string id, string pw)
        {
            if (_driver == null)
                RunDriver();

            bool loginResult = isLogin ? true : Login(id, pw);
            string? result = loginResult ? RoomCheckOut() : null;
            Logout();
            CloseDriver();

            return result;
        }

        public bool Logout()
        {
            if (_driver == null)
                return false;

            string logoutScript = "fnLogoutProc()";
            _driver.ExecuteScript(logoutScript);
            CloseDriver();

            return true;
        }

        private void RunDriver()
        {
            if (_driver == null)
                _driver = new ChromeDriver(_driverService, _options);
        }

        private void CloseDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }

        private void ClosePopup()
        {
            if (_driver == null)
                return;

            try
            {
                IWebElement popup = _driver.FindElement(By.ClassName("pop-event-close"));
                popup.Click();
            }
            catch (NoSuchElementException) { }
        }

        private string? RoomCheckIn()
        {
            if (_driver == null)
                return null;

            try
            {
                IWebElement checkInBtn = _driver.FindElement(By.Id("checkIn"));
                if (checkInBtn == null) return null;
                checkInBtn.Click();

                ClosePopup();
            } 
            catch
            {
                return null;
            }

            IWebElement checkInTime = _driver.FindElement(By.ClassName("inRoomEnd"));
            string time = checkInTime.FindElement(By.ClassName("t1")).Text.Trim();

            return time;
        }

        private string? RoomCheckOut()
        {
            if (_driver == null)
                return null;

            try
            {
                IWebElement checkInBtn = _driver.FindElement(By.Id("checkOut"));
                if (checkInBtn == null) return null;
                checkInBtn.Click();

                ClosePopup();
            }
            catch
            {
                return null;
            }

            IWebElement checkInTime = _driver.FindElement(By.ClassName("outRoomEnd"));
            string time = checkInTime.FindElement(By.ClassName("t1")).Text.Trim();

            return time;
        }
    }
}
