using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest
        {
            // We only need the Id of the activity to delete it
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity from the database
                var activity = await _context.Activities.FindAsync(request.Id);

                // Remove the activity from the database
                _context.Remove(activity);

                // Save the changes
                await _context.SaveChangesAsync();
            }
        }
    }
}