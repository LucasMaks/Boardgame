using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Boardgame.Utilities;
using Boardgame.View;

namespace Boardgame.ViewModel
{
     class NavigationVm : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }

        }
        public ICommand TableBoardGamesCommand { get; set; }
        public ICommand MainCommand { get; set; }
        public ICommand StarPageCommand { get; set; }
        public ICommand DrawingsComannd {  get; set; }

        private void MainWindows(object obj) => CurrentView = new MainWindowVM();
        private void TableBoardGames(object obj) => CurrentView = new TableBoardGamesVM();
        private void StarPage(object obj) => CurrentView = new StarPageVM();
        private void Drawings(object obj) => CurrentView = new DrawingsVM();



        public NavigationVm() {
            MainCommand = new RelayCommand(MainWindows);
            TableBoardGamesCommand = new RelayCommand(TableBoardGames);
            StarPageCommand = new RelayCommand(StarPage);
            DrawingsComannd = new RelayCommand(Drawings);
            
            //Start page
            CurrentView = new StartPage();
        }

    }
}
