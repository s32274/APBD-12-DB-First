using db_first.DAL;
using db_first.DTOs;
using db_first.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace db_first.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString;
    
    public TripsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnectionString") ?? string.Empty;
    }
    
    public async Task<TripsDto> GetTripsAsync(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var context = new ApbdContext();

        var totalTrips = await context.Trips.CountAsync(cancellationToken);
        if (totalTrips == 0)
        {
            throw new NotFoundException("No trips were found.");
        }
        
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);
        
        var trips = await context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var response = new TripsDto()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = trips
        };

        return response;
    }
}