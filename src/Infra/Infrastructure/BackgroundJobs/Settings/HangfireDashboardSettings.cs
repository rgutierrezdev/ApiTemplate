namespace ApiTemplate.Infrastructure.BackgroundJobs.Settings;

public class HangfireDashboardSettings
{
    public string? Route { get; set; }

    public OptionsSettings? Options { get; set; }

    public class OptionsSettings
    {
        public int? StatsPollingInterval { get; set; }
        public string? DashboardTitle { get; set; }
    }
}
