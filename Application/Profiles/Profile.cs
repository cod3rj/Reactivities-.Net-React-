using Domain;

namespace Application.Profiles
{
    public class Profile
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        // We add a list of Photos to the Profile class
        public ICollection<Photo> Photos { get; set; }
    }
}