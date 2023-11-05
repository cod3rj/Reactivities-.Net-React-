using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(a => a.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity from the database
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                if (activity == null) return null; // If the activity is null we return null

                // Update the activity using AutoMapper. Old Values are activity.Title = request.Activity.Title ?? activity.Title; 
                _mapper.Map(request.Activity, activity); // This would update the activity with the new values

                // Save the changes if the result is greater than 0
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update the activity"); // If the result is false we return a failure

                return Result<Unit>.Success(Unit.Value); // If the result is true we return a success with the unit value
            }
        }
    }
}