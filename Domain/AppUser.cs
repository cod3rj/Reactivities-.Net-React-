using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        // We add a list of activities to the AppUser class
        public ICollection<ActivityAttendee> Activities { get; set; } = new List<ActivityAttendee>();
        // We add a list of photos to the AppUser class
        public ICollection<Photo> Photos { get; set; }
        // We add a list of followings to the AppUser class
        public ICollection<UserFollowing> Followings { get; set; }
        // We add a list of followers to the AppUser class
        public ICollection<UserFollowing> Followers { get; set; }
    }
}