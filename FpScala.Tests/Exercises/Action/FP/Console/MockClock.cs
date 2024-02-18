using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Console;

namespace FpScala.Tests.Exercises.Action.FP.Console;

public class MockClock : IClock
{
    public IO<DateTime> Now { get; }

    public MockClock(DateTime now)
    {
        Now = new IO<DateTime>(() => now);
    }
}