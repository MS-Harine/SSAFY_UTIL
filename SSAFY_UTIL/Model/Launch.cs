using System;
using System.Collections.Generic;
using System.Linq;

namespace SSAFY_UTIL.Model
{
    class Launch
    {
        private Location.LocationType LocationType;
        private string Type;
        private List<string> Menus;

        public Launch(string LocationAsString, string Type, List<string> Menus)
        {
            UpdateLocation(LocationAsString);
            UpdateType(Type);
            UpdateMenus(Menus);
        }

        public void UpdateLocation(string LocationAsString)
        {
            if (!Location.LocationInfo.ContainsKey(LocationAsString))
            {
                throw new Exception("Invalid Location Key");
            }
            this.LocationType = Location.LocationInfo[LocationAsString];
        }

        public void UpdateType(string Type)
        {
            this.Type = Type;
        }

        public void UpdateMenus(List<string> Menus)
        {
            this.Menus = Menus;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dict = new()
            {
                { "location", Location.LocationInfoReverse[this.LocationType] },
                { "type", this.Type },
                { "menus", this.Menus }
            };
            return dict;
        }
    }

    class Location
    {
        public enum LocationType { SEOUL, DAJEON, GWANGJU, GUMI, BUSAN };
        public static Dictionary<string, LocationType> LocationInfo = new()
        {
            { "서울", LocationType.SEOUL },
            { "대전", LocationType.DAJEON },
            { "광주", LocationType.GWANGJU },
            { "구미", LocationType.GUMI },
            { "부울경", LocationType.BUSAN }
        };
        public static Dictionary<LocationType, string> LocationInfoReverse
            = LocationInfo.ToDictionary(x => x.Value, x => x.Key);
    }
}
