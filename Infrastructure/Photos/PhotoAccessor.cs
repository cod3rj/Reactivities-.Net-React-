using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if (file.Length > 0)
            {
                // We use the using statement to dispose of the stream after we are done with it
                await using var stream = file.OpenReadStream();

                // We use the ImageUploadParams class to configure the upload
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream), // We pass in the file name and the stream
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill") // We set the transformation to crop the image to 500x500
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams); // We upload the image to Cloudinary

                if (uploadResult.Error != null) // If there is an error, we throw an exception
                {
                    throw new Exception(uploadResult.Error.Message); // We throw an exception with the error message
                }

                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId, // We return the public id of the image
                    Url = uploadResult.SecureUrl.ToString() // We return the secure url of the image
                };
            }

            return null; // If there is no file, we return null
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId); // We create a new DeletionParams object with the public id of the image
            var result = await _cloudinary.DestroyAsync(deleteParams); // We delete the image from Cloudinary
            return result.Result == "ok" ? result.Result : null; // If the result is ok, we return the result, otherwise we return null
        }
    }
}