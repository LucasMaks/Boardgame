using System;
using System.Collections.Generic;

namespace Boardgame.Model
{
    public class PlayerProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nickname { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarInitials => string.IsNullOrWhiteSpace(Nickname) ? "?" : Nickname[..1].ToUpper();
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public PlayerStats Stats { get; set; } = new();
        public List<string> FavoriteGames { get; set; } = [];
        public List<string> OwnedGames { get; set; } = [];
        public List<string> WishList { get; set; } = [];
        public List<GameExchangeOffer> ExchangeOffers { get; set; } = [];
    }

    public class PlayerStats
    {
        public int GamesPlayed { get; set; }
        public int TournamentsPlayed { get; set; }
        public int TournamentsWon { get; set; }
        public int EventsAttended { get; set; }
        public double WinRate => GamesPlayed > 0 ? Math.Round((double)TournamentsWon / TournamentsPlayed * 100, 1) : 0;
        public int TotalPlayTimeHours { get; set; }
    }

    public class GameExchangeOffer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GameOffered { get; set; } = string.Empty;
        public string GameWanted { get; set; } = string.Empty;
        public ExchangeStatus Status { get; set; } = ExchangeStatus.Open;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum ExchangeStatus
    {
        Open,
        Accepted,
        Completed,
        Cancelled
    }
}
