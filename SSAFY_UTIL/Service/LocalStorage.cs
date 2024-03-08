using Windows.Storage;

namespace SSAFY_UTIL.Service
{
    internal static class LocalStorage
    {
        private static readonly ApplicationDataContainer localSettings = 
            ApplicationData.Current.LocalSettings;

        public static object? GetValue(string key, object? defaultValue = null)
        {
            return localSettings.Values[key] ?? defaultValue;
        }

        public static void SetValue(string key, object value)
        {
            localSettings.Values[key] = value;
        }

        public static void Clear(string key)
        {
            localSettings.Values.Remove(key);
        }

        public static void ClearAll()
        {
            localSettings.Values.Clear();
        }
    }
}
