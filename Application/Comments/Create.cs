using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            // Properties for the comment creation command
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            // Validation rules for the comment creation command
            public CommandValidator()
            {
                RuleFor(c => c.Body).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            private readonly DataContext _context;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity from the database based on the provided ActivityId
                var activity = await _context.Activities.FindAsync(request.ActivityId);

                // Check if the activity exists
                if (activity == null) return null;

                // Get the user from the database based on the current user's username
                var user = await _context.Users.Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // Create a new comment using the provided data
                var comment = new Comment
                {
                    Author = user, // Set the author of the comment to the user
                    Activity = activity, // Set the activity of the comment to the activity
                    Body = request.Body // Set the body of the comment to the body of the request
                };

                // Add the comment to the activity's list of comments
                activity.Comments.Add(comment);

                // Save the changes to the database
                var result = await _context.SaveChangesAsync() > 0;

                // Check if the save operation was successful
                if (result) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

                // If the save operation was not successful, return a failure message
                return Result<CommentDto>.Failure("Failed to add comment");
            }
        }
    }
}
