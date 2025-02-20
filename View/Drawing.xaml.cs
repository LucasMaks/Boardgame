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

namespace Boardgame.View
{
    /// <summary>
    /// Logika interakcji dla klasy Drawing.xaml
    /// </summary>
    public partial class Drawing : UserControl
    {
        private DrawingsVM viewModel;
        public Drawing()
        {
            InitializeComponent();
            viewModel = new DrawingsVM();
            DataContext = viewModel;
        }

        private void ColumnDefinition_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void People_GotFocus(object sender, RoutedEventArgs e)
        {

            if (People.Text == "People...")
            {
                People.Text = string.Empty;
                People.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Zmieniamy kolor czcionki na czarny
            }
        }

        private void People_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(People.Text))
            {
                People.Text = "People...";
                People.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Przywracamy szary kolor czcionki
            }
        }

        private void Drawing_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(People.Text, out int numberOfPlayers))
            {
                viewModel.CreateDrawing(numberOfPlayers);
            }
            else
            {
                MessageBox.Show("Please enter a valid number of players.");
            }
        }

        private void Drawing_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BoardGameGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
