using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SSAFY_UTIL.View.Component
{
    public sealed partial class SettingItem : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(SettingItem), new PropertyMetadata(null));
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(String), typeof(SettingItem), new PropertyMetadata(null));
        public static readonly DependencyProperty ActionContentProperty =
            DependencyProperty.Register("ActionContent", typeof(object), typeof(SettingItem), new PropertyMetadata(null));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
        public object ActionContent
        {
            get { return GetValue(ActionContentProperty); }
            set { SetValue(ActionContentProperty, value); }
        }

        public SettingItem()
        {
            this.InitializeComponent();
        }
    }
}
