using System.Diagnostics;

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

    public static T Retry<T>(int maxAttempt, Func<T> func) =>
        maxAttempt switch
        {
            <= 0 => throw new ArgumentException("Must be greater than 0", nameof(maxAttempt)),
            1 => func(),
            _ => Try(func) switch
            {
                Success<T>(var value) => value,
                Failure<T>(_) => Retry(maxAttempt - 1, func),
                _ => throw new UnreachableException("Unknown Result subclass")
            }
        };

    public static T OnError<T>(Func<T> func, Action<Exception> cleanup)
    {
        switch (Try(func))
        {
            case Success<T>(var value):
                return value;
            case Failure<T>(var exception):
                cleanup(exception);
                throw exception;
            default:
                throw new UnreachableException("Unknown Result subclass");
        }
    }

    public static void Require(bool condition, string message)
    {
        if (!condition) throw new ArgumentException(message);
    }

    public static void Require(bool condition, string message, string parameter)
    {
        if (!condition) throw new ArgumentException(message, parameter);
    }

    public static void Require(bool condition, Func<Exception> getException)
    {
        if (!condition) throw getException();
    }

    public static T Id<T>(this T @this) =>
        @this;
}       