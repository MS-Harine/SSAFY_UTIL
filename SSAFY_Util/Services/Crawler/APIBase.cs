using OpenQA.Selenium.Chrome;

namespace SSAFY_Util.Services.Crawler
{
    public class APIBase
    {
        protected ChromeDriverService? _driverService = null;
        protected ChromeOptions? _options = null;
        protected ChromeDriver? _driver = null;

        protected APIBase()
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
            _options.AddArgument("headless");
        }

        protected virtual ChromeDriver GetDriver()
        {
            if (_driver != null)
            {
                return _driver;
            }
            _driver = new ChromeDriver(_driverService, _options);
            return _driver;
        }

        public virtual void QuitDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }
    }
}
