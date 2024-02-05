using System.Windows;

namespace SSAFY_Util
{
    public partial class MainWindow : Window
    {
        private void SetAutoLogin(string id, string pw)
        {
            Properties.Settings.Default.ID = id;
            Properties.Settings.Default.PW = pw;
            Properties.Settings.Default.Save();
        }

        private void ResetAutoLogin()
        {
            Properties.Settings.Default.ID = null;
            Properties.Settings.Default.PW = null;
            Properties.Settings.Default.Save();
        }

        private bool AutoLogin()
        {
            string id = Properties.Settings.Default.ID;
            string pw = Properties.Settings.Default.PW;
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            return service.Login(id, pw);
        }
    }
}
