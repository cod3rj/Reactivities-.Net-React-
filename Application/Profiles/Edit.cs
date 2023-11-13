using Application.Core;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command> // We add a validator for the command class
        {
            public CommandValidator()
            {
                RuleFor(p => p.DisplayName).NotEmpty(); // We add a rule for the Display Name
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;

            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // First we get the user from the database
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // We then validate the user if its null
                if (user == null) return null;

                // We update the user's display name and bio
                user.DisplayName = request.DisplayName ?? user.DisplayName; // If the request display name is null, we set it to the user's display name
                user.Bio = request.Bio ?? user.Bio; // If the request bio is null, we set it to the user's bio

                // We save the changes to the database
                var result = await _context.SaveChangesAsync() > 0;

                // If the result is successful, we return a success with the unit value
                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem updating profile"); // If the result is not successful, we return a failure message
            }
        }
    }
}