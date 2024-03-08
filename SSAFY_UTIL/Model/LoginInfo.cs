using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Service;
using System;

namespace SSAFY_UTIL.Model
{
    internal class LoginInfo
    {
        private static readonly Lazy<LoginInfo> lazy = new Lazy<LoginInfo>(() => new LoginInfo());
        public static LoginInfo Instance { get { return lazy.Value; } }
        private LoginInfo() { }

        private readonly string LOGININFO_KEY = "loginInfo";
        public bool IsDataExist
        {
            get => (bool)LocalStorage.GetValue("autoLogin", false);
            private set => LocalStorage.GetValue("autoLogin", value);
        }

        public void SetLoginInfo(string id, string pw)
        {
            JObject info = new();
            info.Add("LoginId", id);
            info.Add("LoginPw", pw);
            LocalStorage.SetValue(LOGININFO_KEY, info.ToString());
            IsDataExist = true;
        }

        public (string, string) GetLoginInfo()
        {
            string infoString = (string)LocalStorage.GetValue(LOGININFO_KEY);
            string id = String.Empty;
            string pw = String.Empty;

            if (infoString != null)
            {
                JObject info = JObject.Parse(infoString);
                id = info["LoginId"].ToString();
                pw = info["LoginPw"].ToString();
            }
            return (id, pw);
        }

        public void Clear()
        {
            LocalStorage.Clear(LOGININFO_KEY);
            LocalStorage.Clear("autoLogin");
        }
    }
}
