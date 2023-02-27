using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            if (!string.IsNullOrEmpty(config.Value.CloudName)
                && !string.IsNullOrEmpty(config.Value.ApiKey)
                && !string.IsNullOrEmpty(config.Value.ApiSecret))
            {
                var acc = new Account
                (
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );

                _cloudinary = new Cloudinary(acc);

            }
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, int height = 0, int width = 0, string cropOption = "", double aspectRatio = 0)
        {
            var uploadResult = new ImageUploadResult();
            var transformation = new Transformation();

            if(height > 0)
                transformation.Height(height);

            if(width > 0)
                transformation.Height(width);

            if(!string.IsNullOrEmpty(cropOption))
                transformation.Crop(cropOption);

            if(aspectRatio > 0)
                transformation.AspectRatio(aspectRatio);

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = transformation
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}