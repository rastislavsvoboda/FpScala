namespace FpScala.Exercises.Action.FP;

public class LambdaComparer<T> : IComparer<T>
{
    private readonly Func<T, T, int> _compFunc;

    public LambdaComparer(Func<T, T, int> compFunc)
    {
        _compFunc = compFunc;
    }

    public int Compare(T? x, T? y) =>
        _compFunc(x, y);
}