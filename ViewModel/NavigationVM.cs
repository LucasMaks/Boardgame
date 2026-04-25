using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    class NavigationVm : ViewModelBase
    {
        // --- Auth ---
        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set { _isAuthenticated = value; OnPropertyChanged(); }
        }

        private UserEntity? _loggedInUser;
        public string LoggedInUserName => _loggedInUser?.DisplayName ?? string.Empty;

        // --- Login VM ---
        public LoginVM LoginVM { get; }

        // --- Current view ---
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // --- Notifications ---
        private ObservableCollection<NotificationEntity> _notifications = [];
        public ObservableCollection<NotificationEntity> Notifications
        {
            get => _notifications;
            set { _notifications = value; OnPropertyChanged(); }
        }

        private int _unreadCount;
        public int UnreadCount
        {
            get => _unreadCount;
            set { _unreadCount = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasUnread)); }
        }
        public bool HasUnread => UnreadCount > 0;

        private bool _isNotifPanelOpen;
        public bool IsNotifPanelOpen
        {
            get => _isNotifPanelOpen;
            set { _isNotifPanelOpen = value; OnPropertyChanged(); }
        }

        // --- Navigation commands ---
        public ICommand TableBoardGamesCommand { get; set; }
        public ICommand MainCommand { get; set; }
        public ICommand StarPageCommand { get; set; }
        public ICommand DrawingsComannd { get; set; }
        public ICommand TournamentsCommand { get; set; }
        public ICommand EventsCommand { get; set; }
        public ICommand CommunityCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand ToggleNotifPanelCommand { get; set; }
        public ICommand MarkAllReadCommand { get; set; }

        private void TableBoardGames(object obj) => CurrentView = new TableBoardGamesVM();
        private void StarPage(object obj) => CurrentView = CreateStarPageVM();
        private void Drawings(object obj) => CurrentView = new DrawingsVM();
        private void Tournaments(object obj) => CurrentView = new TournamentsVM();
        private void Events(object obj) => CurrentView = new EventsVM();
        private void Community(object obj) => CurrentView = new CommunityVM();

        private StarPageVM CreateStarPageVM() => new(
            goCatalog: () => TableBoardGames(null!),
            goDrawing: () => Drawings(null!),
            goTournaments: () => Tournaments(null!),
            goEvents: () => Events(null!),
            goCommunity: () => Community(null!)
        );

        private void Logout(object obj)
        {
            Session.CurrentUser = null;
            _loggedInUser = null;
            OnPropertyChanged(nameof(LoggedInUserName));
            IsAuthenticated = false;
            IsNotifPanelOpen = false;
            Notifications.Clear();
            UnreadCount = 0;
            CurrentView = null;
        }

        private void OnLoginSuccess(UserEntity user)
        {
            _loggedInUser = user;
            Session.CurrentUser = user;
            OnPropertyChanged(nameof(LoggedInUserName));
            IsAuthenticated = true;
            CurrentView = CreateStarPageVM();
            LoadNotifications();
        }

        private async void LoadNotifications()
        {
            if (_loggedInUser is null) return;
            var notifs = await Db.I.GetNotificationsAsync(_loggedInUser.Id);
            Notifications = new ObservableCollection<NotificationEntity>(notifs);
            UnreadCount = notifs.Count(n => !n.IsRead);
        }

        private void ToggleNotifPanel(object obj) => IsNotifPanelOpen = !IsNotifPanelOpen;

        private async void MarkAllRead(object obj)
        {
            if (_loggedInUser is null) return;
            await Db.I.MarkAllReadAsync(_loggedInUser.Id);
            foreach (var n in Notifications) n.IsRead = true;
            UnreadCount = 0;
            IsNotifPanelOpen = false;
        }

        public NavigationVm()
        {
            LoginVM = new LoginVM(OnLoginSuccess);
            MainCommand = new RelayCommand(_ => { });
            TableBoardGamesCommand = new RelayCommand(TableBoardGames);
            StarPageCommand = new RelayCommand(StarPage);
            DrawingsComannd = new RelayCommand(Drawings);
            TournamentsCommand = new RelayCommand(Tournaments);
            EventsCommand = new RelayCommand(Events);
            CommunityCommand = new RelayCommand(Community);
            LogoutCommand = new RelayCommand(Logout);
            ToggleNotifPanelCommand = new RelayCommand(ToggleNotifPanel);
            MarkAllReadCommand = new RelayCommand(MarkAllRead);
        }
    }
}
