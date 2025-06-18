using db_first.Exceptions;
using db_first.Models;
using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;

namespace db_first.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IDbService _dbService;

    public TripsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _dbService.GetTripsAsync(page, pageSize);
            return Ok(response);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }

        // var response = new
        // {
            // pageNum = page,
            // pageSize,
            // allPages,
            // trips
        // };
    }
}