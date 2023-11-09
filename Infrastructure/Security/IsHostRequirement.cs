using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    // We create a requirement for the authorization policy
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    // We create a handler for the authorization policy
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dbContext;
        public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // We get the username from the token

            if (userId == null) return Task.CompletedTask; // If the username is null we return a completed task

            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString()); // We get the activity id from the route

            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking() // We use AsNoTracking to not track the attendee
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId).Result; // We get the attendee from the database

            if (attendee == null) return Task.CompletedTask; // If the attendee is null we return a completed task

            if (attendee.isHost) context.Succeed(requirement); // If the attendee is the host we succeed

            return Task.CompletedTask; // We return a completed task
        }
    }
}