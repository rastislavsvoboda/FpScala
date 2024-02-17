using FpScala.Exercises.Action.FP;

namespace FpScala.Tests.Exercises.Action.FP;

public class MockClock : IClock
{
    public IO<DateTime> Now { get; }

    public MockClock(DateTime now)
    {
        Now = new IO<DateTime>(() => now);
    }
}