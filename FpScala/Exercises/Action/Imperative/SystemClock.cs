namespace FpScala.Exercises.Action.Imperative;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}