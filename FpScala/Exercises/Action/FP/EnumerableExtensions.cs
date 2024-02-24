namespace FpScala.Exercises.Action.FP;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random) =>
        items.OrderBy(_ => random.Next());
    
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items) =>
        items.SelectMany(x => x);
}