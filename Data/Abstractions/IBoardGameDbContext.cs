using Boardgame.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boardgame.Data.Abstractions;

/// <summary>
/// Abstraction for database access.
/// Allows mocking and testing without concrete DbContext.
/// </summary>
public interface IBoardGameDbContext
{
    DbSet<UserEntity>            Users              { get; }
    DbSet<BoardGameEntity>       BoardGames         { get; }
    DbSet<UserGameLibrary>       UserGameLibraries  { get; }
    DbSet<FriendshipEntity>      Friendships        { get; }
    DbSet<TournamentEntity>      Tournaments        { get; }
    DbSet<TournamentParticipant> TournamentParticipants { get; }
    DbSet<TournamentMatchEntity> TournamentMatches  { get; }
    DbSet<LeagueStandingEntity>  LeagueStandings    { get; }
    DbSet<EventEntity>           Events             { get; }
    DbSet<EventAttendeeEntity>   EventAttendees     { get; }
    DbSet<NotificationEntity>    Notifications      { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
