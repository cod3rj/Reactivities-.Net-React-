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
            CreateMap<ActivityAttendee, Profiles.Profile>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio));
        }
    }
}