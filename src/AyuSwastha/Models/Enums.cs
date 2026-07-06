namespace AyuSwastha.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    /// <summary>Ayurvedic body constitution (Prakriti). Duals allowed.</summary>
    public enum DoshaType
    {
        Unknown,
        Vata,
        Pitta,
        Kapha,
        VataPitta,
        PittaKapha,
        VataKapha,
        Tridoshic
    }

    public enum UserRole
    {
        Admin,
        Doctor,
        Receptionist
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }

    public enum TherapyStatus
    {
        Booked,
        InProgress,
        Completed,
        Cancelled
    }

    public enum PaymentStatus
    {
        Unpaid,
        PartiallyPaid,
        Paid
    }
}
