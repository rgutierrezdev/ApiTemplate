namespace ApiTemplate.Infrastructure.BackgroundJobs.Settings;

public class HangfireServerSettings
{
    public TimeSpan HeartbeatInterval { get; set; }
    public TimeSpan SchedulePollingInterval { get; set; }
    public TimeSpan ServerCheckInterval { get; set; } = default!;
    public int WorkerCount { get; set; }
}
