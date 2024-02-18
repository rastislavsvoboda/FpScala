namespace FpScala.Exercises.Action.FP.Console;

public class SystemClock : IClock
{
    public IO<DateTime> Now =>
        new (() => DateTime.Now);
}