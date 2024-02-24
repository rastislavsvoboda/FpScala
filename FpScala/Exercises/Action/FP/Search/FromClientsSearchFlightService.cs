using System.Collections.Immutable;

namespace FpScala.Exercises.Action.FP.Search;

public class FromClientsSearchFlightService : ISearchFlightService
{
    private readonly ImmutableList<ISearchFlightClient> _clients;

    public FromClientsSearchFlightService(IEnumerable<ISearchFlightClient> clients)
    {
        _clients = clients.ToImmutableList();
    }

    public IO<SearchResult> Search(Airport fromAirport, Airport toAirport, DateTime date)
    {
        return _clients.ParTraverse(SearchByClient)
            .Map(x => x.Flatten())
            .Select(flights => new SearchResult(flights));

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