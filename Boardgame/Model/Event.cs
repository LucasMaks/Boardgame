using System;
using System.Collections.Generic;

namespace Boardgame.Model
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);
        public TimeSpan StartTime { get; set; } = new(18, 0, 0);
        public TimeSpan EndTime { get; set; } = new(22, 0, 0);
        public EventStatus Status { get; set; } = EventStatus.Planned;
        public List<EventAttendee> Attendees { get; set; } = [];
        public List<string> PlannedGames { get; set; } = [];
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class EventAttendee
    {
        public string Name { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Pending;
        public DateTime ResponseDate { get; set; } = DateTime.Now;
    }

    public enum EventStatus
    {
        Planned,
        Confirmed,
        InProgress,
        Completed,
        Cancelled
    }

    public enum AttendanceStatus
    {
        Pending,
        Confirmed,
        Declined,
        Maybe
    }
}
