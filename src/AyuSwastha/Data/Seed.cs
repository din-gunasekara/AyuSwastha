using System.Data.SQLite;
using AyuSwastha.Core;
using AyuSwastha.Models;

namespace AyuSwastha.Data
{
    /// <summary>Populates demo data the first time the database is created.</summary>
    internal static class Seed
    {
        public static void Run()
        {
            using (var conn = Database.CreateConnection())
            {
                if (CountRows(conn, "Users") == 0)
                {
                    InsertUser(conn, "admin", PasswordHasher.Hash("admin123"),
                        "System Administrator", UserRole.Admin);
                }

                if (CountRows(conn, "Doctors") == 0)
                {
                    InsertDoctor(conn, "D001", "Nayana Silva", "Panchakarma", "SLMC-1123", 1500);
                    InsertDoctor(conn, "D002", "Kamal Fernando", "Kayachikitsa", "SLMC-2045", 1200);
                }

                if (CountRows(conn, "Patients") == 0)
                {
                    InsertPatient(conn, "P001", "Nimal Perera", Gender.Male, "1980-05-14",
                        "077 123 4567", "No. 25, Temple Road, Kandy", DoshaType.VataPitta,
                        "Chronic lower-back pain; hypertension (controlled).",
                        "None reported.", "Sedentary office work; irregular sleep.");
                    InsertPatient(conn, "P002", "Kumari Jayasuriya", Gender.Female, "1992-11-02",
                        "071 987 6543", "No. 8, Lake View, Kurunegala", DoshaType.Kapha,
                        "Seasonal sinus congestion.", "Dust.", "Active; vegetarian diet.");
                }

                if (CountRows(conn, "Therapies") == 0)
                {
                    InsertTherapy(conn, "Abhyanga", "Full-body warm herbal-oil massage.", 60, 3500);
                    InsertTherapy(conn, "Shirodhara", "Continuous stream of warm oil on the forehead.", 45, 4500);
                    InsertTherapy(conn, "Panchakarma", "Five-step detoxification programme (per session).", 90, 6000);
                    InsertTherapy(conn, "Swedana", "Herbal steam therapy.", 30, 2000);
                }
            }
        }

        private static long CountRows(SQLiteConnection conn, string table)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM " + table + ";", conn))
                return (long)cmd.ExecuteScalar();
        }

        private static void InsertUser(SQLiteConnection conn, string username, string hash,
            string displayName, UserRole role)
        {
            using (var cmd = new SQLiteCommand(
                "INSERT INTO Users (Username, PasswordHash, DisplayName, Role, IsActive) " +
                "VALUES (@u, @p, @d, @r, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", hash);
                cmd.Parameters.AddWithValue("@d", displayName);
                cmd.Parameters.AddWithValue("@r", (int)role);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertDoctor(SQLiteConnection conn, string code, string name,
            string specialization, string license, decimal fee)
        {
            using (var cmd = new SQLiteCommand(
                "INSERT INTO Doctors (DoctorCode, FullName, Specialization, LicenseNo, ConsultationFee, IsActive) " +
                "VALUES (@c, @n, @s, @l, @f, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@c", code);
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@s", specialization);
                cmd.Parameters.AddWithValue("@l", license);
                cmd.Parameters.AddWithValue("@f", fee);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertPatient(SQLiteConnection conn, string code, string name,
            Gender gender, string dob, string phone, string address, DoshaType prakriti,
            string history, string allergies, string lifestyle)
        {
            using (var cmd = new SQLiteCommand(
                "INSERT INTO Patients (PatientCode, FullName, Gender, DateOfBirth, Phone, Address, " +
                "Prakriti, MedicalHistory, Allergies, Lifestyle) " +
                "VALUES (@c, @n, @g, @dob, @ph, @ad, @pr, @mh, @al, @li);", conn))
            {
                cmd.Parameters.AddWithValue("@c", code);
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@g", (int)gender);
                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@ph", phone);
                cmd.Parameters.AddWithValue("@ad", address);
                cmd.Parameters.AddWithValue("@pr", (int)prakriti);
                cmd.Parameters.AddWithValue("@mh", history);
                cmd.Parameters.AddWithValue("@al", allergies);
                cmd.Parameters.AddWithValue("@li", lifestyle);
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertTherapy(SQLiteConnection conn, string name, string desc,
            int minutes, decimal price)
        {
            using (var cmd = new SQLiteCommand(
                "INSERT INTO Therapies (Name, Description, DurationMinutes, Price) " +
                "VALUES (@n, @d, @m, @p);", conn))
            {
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@d", desc);
                cmd.Parameters.AddWithValue("@m", minutes);
                cmd.Parameters.AddWithValue("@p", price);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
