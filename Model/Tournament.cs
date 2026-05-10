using System;
using System.Collections.Generic;

namespace Boardgame.Model
{
    public class Tournament
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string GameTitle { get; set; } = string.Empty;
        public TournamentType Type { get; set; } = TournamentType.Cup;
        public TournamentStatus Status { get; set; } = TournamentStatus.Planned;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public List<string> Players { get; set; } = [];
        public List<TournamentMatch> Matches { get; set; } = [];
        public List<LeagueStanding> Standings { get; set; } = [];
    }

    public class TournamentMatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Round { get; set; }
        public int MatchNumber { get; set; }
        public string Player1 { get; set; } = string.Empty;
        public string Player2 { get; set; } = string.Empty;
        public int ScorePlayer1 { get; set; }
        public int ScorePlayer2 { get; set; }
        public bool IsCompleted { get; set; }
        public string Winner => IsCompleted
            ? (ScorePlayer1 > ScorePlayer2 ? Player1 : ScorePlayer2 > ScorePlayer1 ? Player2 : "Remis")
            : string.Empty;
        public DateTime? PlayedDate { get; set; }
    }

    public class LeagueStanding
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int Points => Wins * 3 + Draws;
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
    }

    public enum TournamentType
    {
        Cup,
        League
    }

    public enum TournamentStatus
    {
        Planned,
        InProgress,
        Completed
    }
}
