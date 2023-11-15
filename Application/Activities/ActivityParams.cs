using Application.Core;

namespace Application.Activities
{
    public class ActivityParams : PagingParams
    {
        public bool IsGoing { get; set; } // If the user is going to the activity
        public bool IsHost { get; set; } // If the user is the host of the activity
        public DateTime StartDate { get; set; } = DateTime.UtcNow; // Set the default start date to the current date and time
    }
}