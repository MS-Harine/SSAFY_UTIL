using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Model;
using SSAFY_UTIL.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        private HomePage window = ((Application.Current as App)?.Window.Content as HomePage);
        private ContentDialog LoginDialog = new();
        private LoginForm LoginDialogContent = new();

        private WebSsafy WebHelper = WebSsafy.Instance;
        private LoginInfo LoginModel = LoginInfo.Instance;
        private UserInfo UserModel = UserInfo.Instance;

        public AccountPage()
        {
            this.InitializeComponent();
            this.Loaded += Login;
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            bool loginResult = false;

            if (WebHelper.IsLogin)
            {
                SetUserInfoUI(true);
                return;
            }

            if (LoginModel.IsDataExist)
            {
                var (id, pw) = LoginModel.GetLoginInfo();
                loginResult = await WebHelper.Login(id, pw);
            }

            while (!loginResult)
            {
                ContentDialogResult result = await OpenLoginDialog(LoginDialogContent, "EDU SSAFY ·Î±×ÀÎ", "Login");
                if (result == ContentDialogResult.Primary)
                {
                    var (id, pw, autoLogin) = LoginDialogContent.GetInfo();
                    if (LoginDialogContent.IsValid())
                    {
                        await window.LoadingTask(async () =>
                        {
                            loginResult = await WebHelper.Login(id, pw);
                            if (loginResult && autoLogin)
                                LoginModel.SetLoginInfo(id, pw);

                            LoginDialogContent.SetValidation(loginResult);
                        });
                    }
                }
                else
                    break;
            }

            if (!loginResult)
                return;

            SetUserInfoUI(true);
        }

        private async void Logout(object sender, RoutedEventArgs e)
        {
            bool result = await WebHelper.Logout();
            if (result)
            {
                LoginModel.Clear();
                UserModel.Clear();
                SetUserInfoUI(false);
            }
        }

        private async void SetUserInfoUI(bool isLogined)
        {
            if (isLogined)
            {
                JObject? info = UserModel.GetUserInfo();
                if (info == null)
                {
                    info = await WebHelper.GetStudentInfo();
                    UserModel.SetUserInfo(info);
                }
                
                StudentName.Text = info["Name"].ToString();
                StudentNumber.Text = info["Number"].ToString();
                StudentLocation.Text = info["Location"].ToString();
                StudentClass.Text = info["Class"].ToString();
                StudentMileage.Text = info["Mileage"].ToString();
                StudentLevel.Text = info["Level"].ToString();
                StudentLevelPercent.Text = info["LevelPoint"].ToString();

                LoginButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
                LoginDialogContent.ClearInputs();
            }
            else
            {
                StudentName.Text = "±è½ÎÇÇ";
                StudentNumber.Text = "1100001";
                StudentLocation.Text = "¼­¿ï";
                StudentClass.Text = "1¹Ý";
                StudentMileage.Text = "0M";
                StudentLevel.Text = "Bronze 1";
                StudentLevelPercent.Text = "0EXP";

                LoginButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;
            }
        }

        private async Task<ContentDialogResult> OpenLoginDialog(Page content, String title, String buttonText)
        {
            if (LoginDialog.Content == null)
            {
                LoginDialog.XamlRoot = this.XamlRoot;
                LoginDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                LoginDialog.Title = title;
                LoginDialog.PrimaryButtonText = buttonText;
                LoginDialog.CloseButtonText = "Exit";
                LoginDialog.DefaultButton = ContentDialogButton.Primary;
                LoginDialog.Content = content;
            }

            var result = await LoginDialog.ShowAsync();
            return result;
        }
    }
}
