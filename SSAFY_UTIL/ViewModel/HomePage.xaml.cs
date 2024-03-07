using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SSAFY_UTIL.Model;
using SSAFY_UTIL.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private LoginInfo LoginHelper = LoginInfo.Instance;
        private WebSsafy WebHelper = WebSsafy.Instance;

        public HomePage()
        {
            InitializeComponent();
            LoadingTask(CheckLogin);
        }

        private async void CheckLogin()
        {
            if (LoginHelper.IsDataExist)
            {
                var (id, pw) = LoginHelper.GetLoginInfo();
                await WebHelper.Login(id, pw);
            }
        }

        public async Task LoadingTask(Func<Task> callback)
        {
            Modal.Visibility = Visibility.Visible;
            await callback();
            Modal.Visibility = Visibility.Collapsed;
        }

        public void LoadingTask(Action callback)
        {
            Modal.Visibility = Visibility.Visible;
            callback();
            Modal.Visibility = Visibility.Collapsed;
        }

        public void NavigateTo(Type pageType)
        {
            bool state = false;
            foreach (var menuItem in NavigationViewControl.MenuItems)
            {
                if (((NavigationViewItem)menuItem).Tag + "Page" == pageType.Name)
                {
                    state = true;
                    NavigationViewControl.SelectedItem = menuItem;
                    break;
                }
            }
            if (!state)
            {
                foreach (var menuItem in NavigationViewControl.FooterMenuItems)
                {
                    if (((NavigationViewItem)menuItem).Tag + "Page" == pageType.Name)
                    {
                        state = true;
                        NavigationViewControl.SelectedItem = menuItem;
                        break;
                    }
                }
            }

            rootFrame.Navigate(pageType);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                rootFrame.Navigate(typeof(SettingPage));
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = ((string)selectedItem.Tag);
                    string pageName = "SSAFY_UTIL.View." + selectedItemTag + "Page";
                    Type pageType = Type.GetType(pageName);
                    rootFrame.Navigate(pageType);
                }
            }
        }
    }
}
