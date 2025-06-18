using db_first.DTOs;

namespace db_first.Repositories;

public interface ITripRepository
{
    public Task<TripsDto> GetTripsAsync(int page, int pageSize);
    // public Task<

}