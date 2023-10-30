using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Activity Activity { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity from the database
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                // Update the activity using AutoMapper. Old Values are activity.Title = request.Activity.Title ?? activity.Title; 
                _mapper.Map(request.Activity, activity); // This would update the activity with the new values

                // Save the changes
                await _context.SaveChangesAsync();
            }
        }
    }
}