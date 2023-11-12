using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        // We add a method to get a profile
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            // We return the result of the query
            return HandleResult(await Mediator.Send(new Details.Query { Username = username }));
        }

    }
}