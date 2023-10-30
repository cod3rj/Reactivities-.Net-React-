using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator; // This can only be used inside this controller

        // This protected class checks if the _mediator is null and if it is null it will get the service from the http context
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}