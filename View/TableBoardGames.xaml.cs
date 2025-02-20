using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Boardgame.ViewModel;
using Boardgame.Model;
using System.Collections.ObjectModel;

namespace Boardgame.View
{
    /// <summary>
    /// Logika interakcji dla klasy TableBoardGames.xaml
    /// </summary>
    public partial class TableBoardGames : UserControl
    {
        public TableBoardGames()
        {
            InitializeComponent();
        }
            private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
           if (SearchBox.Text== "Search...")
            {
                SearchBox.Text = string.Empty;
                SearchBox.Foreground = new SolidColorBrush(Colors.Black); // Zmieniamy kolor czcionki na czarny
            }
           
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = new SolidColorBrush(Colors.Gray); // Przywracamy szary kolor czcionki
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_BoardGame_Click(object sender, RoutedEventArgs e)
        {
           AddBoardGame addBoardGameWindow = new AddBoardGame();
            bool? result = addBoardGameWindow.ShowDialog();
            if (result == true) 
            {
                // Odśwież dane po zamknięciu okna dodawania
                var viewModel = (TableBoardGamesVM)this.DataContext;
                viewModel.RefreshCollectionSeaveView();
            }
        }

        private void Add_BoardGame_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void EditBoardGame_Click(object sender, RoutedEventArgs e)
        {
            var selectedBoardGame = (BoardGameModel)((RadioButton)sender).DataContext;

            EditBoardGame editBoardGameWindow = new EditBoardGame();
            editBoardGameWindow.BoardGame = selectedBoardGame;
            bool? result = editBoardGameWindow.ShowDialog();
            if (result == true)
            {
                // Odśwież dane po zamknięciu okna dodawania
                var viewModel = (TableBoardGamesVM)this.DataContext;
                viewModel.RefreshCollectionView();
            }
        }

        private void DeleteBoardGame_Click(object sender, RoutedEventArgs e)
        {
            var notificationDelete = new NotificationDelete();
            notificationDelete.ShowDialog();


            if (notificationDelete.IsConfirmed) {
                var boardGameToDelete = (BoardGameModel)((RadioButton)sender).DataContext;

                if (boardGameToDelete != null)
                {
                    // Usuwamy grę z kolekcji BoardGames
                    var viewModel = (TableBoardGamesVM)this.DataContext;
                    viewModel.BoardGames.Remove(boardGameToDelete);

                    // Zapisujemy zmienioną kolekcję do pliku JSON
                    viewModel.SaveBoardGames();
                }
            }
           

        }

        private void BoardGameGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
