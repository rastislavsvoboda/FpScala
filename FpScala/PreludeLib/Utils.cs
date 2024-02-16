namespace PreludeLib;

public static class Utils
{
    public static Result<Unit> Try(Action action)
    {
        try
        {
            action();
            return new Success<Unit>(Unit.Default);
        }
        catch (Exception e)
        {
            return new Failure<Unit>(e);
        }
    }


    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            var value = func();
            return new Success<T>(value);
        }
        catch (Exception e)
        {
            return new Failure<T>(e);
        }
    }
}