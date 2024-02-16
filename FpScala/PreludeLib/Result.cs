namespace PreludeLib;

public abstract record Result<T>(bool IsSuccess)
{
    public bool IsFailure => !IsSuccess;
    // public abstract Exception Failed();
}

public record Failure<T>(Exception Exception) : Result<T>(false)
{
    // public override Exception Failed() => Exception;
}
public record Success<T>(T Value) : Result<T>(true)
{
    // public override Exception Failed() => throw new InvalidOperationException("this result is success");
}