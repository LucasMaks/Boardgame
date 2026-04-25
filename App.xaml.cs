using Boardgame.Services;
using System.Windows;

namespace Boardgame
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                // Auto-create/migrate database on startup
                await Db.I.EnsureCreatedAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Nie udalo sie polaczyc z baza danych.\n\n" +
                    $"Upewnij sie, ze Docker z SQL Server jest uruchomiony.\n\n" +
                    $"Blad: {ex.Message}",
                    "Blad polaczenia z baza",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
