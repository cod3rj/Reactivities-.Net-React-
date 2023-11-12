using System.Security.Cryptography.X509Certificates;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
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
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
            // We are mapping from AppUser -> Profiles.Profile (source -> target) e.g appUser.DisplayName = profile.DisplayName;
            CreateMap<AppUser, Profiles.Profile>()
                 // We map the Image property to the Photo.Url property
                 .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}