using Boardgame.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgame.Data.Configurations;

// ═══════════════════════════════════════════════════════════
//  User & related entities
// ═══════════════════════════════════════════════════════════

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
    }
}

internal sealed class UserGameLibraryConfiguration : IEntityTypeConfiguration<UserGameLibrary>
{
    public void Configure(EntityTypeBuilder<UserGameLibrary> builder)
    {
        builder.HasKey(l => new { l.UserId, l.GameId });

        builder.HasOne(l => l.User)
               .WithMany(u => u.GameLibrary)
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(l => l.Game)
               .WithMany(g => g.SharedWith)
               .HasForeignKey(l => l.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

// ═══════════════════════════════════════════════════════════
//  BoardGame
// ═══════════════════════════════════════════════════════════

internal sealed class BoardGameConfiguration : IEntityTypeConfiguration<BoardGameEntity>
{
    public void Configure(EntityTypeBuilder<BoardGameEntity> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title).IsRequired().HasMaxLength(200);
        builder.Property(g => g.Description).HasMaxLength(1000);
        builder.Property(g => g.Difficulty).HasMaxLength(50);

        builder.HasOne(g => g.Owner)
               .WithMany(u => u.OwnedGames)
               .HasForeignKey(g => g.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

// ═══════════════════════════════════════════════════════════
//  Friendship
// ═══════════════════════════════════════════════════════════

internal sealed class FriendshipConfiguration : IEntityTypeConfiguration<FriendshipEntity>
{
    public void Configure(EntityTypeBuilder<FriendshipEntity> builder)
    {
        builder.HasKey(f => new { f.RequesterId, f.AddresseeId });

        builder.HasOne(f => f.Requester)
               .WithMany(u => u.FriendshipsInitiated)
               .HasForeignKey(f => f.RequesterId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Addressee)
               .WithMany(u => u.FriendshipsReceived)
               .HasForeignKey(f => f.AddresseeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

// ═══════════════════════════════════════════════════════════
//  Tournament & related entities
// ═══════════════════════════════════════════════════════════

internal sealed class TournamentConfiguration : IEntityTypeConfiguration<TournamentEntity>
{
    public void Configure(EntityTypeBuilder<TournamentEntity> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.GameTitle).IsRequired().HasMaxLength(200);

        builder.HasOne(t => t.Creator)
               .WithMany(u => u.Tournaments)
               .HasForeignKey(t => t.CreatorId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class TournamentParticipantConfiguration 
    : IEntityTypeConfiguration<TournamentParticipant>
{
    public void Configure(EntityTypeBuilder<TournamentParticipant> builder)
    {
        builder.HasKey(p => new { p.TournamentId, p.PlayerName });

        builder.Property(p => p.PlayerName).IsRequired().HasMaxLength(100);

        builder.HasOne(p => p.Tournament)
               .WithMany(t => t.Participants)
               .HasForeignKey(p => p.TournamentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class TournamentMatchConfiguration 
    : IEntityTypeConfiguration<TournamentMatchEntity>
{
    public void Configure(EntityTypeBuilder<TournamentMatchEntity> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Player1Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Player2Name).IsRequired().HasMaxLength(100);

        builder.HasOne(m => m.Tournament)
               .WithMany(t => t.Matches)
               .HasForeignKey(m => m.TournamentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class LeagueStandingConfiguration 
    : IEntityTypeConfiguration<LeagueStandingEntity>
{
    public void Configure(EntityTypeBuilder<LeagueStandingEntity> builder)
    {
        builder.HasKey(s => new { s.TournamentId, s.PlayerName });

        builder.Property(s => s.PlayerName).IsRequired().HasMaxLength(100);
        builder.Ignore(s => s.Points); // computed property

        builder.HasOne(s => s.Tournament)
               .WithMany(t => t.Standings)
               .HasForeignKey(s => s.TournamentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

// ═══════════════════════════════════════════════════════════
//  Event & related entities
// ═══════════════════════════════════════════════════════════

internal sealed class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Location).HasMaxLength(200);
        builder.Property(e => e.PlannedGames).HasMaxLength(2000);

        builder.HasOne(e => e.Creator)
               .WithMany(u => u.Events)
               .HasForeignKey(e => e.CreatorId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class EventAttendeeConfiguration 
    : IEntityTypeConfiguration<EventAttendeeEntity>
{
    public void Configure(EntityTypeBuilder<EventAttendeeEntity> builder)
    {
        builder.HasKey(a => new { a.EventId, a.AttendeeName });

        builder.Property(a => a.AttendeeName).IsRequired().HasMaxLength(100);

        builder.HasOne(a => a.Event)
               .WithMany(e => e.Attendees)
               .HasForeignKey(a => a.EventId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

// ═══════════════════════════════════════════════════════════
//  Notification
// ═══════════════════════════════════════════════════════════

internal sealed class NotificationConfiguration 
    : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.SenderName).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Type).IsRequired().HasMaxLength(50);

        builder.HasOne(n => n.TargetUser)
               .WithMany(u => u.Notifications)
               .HasForeignKey(n => n.TargetUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => new { n.TargetUserId, n.IsRead });
    }
}
