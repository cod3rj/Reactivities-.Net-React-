using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {

        // We use IActionResult because we are not returning an object, we are returning a status code
        [HttpGet] // api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities(CancellationToken ct)
        {
            // We send a query to the mediator and the mediator will send the query to the handler
            return await Mediator.Send(new List.Query(), ct);
        }

        [HttpGet("{id}")] // api/activities/id
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }

        [HttpPost] // api/activities/
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            await Mediator.Send(new Create.Command { Activity = activity }); // We send a command to the mediator and the mediator will send the command to the handler

            return Ok();
        }

        [HttpPut("{id}")] // api/activities/id
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            // Set the Activitiy Id to the id from the route
            activity.Id = id;

            // We send the command to the mediator and the mediator will send the command to the handler
            await Mediator.Send(new Edit.Command { Activity = activity });

            // Return a 200 OK response
            return Ok();
        }

        [HttpDelete("{id}")] // api/activities/id
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            // We send the command to the mediator and the mediator will send the command to the handler
            await Mediator.Send(new Delete.Command { Id = id });

            // Return a 200 OK response
            return Ok();
        }
    }
}