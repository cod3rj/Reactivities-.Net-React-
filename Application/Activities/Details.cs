using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // This class will get the request from the get method with the specified id from the database e.g api/activities/id
        public class Query : IRequest<Result<Activity>> // Result <Activity> here will return either a success or failure e.g Values | Null
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Activity>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id); // We get the activity from the database

                return Result<Activity>.Success(activity); // We return the activity
            }
        }
    }
}