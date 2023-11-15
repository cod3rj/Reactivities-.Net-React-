using System.Text.Json.Serialization;

namespace Application.Profiles
{
    // Data Transfer Object (DTO) representing user activity details
    public class UserActivityDto
    {
        // Unique identifier for the activity
        public Guid Id { get; set; }

        // Title of the activity
        public string Title { get; set; }

        // Category of the activity
        public string Category { get; set; }

        // Date and time of the activity
        public DateTime Date { get; set; }

        // Property representing the username of the host
        // JsonIgnore is used to exclude this property when serializing the object to JSON
        [JsonIgnore]
        public string HostUsername { get; set; }
    }
}
