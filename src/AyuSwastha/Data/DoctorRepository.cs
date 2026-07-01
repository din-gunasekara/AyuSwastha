using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class DoctorRepository : IRepository<Doctor>
    {
        public IReadOnlyList<Doctor> GetAll()
        {
            var list = new List<Doctor>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("SELECT * FROM Doctors ORDER BY DoctorCode;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r));
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load doctors.", ex);
            }
            return list;
        }

        public Doctor GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("SELECT * FROM Doctors WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load doctor #" + id + ".", ex);
            }
        }

        public int Add(Doctor d)
        {
            Validate(d);
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Doctors (DoctorCode, FullName, Gender, DateOfBirth, Phone, Address, " +
                    "Specialization, LicenseNo, ConsultationFee, IsActive) " +
                    "VALUES (@code, @name, @gender, @dob, @phone, @addr, @spec, @lic, @fee, @active);" +
                    "SELECT last_insert_rowid();", conn))
                {
                    Bind(cmd, d);
                    d.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    return d.Id;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add doctor.", ex);
            }
        }

        public void Update(Doctor d)
        {
            Validate(d);
            if (d.Id <= 0) throw new ValidationException("Cannot update a doctor without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "UPDATE Doctors SET DoctorCode=@code, FullName=@name, Gender=@gender, DateOfBirth=@dob, " +
                    "Phone=@phone, Address=@addr, Specialization=@spec, LicenseNo=@lic, " +
                    "ConsultationFee=@fee, IsActive=@active WHERE Id=@id;", conn))
                {
                    Bind(cmd, d);
                    cmd.Parameters.AddWithValue("@id", d.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update doctor #" + d.Id + ".", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM Doctors WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete doctor #" + id + ".", ex);
            }
        }

        private static void Validate(Doctor d)
        {
            if (d == null) throw new ValidationException("Doctor is required.");
            if (string.IsNullOrWhiteSpace(d.FullName))
                throw new ValidationException("Doctor name is required.");
        }

        private static void Bind(SQLiteCommand cmd, Doctor d)
        {
            cmd.Parameters.AddWithValue("@code", (object)d.DoctorCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", d.FullName);
            cmd.Parameters.AddWithValue("@gender", (int)d.Gender);
            cmd.Parameters.AddWithValue("@dob",
                d.DateOfBirth.HasValue ? (object)d.DateOfBirth.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", (object)d.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addr", (object)d.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@spec", (object)d.Specialization ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@lic", (object)d.LicenseNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fee", d.ConsultationFee);
            cmd.Parameters.AddWithValue("@active", d.IsActive ? 1 : 0);
        }

        private static Doctor Map(IDataRecord r)
        {
            return new Doctor
            {
                Id = Convert.ToInt32(r["Id"]),
                DoctorCode = AsString(r, "DoctorCode"),
                FullName = AsString(r, "FullName"),
                Gender = (Gender)Convert.ToInt32(r["Gender"]),
                DateOfBirth = ParseDate(AsString(r, "DateOfBirth")),
                Phone = AsString(r, "Phone"),
                Address = AsString(r, "Address"),
                Specialization = AsString(r, "Specialization"),
                LicenseNo = AsString(r, "LicenseNo"),
                ConsultationFee = Convert.ToDecimal(r["ConsultationFee"]),
                IsActive = Convert.ToInt32(r["IsActive"]) != 0
            };
        }

        private static string AsString(IDataRecord r, string col)
        {
            object v = r[col];
            return v == DBNull.Value ? null : Convert.ToString(v);
        }

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.TryParse(value, out var d) ? d : (DateTime?)null;
        }
    }
}
