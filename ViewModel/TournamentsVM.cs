using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class TournamentsVM : ViewModelBase
    {
        private ObservableCollection<TournamentEntity> _tournaments = [];
        public ObservableCollection<TournamentEntity> Tournaments
        {
            get => _tournaments;
            set { _tournaments = value; OnPropertyChanged(); }
        }

        private TournamentEntity? _selectedTournament;
        public TournamentEntity? SelectedTournament
        {
            get => _selectedTournament;
            set { _selectedTournament = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedTournament)); RefreshMatchesAndStandings(); }
        }
        public bool HasSelectedTournament => SelectedTournament is not null;

        private ObservableCollection<TournamentMatchEntity> _currentMatches = [];
        public ObservableCollection<TournamentMatchEntity> CurrentMatches
        {
            get => _currentMatches;
            set { _currentMatches = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LeagueStandingEntity> _currentStandings = [];
        public ObservableCollection<LeagueStandingEntity> CurrentStandings
        {
            get => _currentStandings;
            set { _currentStandings = value; OnPropertyChanged(); }
        }

        // Form
        private string _newTournamentName = string.Empty;
        public string NewTournamentName { get => _newTournamentName; set { _newTournamentName = value; OnPropertyChanged(); } }
        private string _newGameTitle = string.Empty;
        public string NewGameTitle { get => _newGameTitle; set { _newGameTitle = value; OnPropertyChanged(); } }
        private string _newPlayersText = string.Empty;
        public string NewPlayersText { get => _newPlayersText; set { _newPlayersText = value; OnPropertyChanged(); } }
        private bool _isCupSelected = true;
        public bool IsCupSelected { get => _isCupSelected; set { _isCupSelected = value; OnPropertyChanged(); } }
        private bool _isLeagueSelected;
        public bool IsLeagueSelected { get => _isLeagueSelected; set { _isLeagueSelected = value; OnPropertyChanged(); } }

        // Match score form
        private TournamentMatchEntity? _selectedMatch;
        public TournamentMatchEntity? SelectedMatch
        {
            get => _selectedMatch;
            set { _selectedMatch = value; OnPropertyChanged(); }
        }
        private int _score1;
        public int Score1 { get => _score1; set { _score1 = value; OnPropertyChanged(); } }
        private int _score2;
        public int Score2 { get => _score2; set { _score2 = value; OnPropertyChanged(); } }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand CreateTournamentCommand { get; }
        public ICommand DeleteTournamentCommand { get; }
        public ICommand CompleteMatchCommand { get; }

        public TournamentsVM()
        {
            CreateTournamentCommand = new RelayCommand(CreateTournament, _ => !string.IsNullOrWhiteSpace(NewTournamentName));
            DeleteTournamentCommand = new RelayCommand(DeleteTournament, _ => SelectedTournament is not null);
            CompleteMatchCommand = new RelayCommand(CompleteMatch, _ => SelectedMatch is { IsCompleted: false });
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var list = await Db.I.GetTournamentsAsync(Session.CurrentUserId);
                Tournaments = new ObservableCollection<TournamentEntity>(list);
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void CreateTournament(object? _)
        {
            var players = NewPlayersText
                .Split([',', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct().ToList();

            if (players.Count < 2) { StatusMessage = "Potrzeba minimum 2 graczy!"; return; }

            try
            {
                var type = IsLeagueSelected ? TournamentType.League : TournamentType.Cup;
                var t = await Db.I.CreateTournamentAsync(
                    Session.CurrentUserId, NewTournamentName, NewGameTitle,
                    type, players, Session.CurrentDisplayName);

                Tournaments.Add(t);
                SelectedTournament = t;
                NewTournamentName = string.Empty;
                NewGameTitle = string.Empty;
                NewPlayersText = string.Empty;
                StatusMessage = $"Turniej \"{t.Name}\" utworzony z {players.Count} graczami.";
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void CompleteMatch(object? _)
        {
            if (SelectedMatch is null) return;
            try
            {
                await Db.I.CompleteMatchAsync(SelectedMatch.Id, Score1, Score2);
                LoadData(); // Reload full state
                StatusMessage = "Wynik meczu zapisany.";
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private async void DeleteTournament(object? _)
        {
            if (SelectedTournament is null) return;
            try
            {
                await Db.I.DeleteTournamentAsync(SelectedTournament.Id, Session.CurrentUserId);
                Tournaments.Remove(SelectedTournament);
                SelectedTournament = null;
                StatusMessage = "Turniej usuniety.";
            }
            catch (Exception ex) { StatusMessage = $"Blad: {ex.Message}"; }
        }

        private void RefreshMatchesAndStandings()
        {
            if (SelectedTournament is null) { CurrentMatches.Clear(); CurrentStandings.Clear(); return; }
            CurrentMatches = new ObservableCollection<TournamentMatchEntity>(
                SelectedTournament.Matches.OrderBy(m => m.Round).ThenBy(m => m.MatchNumber));
            CurrentStandings = new ObservableCollection<LeagueStandingEntity>(
                SelectedTournament.Standings.OrderByDescending(s => s.Points));
        }
    }
}
