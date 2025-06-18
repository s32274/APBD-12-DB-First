using db_first.DTOs;
using db_first.Exceptions;
using Microsoft.AspNetCore.Mvc;
using db_first.Services;

namespace db_first.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(
        CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10
        )
    {
        try
        {
            var response = await _tripsService.GetTripsAsync(cancellationToken, page, pageSize);
            return Ok(response);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    [Route("{tripId}/clients")]
    public async Task<IActionResult> AssignClientToTripAsync(
        CancellationToken cancellationToken, AssignClientDto assignClientDto
        )
    {
        try
        {
            await _tripsService.AssignClientToTripAsync(cancellationToken, assignClientDto);
            return Created();
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