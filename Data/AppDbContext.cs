using Boardgame.Data.Abstractions;
using Boardgame.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boardgame.Data;

/// <summary>
/// Main database context for BoardGame application.
/// Entity configurations are auto-discovered from assembly (Open/Closed Principle).
/// </summary>
public sealed class AppDbContext : DbContext, IBoardGameDbContext
{
    // ── DbSets (Interface segregation) ─────────────────
    public DbSet<UserEntity>            Users              => Set<UserEntity>();
    public DbSet<BoardGameEntity>       BoardGames         => Set<BoardGameEntity>();
    public DbSet<UserGameLibrary>       UserGameLibraries  => Set<UserGameLibrary>();
    public DbSet<FriendshipEntity>      Friendships        => Set<FriendshipEntity>();
    public DbSet<TournamentEntity>      Tournaments        => Set<TournamentEntity>();
    public DbSet<TournamentParticipant> TournamentParticipants => Set<TournamentParticipant>();
    public DbSet<TournamentMatchEntity> TournamentMatches  => Set<TournamentMatchEntity>();
    public DbSet<LeagueStandingEntity>  LeagueStandings    => Set<LeagueStandingEntity>();
    public DbSet<EventEntity>           Events             => Set<EventEntity>();
    public DbSet<EventAttendeeEntity>   EventAttendees     => Set<EventAttendeeEntity>();
    public DbSet<NotificationEntity>    Notifications      => Set<NotificationEntity>();

    // ── Constructor (Dependency Inversion) ──────────────
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Model configuration (Open/Closed Principle) ─────
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // Auto-discover all IEntityTypeConfiguration<T> in this assembly
        // Adding new entity configuration doesn't require modifying this class
        mb.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
