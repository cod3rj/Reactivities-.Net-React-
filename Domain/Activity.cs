namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public Boolean IsCancelled { get; set; }

        // We add a list of attendees to the Activity class
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();
        // We add a list of comments to the Activity class
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}