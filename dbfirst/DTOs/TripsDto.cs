namespace db_first.DTOs;

public class TripsDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }

    public ICollection<TripDto> Trips { get; set; } = new List<TripDto>();
}

public class TripDto 
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public ICollection<CountryDto> Countries { get; set; } = null!;

    public ICollection<ClientDto> Clients { get; set; } = null!;
}

public class CountryDto
{
    public string Name { get; set; } = null!;
}

public class ClientDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}