using FpScala.Exercises.Action.FP.Search;
using FsCheck;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class FlightGeneratorEx
{
    public static Flight Generate()
    {
        // TODO: how to make Arbitrary<Flight> a have real generator?

        var flightIdGen = Arb.Default.Int16().Filter(n => n > 0).Generator.Select(n => n.ToString());

        var airportGen = Arb.Generate<Airport>().Two()
            .Where(tuple => tuple.Item1 != tuple.Item2)
            .Select(tuple => (airportFrom: tuple.Item1, airportTo: tuple.Item2));

        var airlineGen = Gen.Elements("British Airlines", "Fufthansa", "Air France", "Lastminute.com");

        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        var departureGen = Arb.Default.DateTime().Filter(date => minDate <= date && date <= maxDate).Generator;

        var durationGen = Gen.Choose(20, 2400).Select(minutes => TimeSpan.FromMinutes(minutes));
        var numberOfStopsGen = Gen.Choose(0, 4);
        var priceGen = Gen.Choose(0, 4000).Select(Convert.ToDecimal);

        var (airportFrom, airportTo) = airportGen.Generate();

        return new Flight(
            flightIdGen.Generate(),
            airlineGen.Generate(),
            airportFrom,
            airportTo,
            departureGen.Generate(),
            durationGen.Generate(),
            numberOfStopsGen.Generate(),
            priceGen.Generate(),
            "");
    }
}