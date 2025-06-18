namespace db_first.Services;

public interface IClientsService
{
    public Task DeleteClientByIdAsync(CancellationToken cancellationToken, int clientId);
}