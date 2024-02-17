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

    public IO<T> HandleErrorWith(Func<Exception, IO<T>> callback)
    {
        throw new NotImplementedException();

        // try
        // {
        //     this.UnsafeRun();
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        //     throw;
        // }
    }

    public static IO<T> Fail(Exception error) =>
        new(() => throw error);
}