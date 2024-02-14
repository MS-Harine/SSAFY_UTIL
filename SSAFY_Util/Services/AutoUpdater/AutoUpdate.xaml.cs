using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SSAFY_Util.Services.AutoUpdater
{
    /// <summary>
    /// AutoUpdate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AutoUpdate : Window
    {
        ObservableCollection<Log> logs = new();
        public AutoUpdate()
        {
            InitializeComponent();
            LogEntryList.ItemsSource = logs;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logs.Add(new Log("Start Update"));
        }
    }

    internal class Log(string message)
    {
        public string Timestamp { get; set; } = DateTime.Now.ToString();
        public string Message { get; set; } = message;

        public override string ToString()
        {
            return Timestamp + " - " + Message;
        }
    }
}
