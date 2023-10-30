using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest // This class is used to create a new activity in the database no need to return anything
        {
            public Activity Activity { get; set; } // This is the activity that would be created
        }

        public class Handler : IRequestHandler<Command> // This class is used to handle the command
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Activities.Add(request.Activity); // We add the activity to the database

                await _context.SaveChangesAsync(); // We save the changes to the database
            }
        }
    }
}