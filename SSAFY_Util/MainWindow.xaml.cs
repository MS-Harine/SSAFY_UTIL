using System.Windows;
using System.Windows.Media.Animation;
using SSAFY_Util.Utils;

namespace SSAFY_Util
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon notifyIcon = new();
        private readonly ContextMenuStrip contextMenu = new();
        private readonly WebService service = new();

        private Storyboard closeAnimation = new();
        private Storyboard openAnimation = new();
        private bool isOpen = true;

        private bool isLogined = false;

        public MainWindow()
        {
            InitializeComponent();
            if (AutoLogin())
            {
                isLogined = true;
                UpdateCheckInOutTime();
            }
            LoginWindowSetup(isLogined);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetLocationAndShape();
            SetupNotifyIcon();
            SetupAnimation();
        }

        private void LoginWindowSetup(bool isLogin)
        {
            if (isLogin)
            {
                LoginForm.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;
                ContentGrid.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                LoginForm.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                ContentGrid.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        private void UpdateCheckInOutTime()
        {
            string? CheckInTime = service.GetCheckInTime();
            if (CheckInTime != null)
            {
                CheckInText.Text = CheckInTime;
            }
            string? CheckOutTime = service.GetCheckOutTime();
            if (CheckInTime != null)
            {
                CheckOutText.Text = CheckOutTime;
            }
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
                App.Current.Shutdown();
                notifyIcon.Dispose();
            };

            ToolStripMenuItem settingItem = new();
            settingItem.Text = "Setting";
            settingItem.Click += (s, e) =>
            {
                
            };

            contextMenu.Items.Add(settingItem);
            contextMenu.Items.Add(quitItem);

            notifyIcon.Icon = new Icon(Common.GetPath(@"assets\\logo.ico"));
            notifyIcon.Visible = true;
            notifyIcon.Text = "SSAFY";
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void SetupAnimation()
        {
            DoubleAnimation contentCloseAnimation = new()
            {
                From = ContentGrid.ActualWidth,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };
            DoubleAnimation contentOpenAnimation = new()
            {
                From = 0,
                To = ContentGrid.ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };

            DoubleAnimation windowCloseAnimation = new()
            {
                From = Left,
                To = Left + ContentGrid.ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                },
            };
            DoubleAnimation windowOpenAnimation = new()
            {
                From = Left + ContentGrid.ActualWidth,
                To = Left,
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
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