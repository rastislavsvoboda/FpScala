using FpScala.Exercises.Action.FP.Search;
using FsCheck;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class FlightGenerator
{
    public static Arbitrary<Flight> Generate()
    {
        var flightIdGen = Arb.Generate<Guid>()
            .Select(n => n.ToString());

        var airportGen = Arb.Generate<Airport>().Two()
            .Where(tuple => tuple.Item1 != tuple.Item2)
            .Select(tuple => (airportFrom: tuple.Item1, airportTo: tuple.Item2));

        var airlineGen = Gen.Elements("British Airlines", "Fufthansa", "Air France", "Lastminute.com");

        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        var departureGen = Arb.Generate<DateTime>()
            .Where(date => minDate <= date && date <= maxDate);

        var durationGen = Gen.Choose(20, 2400).Select(minutes => TimeSpan.FromMinutes(minutes));
        var numberOfStopsGen = Gen.Choose(0, 4);
        var priceGen = Gen.Choose(0, 4000).Select(Convert.ToDecimal);

        var flightGen = from id in flightIdGen
            from airline in airlineGen
            from airports in airportGen
            from departure in departureGen
            from duration in durationGen
            from numberOfStops in numberOfStopsGen
            from price in priceGen
            select new Flight(
                FlightId: id,
                Airline: airline,
                From: airports.airportFrom,
                To: airports.airportTo,
                DepartureAt: departure,
                Duration: duration,
                NumberOfStops: numberOfStops,
                UnitPrice: price,
                RedirectLink: "");

        return flightGen.ToArbitrary();
    }
}