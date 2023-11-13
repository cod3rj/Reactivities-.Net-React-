using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        // This method is called when a new comment is sent from the client
        public async Task SendComment(Create.Command command)
        {
            // Send the comment command to the Mediator for processing
            var comments = await _mediator.Send(command);

            // Notify all clients in the same group (activity) about the new comment
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comments.Value);
        }

        // This method is called when a new client connects to the hub
        public override async Task OnConnectedAsync()
        {
            // Get the HttpContext to access the request details
            var httpContext = Context.GetHttpContext();

            // Extract the activityId from the query parameters
            var activityId = httpContext.Request.Query["activityId"];

            // Add the connected client to the group corresponding to the activity
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

            // Retrieve the existing comments for the activity
            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });

            // Send the existing comments to the newly connected client
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}
