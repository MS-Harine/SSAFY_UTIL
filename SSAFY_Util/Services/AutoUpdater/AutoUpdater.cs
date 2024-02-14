using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace SSAFY_Util.Services.AutoUpdater
{
    public partial class AutoUpdate : Window
    {
        private static readonly Uri BaseURI = new("https://github.com/MS-Harine/SSAFY_UTIL/releases/latest/download/");
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

        public async void Update(Action<object?, System.EventArgs>? callback)
        {
            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, UpdateFile);
            string unzipPath = Path.Combine(tempPath, UpdatePathName);
            string runnableFile = Path.Combine(tempPath, UpdatePathName, "setup.exe");

            Thread thread = new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;

                using (HttpClient client = new())
                {
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        logs.Add(new Log("Download update file: " + UpdateFile));
                    }));
                    HttpResponseMessage response = await client.GetAsync(new Uri(BaseURI, UpdateFile));

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    using (FileStream fs = new(filePath, FileMode.OpenOrCreate))
                    {
                        await streamToReadFrom.CopyToAsync(fs);
                    }
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        logs.Add(new Log("Download Complete"));
                    }));
                }

                DispatcherService.Invoke((System.Action)(() =>
                {
                    logs.Add(new Log("Unzip the file: " + UpdateFile));
                }));
                if (Directory.Exists(unzipPath))
                {
                    Directory.Delete(unzipPath, true);
                }

                Process unzip7z = new();
                string args = "x -y \"-o" + unzipPath + "\" \"" + filePath + "\" ";
                unzip7z.StartInfo.FileName = @"Services\7z\7za.exe";
                unzip7z.StartInfo.Arguments = args;
                unzip7z.Start();
                unzip7z.WaitForExit();
                if (unzip7z.ExitCode != 0)
                {
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        logs.Add(new Log($"Failed to Unzip with code {unzip7z.ExitCode}"));
                    }));
                    return;
                }

                DispatcherService.Invoke((System.Action)(() =>
                {
                    logs.Add(new Log("Unzip Complete"));
                    logs.Add(new Log("Update & Restart"));
                }));

                Process process = new();
                process.StartInfo.FileName = runnableFile;
                process.EnableRaisingEvents = true;
                if (callback != null)
                    process.Exited += new EventHandler(callback);
                process.Start();
            });

            await Task.Run(() =>
            {
                thread.Start();
                thread.Join();
            });
        }
    }
}
