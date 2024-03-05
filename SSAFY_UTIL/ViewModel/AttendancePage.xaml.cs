using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SSAFY_UTIL.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AttendancePage : Page
    {
        private WebSsafy WebHelper = WebSsafy.Instance;

        public AttendancePage()
        {
            this.InitializeComponent();
            this.Loaded += async (sender, e) =>
            {
                if (!WebHelper.IsLogin)
                {
                    ContentDialog dialog = new()
                    {
                        XamlRoot = this.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        Title = "Login",
                        PrimaryButtonText = "OK",
                        DefaultButton = ContentDialogButton.Primary,
                        Content = "로그인 후 이용해주십시오"
                    };
                    await dialog.ShowAsync();

                    var homepage = (Application.Current as App)?.Window.Content as HomePage;
                    homepage.NavigateTo(typeof(AccountPage));
                }
            };
        }

        private void SetRoomInTime()
        {

        }
    }
}
