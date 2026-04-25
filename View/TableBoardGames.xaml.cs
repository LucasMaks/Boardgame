using Boardgame.Data.Entities;
using Boardgame.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Boardgame.View
{
    public partial class TableBoardGames : UserControl
    {
        public TableBoardGames()
        {
            InitializeComponent();
        }

        private TableBoardGamesVM VM => (TableBoardGamesVM)DataContext;

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search...")
            {
                SearchBox.Text = string.Empty;
                SearchBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) { }

        private void Add_BoardGame_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBoardGame();
            if (addWindow.ShowDialog() == true)
                VM.RefreshCommand.Execute(null);
        }

        private void Add_BoardGame_Checked(object sender, RoutedEventArgs e) { }

        private void EditBoardGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO: update Edit dialog to use BoardGameEntity
            VM.RefreshCommand.Execute(null);
        }

        private void DeleteBoardGame_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = (sender as FrameworkElement)?.DataContext as BoardGameEntity;
            if (selectedGame is null) return;

            var confirm = new NotificationDelete();
            confirm.ShowDialog();
            if (confirm.IsConfirmed)
            {
                VM.SelectedGame = selectedGame;
                VM.DeleteGameCommand.Execute(null);
            }
        }

        private void BoardGameGridView_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }
}
