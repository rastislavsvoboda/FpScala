namespace FpScala.Exercises.Action.FP;

public class SystemClock : IClock
{
    public IO<DateTime> Now =>
        new (() => DateTime.Now);
}