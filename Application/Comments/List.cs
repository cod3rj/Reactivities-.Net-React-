using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        public class Query : IRequest<Result<List<CommentDto>>>
        {
            // We add the properties for the query
            public Guid ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // We get the comments from the activity from database
                var comments = await _context.Comments.Where(x => x.Activity.Id == request.ActivityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // We return the comments
                return Result<List<CommentDto>>.Success(comments);
            }
        }
    }
}