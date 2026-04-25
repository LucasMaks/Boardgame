using Boardgame.Utilities;
using System;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class StarPageVM : ViewModelBase
    {
        public ICommand GoToCatalogCommand { get; }
        public ICommand GoToDrawingCommand { get; }
        public ICommand GoToTournamentsCommand { get; }
        public ICommand GoToEventsCommand { get; }
        public ICommand GoToCommunityCommand { get; }

        public StarPageVM(
            Action? goCatalog = null,
            Action? goDrawing = null,
            Action? goTournaments = null,
            Action? goEvents = null,
            Action? goCommunity = null)
        {
            GoToCatalogCommand = new RelayCommand(_ => goCatalog?.Invoke());
            GoToDrawingCommand = new RelayCommand(_ => goDrawing?.Invoke());
            GoToTournamentsCommand = new RelayCommand(_ => goTournaments?.Invoke());
            GoToEventsCommand = new RelayCommand(_ => goEvents?.Invoke());
            GoToCommunityCommand = new RelayCommand(_ => goCommunity?.Invoke());
        }
    }
}
