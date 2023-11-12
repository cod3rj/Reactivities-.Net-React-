using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            // We add a property for the file
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
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

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                // We get the user from the database
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null; // If the user does not exist, we return null

                // We upload the photo to Cloudinary
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);

                // We create a new photo object
                var photo = new Photo
                {
                    Url = photoUploadResult.Url, // We set the url of the photo
                    Id = photoUploadResult.PublicId // We set the public id of the photo
                };

                if (!user.Photos.Any(x => x.IsMain)) // If the user does not have a main photo
                {
                    photo.IsMain = true; // We set the photo to be the main photo
                }

                user.Photos.Add(photo); // We add the photo to the user

                var result = await _context.SaveChangesAsync() > 0; // We save the changes to the database

                if (result) return Result<Photo>.Success(photo); // If the result is successful, we return the photo

                return Result<Photo>.Failure("Problem adding photo"); // If the result is not successful, we return a failure message
            }
        }
    }
}