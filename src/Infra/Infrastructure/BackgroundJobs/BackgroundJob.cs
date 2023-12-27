using System.Linq.Expressions;
using ApiTemplate.Application.Common.Interfaces;

namespace ApiTemplate.Infrastructure.BackgroundJobs;

public class BackgroundJob : IBackgroundJob
{
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        var id = Hangfire.BackgroundJob.Enqueue(methodCall);
        return id;
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        var id = Hangfire.BackgroundJob.Enqueue(methodCall);
        return id;
    }
}
