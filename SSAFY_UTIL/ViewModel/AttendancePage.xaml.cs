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
        private UserInfo UserModel = UserInfo.Instance;

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
                else
                {
                    InitializeUI();
                }
            };
        }

        private async void InitializeUI()
        {
            Today.Text = DateTime.Now.ToString("M월 d일 dddd");
            JObject? cache = UserModel.GetUserAttendance();

            string roomInTime = String.Empty, roomInText = String.Empty;
            string roomOutTime = String.Empty, roomOutText = String.Empty;
            string expectedPay = String.Empty;

            if (cache == null)
            {
                (roomInTime, roomInText) = await WebHelper.CheckInTime();
                if (roomInTime != String.Empty)
                {
                    (roomOutTime, roomOutText) = await WebHelper.CheckOutTime();
                }
                expectedPay = await WebHelper.GetExpectedPay(DateTime.Now.Month);
                JObject attendance = await WebHelper.GetAttendance();

                cache = new()
                {
                    { "roomInTime", roomInTime },
                    { "roomInText", roomInText },
                    { "roomOutTime", roomOutTime },
                    { "roomOutText", roomOutText },
                    { "expectedPay", expectedPay },
                    { "AttendanceCount", attendance["AttendanceCount"] },
                    { "AttendanceNormal", attendance["AttendanceNormal"] },
                    { "AttendanceTardy", attendance["AttendanceTardy"] },
                    { "AttendanceLeaveEarly", attendance["AttendanceLeaveEarly"] },
                    { "AttendanceOuting", attendance["AttendanceOuting"] },
                    { "AttendanceAbsentCount", attendance["AttendanceAbsentCount"] },
                    { "AttendanceCertifiedAbsent", attendance["AttendanceCertifiedAbsent"] },
                    { "AttendanceResonableAbsent", attendance["AttendanceResonableAbsent"] },
                    { "AttendanceAbsent", attendance["AttendanceAbsent"] },
                };
                UserModel.SetUserAttendance(cache);
            }
            else
            {
                roomInTime = cache["roomInTime"].ToString();
                roomInText = cache["roomInText"].ToString();
                roomOutTime = cache["roomOutTime"].ToString();
                roomOutText = cache["roomOutText"].ToString();
                expectedPay = cache["expectedPay"].ToString();
            }

            if (roomInTime != String.Empty)
            {
                RoomInText.Text = roomInText;
                RoomInTime.Text = roomInTime;
                RoomInButton.IsEnabled = false;
                RoomOutButton.IsEnabled = true;
            }
            else
            {
                RoomInButton.IsEnabled = true;
                RoomOutButton.IsEnabled = false;
            }

            if (roomOutTime != String.Empty)
            {
                RoomOutText.Text = roomOutText;
                RoomOutTime.Text = roomOutTime;
            }
            ExpectedPay.Text = expectedPay;

            AttendanceCount.Text = cache["AttendanceCount"].ToString();
            AttendanceNormal.Text = cache["AttendanceNormal"].ToString() + "일";
            AttendanceTardy.Text = cache["AttendanceTardy"].ToString() + "일";
            AttendanceLeaveEarly.Text = cache["AttendanceLeaveEarly"].ToString() + "일";
            AttendanceOuting.Text = cache["AttendanceOuting"].ToString() + "일";
            AttendanceAbsentCount.Text = cache["AttendanceAbsentCount"].ToString();
            AttendanceCertifiedAbsent.Text = cache["AttendanceCertifiedAbsent"].ToString() + "일";
            AttendanceResonableAbsent.Text = cache["AttendanceResonableAbsent"].ToString() + "일";
            AttendanceAbsent.Text = cache["AttendanceAbsent"].ToString() + "일";
        }

        private async void RoomInButtonClick(object sender, RoutedEventArgs args)
        {
            bool result = await WebHelper.CheckIn();
            if (!result)
                return;

            var (roomInTime, roomInText) = await WebHelper.CheckInTime();
            JObject cache = UserModel.GetUserAttendance() ?? new();
            cache["roomInTime"] = roomInTime;
            cache["roomInText"] = roomInText;
            UserModel.SetUserAttendance(cache);

            RoomInText.Text = roomInText;
            RoomInTime.Text = roomInTime;
        }

        private async void RoomOutButtonClick(object sender, RoutedEventArgs args)
        {
            bool result = await WebHelper.CheckOut();
            if (!result)
                return;

            var (roomOutTime, roomOutText) = await WebHelper.CheckOutTime();
            JObject cache = UserModel.GetUserAttendance() ?? new();
            cache["roomOutTime"] = roomOutTime;
            cache["roomOutText"] = roomOutText;
            UserModel.SetUserAttendance(cache);

            RoomOutText.Text = roomOutText;
            RoomOutTime.Text = roomOutTime;
        }
    }
}
