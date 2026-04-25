using Boardgame.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Boardgame.Services
{
    public static class NotificationService
    {
        private static readonly string SaveFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveGame");
        private static readonly string FilePath =
            Path.Combine(SaveFolder, "Notifications.json");

        // --- Add helpers ---

        public static void AddWelcome(string targetUsername, string displayName)
        {
            Add(new Notification
            {
                TargetUsername = targetUsername,
                SenderName = "System",
                Title = "Witaj w BoardGames! 🎲",
                Message = $"Cześć {displayName}! Twoje konto zostało utworzone. Miłej gry!",
                Type = NotificationType.Welcome
            });
        }

        public static void AddEventInvite(string targetUsername, string eventName, string senderName)
        {
            Add(new Notification
            {
                TargetUsername = targetUsername,
                SenderName = senderName,
                Title = "Zaproszenie na wydarzenie",
                Message = $"{senderName} dodal Cie do wydarzenia \"{eventName}\".",
                Type = NotificationType.AddedToEvent
            });
        }

        public static void AddTournamentInvite(string targetUsername, string tournamentName, string senderName)
        {
            Add(new Notification
            {
                TargetUsername = targetUsername,
                SenderName = senderName,
                Title = "Dodano do turnieju",
                Message = $"{senderName} dodal Cie do turnieju \"{tournamentName}\".",
                Type = NotificationType.AddedToTournament
            });
        }

        public static void AddSystemMessage(string targetUsername, string title, string message)
        {
            Add(new Notification
            {
                TargetUsername = targetUsername,
                SenderName = "System",
                Title = title,
                Message = message,
                Type = NotificationType.System
            });
        }

        // --- Query helpers ---

        public static List<Notification> GetForUser(string username) =>
            LoadAll()
                .Where(n => n.TargetUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(n => n.CreatedDate)
                .ToList();

        public static List<Notification> GetUnreadForUser(string username) =>
            GetForUser(username).Where(n => !n.IsRead).ToList();

        public static int GetUnreadCount(string username) =>
            GetUnreadForUser(username).Count;

        public static void MarkAsRead(Guid id)
        {
            var all = LoadAll();
            var n = all.FirstOrDefault(x => x.Id == id);
            if (n is not null)
            {
                n.IsRead = true;
                SaveAll(all);
            }
        }

        public static void MarkAllAsRead(string username)
        {
            var all = LoadAll();
            foreach (var n in all.Where(n =>
                n.TargetUsername.Equals(username, StringComparison.OrdinalIgnoreCase)))
                n.IsRead = true;
            SaveAll(all);
        }

        // --- Private ---

        private static void Add(Notification n)
        {
            var all = LoadAll();
            all.Add(n);
            SaveAll(all);
        }

        private static List<Notification> LoadAll()
        {
            if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
            if (!File.Exists(FilePath)) return [];
            try
            {
                return JsonConvert.DeserializeObject<List<Notification>>(
                    File.ReadAllText(FilePath)) ?? [];
            }
            catch { return []; }
        }

        private static void SaveAll(List<Notification> notifications)
        {
            if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
            File.WriteAllText(FilePath,
                JsonConvert.SerializeObject(notifications, Formatting.Indented));
        }
    }
}
