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

namespace SSAFY_UTIL.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginForm : Page
    {
        public LoginForm()
        {
            this.InitializeComponent();
        }

        public bool IsValid()
        {
            string id = LoginId.Text;
            string pw = LoginPw.Password;

            bool result = true;
            if (String.IsNullOrEmpty(id))
            {
                LoginIdWarning.Visibility = Visibility.Visible;
                LoginIdWarning.Text = "���̵� �Է����ּ���.";
                result = false;
            }
            else
            {
                LoginIdWarning.Visibility = Visibility.Collapsed;
            }

            if (String.IsNullOrEmpty(pw))
            {
                LoginPwWarning.Visibility = Visibility.Visible;
                LoginPwWarning.Text = "��й�ȣ�� �Է����ּ���.";
                result = false;
            }
            else
            {
                LoginPwWarning.Visibility = Visibility.Collapsed;
            }

            return result;
        }

        public void SetValidation(bool isValid)
        {
            if (isValid)
            {
                LoginWarning.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginWarning.Visibility = Visibility.Visible;
                LoginWarning.Text = "�α��� ������ ��ġ���� �ʽ��ϴ�.";
            }
        }

        public (string, string, bool) GetInfo()
        {
            string id = LoginId.Text;
            string pw = LoginPw.Password;
            bool auto = AutoLogin.IsChecked ?? false;
            return (id, pw, auto);
        }

        public void ClearInputs()
        {
            LoginId.Text = "";
            LoginPw.Password = "";
            AutoLogin.IsChecked = false;
        }
    }
}
