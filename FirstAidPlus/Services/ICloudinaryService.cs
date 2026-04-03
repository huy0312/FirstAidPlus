using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FirstAidPlus.Services
{
    public interface ICloudinaryService
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
