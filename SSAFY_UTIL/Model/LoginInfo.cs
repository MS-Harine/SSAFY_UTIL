using SSAFY_UTIL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Model
{
    internal class LoginInfo
    {
        private static readonly Lazy<LoginInfo> lazy = new Lazy<LoginInfo>(() => new LoginInfo());
        public static LoginInfo Instance { get { return lazy.Value; } }
        private LoginInfo() { }

        private string LoginId
        {
            get => (string)LocalStorage.GetValue("id", "");
            set => LocalStorage.SetValue("id", value);
        }
        private string LoginPw
        {
            get => (string)LocalStorage.GetValue("pw", "");
            set => LocalStorage.SetValue("pw", value);
        }
        public bool IsDataExist
        {
            get => (bool)LocalStorage.GetValue("autoLogin", false);
            private set => LocalStorage.GetValue("autoLogin", value);
        }

        public void SetLoginInfo(string id, string pw)
        {
            LoginId = id;
            LoginPw = pw;
            IsDataExist = true;
        }

        public (string, string) GetLoginInfo()
        {
            string id = LoginId;
            string pw = LoginPw;
            return (id, pw);
        }

        public void Clear()
        {
            LocalStorage.Clear("id");
            LocalStorage.Clear("pw");
            LocalStorage.Clear("autoLogin");
            IsDataExist = false;
        }
    }
}
