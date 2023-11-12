using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PhotosController : BaseApiController
    {
        // We add a method to add a photo to Cloudinary
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Add.Command command) // We add the [FromForm] attribute to the command
        {
            return HandleResult(await Mediator.Send(command)); // We return the result of the command
        }

        // We add a method to delete a photo from Cloudinary
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // We return the result of the command
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        // We add a method to set a photo as the main photo
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain(string id)
        {
            // We return the result of the command
            return HandleResult(await Mediator.Send(new SetMain.Command { Id = id }));
        }
    }
}