using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SSAFY_UTIL.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;

namespace SSAFY_UTIL.Service
{
    public static class ExceptionHandler
    {
        public static async void ErrorMessage(string message)
        {
            HomePage page = (Application.Current as App).Window.Content as HomePage;
            ContentDialog content = new()
            {
                XamlRoot = page.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Error",
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                Content = message
            };
            await content.ShowAsync();
        }
    }
}
