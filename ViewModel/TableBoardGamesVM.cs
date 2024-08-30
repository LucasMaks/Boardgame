using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boardgame.Model;

namespace Boardgame.ViewModel
{
    class TableBoardGamesVM : Utilities.ViewModelBase
    {
        private readonly BoardGameModel _model;
        public int BoardGameId
        {
            get { return _model.Id; }
            set { _model.Id = value; OnPropertyChanged(); }
        }
        public TableBoardGamesVM()
        {
            _model = new BoardGameModel();
            BoardGameId = 1;
        }


    }
}
