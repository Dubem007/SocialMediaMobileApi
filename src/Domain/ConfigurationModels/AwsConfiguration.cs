namespace Domain.ConfigurationModels
{
    public class AwsConfiguration
    {
        public string Section { get; set; } = "AWS";
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
        public string FolderName { get; set; }
    }
}
