using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Search;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class MockSearchFlightClient : ISearchFlightClient
{
    private readonly IO<IEnumerable<Flight>> _action;

    public MockSearchFlightClient(IO<IEnumerable<Flight>> action)
    {
        _action = action;
    }

    public IO<IEnumerable<Flight>> Search(Airport from, Airport to, DateTime date) =>
        _action;
}