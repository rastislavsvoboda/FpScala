namespace FpScala.Exercises.Action.FP.Search;

public class SearchFlightClient : ISearchFlightClient
{
    public IO<IEnumerable<Flight>> Search(Airport from, Airport to, DateTime date)
    {
        // TODO: this would call some web API to get flights from some provider
        var flights = new Flight[] { };
        return new IO<IEnumerable<Flight>>(() => flights);
    }
}