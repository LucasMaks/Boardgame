using Boardgame.Data;
using Boardgame.Data.Abstractions;
using Boardgame.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boardgame.Services;

/// <summary>
/// Singleton service — all database operations go through here.
/// Creates a fresh DbContext per call (thread-safe for WPF).
/// Uses Dependency Inversion: depends on IDbConnectionProvider abstraction.
/// </summary>
public sealed class Db
{
    private static Db? _instance;
    public static Db I => _instance ??= new Db();

    private readonly DbContextOptions<AppDbContext> _opts;

    private Db()
    {
        var connectionProvider = new DefaultConnectionProvider();
        _opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionProvider.GetConnectionString())
            .Options;
    }

    private AppDbContext Ctx() => new(_opts);

    // ═══════════════════════════════════════
    //  INIT
    // ═══════════════════════════════════════

    public async Task InitAsync()
    {
        using var db = Ctx();
        await db.Database.MigrateAsync();
    }

    public async Task EnsureCreatedAsync()
    {
        using var db = Ctx();
        await db.Database.EnsureCreatedAsync();
    }

    // ═══════════════════════════════════════
    //  AUTH
    // ═══════════════════════════════════════

    public async Task<UserEntity?> LoginAsync(string email, string password)
    {
        using var db = Ctx();
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email == email.ToLower().Trim());

        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<(UserEntity? User, string? Error)> RegisterAsync(
        string email, string password, string displayName)
    {
        email = email.ToLower().Trim();
        displayName = displayName.Trim();

        if (password.Length < 6)
            return (null, "Haslo musi miec minimum 6 znakow.");

        using var db = Ctx();
        if (await db.Users.AnyAsync(u => u.Email == email))
            return (null, "Uzytkownik z tym emailem juz istnieje.");

        var user = new UserEntity
        {
            Email = email,
            DisplayName = displayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        db.Users.Add(user);

        db.Notifications.Add(new NotificationEntity
        {
            TargetUserId = user.Id,
            SenderName = "System",
            Title = "Witaj w BoardGames!",
            Message = $"Czesc {displayName}! Twoje konto zostalo utworzone.",
            Type = "Welcome"
        });

        await db.SaveChangesAsync();
        return (user, null);
    }

    // ═══════════════════════════════════════
    //  GAMES
    // ═══════════════════════════════════════

    public async Task<List<BoardGameEntity>> GetMyGamesAsync(Guid userId)
    {
        using var db = Ctx();
        return await db.BoardGames
            .Where(g => g.OwnerId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BoardGameEntity>> GetAllGamesWithLibraryAsync(Guid userId)
    {
        using var db = Ctx();
        // Own games + games in my library
        var own = await db.BoardGames
            .Where(g => g.OwnerId == userId)
            .ToListAsync();

        var library = await db.UserGameLibraries
            .Where(l => l.UserId == userId)
            .Include(l => l.Game)
            .Select(l => l.Game)
            .ToListAsync();

        return own.Union(library).OrderBy(g => g.Title).ToList();
    }

    public async Task<List<BoardGameEntity>> GetCommunityGamesAsync(Guid userId)
    {
        using var db = Ctx();
        var friendIds = await GetFriendIdsAsync(db, userId);

        return await db.BoardGames
            .Where(g => g.IsShared && (friendIds.Contains(g.OwnerId) || g.OwnerId == userId))
            .Include(g => g.Owner)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<BoardGameEntity> AddGameAsync(Guid userId, string title, string desc,
        int minPlayers, int maxPlayers, int duration, string difficulty, bool isShared)
    {
        using var db = Ctx();
        var game = new BoardGameEntity
        {
            Title = title.Trim(), Description = desc.Trim(),
            MinPlayers = minPlayers, MaxPlayers = maxPlayers,
            DurationMinutes = duration, Difficulty = difficulty,
            IsShared = isShared, OwnerId = userId
        };
        db.BoardGames.Add(game);
        await db.SaveChangesAsync();
        return game;
    }

    public async Task UpdateGameAsync(BoardGameEntity game)
    {
        using var db = Ctx();
        db.BoardGames.Update(game);
        await db.SaveChangesAsync();
    }

    public async Task DeleteGameAsync(Guid gameId, Guid userId)
    {
        using var db = Ctx();
        var game = await db.BoardGames
            .FirstOrDefaultAsync(g => g.Id == gameId && g.OwnerId == userId);
        if (game is not null)
        {
            db.BoardGames.Remove(game);
            await db.SaveChangesAsync();
        }
    }

    public async Task AddToLibraryAsync(Guid userId, Guid gameId)
    {
        using var db = Ctx();
        if (await db.UserGameLibraries.AnyAsync(l => l.UserId == userId && l.GameId == gameId))
            return;
        db.UserGameLibraries.Add(new UserGameLibrary { UserId = userId, GameId = gameId });
        await db.SaveChangesAsync();
    }

    // ═══════════════════════════════════════
    //  TOURNAMENTS
    // ═══════════════════════════════════════

    public async Task<List<TournamentEntity>> GetTournamentsAsync(Guid userId)
    {
        using var db = Ctx();
        return await db.Tournaments
            .Where(t => t.CreatorId == userId)
            .Include(t => t.Participants)
            .Include(t => t.Matches)
            .Include(t => t.Standings)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TournamentEntity> CreateTournamentAsync(
        Guid userId, string name, string gameTitle,
        TournamentType type, List<string> players, string senderName)
    {
        using var db = Ctx();
        var t = new TournamentEntity
        {
            Name = name.Trim(), GameTitle = gameTitle.Trim(),
            Type = type, CreatorId = userId
        };

        foreach (var p in players.Distinct())
            t.Participants.Add(new TournamentParticipant
            {
                TournamentId = t.Id, PlayerName = p
            });

        if (type == TournamentType.Cup) GenerateCupBracket(t, players);
        else GenerateLeague(t, players);

        db.Tournaments.Add(t);

        // Notify registered players
        var registered = await db.Users
            .Where(u => players.Contains(u.Email) || players.Contains(u.DisplayName))
            .ToListAsync();
        foreach (var ru in registered)
            db.Notifications.Add(new NotificationEntity
            {
                TargetUserId = ru.Id, SenderName = senderName,
                Title = "Dodano do turnieju",
                Message = $"{senderName} dodal Cie do turnieju \"{t.Name}\".",
                Type = "AddedToTournament"
            });

        await db.SaveChangesAsync();
        return t;
    }

    public async Task CompleteMatchAsync(Guid matchId, int s1, int s2)
    {
        using var db = Ctx();
        var match = await db.TournamentMatches.FindAsync(matchId);
        if (match is null || match.IsCompleted) return;

        match.ScorePlayer1 = s1;
        match.ScorePlayer2 = s2;
        match.IsCompleted = true;
        match.PlayedAt = DateTime.UtcNow;

        var tournament = await db.Tournaments
            .Include(t => t.Matches).Include(t => t.Standings)
            .FirstAsync(t => t.Id == match.TournamentId);
        tournament.Status = TournamentStatus.InProgress;

        if (tournament.Type == TournamentType.League)
            UpdateLeague(tournament, match);
        else
            AdvanceCup(tournament, match);

        if (tournament.Matches.All(m => m.IsCompleted))
            tournament.Status = TournamentStatus.Completed;

        await db.SaveChangesAsync();
    }

    public async Task DeleteTournamentAsync(Guid id, Guid userId)
    {
        using var db = Ctx();
        var t = await db.Tournaments.FirstOrDefaultAsync(x => x.Id == id && x.CreatorId == userId);
        if (t is not null) { db.Tournaments.Remove(t); await db.SaveChangesAsync(); }
    }

    // ═══════════════════════════════════════
    //  EVENTS
    // ═══════════════════════════════════════

    public async Task<List<EventEntity>> GetEventsAsync(Guid userId)
    {
        using var db = Ctx();
        return await db.Events
            .Where(e => e.CreatorId == userId || e.Attendees.Any(a => a.UserId == userId))
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<EventEntity> CreateEventAsync(
        Guid userId, string name, string desc, string location,
        DateTime date, string startTime, string endTime, string games)
    {
        using var db = Ctx();
        var evt = new EventEntity
        {
            Name = name.Trim(), Description = desc.Trim(),
            Location = location.Trim(), EventDate = date,
            StartTime = startTime, EndTime = endTime,
            PlannedGames = games.Trim(), CreatorId = userId
        };
        db.Events.Add(evt);
        await db.SaveChangesAsync();
        return evt;
    }

    public async Task AddAttendeeAsync(Guid eventId, string name, string senderName)
    {
        using var db = Ctx();
        var evt = await db.Events.Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId);
        if (evt is null) return;
        if (evt.Attendees.Any(a => a.AttendeeName == name.Trim())) return;

        var registered = await db.Users
            .FirstOrDefaultAsync(u => u.Email == name || u.DisplayName == name);

        evt.Attendees.Add(new EventAttendeeEntity
        {
            EventId = eventId,
            AttendeeName = name.Trim(),
            UserId = registered?.Id
        });

        if (registered is not null)
            db.Notifications.Add(new NotificationEntity
            {
                TargetUserId = registered.Id, SenderName = senderName,
                Title = "Zaproszenie na wydarzenie",
                Message = $"{senderName} dodal Cie do wydarzenia \"{evt.Name}\".",
                Type = "AddedToEvent"
            });

        await db.SaveChangesAsync();
    }

    public async Task UpdateEventStatusAsync(Guid eventId, EventStatusDb status)
    {
        using var db = Ctx();
        var evt = await db.Events.FindAsync(eventId);
        if (evt is not null) { evt.Status = status; await db.SaveChangesAsync(); }
    }

    public async Task DeleteEventAsync(Guid id, Guid userId)
    {
        using var db = Ctx();
        var e = await db.Events.FirstOrDefaultAsync(x => x.Id == id && x.CreatorId == userId);
        if (e is not null) { db.Events.Remove(e); await db.SaveChangesAsync(); }
    }

    // ═══════════════════════════════════════
    //  COMMUNITY / FRIENDS
    // ═══════════════════════════════════════

    public async Task<List<(UserEntity Friend, FriendshipStatus Status)>> GetFriendsAsync(Guid userId)
    {
        using var db = Ctx();
        var ships = await db.Friendships
            .Where(f => (f.RequesterId == userId || f.AddresseeId == userId)
                        && f.Status == FriendshipStatus.Accepted)
            .Include(f => f.Requester).Include(f => f.Addressee)
            .ToListAsync();

        return ships.Select(f =>
        {
            var friend = f.RequesterId == userId ? f.Addressee : f.Requester;
            return (friend, f.Status);
        }).ToList();
    }

    public async Task<List<(UserEntity Requester, DateTime CreatedAt)>> GetPendingRequestsAsync(Guid userId)
    {
        using var db = Ctx();
        var reqs = await db.Friendships
            .Where(f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Requester)
            .ToListAsync();
        return reqs.Select(f => (f.Requester, f.CreatedAt)).ToList();
    }

    public async Task<string?> SendFriendRequestAsync(Guid userId, string email, string senderName)
    {
        email = email.ToLower().Trim();
        using var db = Ctx();
        var target = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (target is null) return "Nie znaleziono uzytkownika z tym emailem.";
        if (target.Id == userId) return "Nie mozna dodac samego siebie.";

        var existing = await db.Friendships
            .FirstOrDefaultAsync(f =>
                (f.RequesterId == userId && f.AddresseeId == target.Id) ||
                (f.RequesterId == target.Id && f.AddresseeId == userId));
        if (existing is not null) return "Prosba juz istnieje lub jestes juz znajomym.";

        db.Friendships.Add(new FriendshipEntity
        {
            RequesterId = userId, AddresseeId = target.Id
        });
        db.Notifications.Add(new NotificationEntity
        {
            TargetUserId = target.Id, SenderName = senderName,
            Title = "Nowe zaproszenie do znajomych",
            Message = $"{senderName} chce dodac Cie do znajomych.",
            Type = "FriendRequest"
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task RespondToFriendRequestAsync(Guid requesterId, Guid myId, bool accept)
    {
        using var db = Ctx();
        var f = await db.Friendships.FirstOrDefaultAsync(
            x => x.RequesterId == requesterId && x.AddresseeId == myId && x.Status == FriendshipStatus.Pending);
        if (f is null) return;
        f.Status = accept ? FriendshipStatus.Accepted : FriendshipStatus.Blocked;
        await db.SaveChangesAsync();
    }

    public async Task RemoveFriendAsync(Guid userId, Guid friendId)
    {
        using var db = Ctx();
        var f = await db.Friendships.FirstOrDefaultAsync(x =>
            (x.RequesterId == userId && x.AddresseeId == friendId) ||
            (x.RequesterId == friendId && x.AddresseeId == userId));
        if (f is not null) { db.Friendships.Remove(f); await db.SaveChangesAsync(); }
    }

    public async Task<List<UserEntity>> SearchUsersAsync(string query, Guid excludeId)
    {
        using var db = Ctx();
        var q = query.ToLower().Trim();
        return await db.Users
            .Where(u => u.Id != excludeId &&
                (u.DisplayName.ToLower().Contains(q) || u.Email.Contains(q)))
            .Take(20).ToListAsync();
    }

    // ═══════════════════════════════════════
    //  NOTIFICATIONS
    // ═══════════════════════════════════════

    public async Task<List<NotificationEntity>> GetNotificationsAsync(Guid userId)
    {
        using var db = Ctx();
        return await db.Notifications
            .Where(n => n.TargetUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50).ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        using var db = Ctx();
        return await db.Notifications.CountAsync(n => n.TargetUserId == userId && !n.IsRead);
    }

    public async Task MarkAllReadAsync(Guid userId)
    {
        using var db = Ctx();
        await db.Notifications.Where(n => n.TargetUserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    // ═══════════════════════════════════════
    //  HELPERS (private)
    // ═══════════════════════════════════════

    private static async Task<List<Guid>> GetFriendIdsAsync(AppDbContext db, Guid userId) =>
        await db.Friendships
            .Where(f => (f.RequesterId == userId || f.AddresseeId == userId)
                        && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToListAsync();

    private static void GenerateCupBracket(TournamentEntity t, List<string> players)
    {
        Shuffle(players);
        int n = 0;
        for (int i = 0; i < players.Count - 1; i += 2)
            t.Matches.Add(new TournamentMatchEntity
            {
                TournamentId = t.Id, Round = 1, MatchNumber = ++n,
                Player1Name = players[i],
                Player2Name = i + 1 < players.Count ? players[i + 1] : "BYE"
            });
    }

    private static void AdvanceCup(TournamentEntity t, TournamentMatchEntity done)
    {
        var roundMatches = t.Matches.Where(m => m.Round == done.Round).ToList();
        if (!roundMatches.All(m => m.IsCompleted) || roundMatches.Count <= 1) return;

        var winners = roundMatches
            .Select(m => m.ScorePlayer1 >= m.ScorePlayer2 ? m.Player1Name : m.Player2Name)
            .ToList();
        int n = 0;
        for (int i = 0; i < winners.Count - 1; i += 2)
            t.Matches.Add(new TournamentMatchEntity
            {
                TournamentId = t.Id, Round = done.Round + 1, MatchNumber = ++n,
                Player1Name = winners[i],
                Player2Name = i + 1 < winners.Count ? winners[i + 1] : "BYE"
            });
    }

    private static void GenerateLeague(TournamentEntity t, List<string> players)
    {
        foreach (var p in players)
            t.Standings.Add(new LeagueStandingEntity { TournamentId = t.Id, PlayerName = p });

        int n = 0;
        for (int i = 0; i < players.Count; i++)
            for (int j = i + 1; j < players.Count; j++)
                t.Matches.Add(new TournamentMatchEntity
                {
                    TournamentId = t.Id, Round = 1, MatchNumber = ++n,
                    Player1Name = players[i], Player2Name = players[j]
                });
    }

    private static void UpdateLeague(TournamentEntity t, TournamentMatchEntity m)
    {
        var s1 = t.Standings.FirstOrDefault(s => s.PlayerName == m.Player1Name);
        var s2 = t.Standings.FirstOrDefault(s => s.PlayerName == m.Player2Name);
        if (s1 is null || s2 is null) return;

        s1.Played++; s2.Played++;
        s1.GoalsFor += m.ScorePlayer1; s1.GoalsAgainst += m.ScorePlayer2;
        s2.GoalsFor += m.ScorePlayer2; s2.GoalsAgainst += m.ScorePlayer1;

        if (m.ScorePlayer1 > m.ScorePlayer2) { s1.Wins++; s2.Losses++; }
        else if (m.ScorePlayer2 > m.ScorePlayer1) { s2.Wins++; s1.Losses++; }
        else { s1.Draws++; s2.Draws++; }
    }

    private static void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
