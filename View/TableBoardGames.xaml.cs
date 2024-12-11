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
using Boardgame.Service;

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
           WindowsServices windowsServices = new WindowsServices();
            windowsServices.OpenWindow();
        }
    }
}
