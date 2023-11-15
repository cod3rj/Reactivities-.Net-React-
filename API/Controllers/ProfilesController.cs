using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        // We add a an endpoint for getting the profile
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            // We return the result of the query
            return HandleResult(await Mediator.Send(new Details.Query { Username = username }));
        }

        // We add an endpoint for editing the profile
        [HttpPut]
        public async Task<IActionResult> EditProfile(Edit.Command command)
        {
            // We return the result of the query
            return HandleResult(await Mediator.Send(command));
        }

        // We add an endpoint for getting the list of user activities
        [HttpGet("{username}/activities")]
        public async Task<IActionResult> GetUserActivities(string username, string predicate)
        {
            // We return the result of the query
            return HandleResult(await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate }));
        }
    }
}