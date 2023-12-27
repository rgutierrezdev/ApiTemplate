using System.Linq.Expressions;

namespace ApiTemplate.Application.Common.Interfaces;

public interface IBackgroundJob : IScopedService
{
    public string Enqueue<T>(Expression<Action<T>> methodCall);

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall);
}
