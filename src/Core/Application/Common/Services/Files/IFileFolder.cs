namespace ApiTemplate.Application.Common.Services.Files;

public interface IFileFolder
{
    public string Path { get; }
}

public class CompanyRegistrationsFolder : IFileFolder
{
    public string Path { get; }

    public CompanyRegistrationsFolder(Guid companyId)
    {
        Path = $"companies/{companyId}/registrations";
    }
}

public class CompanyDocumentsFolder : IFileFolder
{
    public string Path { get; }

    public CompanyDocumentsFolder(Guid companyId)
    {
        Path = $"companies/{companyId}/documents";
    }
}

public class CompanyImagesFolder : IFileFolder
{
    public string Path { get; }

    public CompanyImagesFolder(Guid companyId)
    {
        Path = $"companies/{companyId}/images";
    }
}
