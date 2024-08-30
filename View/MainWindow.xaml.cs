using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Boardgame.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight=SystemParameters.MaximizedPrimaryScreenHeight;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd,int msg, IntPtr wParam, IntPtr lParam);
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }
        private void RadioButton_Click_BoardGame(object sender, RoutedEventArgs e)
        {

        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState= WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
           if(this.WindowState  == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
           else
                this.WindowState=WindowState.Normal;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RadioButton_Click_Draw(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Click_AddBoardGame(object sender, RoutedEventArgs e)
        {

        }

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight= SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void RadioButton_Calndar(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Click_CalendarGame(object sender, RoutedEventArgs e)
        {

        }
    }
}