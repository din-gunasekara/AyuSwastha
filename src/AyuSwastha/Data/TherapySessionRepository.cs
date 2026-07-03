using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class TherapySessionRepository : IRepository<TherapySession>
    {
        private const string SelectSql =
            "SELECT s.*, t.Name AS TherapyName, p.FullName AS PatientName, d.FullName AS DoctorName " +
            "FROM TherapySessions s " +
            "JOIN Therapies t ON t.Id = s.TherapyId " +
            "JOIN Patients p ON p.Id = s.PatientId " +
            "LEFT JOIN Doctors d ON d.Id = s.DoctorId ";

        public IReadOnlyList<TherapySession> GetAll()
        {
            var list = new List<TherapySession>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "ORDER BY s.ScheduledAt DESC;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r));
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load therapy sessions.", ex);
            }
            return list;
        }

        public TherapySession GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "WHERE s.Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load therapy session #" + id + ".", ex);
            }
        }

        public int Add(TherapySession s)
        {
            Validate(s);
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO TherapySessions (TherapyId, PatientId, DoctorId, ScheduledAt, Status, Notes) " +
                    "VALUES (@tid, @pid, @did, @when, @status, @notes);SELECT last_insert_rowid();", conn))
                {
                    Bind(cmd, s);
                    s.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    return s.Id;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add therapy session.", ex);
            }
        }

        public void Update(TherapySession s)
        {
            Validate(s);
            if (s.Id <= 0) throw new ValidationException("Cannot update without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "UPDATE TherapySessions SET TherapyId=@tid, PatientId=@pid, DoctorId=@did, ScheduledAt=@when, " +
                    "Status=@status, Notes=@notes WHERE Id=@id;", conn))
                {
                    Bind(cmd, s);
                    cmd.Parameters.AddWithValue("@id", s.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update therapy session #" + s.Id + ".", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM TherapySessions WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete therapy session #" + id + ".", ex);
            }
        }

        public IReadOnlyList<Therapy> GetAllTherapies()
        {
            var list = new List<Therapy>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("SELECT * FROM Therapies ORDER BY Name;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(new Therapy {
                        Id = Convert.ToInt32(r["Id"]),
                        Name = Convert.ToString(r["Name"]),
                        Description = r["Description"] == DBNull.Value ? null : Convert.ToString(r["Description"]),
                        DurationMinutes = Convert.ToInt32(r["DurationMinutes"]),
                        Price = Convert.ToDecimal(r["Price"])
                    });
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load therapies.", ex);
            }
            return list;
        }

        private static void Validate(TherapySession s)
        {
            if (s == null) throw new ValidationException("Therapy session is required.");
            if (s.TherapyId <= 0) throw new ValidationException("A therapy must be selected.");
            if (s.PatientId <= 0) throw new ValidationException("A patient must be selected.");
        }

        private static void Bind(SQLiteCommand cmd, TherapySession s)
        {
            cmd.Parameters.AddWithValue("@tid", s.TherapyId);
            cmd.Parameters.AddWithValue("@pid", s.PatientId);
            cmd.Parameters.AddWithValue("@did", s.DoctorId > 0 ? (object)s.DoctorId : DBNull.Value);
            cmd.Parameters.AddWithValue("@when", s.ScheduledAt.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("@status", (int)s.Status);
            cmd.Parameters.AddWithValue("@notes", (object)s.Notes ?? DBNull.Value);
        }

        private static TherapySession Map(IDataRecord r)
        {
            return new TherapySession
            {
                Id = Convert.ToInt32(r["Id"]),
                TherapyId = Convert.ToInt32(r["TherapyId"]),
                PatientId = Convert.ToInt32(r["PatientId"]),
                DoctorId = r["DoctorId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["DoctorId"]),
                ScheduledAt = DateTime.Parse(Convert.ToString(r["ScheduledAt"])),
                Status = (TherapyStatus)Convert.ToInt32(r["Status"]),
                Notes = r["Notes"] == DBNull.Value ? null : Convert.ToString(r["Notes"]),
                TherapyName = Convert.ToString(r["TherapyName"]),
                PatientName = Convert.ToString(r["PatientName"]),
                DoctorName = r["DoctorName"] == DBNull.Value ? null : Convert.ToString(r["DoctorName"])
            };
        }
    }
}
