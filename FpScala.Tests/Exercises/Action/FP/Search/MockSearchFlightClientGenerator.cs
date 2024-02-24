using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Search;
using FsCheck;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class MockSearchFlightClientGenerator
{
    private static Exception _generalException = new("Boom");
    private static InvalidOperationException _invalidOperationException = new("Operation failed");
    private static IOException _ioException = new("Communication failed");

    public static Arbitrary<MockSearchFlightClient> Generate() =>
        Gen.Frequency(
                new WeightAndValue<Gen<MockSearchFlightClient>>(9, PassingClientGen),
                new WeightAndValue<Gen<MockSearchFlightClient>>(1, FailingClientGen))
            .Select(x => x)
            .ToArbitrary();

    public static Gen<MockSearchFlightClient> PassingClientGen =>
        Gen.ListOf(FlightGenerator.Generate().Generator)
            .Select(flights => new MockSearchFlightClient(new IO<IEnumerable<Flight>>(() => flights)));

    public static Gen<MockSearchFlightClient> FailingClientGen =>
        Gen.Elements(_generalException, _invalidOperationException, _ioException)
            .Select(ex => new MockSearchFlightClient(IO<IEnumerable<Flight>>.Fail(ex)));
}