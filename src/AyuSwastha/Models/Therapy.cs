namespace AyuSwastha.Models
{
    /// <summary>A therapy offered by the clinic, e.g. Abhyanga, Shirodhara, Panchakarma.</summary>
    public class Therapy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }

        public override string ToString() => Name;
    }
}
