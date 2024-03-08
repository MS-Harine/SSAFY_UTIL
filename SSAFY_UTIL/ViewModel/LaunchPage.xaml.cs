using Microsoft.UI.Xaml.Controls;
using System;

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchPage : Page
    {
        public LaunchPage()
        {
            this.InitializeComponent();
            SelectedDate.Date = DateTime.Now;
            SelectedDate.MinDate = DateTime.Now.AddDays(-7);
            SelectedDate.MaxDate = DateTime.Now.AddDays(7);
        }
    }
}
