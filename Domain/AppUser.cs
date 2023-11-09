using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        // We add a list of activities to the AppUser class
        public ICollection<ActivityAttendee> Activities { get; set; } = new List<ActivityAttendee>();
    }
}