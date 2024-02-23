using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SSAFY_UTIL.Model
{
    class Settings
    {
        private static readonly Lazy<Settings> lazy = new Lazy<Settings>(() => new Settings());
        public static Settings Instance { get { return lazy.Value; } }
        private Settings() { }

        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public bool RunOnStartup
        {
            get
            {
                var value = localSettings.Values["runOnStartup"];
                if (value == null)
                {
                    localSettings.Values["runOnStartup"] = false;
                    return false;
                }
                return (bool)value;
            }
            set
            {
                localSettings.Values["runOnStartup"] = value;
            }
        }

        public void ClearSettings()
        {
            localSettings.Values.Clear();
        }
    }
}
