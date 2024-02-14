using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace SSAFY_Util.Services.AutoUpdater
{
    internal static class AutoUpdater
    {
        private static readonly Uri BaseURI = new("https://github.com/MS-Harine/SSAFY_UTIL/releases/latest/download");
        private static readonly string VersionFile = "version.json";
        private static readonly string UpdateFile = "ssafy_util.zip";
        private static readonly string UpdatePathName = "ssafy_util";

        public static async Task<bool> CheckUpdate()
        {
            bool result = false;
            using (HttpClient client = new())
            {
                string versionJson = await client.GetStringAsync(new Uri(BaseURI, VersionFile));
                Version? versionInfo = JsonSerializer.Deserialize<Version>(versionJson);
                Version? current = Assembly.GetExecutingAssembly().GetName().Version;

                if (current?.CompareTo(versionInfo) < 0)
                    result = true;
            }
            return result;
        }

        public static async Task<Process> Update(Action<object?, System.EventArgs> callback)
        {
            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, UpdateFile);
            string unzipPath = Path.Combine(tempPath, UpdatePathName);
            string runnableFile = Path.Combine(tempPath, UpdatePathName, "setup.exe");

            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(BaseURI, UpdateFile));

                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                using (FileStream fs = new(filePath, FileMode.OpenOrCreate))
                {
                    await streamToReadFrom.CopyToAsync(fs);
                }
            }

            ZipFile.ExtractToDirectory(filePath, unzipPath);

            Process process = new();
            process.StartInfo.FileName = runnableFile;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(callback);
            process.Start();
            return process;
        }
    }
}
