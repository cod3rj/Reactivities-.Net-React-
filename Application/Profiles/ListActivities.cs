using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    // This class contains a query and its handler to list user activities based on certain criteria.
    public class ListActivities
    {
        // Represents the request to list user activities
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string Username { get; set; } // The username for which activities are requested
            public string Predicate { get; set; } // The criteria for filtering activities (e.g., "past", "hosting")
        }

        // Handles the request to list user activities
        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly IMapper _mapper; // AutoMapper instance for mapping entities to DTOs
            private readonly DataContext _context; // Entity Framework DbContext for data access

            // Constructor to initialize the handler with required dependencies
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            // Handles the request to list user activities
            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Query the database to get activities based on the provided username
                var query = _context.ActivityAttendees
                    .Where(u => u.AppUser.UserName == request.Username) // Filter by username
                    .OrderBy(a => a.Activity.Date) // Order by activity date
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider) // Project to the UserActivityDto
                    .AsQueryable();

                // Apply additional filtering based on the provided predicate using a switch expression
                query = request.Predicate switch
                {
                    "past" => query.Where(a => a.Date <= DateTime.UtcNow), // If the predicate is "past", filter by activities in the past
                    "hosting" => query.Where(a => a.HostUsername == request.Username), // If the predicate is "hosting", filter by activities hosted by the user
                    _ => query.Where(a => a.Date >= DateTime.UtcNow) // If the predicate is not "past" or "hosting", filter by activities in the future
                };

                // Execute the query and retrieve the list of user activity DTOs
                var activities = await query.ToListAsync();

                // Return the result, indicating success and providing the list of user activity DTOs
                return Result<List<UserActivityDto>>.Success(activities);
            }
        }
    }
}
