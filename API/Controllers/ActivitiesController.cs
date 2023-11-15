using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {

        // We use IActionResult because we are not returning an object, we are returning a status code
        [HttpGet] // api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities([FromQuery] ActivityParams param)
        {
            // We send a query to the mediator and the mediator will send the query to the handler
            return HandlePageResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [Authorize]
        [HttpGet("{id}")] // api/activities/id
        public async Task<IActionResult> GetActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost] // api/activities/
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            // We send a command to the mediator and the mediator will send the command to the handler
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")] // api/activities/id
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            // Set the Activitiy Id to the id from the route
            activity.Id = id;
            // We send the command to the mediator and the mediator will send the command to the handler
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")] // api/activities/id
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            // We send the command to the mediator and the mediator will send the command to the handler
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPost("{id}/attend")] // api/activities/id/attend
        public async Task<IActionResult> Attend(Guid id)
        {
            // We send the command to the mediator and the mediator will send the command to the handler
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }
    }
}