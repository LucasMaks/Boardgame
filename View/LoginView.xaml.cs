using Boardgame.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Boardgame.View
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginVM vm)
                vm.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginVM vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null) w.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w == null) return;
            w.WindowState = w.WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (e.ClickCount == 2)
            {
                w!.WindowState = w.WindowState == WindowState.Normal
                    ? WindowState.Maximized
                    : WindowState.Normal;
            }
            else
            {
                w?.DragMove();
            }
        }
    }
}
