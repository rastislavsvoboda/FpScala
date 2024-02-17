namespace FpScala.Exercises.Action.FP;

public interface IClock
{
    IO<DateTime> Now { get; }
}