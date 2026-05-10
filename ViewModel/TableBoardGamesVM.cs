using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class TableBoardGamesVM : ViewModelBase
    {
        private ObservableCollection<BoardGameEntity> _boardGames = [];
        public ObservableCollection<BoardGameEntity> BoardGames
        {
            get => _boardGames;
            set { _boardGames = value; OnPropertyChanged(); }
        }

        private BoardGameEntity? _selectedGame;
        public BoardGameEntity? SelectedGame
        {
            get => _selectedGame;
            set { _selectedGame = value; OnPropertyChanged(); }
        }

        // Add form
        private string _newTitle = string.Empty;
        public string NewTitle { get => _newTitle; set { _newTitle = value; OnPropertyChanged(); } }
        private string _newDescription = string.Empty;
        public string NewDescription { get => _newDescription; set { _newDescription = value; OnPropertyChanged(); } }
        private int _newMinPlayers = 2;
        public int NewMinPlayers { get => _newMinPlayers; set { _newMinPlayers = value; OnPropertyChanged(); } }
        private int _newMaxPlayers = 4;
        public int NewMaxPlayers { get => _newMaxPlayers; set { _newMaxPlayers = value; OnPropertyChanged(); } }
        private int _newDuration = 60;
        public int NewDuration { get => _newDuration; set { _newDuration = value; OnPropertyChanged(); } }
        private string _newDifficulty = "Medium";
        public string NewDifficulty { get => _newDifficulty; set { _newDifficulty = value; OnPropertyChanged(); } }
        private bool _newIsShared;
        public bool NewIsShared { get => _newIsShared; set { _newIsShared = value; OnPropertyChanged(); } }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand AddGameCommand { get; }
        public ICommand DeleteGameCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAddFormCommand { get; }

        private bool _isAddFormOpen;
        public bool IsAddFormOpen
        {
            get => _isAddFormOpen;
            set { _isAddFormOpen = value; OnPropertyChanged(); }
        }

        public TableBoardGamesVM()
        {
            AddGameCommand = new RelayCommand(AddGame, _ => !string.IsNullOrWhiteSpace(NewTitle));
            DeleteGameCommand = new RelayCommand(DeleteGame, _ => SelectedGame is not null);
            RefreshCommand = new RelayCommand(_ => LoadGames());
            ToggleAddFormCommand = new RelayCommand(_ => IsAddFormOpen = !IsAddFormOpen);
            LoadGames();
        }

        private async void LoadGames()
        {
            try
            {
                var games = await Db.I.GetMyGamesAsync(Session.CurrentUserId);
                BoardGames = new ObservableCollection<BoardGameEntity>(games);
                StatusMessage = $"Zaladowano {games.Count} gier.";
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void AddGame(object? _)
        {
            try
            {
                var game = await Db.I.AddGameAsync(
                    Session.CurrentUserId,
                    NewTitle, NewDescription,
                    NewMinPlayers, NewMaxPlayers,
                    NewDuration, NewDifficulty, NewIsShared);

                BoardGames.Add(game);
                StatusMessage = $"Dodano \"{game.Title}\".";
                IsAddFormOpen = false;

                // Clear form
                NewTitle = string.Empty;
                NewDescription = string.Empty;
                NewMinPlayers = 2;
                NewMaxPlayers = 4;
                NewDuration = 60;
                NewDifficulty = "Medium";
                NewIsShared = false;
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void DeleteGame(object? _)
        {
            if (SelectedGame is null) return;
            try
            {
                await Db.I.DeleteGameAsync(SelectedGame.Id, Session.CurrentUserId);
                BoardGames.Remove(SelectedGame);
                SelectedGame = null;
                StatusMessage = "Gra usunieta.";
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }
    }
}
