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
        }
    }
}