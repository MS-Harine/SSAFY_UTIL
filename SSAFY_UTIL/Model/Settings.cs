using SSAFY_UTIL.Service;
using System;

namespace SSAFY_UTIL.Model
{
    class Settings
    {
        private static readonly Lazy<Settings> lazy = new Lazy<Settings>(() => new Settings());
        public static Settings Instance { get { return lazy.Value; } }
        private Settings() { }

        public bool RunOnStartup
        {
            get
            {
                var value = LocalStorage.GetValue("runOnStartup");
                if (value == null)
                {
                    LocalStorage.SetValue("runOnStartup", false);
                    return false;
                }
                return (bool)value;
            }
            set
            {
                LocalStorage.SetValue("runOnStartup", value);
            }
        }

        public void ClearSettings()
        {
            LocalStorage.ClearAll();
        }
    }
}
