using Boardgame.Data.Entities;
using System;

namespace Boardgame.Services
{
    /// <summary>Holds the currently logged-in user for the session.</summary>
    public static class Session
    {
        public static UserEntity? CurrentUser { get; set; }
        public static Guid CurrentUserId => CurrentUser?.Id ?? Guid.Empty;
        public static string CurrentEmail => CurrentUser?.Email ?? string.Empty;
        public static string CurrentDisplayName => CurrentUser?.DisplayName ?? string.Empty;
    }
}
