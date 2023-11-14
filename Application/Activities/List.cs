using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        // This class is used to query the database for a list of activities e.g api/activities
        public class Query : IRequest<Result<List<ActivityDto>>> { }
        // This class is used to handle the query that would be sent to the database
        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>> // The first parameter is the query and the second parameter is the result of the query
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
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
                var activities = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .ToListAsync(cancellationToken);

                return Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}