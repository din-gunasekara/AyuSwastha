using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        // Joins in patient/doctor names for display.
        private const string SelectSql =
            "SELECT a.*, p.FullName AS PatientName, d.FullName AS DoctorName " +
            "FROM Appointments a " +
            "JOIN Patients p ON p.Id = a.PatientId " +
            "JOIN Doctors  d ON d.Id = a.DoctorId ";

        public IReadOnlyList<Appointment> GetAll()
        {
            var list = new List<Appointment>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "ORDER BY a.ScheduledAt DESC;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r));
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load appointments.", ex);
            }
            return list;
        }

        /// <summary>Appointments scheduled on a given calendar day.</summary>
        public IReadOnlyList<Appointment> GetForDate(DateTime day)
        {
            var list = new List<Appointment>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    SelectSql + "WHERE date(a.ScheduledAt) = @day ORDER BY a.ScheduledAt;", conn))
                {
                    cmd.Parameters.AddWithValue("@day", day.ToString("yyyy-MM-dd"));
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(Map(r));
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load appointments for " + day.ToShortDateString() + ".", ex);
            }
            return list;
        }

        public Appointment GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "WHERE a.Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load appointment #" + id + ".", ex);
            }
        }

        public int Add(Appointment a)
        {
            Validate(a);
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Appointments (PatientId, DoctorId, ScheduledAt, Reason, Status) " +
                    "VALUES (@pid, @did, @when, @reason, @status);SELECT last_insert_rowid();", conn))
                {
                    Bind(cmd, a);
                    a.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    return a.Id;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add appointment.", ex);
            }
        }

        public void Update(Appointment a)
        {
            Validate(a);
            if (a.Id <= 0) throw new ValidationException("Cannot update an appointment without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "UPDATE Appointments SET PatientId=@pid, DoctorId=@did, ScheduledAt=@when, " +
                    "Reason=@reason, Status=@status WHERE Id=@id;", conn))
                {
                    Bind(cmd, a);
                    cmd.Parameters.AddWithValue("@id", a.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update appointment #" + a.Id + ".", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM Appointments WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete appointment #" + id + ".", ex);
            }
        }

        private static void Validate(Appointment a)
        {
            if (a == null) throw new ValidationException("Appointment is required.");
            if (a.PatientId <= 0) throw new ValidationException("A patient must be selected.");
            if (a.DoctorId <= 0) throw new ValidationException("A doctor must be selected.");
        }

        private static void Bind(SQLiteCommand cmd, Appointment a)
        {
            cmd.Parameters.AddWithValue("@pid", a.PatientId);
            cmd.Parameters.AddWithValue("@did", a.DoctorId);
            cmd.Parameters.AddWithValue("@when", a.ScheduledAt.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("@reason", (object)a.Reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", (int)a.Status);
        }

        private static Appointment Map(IDataRecord r)
        {
            return new Appointment
            {
                Id = Convert.ToInt32(r["Id"]),
                PatientId = Convert.ToInt32(r["PatientId"]),
                DoctorId = Convert.ToInt32(r["DoctorId"]),
                ScheduledAt = DateTime.Parse(Convert.ToString(r["ScheduledAt"])),
                Reason = r["Reason"] == DBNull.Value ? null : Convert.ToString(r["Reason"]),
                Status = (AppointmentStatus)Convert.ToInt32(r["Status"]),
                PatientName = Convert.ToString(r["PatientName"]),
                DoctorName = Convert.ToString(r["DoctorName"])
            };
        }
    }
}
