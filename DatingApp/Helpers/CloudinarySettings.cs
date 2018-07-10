namespace DatingApp.Helpers
{
    public class CloudinarySettings
    {
        // Need to inject the service at the Satrtup.cs class.
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}