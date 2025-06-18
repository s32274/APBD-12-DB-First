namespace db_first.Services;

public interface IClientsService
{
    public Task<bool> DeleteClientByIdAsync(CancellationToken cancellationToken, int clientId);
}