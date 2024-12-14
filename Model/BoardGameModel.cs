
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Boardgame.Model
{
    public class BoardGameModel
    {
        public Guid Id { get; set; }
        
        public  string Title { get; set; }
        public  string Description { get; set; }
        
        public int People { get; set; }
        public int Hours { get; set; }
        public bool Accessibility { get; set; }
        public  string Owner { get; set; }

        public void Save(string filePath)
        {
            List<BoardGameModel> boardGames = new List<BoardGameModel>();

            // Jeśli plik istnieje, wczytaj istniejące dane
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                boardGames = JsonConvert.DeserializeObject<List<BoardGameModel>>(json) ?? new List<BoardGameModel>();
            }

            // Dodaj nową grę do listy
            boardGames.Add(this);

            // Zapisz zaktualizowaną listę do pliku
            string updatedJson = JsonConvert.SerializeObject(boardGames, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }
    }
}
