using db_first.DAL;
using db_first.DTOs;
using db_first.Exceptions;
using db_first.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace db_first.Services;

public class TripsService : ITripsService
{
    private readonly ApbdContext _apbdContext;
    
    public TripsService(ApbdContext apbdContext)
    {
        _apbdContext = apbdContext;
    }
    
    public async Task<TripsDto> GetTripsAsync(
        CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10
        )
    {
        var totalTrips = await _apbdContext.Trips.CountAsync(cancellationToken);
        if (totalTrips == 0)
        {
            throw new NotFoundException("No trips were found.");
        }
        
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);
        
        var trips = await _apbdContext.Trips
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

    public async Task<AssignClientDto> AssignClientToTripAsync(
        CancellationToken cancellationToken, [FromQuery] AssignClientDto assignClientDto
        )
    {
        // 1. Sprawdzenie, czy klient o podanym numerze PESEL istnieje 
        bool clientsPeselExists = await _apbdContext.Clients
            .AnyAsync(c => c.Pesel == assignClientDto.Pesel, cancellationToken);

        if (clientsPeselExists)
        {
            throw new ConflictException("Client with PESEL " + assignClientDto.Pesel + " already exists.");
        }

        // 2. Sprawdzenie, czy klient o podanym numerze PESEL już jest zapisany na daną wycieczkę
        bool clientAlreadyAssigned = await _apbdContext.ClientTrips
            .AnyAsync(ct => ct.IdClientNavigation.Pesel == assignClientDto.Pesel, cancellationToken);

        if (clientAlreadyAssigned)
        {
            throw new ConflictException(
                "Client with PESEL " + assignClientDto.Pesel 
                + " is already assigned to trip with id " + assignClientDto.IdTrip + "."
            );
        }
        
        // 3. Sprawdzenie, czy dana wycieczka istnieje i czy DateFrom jest w przyszłości (czy dopiero się odbędzie)
        bool tripExists = await _apbdContext.Trips
            .AnyAsync(t => t.IdTrip == assignClientDto.IdTrip, cancellationToken);

        if (!tripExists)
        {
            throw new NotFoundException("Trip with id " + assignClientDto.IdTrip + " was not found.");
        }

        bool tripAlreadyBegan = await _apbdContext.Trips
            .AnyAsync(t => t.DateFrom < DateTime.Now, cancellationToken);

        if (tripAlreadyBegan)
        {
            throw new ConflictException("Trip with id " + assignClientDto.IdTrip + " has already begun.");
        }
        
        // 4.  PaymentDate może mieć wartość NULL, jeśli klient jeszcze nie zapłacił za wycieczkę 
        //      RegisteredAt w tabeli Client_Trip jest zgodne z czasem przyjęcia żądania na serwer
        _apbdContext.Clients.Add(new Client
        {
            FirstName = assignClientDto.FirstName,
            LastName = assignClientDto.LastName,
            Email = assignClientDto.Email,
            Telephone = assignClientDto.Telephone,
            Pesel = assignClientDto.Pesel
        });

        var clientId = await _apbdContext.Clients
            .Where(c => c.Pesel == assignClientDto.Pesel)
            .Select(c => c.IdClient)
            .FirstOrDefaultAsync(cancellationToken);
            
        _apbdContext.ClientTrips.Add(new ClientTrip
        {
            IdClient = clientId,
            IdTrip = assignClientDto.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = assignClientDto.PaymentDate
        });

        return assignClientDto;
    }
}