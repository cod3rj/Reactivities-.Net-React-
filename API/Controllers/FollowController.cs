using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        // Endpoint to follow a user by sending a POST request with the target username
        [HttpPost("{username}")]
        public async Task<IActionResult> Follow(string username)
        {
            // Send a command to follow the specified username and return the result
            return Ok(await Mediator.Send(new FollowToggle.Command { TargetUsername = username }));
        }

        // Endpoint to get followings based on the specified username and predicate
        [HttpGet("{username}")]
        public async Task<IActionResult> GetFollowings(string username, string predicate)
        {
            // Send a query to get followings based on the specified username and predicate
            // and return the result using the HandleResult method
            return HandleResult(await Mediator.Send(new List.Query { Username = username, Predicate = predicate }));
        }
    }
}
