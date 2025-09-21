using Microsoft.AspNetCore.Mvc;
using Bouncer.Api.Models;

namespace Bouncer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrincipalController : ControllerBase
{
    private static readonly List<Principal> _principals = new()
    {
        new Principal { Id = 1, ExternalId = "user123", CreatedAt = DateTime.UtcNow.AddDays(-10) },
        new Principal { Id = 2, ExternalId = "client456", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new Principal { Id = 3, ExternalId = "system789", CreatedAt = DateTime.UtcNow.AddDays(-2) }
    };

    private static int _nextId = 4;

    private readonly ILogger<PrincipalController> _logger;

    public PrincipalController(ILogger<PrincipalController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Principal>> GetAll()
    {
        _logger.LogInformation("Getting all principals");
        return Ok(_principals);
    }

    [HttpGet("{id}")]
    public ActionResult<Principal> GetById(int id)
    {
        _logger.LogInformation("Getting principal with id: {Id}", id);

        var principal = _principals.FirstOrDefault(p => p.Id == id);
        if (principal == null)
        {
            return NotFound($"Principal with id {id} not found");
        }

        return Ok(principal);
    }

    [HttpGet("external/{externalId}")]
    public ActionResult<Principal> GetByExternalId(string externalId)
    {
        _logger.LogInformation("Getting principal with external id: {ExternalId}", externalId);

        var principal = _principals.FirstOrDefault(p => p.ExternalId == externalId);
        if (principal == null)
        {
            return NotFound($"Principal with external id '{externalId}' not found");
        }

        return Ok(principal);
    }

    [HttpPost]
    public ActionResult<Principal> Create(CreatePrincipalRequest request)
    {
        _logger.LogInformation("Creating new principal with external id: {ExternalId}", request.ExternalId);

        if (string.IsNullOrWhiteSpace(request.ExternalId))
        {
            return BadRequest("ExternalId is required");
        }

        // Check if ExternalId already exists
        if (_principals.Any(p => p.ExternalId == request.ExternalId))
        {
            return Conflict($"Principal with external id '{request.ExternalId}' already exists");
        }

        var principal = new Principal
        {
            Id = _nextId++,
            ExternalId = request.ExternalId,
            CreatedAt = DateTime.UtcNow
        };

        _principals.Add(principal);

        return CreatedAtAction(nameof(GetById), new { id = principal.Id }, principal);
    }

    [HttpPut("{id}")]
    public ActionResult<Principal> Update(int id, UpdatePrincipalRequest request)
    {
        _logger.LogInformation("Updating principal with id: {Id}", id);

        var principal = _principals.FirstOrDefault(p => p.Id == id);
        if (principal == null)
        {
            return NotFound($"Principal with id {id} not found");
        }

        if (string.IsNullOrWhiteSpace(request.ExternalId))
        {
            return BadRequest("ExternalId is required");
        }

        // Check if ExternalId already exists (excluding current principal)
        if (_principals.Any(p => p.ExternalId == request.ExternalId && p.Id != id))
        {
            return Conflict($"Principal with external id '{request.ExternalId}' already exists");
        }

        principal.ExternalId = request.ExternalId;
        principal.UpdatedAt = DateTime.UtcNow;

        return Ok(principal);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _logger.LogInformation("Deleting principal with id: {Id}", id);

        var principal = _principals.FirstOrDefault(p => p.Id == id);
        if (principal == null)
        {
            return NotFound($"Principal with id {id} not found");
        }

        _principals.Remove(principal);

        return NoContent();
    }
}