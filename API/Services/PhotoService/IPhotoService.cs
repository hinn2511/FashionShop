using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file, int height = 0, int width = 0, string cropOption = "", double aspectRatio = 0);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}