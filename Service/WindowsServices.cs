using Boardgame.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Boardgame.Service
{
    public class WindowsServices : Window, IAddBoardGame
    {
       public void OpenWindow()
        {
            var openAddBoard = new AddBoardGame();
            openAddBoard.ShowDialog();
        }

        public void CloseWindow()
        {
            var closeAddBoard = Application.Current.Windows.OfType<AddBoardGame>().SingleOrDefault(x => x.IsActive);
            if (closeAddBoard != null) 
            {
                closeAddBoard.Close();
            }
        }

      
    }
}
