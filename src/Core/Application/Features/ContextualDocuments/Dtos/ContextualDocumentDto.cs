using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.Features.ContextualDocuments.Dtos;

public class ContextualDocumentDto
{
    public ContextualDocumentType Type { get; set; }
    public int Version { get; set; }
    public string Content { get; set; } = default!;
}
