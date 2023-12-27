namespace ApiTemplate.Infrastructure.BackgroundJobs.Settings;

public class HangfireStorageSettings
{
    public string? ConnectionString { get; set; }

    public OptionsSettings? Options { get; set; }

    public class OptionsSettings
    {
        public TimeSpan QueuePollInterval { get; set; }
        public bool UseRecommendedIsolationLevel { get; set; }
        public bool DisableGlobalLocks { get; set; }
    }
}
