using Google.Cloud.Firestore;
using SSAFY_UTIL.Service.Database;
using System.Collections.Generic;

namespace SSAFY_UTIL.Model
{
    class Launch
    {
        public static Dictionary<string, string> Locations = new()
        {
            { "서울", "seoul" },
            { "대전", "dajeon" },
            { "광주", "gwangju" },
            { "구미", "gumi" },
            { "부울경", "busan" }
        };

        List<string> Menus;

        public void AddMenu(string menu)
        {

        }
    }
}
