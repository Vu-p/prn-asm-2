using Microsoft.AspNetCore.Http;

namespace services;

public interface ICloudinaryService
{
    Task<string?> UploadImageAsync(IFormFile file);
}
