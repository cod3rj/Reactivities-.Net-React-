using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _context = context;
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // We get the user from the database
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null; // If the user does not exist, we return null

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id); // We get the photo from the user

                if (photo == null) return null; // If the photo does not exist, we return null

                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo"); // If the photo is the main photo, we return an error

                var result = await _photoAccessor.DeletePhoto(photo.Id); // We delete the photo from Cloudinary

                if (result == null) return Result<Unit>.Failure("Problem deleting photo from Cloudinary"); // If there is a problem deleting the photo, we return an error

                user.Photos.Remove(photo); // We remove the photo from the user

                var success = await _context.SaveChangesAsync() > 0; // We save the changes to the database

                if (success) return Result<Unit>.Success(Unit.Value); // If the result is successful, we return Unit.Value

                return Result<Unit>.Failure("Problem deleting photo from API"); // If there is a problem deleting the photo, we return an error
            }
        }
    }
}