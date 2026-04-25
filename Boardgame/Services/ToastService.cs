using Boardgame.Model;
using Boardgame.View;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Boardgame.Services
{
    public static class ToastService
    {
        private static int _activeCount;
        private static readonly object _lock = new();

        public static void Show(Notification notification)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int index;
                lock (_lock) { index = _activeCount++; }

                var toast = new ToastWindow(notification, index);
                toast.Closed += (_, _) => { lock (_lock) { _activeCount--; } };
                toast.Show();
            });
        }

        public static void Show(string title, string message,
            NotificationType type = NotificationType.System)
        {
            Show(new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                CreatedDate = DateTime.Now
            });
        }

        /// <summary>Shows multiple notifications with a short delay between each.</summary>
        public static void ShowAll(IEnumerable<Notification> notifications)
        {
            int delay = 0;
            foreach (var n in notifications)
            {
                var local = n;
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(delay)
                };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    Show(local);
                };
                timer.Start();
                delay += 600;
            }
        }
    }
}
