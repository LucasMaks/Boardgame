using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class CommunityVM : ViewModelBase
    {
        // --- Friends ---
        private ObservableCollection<UserEntity> _friends = [];
        public ObservableCollection<UserEntity> Friends
        {
            get => _friends;
            set { _friends = value; OnPropertyChanged(); }
        }

        private UserEntity? _selectedFriend;
        public UserEntity? SelectedFriend
        {
            get => _selectedFriend;
            set { _selectedFriend = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedFriend)); }
        }
        public bool HasSelectedFriend => SelectedFriend is not null;

        // --- Pending requests ---
        private ObservableCollection<UserEntity> _pendingRequests = [];
        public ObservableCollection<UserEntity> PendingRequests
        {
            get => _pendingRequests;
            set { _pendingRequests = value; OnPropertyChanged(); }
        }

        // --- Community games ---
        private ObservableCollection<BoardGameEntity> _communityGames = [];
        public ObservableCollection<BoardGameEntity> CommunityGames
        {
            get => _communityGames;
            set { _communityGames = value; OnPropertyChanged(); }
        }

        // --- Search ---
        private ObservableCollection<UserEntity> _searchResults = [];
        public ObservableCollection<UserEntity> SearchResults
        {
            get => _searchResults;
            set { _searchResults = value; OnPropertyChanged(); }
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        // --- Add friend by email ---
        private string _friendEmail = string.Empty;
        public string FriendEmail
        {
            get => _friendEmail;
            set { _friendEmail = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        // --- Commands ---
        public ICommand SendFriendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand SearchUsersCommand { get; }
        public ICommand AddToLibraryCommand { get; }

        public CommunityVM()
        {
            SendFriendRequestCommand = new RelayCommand(SendRequest, _ => !string.IsNullOrWhiteSpace(FriendEmail));
            AcceptRequestCommand = new RelayCommand(AcceptRequest);
            DeclineRequestCommand = new RelayCommand(DeclineRequest);
            RemoveFriendCommand = new RelayCommand(RemoveFriend, _ => SelectedFriend is not null);
            SearchUsersCommand = new RelayCommand(SearchUsers, _ => !string.IsNullOrWhiteSpace(SearchQuery));
            AddToLibraryCommand = new RelayCommand(AddToLibrary);
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var friends = await Db.I.GetFriendsAsync(Session.CurrentUserId);
                Friends = new ObservableCollection<UserEntity>(
                    friends.ConvertAll(f => f.Friend));

                var pending = await Db.I.GetPendingRequestsAsync(Session.CurrentUserId);
                PendingRequests = new ObservableCollection<UserEntity>(
                    pending.ConvertAll(p => p.Requester));

                var games = await Db.I.GetCommunityGamesAsync(Session.CurrentUserId);
                CommunityGames = new ObservableCollection<BoardGameEntity>(games);
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void SendRequest(object? _)
        {
            try
            {
                var error = await Db.I.SendFriendRequestAsync(
                    Session.CurrentUserId, FriendEmail, Session.CurrentDisplayName);

                StatusMessage = error ?? "Zaproszenie wyslane!";
                FriendEmail = string.Empty;
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void AcceptRequest(object? param)
        {
            if (param is not UserEntity requester) return;
            await Db.I.RespondToFriendRequestAsync(requester.Id, Session.CurrentUserId, true);
            StatusMessage = $"Zaakceptowano zaproszenie od {requester.DisplayName}.";
            LoadData();
        }

        private async void DeclineRequest(object? param)
        {
            if (param is not UserEntity requester) return;
            await Db.I.RespondToFriendRequestAsync(requester.Id, Session.CurrentUserId, false);
            StatusMessage = "Zaproszenie odrzucone.";
            LoadData();
        }

        private async void RemoveFriend(object? _)
        {
            if (SelectedFriend is null) return;
            await Db.I.RemoveFriendAsync(Session.CurrentUserId, SelectedFriend.Id);
            Friends.Remove(SelectedFriend);
            SelectedFriend = null;
            StatusMessage = "Znajomy usuniety.";
        }

        private async void SearchUsers(object? _)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery)) return;
            var results = await Db.I.SearchUsersAsync(SearchQuery, Session.CurrentUserId);
            SearchResults = new ObservableCollection<UserEntity>(results);
        }

        private async void AddToLibrary(object? param)
        {
            if (param is not BoardGameEntity game) return;
            await Db.I.AddToLibraryAsync(Session.CurrentUserId, game.Id);
            StatusMessage = $"Dodano \"{game.Title}\" do Twojej biblioteki.";
        }
    }
}
