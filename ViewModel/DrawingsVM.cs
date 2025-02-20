using Boardgame.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Boardgame.ViewModel
{
    public class DrawingsVM : INotifyPropertyChanged
    {
        private ObservableCollection<BoardGameModel> _boardGames;
        public ObservableCollection<BoardGameModel> BoardGames
        {
            get => _boardGames;
            set
            {
                _boardGames = value;
                OnPropertyChanged(nameof(BoardGames));
            }
        }

        private ObservableCollection<GameSession> _gameSessions;
        public ObservableCollection<GameSession> GameSessions
        {
            get => _gameSessions;
            set
            {
                _gameSessions = value;
                OnPropertyChanged(nameof(GameSessions));
            }
        }

        private SeriesCollection _gameDurations;
        public SeriesCollection GameDurations
        {
            get => _gameDurations;
            set
            {
                _gameDurations = value;
                OnPropertyChanged(nameof(GameDurations));
            }
        }

        private decimal _totalDuration;
        public decimal TotalDuration
        {
            get => _totalDuration;
            set
            {
                _totalDuration = value;
                OnPropertyChanged(nameof(TotalDuration));
            }
        }

        private Visibility _isTotalVisible;
        public Visibility IsTotalVisible
        {
            get => _isTotalVisible;
            set
            {
                _isTotalVisible = value;
                OnPropertyChanged(nameof(IsTotalVisible));
            }
        }

        private Random _random;

        public DrawingsVM()
        {
            LoadBoardGames();
            GameSessions = new ObservableCollection<GameSession>();
            GameDurations = new SeriesCollection();
            IsTotalVisible = Visibility.Collapsed; // Domyślnie ukryty
            _random = new Random(); // Inicjalizacja generatora liczb losowych
        }

        private void LoadBoardGames()
        {
            string saveGameFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveGame");

            // Sprawdź, czy folder istnieje, a jeśli nie, utwórz go
            if (!Directory.Exists(saveGameFolderPath))
            {
                Directory.CreateDirectory(saveGameFolderPath);
                Console.WriteLine($"Utworzono folder: {saveGameFolderPath}");
            }
            else
            {
                Console.WriteLine($"Folder już istnieje: {saveGameFolderPath}");
            }

            string filePath = Path.Combine(saveGameFolderPath, "BoardGame.json"); ;
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                try
                {
                    BoardGames = JsonConvert.DeserializeObject<ObservableCollection<BoardGameModel>>(json);
                }
                catch (JsonSerializationException)
                {
                    var singleBoardGame = JsonConvert.DeserializeObject<BoardGameModel>(json);
                    BoardGames = new ObservableCollection<BoardGameModel> { singleBoardGame };
                }
            }
            else
            {
                BoardGames = new ObservableCollection<BoardGameModel>();
            }
        }

        public void CreateDrawing(int numberOfPlayers)
        {
            // Filtruj gry, które są dostępne (Accessibility == true)
            var availableGames = BoardGames
                .Where(game => game.Accessibility && game.People >= numberOfPlayers)
                .ToList();

            if (!availableGames.Any()) return;

            // Losuj kolejność gier
            var shuffledGames = availableGames.OrderBy(g => _random.Next()).ToList();

            // Tworzenie sesji gier
            GameSessions.Clear();
            int sessionNumber = 1;

            foreach (var game in shuffledGames)
            {
                GameSessions.Add(new GameSession { Number = sessionNumber, Title = game.Title, Duration = game.Hours });
                sessionNumber++;
            }

            // Aktualizuj wykres
            UpdateChart();

            // Pokaż TextBlock z łącznym czasem
            IsTotalVisible = Visibility.Visible;

            SaveDrawingResult();
        }

        private void UpdateChart()
        {
            GameDurations.Clear();
            TotalDuration = 0;

            foreach (var session in GameSessions)
            {
                GameDurations.Add(new PieSeries
                {
                    Title = session.Title,
                    Values = new ChartValues<decimal> { session.Duration },
                    DataLabels = true
                });

                TotalDuration += session.Duration;
            }
        }

        private void SaveDrawingResult()
        {
            string saveGameFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveGame");

            // Sprawdź, czy folder istnieje, a jeśli nie, utwórz go
            if (!Directory.Exists(saveGameFolderPath))
            {
                Directory.CreateDirectory(saveGameFolderPath);
                Console.WriteLine($"Utworzono folder: {saveGameFolderPath}");
            }
            else
            {
                Console.WriteLine($"Folder już istnieje: {saveGameFolderPath}");
            }

            string drawingFilePath = Path.Combine(saveGameFolderPath, "DrawingResult.json");
            var result = new { GameSessions = GameSessions };
            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(drawingFilePath, json);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GameSession
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public decimal Duration { get; set; }
    }
}