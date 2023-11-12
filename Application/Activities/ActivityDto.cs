using Application.Profiles;

namespace Application.Activities
{
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public string HostUsername { get; set; }
        public Boolean IsCancelled { get; set; }

        // We add a collection of profiles to the ActivityDto class
        public ICollection<AttendeeDto> Attendees { get; set; }
    }
}