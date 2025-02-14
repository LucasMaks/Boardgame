using Boardgame.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Boardgame.View
{
    /// <summary>
    /// Logika interakcji dla klasy EditBoardGame.xaml
    /// </summary>
    public partial class EditBoardGame : Window
    {
        public BoardGameModel BoardGame { get; set; }
        public EditBoardGame()
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

        

        private void SaveBoardGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BoardGame.Title = Title.Text;
                BoardGame.Description = Description.Text;
                BoardGame.People = int.Parse(People.Text);
                BoardGame.Hours = int.Parse(Hours.Text);
                BoardGame.Accessibility = Accessibility.IsChecked == true ? true : false;
                this.DialogResult = true; // Potwierdzenie zapisania
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values for People and Hours.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBoardGame_Checked(object sender, RoutedEventArgs e)
        {

          
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (BoardGame != null)
            {
                // Załaduj dane do kontrolek w oknie edycji
                Title.Text = BoardGame.Title;
                Description.Text = BoardGame.Description;
                People.Text = BoardGame.People.ToString();
                Hours.Text = BoardGame.Hours.ToString();
                Accessibility.IsChecked = BoardGame.Accessibility;
            }
        }
    }
}
