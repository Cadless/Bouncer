using Microsoft.AspNetCore.Mvc;
using Bouncer.Api.Models;
using Bouncer.Api.Services;

namespace Bouncer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrincipalController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<PrincipalController> _logger;

    public PrincipalController(IDatabaseService databaseService, ILogger<PrincipalController> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Principal>>> GetAll()
    {
        _logger.LogInformation("Getting all principals");
        var principals = await _databaseService.GetAllPrincipalsAsync();
        return Ok(principals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Principal>> GetById(int id)
    {
        _logger.LogInformation("Getting principal with id: {Id}", id);

        var principal = await _databaseService.GetPrincipalByIdAsync(id);
        if (principal == null)
        {
            return NotFound($"Principal with id {id} not found");
        }

        return Ok(principal);
    }

    [HttpGet("external/{externalId}")]
    public async Task<ActionResult<Principal>> GetByExternalId(string externalId)
    {
        _logger.LogInformation("Getting principal with external id: {ExternalId}", externalId);

        var principal = await _databaseService.GetPrincipalByExternalIdAsync(externalId);
        if (principal == null)
        {
            return NotFound($"Principal with external id '{externalId}' not found");
        }

        return Ok(principal);
    }

    [HttpPost]
    public async Task<ActionResult<Principal>> Create(CreatePrincipalRequest request)
    {
        _logger.LogInformation("Creating new principal with external id: {ExternalId}", request.ExternalId);

        if (string.IsNullOrWhiteSpace(request.ExternalId))
        {
            return BadRequest("ExternalId is required");
        }

        // Check if ExternalId already exists
        var existingPrincipal = await _databaseService.GetPrincipalByExternalIdAsync(request.ExternalId);
        if (existingPrincipal != null)
        {
            return Conflict($"Principal with external id '{request.ExternalId}' already exists");
        }

        try
        {
            var principal = await _databaseService.CreatePrincipalAsync(request.ExternalId);
            return CreatedAtAction(nameof(GetById), new { id = principal.Id }, principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating principal with external id: {ExternalId}", request.ExternalId);
            return StatusCode(500, "An error occurred while creating the principal");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Principal>> Update(int id, UpdatePrincipalRequest request)
    {
        _logger.LogInformation("Updating principal with id: {Id}", id);

        if (string.IsNullOrWhiteSpace(request.ExternalId))
        {
            return BadRequest("ExternalId is required");
        }

        // Check if ExternalId already exists (excluding current principal)
        var existingPrincipal = await _databaseService.GetPrincipalByExternalIdAsync(request.ExternalId);
        if (existingPrincipal != null && existingPrincipal.Id != id)
        {
            return Conflict($"Principal with external id '{request.ExternalId}' already exists");
        }

        try
        {
            var updatedPrincipal = await _databaseService.UpdatePrincipalAsync(id, request.ExternalId);
            if (updatedPrincipal == null)
            {
                return NotFound($"Principal with id {id} not found");
            }

            return Ok(updatedPrincipal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating principal with id: {Id}", id);
            return StatusCode(500, "An error occurred while updating the principal");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting principal with id: {Id}", id);

        try
        {
            var deleted = await _databaseService.DeletePrincipalAsync(id);
            if (!deleted)
            {
                return NotFound($"Principal with id {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting principal with id: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the principal");
        }
    }
}