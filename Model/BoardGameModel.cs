using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgame.Model
{
    public class BoardGameModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int People { get; set; }
        public int Hours { get; set; }
        public bool Accessibility { get; set; }
        public string Owner { get; set; }
    }
}
