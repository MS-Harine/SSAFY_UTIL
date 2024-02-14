using System.Windows;
using System.Windows.Media.Animation;
using SSAFY_Util.Utils;
using SSAFY_Util.Services.Crawler;

namespace SSAFY_Util
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon notifyIcon = new();
        private readonly ContextMenuStrip contextMenu = new();
        private readonly SSAFYWeb service = new();

        private readonly Storyboard closeAnimation = new();
        private readonly Storyboard openAnimation = new();
        private readonly string appName = "SSAFY_UTIL";
        private bool isOpen = true;

        private bool isLogined = false;

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetLocationAndShape();
            SetupNotifyIcon();
            SetupAnimation();
            if (!CheckStartUp(appName))
            {
                SetStartUp(appName);
            }

            if (AutoLogin())
            {
                isLogined = true;
                UpdateCheckInOutTime();
            }
            LoginWindowSetup(isLogined);
        }

        private void SetLocationAndShape()
        {
            this.Height = SystemParameters.WorkArea.Height;
            this.Left = SystemParameters.WorkArea.Width - ContentGrid.ActualWidth - ToggleGrid.ActualWidth;
            this.Top = SystemParameters.WorkArea.Top;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            ContentGrid.Height = this.Height;
        }

        private void SetupNotifyIcon()
        {
            ToolStripMenuItem quitItem = new();
            quitItem.Text = "Exit";
            quitItem.Click += (s, e) =>
            {
                service.QuitDriver();
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                App.Current.Shutdown();
            };

            // contextMenu.Items.Add(settingItem);
            contextMenu.Items.Add(quitItem);

            notifyIcon.Icon = new Icon(Common.GetPath(@"logo.ico"));
            notifyIcon.Visible = true;
            notifyIcon.Text = "SSAFY";
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void SetStartUp(string appName)
        {
            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            Microsoft.Win32.RegistryKey? startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey);

            startupKey?.Close();
            startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey, true);
            startupKey?.SetValue(appName, System.Windows.Forms.Application.ExecutablePath.ToString());
            startupKey?.Close();
        }

        private bool CheckStartUp(string appName)
        {
            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            Microsoft.Win32.RegistryKey? startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey);
            if (startupKey?.GetValue(appName) == null)
                return false;
            else
                return true;
        }

        private void ToggleContent(object sender, RoutedEventArgs e)
        {
            isOpen = !isOpen;
            if (isOpen) openAnimation.Begin();
            else closeAnimation.Begin();
        }

        private void SetupAnimation()
        {
            DoubleAnimation contentCloseAnimation = new()
            {
                From = ContentGrid.ActualWidth,
                To = 0,
                BeginTime = new TimeSpan() + TimeSpan.FromMilliseconds(1000),
                Duration = new Duration(TimeSpan.FromMilliseconds(100)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };
            DoubleAnimation contentOpenAnimation = new()
            {
                From = 0,
                To = ContentGrid.ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(100)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };

            DoubleAnimation windowCloseAnimation = new()
            {
                From = Left,
                To = Left + ContentGrid.ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };
            DoubleAnimation windowOpenAnimation = new()
            {
                From = Left + ContentGrid.ActualWidth,
                To = Left,
                BeginTime = new TimeSpan() + TimeSpan.FromMilliseconds(100),
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };

            closeAnimation.Children.Add(contentCloseAnimation);
            closeAnimation.Children.Add(windowCloseAnimation);
            Storyboard.SetTarget(contentCloseAnimation, ContentGrid);
            Storyboard.SetTargetProperty(contentCloseAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTarget(windowCloseAnimation, this);
            Storyboard.SetTargetProperty(windowCloseAnimation, new PropertyPath(LeftProperty));

            openAnimation.Children.Add(contentOpenAnimation);
            openAnimation.Children.Add(windowOpenAnimation);
            Storyboard.SetTarget(contentOpenAnimation, ContentGrid);
            Storyboard.SetTargetProperty(contentOpenAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTarget(windowOpenAnimation, this);
            Storyboard.SetTargetProperty(windowOpenAnimation, new PropertyPath(LeftProperty));
        }
    }
}