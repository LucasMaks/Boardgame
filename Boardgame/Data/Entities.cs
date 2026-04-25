using System;
using System.Collections.Generic;

namespace Boardgame.Data.Entities;

// ─── User ───────────────────────────────────
public class UserEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<BoardGameEntity> OwnedGames { get; set; } = [];
    public ICollection<UserGameLibrary> GameLibrary { get; set; } = [];
    public ICollection<FriendshipEntity> FriendshipsInitiated { get; set; } = [];
    public ICollection<FriendshipEntity> FriendshipsReceived { get; set; } = [];
    public ICollection<TournamentEntity> Tournaments { get; set; } = [];
    public ICollection<EventEntity> Events { get; set; } = [];
    public ICollection<NotificationEntity> Notifications { get; set; } = [];
}

// ─── BoardGame ──────────────────────────────
public class BoardGameEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinPlayers { get; set; } = 2;
    public int MaxPlayers { get; set; } = 4;
    public int DurationMinutes { get; set; } = 60;
    public string Difficulty { get; set; } = "Medium";
    public bool IsShared { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity Owner { get; set; } = null!;
    public ICollection<UserGameLibrary> SharedWith { get; set; } = [];
}

public class UserGameLibrary
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public UserEntity User { get; set; } = null!;
    public BoardGameEntity Game { get; set; } = null!;
}

// ─── Friendship ─────────────────────────────
public class FriendshipEntity
{
    public Guid RequesterId { get; set; }
    public Guid AddresseeId { get; set; }
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity Requester { get; set; } = null!;
    public UserEntity Addressee { get; set; } = null!;
}

public enum FriendshipStatus { Pending, Accepted, Blocked }

// ─── Tournament ─────────────────────────────
public class TournamentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string GameTitle { get; set; } = string.Empty;
    public TournamentType Type { get; set; } = TournamentType.Cup;
    public TournamentStatus Status { get; set; } = TournamentStatus.Planned;
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity Creator { get; set; } = null!;
    public ICollection<TournamentParticipant> Participants { get; set; } = [];
    public ICollection<TournamentMatchEntity> Matches { get; set; } = [];
    public ICollection<LeagueStandingEntity> Standings { get; set; } = [];
}

public class TournamentParticipant
{
    public Guid TournamentId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }

    public TournamentEntity Tournament { get; set; } = null!;
    public UserEntity? User { get; set; }
}

public class TournamentMatchEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TournamentId { get; set; }
    public int Round { get; set; }
    public int MatchNumber { get; set; }
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public int ScorePlayer1 { get; set; }
    public int ScorePlayer2 { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? PlayedAt { get; set; }

    public TournamentEntity Tournament { get; set; } = null!;
}

public class LeagueStandingEntity
{
    public Guid TournamentId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int Points => Wins * 3 + Draws;

    public TournamentEntity Tournament { get; set; } = null!;
}

public enum TournamentType { Cup, League }
public enum TournamentStatus { Planned, InProgress, Completed }

// ─── Event ──────────────────────────────────
public class EventEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public EventStatusDb Status { get; set; } = EventStatusDb.Planned;
    public string PlannedGames { get; set; } = string.Empty;   // comma-separated
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity Creator { get; set; } = null!;
    public ICollection<EventAttendeeEntity> Attendees { get; set; } = [];
}

public class EventAttendeeEntity
{
    public Guid EventId { get; set; }
    public string AttendeeName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public AttendeeStatusDb Status { get; set; } = AttendeeStatusDb.Pending;
    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;

    public EventEntity Event { get; set; } = null!;
    public UserEntity? User { get; set; }
}

public enum EventStatusDb { Planned, Confirmed, InProgress, Completed, Cancelled }
public enum AttendeeStatusDb { Pending, Confirmed, Declined, Maybe }

// ─── Notification ───────────────────────────
public class NotificationEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TargetUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "System";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity TargetUser { get; set; } = null!;
}
