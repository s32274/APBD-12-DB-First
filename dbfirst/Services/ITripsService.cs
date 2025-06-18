using db_first.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace db_first.Services;

public interface ITripsService
{
    public Task<TripsDto> GetTripsAsync(CancellationToken cancellationToken, int page, int pageSize);
    public Task<AssignClientDto> AssignClientToTripAsync(CancellationToken cancellationToken, [FromBody] AssignClientDto assignClientDto);
}