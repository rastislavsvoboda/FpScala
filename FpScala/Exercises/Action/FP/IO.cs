using System.Diagnostics;
using PreludeLib;
using static PreludeLib.Utils;

namespace FpScala.Exercises.Action.FP;

public class IO<T>
{
    private readonly Func<T> _func;

    public IO(Func<T> func)
    {
        _func = func;
    }

    public T UnsafeRun() =>
        _func();

    public IO<TOther> AndThen<TOther>(IO<TOther> other) =>
        new(() =>
        {
            // run current action but discard result
            UnsafeRun();
            // run other and return it's result
            return other.UnsafeRun();
        });

    public IO<TNext> Map<TNext>(Func<T, TNext> callback) =>
        new(() => callback(UnsafeRun()));

    public IO<TNext> FlatMap<TNext>(Func<T, IO<TNext>> callback) =>
        new(() => callback(UnsafeRun()).UnsafeRun());

    public IO<T> OnError<TOther>(Func<Exception, IO<TOther>> cleanup) =>
        new(() =>
        {
            switch (Try(UnsafeRun))
            {
                case Success<T>(var value):
                    return value;
                case Failure<T>(var exception):
                    cleanup(exception).UnsafeRun();
                    throw exception;
                default:
                    throw new UnreachableException("Unknown Result subclass");
            }
        });

    public IO<T> Retry(int maxAttempt) =>
        new(() => maxAttempt switch
        {
            <= 0 => throw new ArgumentException("Must be greater than 0", nameof(maxAttempt)),
            1 => UnsafeRun(),
            _ => Try(UnsafeRun) switch
            {
                Success<T>(var value) => value,
                Failure<T>(_) => Retry(maxAttempt - 1).UnsafeRun(),
                _ => throw new UnreachableException("Unknown Result subclass")
            }
        });

    // public IO<T> HandleErrorWith(IO<Unit> callback)
    // {
    //     callback;
    //     return this;
    // }

    public static IO<T> Fail(Exception error) =>
        new(() => throw error);
}