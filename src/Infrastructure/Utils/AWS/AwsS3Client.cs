using Amazon.S3;
using Amazon.S3.Model;
using Domain.ConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Utils.AWS
{
    public class AwsS3Client : IAwsS3Client
    {
        private readonly IConfiguration _configuration;
        private readonly AwsConfiguration _awsConfigurations;

        public AwsS3Client(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _awsConfigurations = new AwsConfiguration();
            _configuration.Bind(_awsConfigurations.Section, _awsConfigurations);
        }

        public async Task<string> UploadFileAsync(IFormFile formFile)
        {
            var now = DateTime.Now.ToFileTimeUtc().ToString();
            var location = $"uploads/{now}-{formFile.FileName.Replace(" ", "-")}";
            var awsBucketName = _configuration.GetValue<string>("AWS:BucketName");
            await using var stream = formFile.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                Key = location,
                BucketName = awsBucketName,
                InputStream = stream,
                AutoCloseStream = true,
                ContentType = formFile.ContentType
            };

            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");

            var s3 = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest2);
            var res = await s3.PutObjectAsync(putRequest);
            return GetUploadedUrl(location);
        }

        private string GetUploadedUrl(string location)
        {
            var awsBucketName = _configuration.GetValue<string>("AWS:BucketName");
            var awsRegion = _configuration.GetValue<string>("AWS:Region");

            var result = $"https://{awsBucketName}.s3.{awsRegion}.amazonaws.com/{location}";
            return result;
        }

        public async Task<bool> RemoveObject(string fileName)
        {
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var awsBucketName = _configuration.GetValue<string>("AWS:BucketName");

            var s3 = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest2);

            var request = new DeleteObjectRequest
            {
                BucketName = awsBucketName,
                Key = fileName
            };

            var response = await s3.DeleteObjectAsync(request);

            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}
