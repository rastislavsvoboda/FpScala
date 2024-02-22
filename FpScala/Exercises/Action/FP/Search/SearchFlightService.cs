namespace FpScala.Exercises.Action.FP.Search;

public class FromTwoClientsSearchFlightService : ISearchFlightService
{
    private readonly ISearchFlightClient _client1;
    private readonly ISearchFlightClient _client2;

    public FromTwoClientsSearchFlightService(ISearchFlightClient client1, ISearchFlightClient client2)
    {
        _client1 = client1;
        _client2 = client2;
    }

    public IO<SearchResult> Search(Airport fromAirport, Airport toAirport, DateTime date)
    {
        return
            from flights1 in SearchByClient(_client1)
            from flights2 in SearchByClient(_client2)
            select new SearchResult(flights1.Concat(flights2));

        // helper func
        IO<IEnumerable<Flight>> SearchByClient(ISearchFlightClient client) =>
            client
                .Search(fromAirport, toAirport, date)
                .HandleErrorWith(e =>
                    IO<IEnumerable<Flight>>
                        .Debug($"Oops an error occured: ${e}")
                        .AndThen(new IO<IEnumerable<Flight>>(Array.Empty<Flight>)));
    }
}