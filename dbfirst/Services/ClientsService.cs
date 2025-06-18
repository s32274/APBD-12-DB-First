using db_first.DAL;
using db_first.Exceptions;
using db_first.Models;
using Microsoft.EntityFrameworkCore;

namespace db_first.Services;

public class ClientsService : IClientsService
{
    private readonly ApbdContext _apbdContext;
    
    public ClientsService(ApbdContext apbdContext)
    {
        _apbdContext = apbdContext;
    }
    
    public async Task DeleteClientByIdAsync(CancellationToken cancellationToken, int clientId)
    {
        var client = await _apbdContext.Clients
            .FirstOrDefaultAsync(c => c.IdClient == clientId, cancellationToken);
        if (client == null)
        {
            throw new NotFoundException("Client with id " + clientId + " was not found.");
        }
        
        bool clientHasTrip = await _apbdContext.ClientTrips
            .AnyAsync(ct => ct.IdClient == clientId, cancellationToken);
        if (clientHasTrip)
        {
            throw new ConflictException("Client with id " + clientId + " is assigned to a trip. Deletion stopped.");
        }


        _apbdContext.Clients.Remove(client);
        await _apbdContext.SaveChangesAsync(cancellationToken);
    }
}