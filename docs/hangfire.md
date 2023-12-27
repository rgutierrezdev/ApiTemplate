# Hangfire

In order to handle background jobs (Queuing, Scheduling) we use [Hangfire](https://www.hangfire.io/).

See also:

- [Development Environment](development-environment.md#sql-server)
- [Configurations](configurations.md#hangfire)

## Dashboard

The Hangfire Dashboard along with the Hangfire Server will run under the `UsersAPI` web project, but you can enqueue
jobs from any other web API project.

You can access the Hangfire Dashboard in the url `https://localhost:5000/hangfire`

![Swagger Screenshot](assets/hangfire1.jpg)

## Recurring Jobs

Create a class that implements the `IScheduledJob` interface. Normally this new class will be located
in `src/Infra/Infrastructure/BackgroundJobs/Scheduled/<NewClass>.cs`

```csharp
public class CheckInactiveUsers : IScheduledJob
{
    public DeleteExpiredBlacklistedTokens()
    {
    }

    public async Task Invoke()
    {
        // Your logic
    }
}
```

Then you have to register the periodicity of the recurring job
in `src/Infra/Infrastructure/BackgroundJobs/Startup.cs` -> `UseBackgroundJobs` function, for this example let's schedule
it daily at midnight:

```csharp
RecurringJob.AddOrUpdate<CheckInactiveUsers>(
    nameof(CheckInactiveUsers),
    service => service.Invoke(),
    Cron.Daily(0)
);
```

You can also use [CRON Expressions](https://en.wikipedia.org/wiki/Cron#CRON_expression) to specify a more complex
schedule.

## Queueing

If you want to enqueue a job, the recommended way is to use services so you can inject dependencies into them.

let's say you have a Class service called `NotifyUser`.

1. Inject the `IBackgroundJob` service.
2. Enqueue the job by passing the Service class as a generic and then calling the function you need to execute.

```csharp
_backgroundJob.Enqueue<NotifyUser>(service => service.Notify(userId));
```
