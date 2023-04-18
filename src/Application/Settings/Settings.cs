namespace Application.Settings
{
    public class FileSettings
    {
        public string[] DefaultFileExtensionsAllowed { get; set; }
        public int DefaultFileSizeLimitInMb { get; set; }
        public string DefaultStorageProvider { get; set; }
    }
}
