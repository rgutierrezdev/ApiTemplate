namespace ApiTemplate.Infrastructure.BackgroundJobs;

public interface IScheduledJob
{
    public Task Invoke();
}
