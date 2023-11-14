using Domain;

namespace Application.Profiles
{
    public class Profile
    {
        // The username of the profile
        public string Username { get; set; }

        // The display name chosen by the user
        public string DisplayName { get; set; }

        // The biography or description provided by the user
        public string Bio { get; set; }

        // The URL or path to the user's profile image
        public string Image { get; set; }

        // Indicates whether the currently logged-in user is following this profile
        public bool Following { get; set; }

        // The count of followers for this profile
        public int FollowersCount { get; set; }

        // The count of users that this profile is following
        public int FollowingCount { get; set; }

        // Collection of photos associated with this profile
        public ICollection<Photo> Photos { get; set; }
    }
}
