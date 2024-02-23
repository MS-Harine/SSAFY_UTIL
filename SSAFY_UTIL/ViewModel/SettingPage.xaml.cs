using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SSAFY_UTIL.Model;
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
                Title = "���� Ȯ��",
                PrimaryButtonText = "����",
                CloseButtonText = "���",
                DefaultButton = ContentDialogButton.Close,
                Content = new TextBlock()
            };
            ((TextBlock)deleteDialog.Content).Text = "������ �����Ͻðڽ��ϱ�?";

            var result = await deleteDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                settings.ClearSettings();

                ContentDialog okDialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "���� �Ϸ�",
                    PrimaryButtonText = "Ȯ��",
                    DefaultButton = ContentDialogButton.Primary,
                    Content = new TextBlock()
                };
                ((TextBlock)okDialog.Content).Text = "�����Ǿ����ϴ�.";
                await okDialog.ShowAsync();
                RunOnStartupSwitch.IsOn = false;
            }
        }
    }
}
