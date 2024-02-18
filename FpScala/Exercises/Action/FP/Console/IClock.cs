namespace FpScala.Exercises.Action.FP.Console;

public interface IClock
{
    IO<DateTime> Now { get; }
}