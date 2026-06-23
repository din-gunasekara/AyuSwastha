namespace AyuSwastha.Models
{
    /// <summary>A login account. Optionally linked to a Doctor record.</summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public UserRole Role { get; set; }
        public int? DoctorId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
