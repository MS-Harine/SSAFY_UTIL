using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Model
{
    internal class UserInfo
    {
        private static readonly Lazy<UserInfo> lazy = new Lazy<UserInfo>(() => new UserInfo());
        public static UserInfo Instance { get { return lazy.Value; } }
        private UserInfo() { }

        public readonly string USERINFO_KEY = "userInfo";
        public readonly string ATTENDANCE_KEY = "userAttendance";

        public void SetUserAttendance(JObject attendanceInfo)
        {
            attendanceInfo.Add("LastTime", DateTime.Now);
            LocalStorage.SetValue(ATTENDANCE_KEY, attendanceInfo.ToString());
        }

        public JObject? GetUserAttendance()
        {
            JObject result = null;
            string? infoString = (string?)LocalStorage.GetValue(ATTENDANCE_KEY);
            if (infoString != null)
            {
                result = JObject.Parse(infoString);
                if ((DateTime.Now - result["LastTime"].ToObject<DateTime>()).TotalMinutes > 10)
                    result = null;
            }
            return result;
        }

        public void SetUserInfo(JObject userInfo)
        {
            String[] keys = { "Name", "Number", "Location", "Class", "Mileage", "Level", "LevelPoint" };
            foreach (var key in keys)
                Debug.Assert(userInfo[key] != null);

            userInfo.Add("LastTime", DateTime.Now);
            LocalStorage.SetValue(USERINFO_KEY, userInfo.ToString());
        }

        public JObject? GetUserInfo()
        {
            JObject result = null;
            string? infoString = (string?)LocalStorage.GetValue(USERINFO_KEY);
            if (infoString != null)
            {
                result = JObject.Parse(infoString);
                if ((DateTime.Now - result["LastTime"].ToObject<DateTime>()).TotalMinutes > 10)
                    result = null;
            }
            return result;
        }

        public void Clear()
        {
            LocalStorage.Clear(USERINFO_KEY);
        }
    }
}
