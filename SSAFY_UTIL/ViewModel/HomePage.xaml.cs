using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public HomePage()
        {
            InitializeComponent();
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
