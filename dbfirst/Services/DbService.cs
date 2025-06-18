using db_first.DTOs;
using db_first.Exceptions;
using db_first.Models;
using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;

namespace db_first.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;
    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnectionString") ?? string.Empty;
    }
    
    public async Task<TripsDto> GetTripsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var context = new APBDContext();

        var totalTrips = context.Trips.Count();
        if (totalTrips == 0)
        {
            throw new NotFoundException("No trips were found.");
        }
        
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);
        
        var trips = (from t in context.Trips
            orderby t.DateFrom descending
            select new
            {
                t.Name,
                Clients = (from ct in context.Client_Trips
                    join c in context.Clients on ct.IdClient equals c.IdClient
                    where ct.IdTrip == t.IdTrip
                    select new
                    {
                        c.FirstName,
                        c.LastName
                    })
            }).ToList();

        var response = new TripsDto()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            // Trips = trips
        };

        return response;
    }
}