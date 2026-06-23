using System;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Services
{
    /// <summary>Authenticates users against the Users table.</summary>
    public class AuthService
    {
        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ValidationException("Username is required.");
            if (string.IsNullOrEmpty(password))
                throw new ValidationException("Password is required.");

            User user;
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "SELECT * FROM Users WHERE Username = @u;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username.Trim());
                    using (var r = cmd.ExecuteReader())
                        user = r.Read() ? Map(r) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Login failed while reading the user account.", ex);
            }

            if (user == null || !PasswordHasher.Verify(password, user.PasswordHash))
                throw new AuthenticationException("Invalid username or password.");
            if (!user.IsActive)
                throw new AuthenticationException("This account has been deactivated.");

            Session.CurrentUser = user;
            return user;
        }

        private static User Map(IDataRecord r)
        {
            return new User
            {
                Id = Convert.ToInt32(r["Id"]),
                Username = Convert.ToString(r["Username"]),
                PasswordHash = Convert.ToString(r["PasswordHash"]),
                DisplayName = r["DisplayName"] == DBNull.Value ? null : Convert.ToString(r["DisplayName"]),
                Role = (UserRole)Convert.ToInt32(r["Role"]),
                DoctorId = r["DoctorId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DoctorId"]),
                IsActive = Convert.ToInt32(r["IsActive"]) != 0
            };
        }
    }
}
