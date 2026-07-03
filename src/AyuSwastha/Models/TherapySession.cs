using System;

namespace AyuSwastha.Models
{
    /// <summary>A scheduled instance of a <see cref="Therapy"/> for a patient.</summary>
    public class TherapySession
    {
        public int Id { get; set; }
        public int TherapyId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public TherapyStatus Status { get; set; } = TherapyStatus.Booked;
        public string Notes { get; set; }

        public string TherapySessionCode => $"THR-{Id:D4}";

        // Display helpers populated via joins.
        public string TherapyName { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }

        public override string ToString()
        {
            string pName = string.IsNullOrEmpty(PatientName) ? $"Patient #{PatientId}" : PatientName;
            string tName = string.IsNullOrEmpty(TherapyName) ? $"Therapy #{TherapyId}" : TherapyName;
            return $"THR-{Id:D4} | {pName} - {tName}";
        }
    }
}
