using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Boardgame.Model;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Boardgame.ViewModel
{
    public class TableBoardGamesVM : INotifyPropertyChanged
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

        public TableBoardGamesVM()
        {
            LoadBoardGames();
            RefreshCollectionView();
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

            string filePath = Path.Combine(saveGameFolderPath, "BoardGame.json");
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

        public void SaveBoardGames()

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

            string filePath = Path.Combine(saveGameFolderPath, "BoardGame.json");
            string json = JsonConvert.SerializeObject(BoardGames, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void RefreshCollectionView()
        {
            SaveBoardGames();
            LoadBoardGames();// Wczytaj ponownie dane z JSON
            OnPropertyChanged(nameof(BoardGames)); // Powiadom widok o zmianach
        }
        public void RefreshCollectionSeaveView()
        {
            
            LoadBoardGames();// Wczytaj ponownie dane z JSON
            OnPropertyChanged(nameof(BoardGames)); // Powiadom widok o zmianach
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
