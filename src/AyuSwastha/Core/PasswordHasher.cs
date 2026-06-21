using System;
using System.Security.Cryptography;
using System.Text;

namespace AyuSwastha.Core
{
    /// <summary>
    /// Salted SHA-256 password hashing. Stored form is "salt:hash" (both Base64).
    /// Adequate for a coursework demo; a production system would use PBKDF2/bcrypt.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash = ComputeHash(password, salt);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored) || !stored.Contains(":"))
                return false;

            string[] parts = stored.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expected = Convert.FromBase64String(parts[1]);
            byte[] actual = ComputeHash(password, salt);

            return FixedTimeEquals(expected, actual);
        }

        private static byte[] ComputeHash(string password, byte[] salt)
        {
            byte[] pwd = Encoding.UTF8.GetBytes(password);
            byte[] combined = new byte[salt.Length + pwd.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(pwd, 0, combined, salt.Length, pwd.Length);

            using (var sha = SHA256.Create())
                return sha.ComputeHash(combined);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
