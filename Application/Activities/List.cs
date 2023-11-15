using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Activities
{
    // This class is responsible for querying the database for a list of activities (e.g., api/activities).
    public class List
    {
        // Represents the query to retrieve a list of activities
        public class Query : IRequest<Result<PageList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; } // Parameters for filtering and pagination
        }

        // Handles the query to retrieve a list of activities
        public class Handler : IRequestHandler<Query, Result<PageList<ActivityDto>>>
        {
            private readonly DataContext _context; // Entity Framework DbContext for data access
            private readonly IMapper _mapper; // AutoMapper instance for mapping entities to DTOs
            private readonly IUserAccessor _userAccessor; // Service to access user-related information

            // Constructor to initialize the handler with required dependencies
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            // Handles the query to retrieve a list of activities
            public async Task<Result<PageList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                /* 
                    We use the cancellationToken to cancel the request if the user cancels the request before it is completed

                    try
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await Task.Delay(1000, cancellationToken);
                            _logger.LogInformation($"Task {i} has completed");
                        }
                    }
                    catch (System.Exception)
                    {
                        _logger.LogInformation("Task was cancelled");
                    }
                */

                // This method utilizes ProjectTo from AutoMapper to project the query to the ActivityDto
                var query = _context.Activities
                    .Where(d => d.Date >= request.Params.StartDate) // Filter the activities by date
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .AsQueryable();

                // Apply additional filters based on the ActivityParams
                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                // Create and return a paginated result using the PageList class
                return Result<PageList<ActivityDto>>.Success(
                    await PageList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}
