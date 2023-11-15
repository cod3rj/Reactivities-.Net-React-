using System.Security.Cryptography.X509Certificates;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Initialize currentUsername variable with null
            string currentUsername = null;

            // We are mapping from Activity -> Activity (source -> target) e.g activity.Title = request.Activity.Title ?? activity.Title; 
            CreateMap<Activity, Activity>();

            // We are mapping from Activity -> ActivityDto (source -> target) e.g activity.Title = request.Activity.Title ?? ActivityDto.Title;
            CreateMap<Activity, ActivityDto>()
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees
                .FirstOrDefault(x => x.isHost).AppUser.UserName)); // We map the HostUsername property to the AppUser.UserName property

            // We are mapping from ActivityAttendee -> Profiles.Profile (source -> target) e.g activityAttendee.AppUser.DisplayName = profile.DisplayName;
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                // Map the count of followers from AppUser to the FollowersCount property in the Profile
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
                // Map the count of following users from AppUser to the FollowingCount property in the Profile
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
                // Map 'Following' based on whether there are followers with the current username
                .ForMember(d => d.Following, o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));

            // AutoMapper configuration for mapping properties from AppUser to Profiles.Profile
            CreateMap<AppUser, Profiles.Profile>()
                // Map the Image property from AppUser to the Photo.Url property in the Profile
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
                // Map the count of followers from AppUser to the FollowersCount property in the Profile
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                // Map the count of following users from AppUser to the FollowingCount property in the Profile
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
                // Map 'Following' based on whether there are followers with the current username
                .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));

            // We are mapping from Comment -> CommentDto (source -> target) e.g comment.Body = commentDto.Body;
            CreateMap<Comment, CommentDto>()
                // We map the DisplayName property to the AppUser.DisplayName property
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                // We map the Username property to the AppUser.UserName property
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
                // We map the Image property to the AppUser.Photos.Url property
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));

            // CreateMap is used to define a mapping from ActivityAttendee to UserActivityDto
            CreateMap<ActivityAttendee, Profiles.UserActivityDto>()
                // ForMember is used to configure specific members of the destination type (UserActivityDto)
                // Map the Id property of UserActivityDto to the Id property of ActivityAttendee's associated Activity
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id))
                // Map the Date property of UserActivityDto to the Date property of ActivityAttendee's associated Activity
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date))
                // Map the Title property of UserActivityDto to the Title property of ActivityAttendee's associated Activity
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
                // Map the Category property of UserActivityDto to the Category property of ActivityAttendee's associated Activity
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
                // Map the HostUsername property of UserActivityDto to the username of the host of the associated Activity
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Activity.Attendees.FirstOrDefault(x => x.isHost).AppUser.UserName));
        }
    }
}