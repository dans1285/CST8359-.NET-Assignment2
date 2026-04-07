namespace EventSignup.Models
{
    public class Attendee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EventId { get; set; }
    }
}
