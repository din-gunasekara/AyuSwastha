using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class PatientRepository : IRepository<Patient>
    {
        public IReadOnlyList<Patient> GetAll()
        {
            var list = new List<Patient>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("SELECT * FROM Patients ORDER BY PatientCode;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r));
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load patients.", ex);
            }
            return list;
        }

        public Patient GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("SELECT * FROM Patients WHERE Id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load patient #" + id + ".", ex);
            }
        }

        public int Add(Patient p)
        {
            Validate(p);
            try
            {
                using (var conn = Database.CreateConnection())
                {
                    if (string.IsNullOrWhiteSpace(p.PatientCode))
                        p.PatientCode = NextCode(conn);

                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Patients (PatientCode, FullName, Gender, DateOfBirth, Phone, Address, " +
                        "Prakriti, MedicalHistory, Allergies, Lifestyle, Notes, PhotoPath) " +
                        "VALUES (@code, @name, @gender, @dob, @phone, @addr, @prakriti, @mh, @al, @li, @notes, @photo);" +
                        "SELECT last_insert_rowid();", conn))
                    {
                        Bind(cmd, p);
                        p.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        return p.Id;
                    }
                }
            }
            catch (DataAccessException) { throw; }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add patient.", ex);
            }
        }

        public void Update(Patient p)
        {
            Validate(p);
            if (p.Id <= 0) throw new ValidationException("Cannot update a patient without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(
                    "UPDATE Patients SET PatientCode=@code, FullName=@name, Gender=@gender, DateOfBirth=@dob, " +
                    "Phone=@phone, Address=@addr, Prakriti=@prakriti, MedicalHistory=@mh, Allergies=@al, " +
                    "Lifestyle=@li, Notes=@notes, PhotoPath=@photo WHERE Id=@id;", conn))
                {
                    Bind(cmd, p);
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update patient #" + p.Id + ".", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM Patients WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete patient #" + id + ".", ex);
            }
        }

        private static void Validate(Patient p)
        {
            if (p == null) throw new ValidationException("Patient is required.");
            if (string.IsNullOrWhiteSpace(p.FullName))
                throw new ValidationException("Full name is required.");
        }

        private static string NextCode(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Patients;", conn))
            {
                long count = (long)cmd.ExecuteScalar();
                return "P" + (count + 1).ToString("D3");
            }
        }

        private static void Bind(SQLiteCommand cmd, Patient p)
        {
            cmd.Parameters.AddWithValue("@code", (object)p.PatientCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", p.FullName);
            cmd.Parameters.AddWithValue("@gender", (int)p.Gender);
            cmd.Parameters.AddWithValue("@dob",
                p.DateOfBirth.HasValue ? (object)p.DateOfBirth.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", (object)p.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@addr", (object)p.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prakriti", (int)p.Prakriti);
            cmd.Parameters.AddWithValue("@mh", (object)p.MedicalHistory ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@al", (object)p.Allergies ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@li", (object)p.Lifestyle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", (object)p.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@photo", (object)p.PhotoPath ?? DBNull.Value);
        }

        private static Patient Map(IDataRecord r)
        {
            return new Patient
            {
                Id = Convert.ToInt32(r["Id"]),
                PatientCode = AsString(r, "PatientCode"),
                FullName = AsString(r, "FullName"),
                Gender = (Gender)Convert.ToInt32(r["Gender"]),
                DateOfBirth = ParseDate(AsString(r, "DateOfBirth")),
                Phone = AsString(r, "Phone"),
                Address = AsString(r, "Address"),
                Prakriti = (DoshaType)Convert.ToInt32(r["Prakriti"]),
                MedicalHistory = AsString(r, "MedicalHistory"),
                Allergies = AsString(r, "Allergies"),
                Lifestyle = AsString(r, "Lifestyle"),
                Notes = AsString(r, "Notes"),
                PhotoPath = AsString(r, "PhotoPath")
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
