using System.ComponentModel.DataAnnotations;

namespace ApiTemplate.Persistence;

public class DatabaseSettings : IValidatableObject
{
    public string ConnectionString { get; set; } = string.Empty;

    public DatabaseLoggingSettings? Logging { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) }
            );
        }
    }
}

public class DatabaseLoggingSettings
{
    public bool LogSQLData { get; set; }
    public bool LogToOutput { get; set; }
}
