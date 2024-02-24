using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Search;
using FsCheck;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class MockSearchFlightClientGenerator
{
    private static Exception _generalException = new("Boom");
    private static InvalidOperationException _invalidOperationException = new("Operation failed");
    private static IOException _ioException = new("Communication failed");

    public static Arbitrary<ISearchFlightClient> Generate() =>
        Gen.Frequency(
                new WeightAndValue<Gen<ISearchFlightClient>>(9, PassingClientGen),
                new WeightAndValue<Gen<ISearchFlightClient>>(1, FailingClientGen))
            .Select(x => x)
            .ToArbitrary();

    public static Gen<ISearchFlightClient> PassingClientGen =>
        Gen.ListOf(FlightGenerator.Generate().Generator)
            .Select(flights => (ISearchFlightClient)new MockSearchFlightClient(new IO<IEnumerable<Flight>>(() => flights)));

    public static Gen<ISearchFlightClient> FailingClientGen =>
        Gen.Elements(_generalException, _invalidOperationException, _ioException)
            .Select(ex => (ISearchFlightClient)new MockSearchFlightClient(IO<IEnumerable<Flight>>.Fail(ex)));
}