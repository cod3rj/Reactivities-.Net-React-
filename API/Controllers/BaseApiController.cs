using API.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // A base controller for API controllers, providing common functionality
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // Private field to hold the Mediator instance
        private IMediator _mediator;

        // Protected property to access the Mediator, lazy-loaded using the HttpContext.RequestServices
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        // Helper method to handle the result of an operation that returns a single value
        // Parameters:
        //   - result: The result of the operation
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            // If the result is null, return NotFound
            if (result == null) return NotFound();

            // If the operation is successful and a value is present, return Ok with the value
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            // If the operation is successful but the value is null, return NotFound
            if (result.IsSuccess && result.Value == null)
                return NotFound();

            // If the operation is not successful, return BadRequest with the error message
            return BadRequest(result.Error);
        }

        /*
             Helper method to handle the result of an operation that returns a paginated list
             Parameters:
             - result: The result of the operation
        */
        protected ActionResult HandlePageResult<T>(Result<PageList<T>> result)
        {
            // If the result is null, return null
            if (result == null) return null;

            // If the operation is successful and a value is present, set pagination headers and return Ok with the paginated list
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(result.Value.CurrentPage, result.Value.PageSize,
                    result.Value.TotalCount, result.Value.TotalPages);
                return Ok(result.Value);
            }

            // If the operation is successful but the value is null, return NotFound
            if (result.IsSuccess && result.Value == null)
                return NotFound();

            // If the operation is not successful, return BadRequest with the error message
            return BadRequest(result.Error);
        }
    }
}
