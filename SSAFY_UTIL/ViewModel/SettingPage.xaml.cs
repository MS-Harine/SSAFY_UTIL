using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SSAFY_UTIL.Model;
using System;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        Settings settings = Settings.Instance;

        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void RunOnStartupSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            settings.RunOnStartup = toggleSwitch.IsOn;
        }

        private async void DeleteCacheButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "삭제 확인",
                PrimaryButtonText = "삭제",
                CloseButtonText = "취소",
                DefaultButton = ContentDialogButton.Close,
                Content = new TextBlock()
            };
            ((TextBlock)deleteDialog.Content).Text = "정말로 삭제하시겠습니까?";

            var result = await deleteDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                settings.ClearSettings();

                ContentDialog okDialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "삭제 완료",
                    PrimaryButtonText = "확인",
                    DefaultButton = ContentDialogButton.Primary,
                    Content = new TextBlock()
                };
                ((TextBlock)okDialog.Content).Text = "삭제되었습니다.";
                await okDialog.ShowAsync();
                RunOnStartupSwitch.IsOn = false;
            }
        }
    }
}
