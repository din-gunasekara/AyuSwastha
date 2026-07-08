using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using AyuSwastha.Core;

namespace AyuSwastha.Data
{
    /// <summary>
    /// Owns the SQLite connection, creates the schema on first run, and seeds demo data.
    /// The database file lives next to the executable.
    /// </summary>
    public static class Database
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    string file = ConfigurationManager.AppSettings["DatabaseFile"];
                    if (string.IsNullOrWhiteSpace(file)) file = "AyuSwastha.db";
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                    _connectionString = "Data Source=" + path + ";Version=3;";
                }
                return _connectionString;
            }
        }

        /// <summary>Opens a fresh connection. Callers own disposal (use a <c>using</c>).</summary>
        public static SQLiteConnection CreateConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn))
                pragma.ExecuteNonQuery();
            return conn;
        }

        /// <summary>Creates tables (if missing) and seeds demo data. Safe to call repeatedly.</summary>
        public static void Initialize()
        {
            try
            {
                using (var conn = CreateConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SchemaSql;
                    cmd.ExecuteNonQuery();
                }
                Seed.Run();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Database initialization failed.", ex);
            }
        }

        private const string SchemaSql = @"
CREATE TABLE IF NOT EXISTS Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Username     TEXT    NOT NULL UNIQUE,
    PasswordHash TEXT    NOT NULL,
    DisplayName  TEXT,
    Role         INTEGER NOT NULL,
    DoctorId     INTEGER,
    IsActive     INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Doctors (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    DoctorCode      TEXT    UNIQUE,
    FullName        TEXT    NOT NULL,
    Gender          INTEGER NOT NULL DEFAULT 0,
    DateOfBirth     TEXT,
    Phone           TEXT,
    Address         TEXT,
    Specialization  TEXT,
    LicenseNo       TEXT,
    ConsultationFee REAL    NOT NULL DEFAULT 0,
    IsActive        INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Patients (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientCode    TEXT    UNIQUE,
    FullName       TEXT    NOT NULL,
    Gender         INTEGER NOT NULL DEFAULT 0,
    DateOfBirth    TEXT,
    Phone          TEXT,
    Address        TEXT,
    Prakriti       INTEGER NOT NULL DEFAULT 0,
    MedicalHistory TEXT,
    Allergies      TEXT,
    Lifestyle      TEXT,
    Notes          TEXT,
    PhotoPath      TEXT
);

CREATE TABLE IF NOT EXISTS Appointments (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientId   INTEGER NOT NULL,
    DoctorId    INTEGER NOT NULL,
    ScheduledAt TEXT    NOT NULL,
    Reason      TEXT,
    Status      INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (PatientId) REFERENCES Patients(Id),
    FOREIGN KEY (DoctorId)  REFERENCES Doctors(Id)
);

CREATE TABLE IF NOT EXISTS Therapies (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Name            TEXT    NOT NULL,
    Description     TEXT,
    DurationMinutes INTEGER NOT NULL DEFAULT 60,
    Price           REAL    NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS TherapySessions (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    TherapyId   INTEGER NOT NULL,
    PatientId   INTEGER NOT NULL,
    DoctorId    INTEGER,
    ScheduledAt TEXT    NOT NULL,
    Status      INTEGER NOT NULL DEFAULT 0,
    Notes       TEXT,
    FOREIGN KEY (TherapyId) REFERENCES Therapies(Id),
    FOREIGN KEY (PatientId) REFERENCES Patients(Id)
);

CREATE TABLE IF NOT EXISTS Invoices (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientId  INTEGER NOT NULL,
    IssuedAt   TEXT    NOT NULL,
    Status     INTEGER NOT NULL DEFAULT 0,
    SubTotal   REAL    NOT NULL DEFAULT 0,
    Tax        REAL    NOT NULL DEFAULT 0,
    Total      REAL    NOT NULL DEFAULT 0,
    Notes      TEXT,
    FOREIGN KEY (PatientId) REFERENCES Patients(Id)
);

CREATE TABLE IF NOT EXISTS InvoiceItems (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    InvoiceId   INTEGER NOT NULL,
    Description TEXT    NOT NULL,
    Quantity    INTEGER NOT NULL DEFAULT 1,
    UnitPrice   REAL    NOT NULL DEFAULT 0,
    LineTotal   REAL    NOT NULL DEFAULT 0,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE
);
";
    }
}
