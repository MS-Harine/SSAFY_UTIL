using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace SSAFY_Util
{
    public partial class MainWindow : Window
    {
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginBtn.IsEnabled = false;

            string email = EmailTextBox.Text;
            string pw = PasswordBox.Password;

            bool result = service.Login(email, pw);
            if (!result)
            {
                MessageBox.Show("로그인 정보를 확인해 주십시오", "Login Error");
                LoginBtn.IsEnabled = true;
                return;
            }

            LoginBtn.IsEnabled = true;
            if (AutoLoginCheckBox.IsChecked == true)
            {
                SetAutoLogin(email, pw);
            }
            LoginWindowSetup(true);
            UpdateCheckInOutTime();
            
        }

        private void LogoutBtn_Click(object obj, RoutedEventArgs e)
        {
            service.Logout();
            ResetAutoLogin();
            LoginWindowSetup(false);
        }

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

        private void LoginWindowSetup(bool isLogin)
        {
            if (isLogin)
            {
                LoginForm.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;
                ContentGrid.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                LoginForm.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                ContentGrid.VerticalAlignment = VerticalAlignment.Center;
            }
        }
    }
}
