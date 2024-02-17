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
        FlatMap(_ => other);

    public IO<TNext> Map<TNext>(Func<T, TNext> callback)
        => FlatMap(value => new IO<TNext>(() => callback(value)));

    public IO<TNext> FlatMap<TNext>(Func<T, IO<TNext>> callback) =>
        new(() => callback(UnsafeRun()).UnsafeRun());

    public IO<T> OnError<TOther>(Func<Exception, IO<TOther>> cleanup) =>
        HandleErrorWith(ex => cleanup(ex).AndThen(Fail(ex)));

    public IO<T> Retry(int maxAttempt) =>
        maxAttempt switch
        {
            <= 0 => Fail(new ArgumentException("Must be greater than 0", nameof(maxAttempt))),
            1 => this,
            _ => HandleErrorWith(ex => Retry(maxAttempt - 1))
        };

    public IO<T> HandleErrorWith(Func<Exception, IO<T>> callback) =>
        Attempt().FlatMap(result => result switch
        {
            Success<T> (var value) => new IO<T>(() => value),
            Failure<T> (var ex) => callback(ex),
            _ => throw new UnreachableException("Unknown Result subclass")
        });

    public IO<Result<T>> Attempt() =>
        new(() => Try(UnsafeRun));

    public static IO<T> Fail(Exception error) =>
        new(() => throw error);
}