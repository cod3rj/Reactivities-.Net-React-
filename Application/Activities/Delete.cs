using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            // We only need the Id of the activity to delete it
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity from the database
                var activity = await _context.Activities.FindAsync(request.Id);

                if (activity == null) return null; // If the activity is null we return null

                // Remove the activity from the database
                _context.Remove(activity);

                // Save the changes if the result is greater than 0
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the activity"); // If the result is false we return a failure

                return Result<Unit>.Success(Unit.Value); // If the result is true we return a success with the unit value
            }
        }
    }
}