using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        // Method to add a photo to Cloudinary
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        // Method to delete a photo from Cloudinary
        Task<string> DeletePhoto(string publicId);
    }
}