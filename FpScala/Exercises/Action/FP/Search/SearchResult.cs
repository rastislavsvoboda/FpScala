using System.Collections.Immutable;

namespace FpScala.Exercises.Action.FP.Search;

public record SearchResult
{
    public Option<Flight> Cheapest => Flights.MinBy(x => x.UnitPrice).ToOption();
    public Option<Flight> Fastest => Flights.MinBy(x => x.Duration).ToOption();
    public Option<Flight> Best => Flights.MinBy(BestOrdering).ToOption();

    public static Func<Flight, (int NumberOfStops, decimal UnitPrice)> BestOrdering =>
        flight => (flight.NumberOfStops, flight.UnitPrice);

    public ImmutableList<Flight> Flights { get; }

    public SearchResult(IEnumerable<Flight> flights)
    {
        Flights = flights
            .GroupBy(f => f.FlightId)
            .Select(grp => grp.MinBy(f => f.UnitPrice)!) // take cheapest when there are duplicate flights
            .OrderBy(BestOrdering)
            .ThenBy(x => x.FlightId) // sort by some unique field to preserve ordering
            .ToImmutableList();
    }

    public SearchResult(params Flight[] flights) : this(flights.AsEnumerable())
    {
    }

    // optionally via LambdaComparer
    // private static LambdaComparer<Flight> BestFlightComparer { get; } =
    //     new((f1, f2) => (f1.NumberOfStops, f1.UnitPrice).CompareTo((f2.NumberOfStops, f2.UnitPrice)));

    // internal IEnumerable<Flight> OrderedList() =>
    //     Flights.OrderBy(BestOrdering);
}