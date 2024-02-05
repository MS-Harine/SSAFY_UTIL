using System.Windows;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using Brush = System.Windows.Media.Brush;

namespace SSAFY_Util
{
    public partial class MainWindow : Window
    {
        private string email = "";
        private string password = "";

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

            LoginWindowSetup(true);
            UpdateCheckInOutTime();
            if (AutoLoginCheckBox.IsChecked == true)
            {
                SetAutoLogin(email, pw);
            }
            this.email = email;
            this.password = pw;
        }

        private void LogoutBtn_Click(object obj, RoutedEventArgs e)
        {
            service.Logout();
            ResetAutoLogin();
            LoginWindowSetup(false);
        }

        private void CheckInBtn_Click(object obj, RoutedEventArgs e)
        {
            string? result = service.CheckIn(email, password);
            CheckInText.Text = result == null ? "Error" : result.ToString();
        }

        private void CheckOutBtn_Click(object sender, RoutedEventArgs e)
        {
            string? result = service.CheckOut(email, password);
            CheckOutText.Text = result == null ? "Error" : result.ToString();
        }

        private void ToggleContent(object sender, RoutedEventArgs e)
        {
            isOpen = !isOpen;
            if (isOpen) openAnimation.Begin();
            else closeAnimation.Begin();
        }
    }
}
