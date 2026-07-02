using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class PrescriptionRepository : IRepository<Prescription>
    {
        private const string SelectSql =
            "SELECT p.*, pat.FullName AS PatientName, d.FullName AS DoctorName " +
            "FROM Prescriptions p " +
            "JOIN Patients pat ON pat.Id = p.PatientId " +
            "JOIN Doctors d ON d.Id = p.DoctorId ";

        public IReadOnlyList<Prescription> GetAll()
        {
            var list = new List<Prescription>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "ORDER BY p.IssuedOn DESC;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r, conn)); // N+1 but fine for local SQLite demo
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load prescriptions.", ex);
            }
            return list;
        }

        public Prescription GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "WHERE p.Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r, conn) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load prescription #" + id + ".", ex);
            }
        }

        public int Add(Prescription p)
        {
            Validate(p);
            try
            {
                using (var conn = Database.CreateConnection())
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Prescriptions (PatientId, DoctorId, IssuedOn, GeneralInstructions) " +
                        "VALUES (@pid, @did, @when, @instr);SELECT last_insert_rowid();", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@pid", p.PatientId);
                        cmd.Parameters.AddWithValue("@did", p.DoctorId);
                        cmd.Parameters.AddWithValue("@when", p.IssuedOn.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@instr", (object)p.GeneralInstructions ?? DBNull.Value);
                        p.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    SaveItems(p, conn, trans);
                    trans.Commit();
                    return p.Id;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add prescription.", ex);
            }
        }

        public void Update(Prescription p)
        {
            Validate(p);
            if (p.Id <= 0) throw new ValidationException("Cannot update without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Prescriptions SET PatientId=@pid, DoctorId=@did, IssuedOn=@when, " +
                        "GeneralInstructions=@instr WHERE Id=@id;", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@pid", p.PatientId);
                        cmd.Parameters.AddWithValue("@did", p.DoctorId);
                        cmd.Parameters.AddWithValue("@when", p.IssuedOn.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@instr", (object)p.GeneralInstructions ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", p.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand("DELETE FROM PrescriptionItems WHERE PrescriptionId=@id;", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", p.Id);
                        cmd.ExecuteNonQuery();
                    }

                    SaveItems(p, conn, trans);
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update prescription #" + p.Id + ".", ex);
            }
        }

        private static void SaveItems(Prescription p, SQLiteConnection conn, SQLiteTransaction trans)
        {
            foreach (var item in p.Items)
            {
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO PrescriptionItems (PrescriptionId, HerbName, Dosage, Duration, Instructions) " +
                    "VALUES (@pid, @herb, @dose, @dur, @instr);", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@pid", p.Id);
                    cmd.Parameters.AddWithValue("@herb", item.HerbName);
                    cmd.Parameters.AddWithValue("@dose", (object)item.Dosage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@dur", (object)item.Duration ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@instr", (object)item.Instructions ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM Prescriptions WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete prescription #" + id + ".", ex);
            }
        }

        private static void Validate(Prescription p)
        {
            if (p == null) throw new ValidationException("Prescription is required.");
            if (p.PatientId <= 0) throw new ValidationException("A patient must be selected.");
            if (p.DoctorId <= 0) throw new ValidationException("A prescribing doctor must be selected.");
        }

        private static Prescription Map(IDataRecord r, SQLiteConnection conn)
        {
            var p = new Prescription
            {
                Id = Convert.ToInt32(r["Id"]),
                PatientId = Convert.ToInt32(r["PatientId"]),
                DoctorId = Convert.ToInt32(r["DoctorId"]),
                IssuedOn = DateTime.Parse(Convert.ToString(r["IssuedOn"])),
                GeneralInstructions = r["GeneralInstructions"] == DBNull.Value ? null : Convert.ToString(r["GeneralInstructions"]),
                PatientName = Convert.ToString(r["PatientName"]),
                DoctorName = Convert.ToString(r["DoctorName"])
            };

            using (var cmd = new SQLiteCommand("SELECT * FROM PrescriptionItems WHERE PrescriptionId=@id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", p.Id);
                using (var ir = cmd.ExecuteReader())
                {
                    while (ir.Read())
                    {
                        p.Items.Add(new PrescriptionItem
                        {
                            Id = Convert.ToInt32(ir["Id"]),
                            PrescriptionId = p.Id,
                            HerbName = Convert.ToString(ir["HerbName"]),
                            Dosage = ir["Dosage"] == DBNull.Value ? null : Convert.ToString(ir["Dosage"]),
                            Duration = ir["Duration"] == DBNull.Value ? null : Convert.ToString(ir["Duration"]),
                            Instructions = ir["Instructions"] == DBNull.Value ? null : Convert.ToString(ir["Instructions"])
                        });
                    }
                }
            }

            return p;
        }
    }
}
