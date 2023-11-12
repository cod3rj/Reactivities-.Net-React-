using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
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
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // If the user is null, we return null
                if (user == null) return null;

                // We get the photo from the user
                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                // If the photo is null, we return null
                if (photo == null) return null;

                // We get the current main photo of the user from the database
                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                // If the current main photo is not null, we set it to false
                if (currentMain != null) currentMain.IsMain = false;

                // We set the photo to be the main photo
                photo.IsMain = true;

                // We save the changes to the database | If the result is greater than 0, we return Unit.Value 
                var success = await _context.SaveChangesAsync() > 0;

                // If the result is successful, we return Unit.Value
                if (success) return Result<Unit>.Success(Unit.Value);

                // If there is a problem saving the changes, we return an error
                return Result<Unit>.Failure("Problem setting main photo");
            }
        }
    }
}