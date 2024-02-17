namespace FpScala.Exercises.Action.FP;

public static class IOExtensions
{
    public static IO<TResult> Select<TSource, TResult>(this IO<TSource> source, Func<TSource, TResult> selector) =>
        new(() =>
        {
            var res1 = source.UnsafeRun();
            var res2 = selector(res1);
            return res2;
        });
    
    public static IO<TResult> SelectMany<TSource, TCollection, TResult>(
        this IO<TSource> source,
        Func<TSource, IO<TCollection>> collectionSelector,
        Func<TSource, TCollection, TResult> resultSelector) =>
        new(() =>
        {
            var res1 = source.UnsafeRun();
            var res2 = collectionSelector(res1).UnsafeRun();
            var res3 = resultSelector(res1, res2);
            return res3;
        });
}