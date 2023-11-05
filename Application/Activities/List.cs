using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        // This class is used to query the database for a list of activities e.g api/activities
        public class Query : IRequest<Result<List<Activity>>> { }
        // This class is used to handle the query that would be sent to the database
        public class Handler : IRequestHandler<Query, Result<List<Activity>>> // The first parameter is the query and the second parameter is the result of the query
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
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
                return Result<List<Activity>>.Success(await _context.Activities.ToListAsync(cancellationToken));
            }
        }
    }
}