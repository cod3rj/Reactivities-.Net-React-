using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetUsername { get; set; }
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
                // Get the user who is currently logged in (the observer)
                var observer = await _context.Users
                    .Include(u => u.Followings)  // Include the Followings collection for eager loading
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // Get the user to be followed or unfollowed (the target)
                var target = await _context.Users
                    .Include(u => u.Followers)  // Include the Followers collection for eager loading
                    .FirstOrDefaultAsync(x => x.UserName == request.TargetUsername);

                // If the target user doesn't exist, return null
                if (target == null) return null;

                // Check if the observer is already following the target
                var following = observer.Followings.FirstOrDefault(f => f.TargetId == target.Id);

                // If not following, create a new following relationship
                if (following == null)
                {
                    // Create a new UserFollowing entity
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };

                    // Add the new following relationship to both observer and target
                    observer.Followings.Add(following);
                    target.Followers.Add(following);
                }
                else
                {
                    // If already following, remove the following relationship
                    _context.UserFollowings.Remove(following);
                }

                // Save the changes to the database
                var success = await _context.SaveChangesAsync() > 0;

                // If the save changes was successful, return success
                if (success) return Result<Unit>.Success(Unit.Value);

                // If the save changes was not successful, return an error message
                return Result<Unit>.Failure("Failed to update following");
            }
        }
    }
}