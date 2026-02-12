namespace PropMT5ConnectionService.Configuration
{
    public class WebServerSettings
    {
        public string BaseUri { get; set; }
    }

    public class BackgroundJobsSettings
    {
        public int LiquidationCheckIntervalSeconds { get; set; }
    }
}
