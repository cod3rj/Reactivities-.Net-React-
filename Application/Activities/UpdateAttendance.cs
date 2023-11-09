using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>> // We use Unit because we are not returning anything
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>> // Command is the request and Result<Unit> is the response
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(a => a.Attendees).ThenInclude(u => u.AppUser) // We include the attendees and the appuser
                    .FirstOrDefaultAsync(x => x.Id == request.Id); // We get the activity with the specified id

                if (activity == null) return null; // If the activity is null we return null

                // We get the user from the database and check if the username is the same as the user that is logged in
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null; // If the user is null we return null

                var hostUsername = activity.Attendees.FirstOrDefault(x => x.isHost)?.AppUser?.UserName; // We get the host username

                var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName); // We get the attendance

                if (attendance != null && hostUsername == user.UserName) // If the attendance is not null and the host username is the same as the user that is logged in
                {
                    activity.IsCancelled = !activity.IsCancelled; // We cancel the activity
                }

                if (attendance != null && hostUsername != user.UserName) // If the attendance is not null and the host username is not the same as the user that is logged in
                {
                    activity.Attendees.Remove(attendance); // We remove the attendance
                }

                if (attendance == null)
                {
                    attendance = new ActivityAttendee // We create a new attendance
                    {
                        AppUser = user,
                        Activity = activity,
                        isHost = false
                    };

                    activity.Attendees.Add(attendance); // We add the attendance to the activity
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
            }
        }
    }
}