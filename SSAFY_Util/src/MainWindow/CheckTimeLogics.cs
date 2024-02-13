using System.Windows;

namespace SSAFY_Util
{
    public partial class MainWindow : Window
    {
        private void UpdateCheckInOutTime()
        {
            string? CheckInTime = service.GetCheckTime(true);
            if (CheckInTime != null)
            {
                CheckInText.Text = CheckInTime;
            }
            string? CheckOutTime = service.GetCheckTime(false);
            if (CheckInTime != null)
            {
                CheckOutText.Text = CheckOutTime;
            }
        }

        private void CheckInBtn_Click(object obj, RoutedEventArgs e)
        {
            string? result = null;
            if (service.CheckAttendance(true))
                result = service.GetCheckTime(true);
            CheckInText.Text = result?.ToString() ?? "Error";
        }

        private void CheckOutBtn_Click(object sender, RoutedEventArgs e)
        {
            string? result = null;
            if (service.CheckAttendance(false))
                result = service.GetCheckTime(false);
            CheckOutText.Text = result?.ToString() ?? "Error";
        }
    }
}
