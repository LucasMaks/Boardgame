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
     class NavigationVm:ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }

        }
        public ICommand TableBoardGamesCommand { get; set; }
        public ICommand CalendarCommand { get; set; }
        public ICommand MainCommand { get; set; }
        public ICommand StarPageCommand { get; set; }
        public ICommand AddBoardGameCommand { get; set; }

        private void MainWindows(object obj) => CurrentView = new MainWindowVM();

        private void AddBoardGame(object obj) => CurrentView = new AddBoaerdGameVM();
        private void Calendar(object obj) => CurrentView = new CalendarVM();
        private void TableBoardGames(object obj) => CurrentView = new TableBoardGamesVM();
        private void StarPage(object obj) => CurrentView = new StarPageVM();


        public NavigationVm() {
            MainCommand = new RelayCommand(MainWindows);
            CalendarCommand = new RelayCommand(Calendar);
            TableBoardGamesCommand = new RelayCommand(TableBoardGames);
            StarPageCommand = new RelayCommand(StarPage);
            
            //Start page
            CurrentView = new StartPage();
        }

    }
}
