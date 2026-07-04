using System;

namespace AyuSwastha.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Reason { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        // Populated by joins for display convenience (not persisted directly).
        public string PatientName { get; set; }
        public string DoctorName { get; set; }

        public string AppointmentCode => $"APT-{Id:D4}";

        public override string ToString()
        {
            string pName = string.IsNullOrEmpty(PatientName) ? $"Patient #{PatientId}" : PatientName;
            return $"APT-{Id:D4} | {pName} - {ScheduledAt:MMM dd, HH:mm}";
        }
    }
}
