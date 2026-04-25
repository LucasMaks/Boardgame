using Boardgame.Model;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Boardgame.View
{
    public partial class ToastWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly int _stackIndex;
        private const double ToastHeight = 86;
        private const double Margin = 12;

        public ToastWindow(Notification notification, int stackIndex = 0)
        {
            InitializeComponent();
            _stackIndex = stackIndex;

            TitleText.Text = notification.Title;
            MessageText.Text = notification.Message;
            TimeText.Text = notification.CreatedDate.ToString("HH:mm  dd.MM.yyyy");

            ApplyTypeStyle(notification.Type);
            PositionWindow();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _timer.Tick += (_, _) => StartFadeOut();
            _timer.Start();

            Loaded += (_, _) =>
            {
                ((Storyboard)Resources["SlideIn"]).Begin(this);
                AnimateProgressBar();
            };
        }

        private void ApplyTypeStyle(NotificationType type)
        {
            var (color, icon, bgColor) = type switch
            {
                NotificationType.Welcome => ("#FFC047", FontAwesome.Sharp.IconChar.Star, "#3A2800"),
                NotificationType.AddedToEvent => ("#78A3FC", FontAwesome.Sharp.IconChar.CalendarDays, "#0A1A3A"),
                NotificationType.AddedToTournament => ("#FFC047", FontAwesome.Sharp.IconChar.Trophy, "#3A2800"),
                NotificationType.ExchangeOffer => ("#4ADAEC", FontAwesome.Sharp.IconChar.ArrowRightArrowLeft, "#0A2A2A"),
                _ => ("#9B59B6", FontAwesome.Sharp.IconChar.BellConcierge, "#1A0A2A")
            };

            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            AccentBar.Background = brush;
            IconCircle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
            IconImg.Icon = icon;
            IconImg.Foreground = brush;
            ProgressBar.Background = brush;
        }

        private void PositionWindow()
        {
            var workArea = SystemParameters.WorkArea;
            double offsetY = (_stackIndex * (ToastHeight + Margin));
            Left = workArea.Right - Width - 16;
            Top = workArea.Bottom - ToastHeight - 16 - offsetY;
        }

        private void AnimateProgressBar()
        {
            ProgressBar.Width = Root.ActualWidth > 0 ? Root.ActualWidth : 310;
            var anim = new DoubleAnimation(ProgressBar.Width, 0, new Duration(TimeSpan.FromSeconds(4)));
            ProgressBar.BeginAnimation(WidthProperty, anim);
        }

        private void StartFadeOut()
        {
            _timer.Stop();
            ((Storyboard)Resources["FadeOut"]).Begin(this);
        }

        private void FadeOut_Completed(object sender, EventArgs e) => Close();

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            StartFadeOut();
        }
    }
}
