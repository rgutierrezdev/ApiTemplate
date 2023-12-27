namespace ApiTemplate.Application.Common.Mailing;

public interface IEmailTemplate<out T>
{
    public string Name { get; }
    public string Subject { get; }
    public T Vars { get; }
    public IEnumerable<string>? Tags { get; }
}
