using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Search;
using FsCheck;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class MockSearchFlightClientGenerator
{
    public static Arbitrary<MockSearchFlightClient> Generate()
    {
        var flight = FlightGeneratorEx.Generate();
        var passing = Gen.Constant(new MockSearchFlightClient(new IO<IEnumerable<Flight>>(() => new[] {flight})));
        var failing = Gen.Constant(new MockSearchFlightClient(IO<IEnumerable<Flight>>.Fail(new Exception("Boom"))));

        var frq = Gen.Frequency(
            new WeightAndValue<Gen<MockSearchFlightClient>>(9, passing),
            new WeightAndValue<Gen<MockSearchFlightClient>>(1, failing));

        return frq.Select(x => x).ToArbitrary();
    }
}