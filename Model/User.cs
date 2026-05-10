using System;
using System.Security.Cryptography;
using System.Text;

namespace Boardgame.Model
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public static string HashPassword(string password) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        public bool CheckPassword(string password) =>
            PasswordHash == HashPassword(password);
    }
}
