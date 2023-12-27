namespace ApiTemplate.Application.Common.Mailing;

public interface IMailService : ITransientService
{
    Task<bool> SendAsync(SimpleMailRequest mailRequest, CancellationToken cancellationToken = default);

    Task<bool> SendBatchAsync(BatchMailRequest mailRequest, CancellationToken cancellationToken = default);

    Task<bool> SendTemplatedEmail<T>(
        IEnumerable<string> to,
        IEmailTemplate<T> template,
        CancellationToken cancellationToken = default
    );
}
