using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Windows.Forms;

namespace YetiFTPClient
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GetLatestVersion();

            //Show loading screen until initial scan has run then close loading screen
            Loading.ShowLoading();
            MainForm mainForm = new MainForm();
            Loading.CloseForm();
            Application.Run(mainForm);
        }

        private static async void GetLatestVersion()
        {
            HttpClient client = new HttpClient();
            string url = "https://version.yetitool.com/api/version";
            string version;
            Versions versions = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    content = JToken.Parse(content).ToString();

                    versions = JsonConvert.DeserializeObject<Versions>(content);
                    version = versions.Latest;
                }
                else
                {
                    version = Application.ProductVersion;
                }

                var latest = new Version(version);
                var current = new Version(Application.ProductVersion);

                if (latest.CompareTo(current) > 0)
                {
                    Update update = new Update(versions);
                    update.ShowDialog();
                }
            } catch
            {
                return;
            }
        }
    }
}
