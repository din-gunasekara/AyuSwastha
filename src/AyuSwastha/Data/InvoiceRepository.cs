using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    public class InvoiceRepository : IRepository<Invoice>
    {
        private const string SelectSql =
            "SELECT i.*, p.FullName AS PatientName " +
            "FROM Invoices i " +
            "JOIN Patients p ON p.Id = i.PatientId ";

        public IReadOnlyList<Invoice> GetAll()
        {
            var list = new List<Invoice>();
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "ORDER BY i.IssuedAt DESC;", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(Map(r, conn)); // N+1 but fine for local demo
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load invoices.", ex);
            }
            return list;
        }

        public Invoice GetById(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand(SelectSql + "WHERE i.Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var r = cmd.ExecuteReader())
                        return r.Read() ? Map(r, conn) : null;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to load invoice #" + id + ".", ex);
            }
        }

        public int Add(Invoice i)
        {
            Validate(i);
            try
            {
                using (var conn = Database.CreateConnection())
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Invoices (PatientId, IssuedAt, Status, SubTotal, Tax, Total, Notes) " +
                        "VALUES (@pid, @when, @status, @sub, @tax, @tot, @notes);SELECT last_insert_rowid();", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@pid", i.PatientId);
                        cmd.Parameters.AddWithValue("@when", i.IssuedAt.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@status", (int)i.Status);
                        cmd.Parameters.AddWithValue("@sub", i.SubTotal);
                        cmd.Parameters.AddWithValue("@tax", i.Tax);
                        cmd.Parameters.AddWithValue("@tot", i.Total);
                        cmd.Parameters.AddWithValue("@notes", (object)i.Notes ?? DBNull.Value);
                        i.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    SaveItems(i, conn, trans);
                    trans.Commit();
                    return i.Id;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to add invoice.", ex);
            }
        }

        public void Update(Invoice i)
        {
            Validate(i);
            if (i.Id <= 0) throw new ValidationException("Cannot update without an id.");
            try
            {
                using (var conn = Database.CreateConnection())
                using (var trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Invoices SET PatientId=@pid, IssuedAt=@when, Status=@status, " +
                        "SubTotal=@sub, Tax=@tax, Total=@tot, Notes=@notes WHERE Id=@id;", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@pid", i.PatientId);
                        cmd.Parameters.AddWithValue("@when", i.IssuedAt.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@status", (int)i.Status);
                        cmd.Parameters.AddWithValue("@sub", i.SubTotal);
                        cmd.Parameters.AddWithValue("@tax", i.Tax);
                        cmd.Parameters.AddWithValue("@tot", i.Total);
                        cmd.Parameters.AddWithValue("@notes", (object)i.Notes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", i.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand("DELETE FROM InvoiceItems WHERE InvoiceId=@id;", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@id", i.Id);
                        cmd.ExecuteNonQuery();
                    }

                    SaveItems(i, conn, trans);
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to update invoice #" + i.Id + ".", ex);
            }
        }

        private static void SaveItems(Invoice i, SQLiteConnection conn, SQLiteTransaction trans)
        {
            foreach (var item in i.Items)
            {
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO InvoiceItems (InvoiceId, Description, Quantity, UnitPrice, LineTotal) " +
                    "VALUES (@iid, @desc, @qty, @up, @lt);", conn, trans))
                {
                    cmd.Parameters.AddWithValue("@iid", i.Id);
                    cmd.Parameters.AddWithValue("@desc", item.Description);
                    cmd.Parameters.AddWithValue("@qty", item.Quantity);
                    cmd.Parameters.AddWithValue("@up", item.UnitPrice);
                    cmd.Parameters.AddWithValue("@lt", item.LineTotal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var conn = Database.CreateConnection())
                using (var cmd = new SQLiteCommand("DELETE FROM Invoices WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to delete invoice #" + id + ".", ex);
            }
        }

        private static void Validate(Invoice i)
        {
            if (i == null) throw new ValidationException("Invoice is required.");
            if (i.PatientId <= 0) throw new ValidationException("A patient must be selected.");
        }

        private static Invoice Map(IDataRecord r, SQLiteConnection conn)
        {
            var i = new Invoice
            {
                Id = Convert.ToInt32(r["Id"]),
                PatientId = Convert.ToInt32(r["PatientId"]),
                IssuedAt = DateTime.Parse(Convert.ToString(r["IssuedAt"])),
                Status = (InvoiceStatus)Convert.ToInt32(r["Status"]),
                SubTotal = Convert.ToDouble(r["SubTotal"]),
                Tax = Convert.ToDouble(r["Tax"]),
                Total = Convert.ToDouble(r["Total"]),
                Notes = r["Notes"] == DBNull.Value ? null : Convert.ToString(r["Notes"]),
                PatientName = Convert.ToString(r["PatientName"])
            };

            using (var cmd = new SQLiteCommand("SELECT * FROM InvoiceItems WHERE InvoiceId=@id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", i.Id);
                using (var ir = cmd.ExecuteReader())
                {
                    while (ir.Read())
                    {
                        i.Items.Add(new InvoiceItem
                        {
                            Id = Convert.ToInt32(ir["Id"]),
                            InvoiceId = i.Id,
                            Description = Convert.ToString(ir["Description"]),
                            Quantity = Convert.ToInt32(ir["Quantity"]),
                            UnitPrice = Convert.ToDouble(ir["UnitPrice"]),
                            LineTotal = Convert.ToDouble(ir["LineTotal"])
                        });
                    }
                }
            }

            return i;
        }
    }
}
