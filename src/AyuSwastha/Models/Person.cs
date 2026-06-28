using System;

namespace AyuSwastha.Models
{
    /// <summary>
    /// Abstract base for people in the system. Demonstrates <b>abstraction</b> and
    /// is the root of the <b>inheritance</b> hierarchy (Patient, Doctor).
    /// </summary>
    public abstract class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        /// <summary>Age in whole years, or null when DOB is unknown.</summary>
        public int? Age
        {
            get
            {
                if (DateOfBirth == null) return null;
                DateTime dob = DateOfBirth.Value;
                DateTime today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>Role label shown in the UI. Overridden by each subclass (polymorphism).</summary>
        public abstract string DisplayRole { get; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(FullName) ? DisplayRole : FullName;
        }
    }
}
