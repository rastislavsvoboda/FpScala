using FpScala.Exercises.Action.Imperative;

namespace FpScala.Tests.Exercises.Action.Imperative;

public class MockClock : IClock
{
    public DateTime Now { get; }

    public MockClock(DateTime now)
    {
        Now = now;
    }
}