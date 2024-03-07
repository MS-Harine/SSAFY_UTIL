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
        private WebSsafy WebHelper = WebSsafy.Instance;
        private LoginInfo LoginHelper = LoginInfo.Instance;
        private ContentDialog LoginDialog = new();
        private LoginForm LoginDialogContent = new();

        private readonly string USERINFO_KEY = "studentInfo";

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

            if (LoginHelper.IsDataExist)
            {
                var (id, pw) = LoginHelper.GetLoginInfo();
                loginResult = await WebHelper.Login(id, pw);
            }

            while (!loginResult)
            {
                ContentDialogResult result = await OpenLoginDialog(LoginDialogContent, "EDU SSAFY �α���", "Login");
                if (result == ContentDialogResult.Primary)
                {
                    var (id, pw, autoLogin) = LoginDialogContent.GetInfo();
                    if (LoginDialogContent.IsValid())
                    {
                        loginResult = await WebHelper.Login(id, pw);
                        if (loginResult && autoLogin)
                            LoginHelper.SetLoginInfo(id, pw);

                        LoginDialogContent.SetValidation(loginResult);
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
                LoginHelper.Clear();
                SetUserInfoUI(false);
                LocalStorage.Clear(USERINFO_KEY);
            }
        }

        private async void SetUserInfoUI(bool isLogined)
        {
            if (isLogined)
            {
                string? infoString = (string?)LocalStorage.GetValue(USERINFO_KEY);
                JObject? info = infoString == null ? null : JObject.Parse(infoString);
                if (info == null || (DateTime.Now - info["LastTime"].ToObject<DateTime>()).TotalMinutes > 10)
                {
                    info = await WebHelper.GetStudentInfo();
                    info.Add("LastTime", DateTime.Now);
                    LocalStorage.SetValue(USERINFO_KEY, info.ToString());
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
                StudentName.Text = "�����";
                StudentNumber.Text = "1100001";
                StudentLocation.Text = "����";
                StudentClass.Text = "1��";
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
