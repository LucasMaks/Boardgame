using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Interop;
using Boardgame.Model;
using System.IO;
using Path = System.IO.Path;

namespace Boardgame.View
{
    /// <summary>
    /// Logika interakcji dla klasy AddBoardGame.xaml
    /// </summary>
    public partial class AddBoardGame : Window
    {
        

        public AddBoardGame()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Title_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Title.Text == "Title...")
            {
                Title.Text = string.Empty;
                Title.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Zmieniamy kolor czcionki na czarny
            }
        }

        private void Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Title.Text))
            {
                Title.Text = "Title...";
                Title.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Przywracamy szary kolor czcionki
            }
        }

        private void Description_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Description.Text == "Description...")
            {
                Description.Text = string.Empty;
                Description.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Zmieniamy kolor czcionki na czarny
            }
            
        }

        private void Description_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Description.Text))
            {
                Description.Text = "Description...";
                Description.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Przywracamy szary kolor czcionki
            }
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

        private void Hours_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Hours.Text == "Hours...")
            {
                Hours.Text = string.Empty;
                Hours.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Zmieniamy kolor czcionki na czarny
            }
        }

        private void Hours_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Hours.Text))
            {
                Hours.Text = "Hours...";
                Hours.Foreground = new SolidColorBrush(Color.FromRgb(224, 225, 241)); // Przywracamy szary kolor czcionki
            }
        }

       

        private void SaveBoardGame_Click(object sender, RoutedEventArgs e)
        {
           
                var boardGame = new BoardGameModel
                {
                    Id = Guid.NewGuid(),
                    Title = Title.Text,
                    Description = Description.Text,
                    People = int.TryParse(People.Text, out int players) ? players : 0,
                    Hours = int.TryParse(Hours.Text, out int hours) ? hours : 0,
                    Accessibility=Accessibility.IsChecked==true? true:false,
                    Owner= "Default"
                };

                string filePath = Path.Combine("C:\\Users\\ermsj\\source\\repos\\LucasMaks\\Boardgame\\SaveGame", "BoardGame.json");

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            boardGame.Save(filePath);
            this.DialogResult = true;
            this.Close();
               
           
            
        }

        private void SaveBoardGame_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
