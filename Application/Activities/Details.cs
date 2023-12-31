using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // This class will get the request from the get method with the specified id from the database e.g api/activities/id
        public class Query : IRequest<Result<ActivityDto>> // Result <Activity> here will return either a success or failure e.g Values | Null
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
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
            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() }) // We use ProjectTo to project the query to the ActivityDto
                    .FirstOrDefaultAsync(x => x.Id == request.Id); // We get the activity with the specified id

                return Result<ActivityDto>.Success(activity); // We return the activity
            }
        }
    }
}