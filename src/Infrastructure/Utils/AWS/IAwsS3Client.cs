using Microsoft.AspNetCore.Http;

namespace Infrastructure.Utils.AWS
{
    public interface IAwsS3Client
    {
        Task<bool> RemoveObject(string fileName);
        Task<string> UploadFileAsync(IFormFile formFile);
    }
}
