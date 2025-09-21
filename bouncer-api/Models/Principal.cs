namespace Bouncer.Api.Models;

public class Principal
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePrincipalRequest
{
    public string ExternalId { get; set; } = string.Empty;
}

public class UpdatePrincipalRequest
{
    public string ExternalId { get; set; } = string.Empty;
}