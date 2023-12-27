namespace ApiTemplate.Application.Common.Pdf;

public interface IPdfGeneratorService<in T> : IScopedService
{
    byte[] Generate(T model);
}
