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

    public static IO<IEnumerable<T>> Sequence(IEnumerable<IO<T>> actions) =>
        new(() => actions.Select(x => x.UnsafeRun()));

    // public static IO<IEnumerable<T>> Sequence2(IEnumerable<IO<T>> actions) =>
    //     new(() => actions.Aggregate(
    //         Enumerable.Empty<T>(),
    //         (acc, item) => acc.Append(item.UnsafeRun())));

    // public static IO<IEnumerable<T>> Sequence3(IEnumerable<IO<T>> actions)
    // {
    //     if (!actions.Any()) return new IO<IEnumerable<T>>(Enumerable.Empty<T>);
    //
    //     var head = actions.First();
    //     var tail = actions.Skip(1);
    //
    //     return from result1 in head
    //         from result2 in Sequence3(tail)
    //         select new List<T> {result1}.Concat(result2);
    // }

    // public static IO<IEnumerable<T>> Sequence4(IEnumerable<IO<T>> actions) =>
    //     actions.Aggregate(new IO<IEnumerable<T>>(Enumerable.Empty<T>),
    //         (state, action) =>
    //             from result1 in state
    //             from result2 in action
    //             select result1.Append(result2));

    public static IO<IEnumerable<TResult>> Traverse<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IO<TResult>> selector) =>
        IO<TResult>.Sequence(source.Select(selector));

    public static IO<T> Fail(Exception error) =>
        new(() => throw error);

    internal static IO<Unit> Debug(string message) =>
        new(() =>
        {
            System.Diagnostics.Debug.WriteLine(message);
            return Unit.Default;
        });
}