using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        // This class is used to create a new activity in the database no need to return anything
        public class Command : IRequest<Result<Unit>> // We use Result<Unit> to use Result class and Unit to return nothing
        {
            public Activity Activity { get; set; } // This is the activity that would be created
        }

        // This class is used to validate the command
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(a => a.Activity).SetValidator(new ActivityValidator()); // We set the validator for the activity
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>> // This class is used to handle the command
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Activities.Add(request.Activity); // We add the activity to the database

                var result = await _context.SaveChangesAsync() > 0; // We save the changes to the database if the result is greater than 0 otherwise it would return false

                if (!result) return Result<Unit>.Failure("Failed to create activity"); // If the result is false we return a failure

                return Result<Unit>.Success(Unit.Value); // If the result is true we return a success with the unit value
            }
        }
    }
}