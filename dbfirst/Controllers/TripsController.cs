using db_first.Models;
using Microsoft.AspNetCore.Mvc;

namespace db_first.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTripsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var context = new APBDContext();

        var totalTrips = context.Trips.Count();
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);

        // var trips = context.Trips
        //     .OrderByDescending(t => t.DateFrom)
        //     .Skip((page - 1) * pageSize)
        //     .Take(pageSize)
        //     .Select(t => new
        //     {
        //         t.Name,
        //         t.Description,
        //         t.DateFrom,
        //         t.DateTo,
        //         t.MaxPeople,
        //         Countries = t.IdCountries
        //             .Select(ct => new
        //             {
        //                 ct.Name
        //             }),
        //         Clients = t.Client_Trips
        //             .Select(ctt => new
        //             {
        //                 ctt.Client.FirstName,
        //                 ctt.Client.LastName
        //             })
        //     })
        //     .ToList();
        
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

        var response = new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips
        };

        return Ok(response);
    }
}