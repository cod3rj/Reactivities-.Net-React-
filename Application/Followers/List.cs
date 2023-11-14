using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string Predicate { get; set; } // Predicate is the filter that we want to apply to the list of followers
            public string Username { get; set; } // Username is the username of the profile that we want to get the list of followers for
        }

        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Initialize a list to store the profiles
                var profiles = new List<Profiles.Profile>();

                // Switch statement to handle different follower/following scenarios
                switch (request.Predicate)
                {
                    // Case for getting followers
                    case "followers":
                        // Query to get followers based on the provided username
                        profiles = await _context.UserFollowings.Where(x => x.Target.UserName == request.Username)
                            .Select(u => u.Observer) // Select the observer (follower) from UserFollowings
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() }) // Map to Profile DTO using AutoMapper and pass in the current username
                            .ToListAsync(); // Convert the results to a list
                        break;

                    // Case for getting following
                    case "following":
                        // Query to get following based on the provided username
                        profiles = await _context.UserFollowings.Where(x => x.Observer.UserName == request.Username)
                            .Select(u => u.Target) // Select the target (following) from UserFollowings
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                            new { currentUsername = _userAccessor.GetUsername() }) // Map to Profile DTO using AutoMapper
                            .ToListAsync(); // Convert the results to a list
                        break;
                }

                // Return the list of profiles as a success result
                return Result<List<Profiles.Profile>>.Success(profiles);
            }
        }
    }
}
