using db_first.Exceptions;
using db_first.Services;
using Microsoft.AspNetCore.Mvc;

namespace db_first.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;
    
    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }
    
    [HttpDelete]
    [Route("{clientId}")]
    public async Task<IActionResult> DeleteClientByIdAsync(CancellationToken cancellationToken, int clientId)
    {
        try
        {
            await _clientsService.DeleteClientByIdAsync(cancellationToken, clientId);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}