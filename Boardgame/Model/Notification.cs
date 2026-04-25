using System;

namespace Boardgame.Model
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TargetUsername { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.System;
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum NotificationType
    {
        Welcome,
        AddedToEvent,
        AddedToTournament,
        ExchangeOffer,
        System
    }
}
