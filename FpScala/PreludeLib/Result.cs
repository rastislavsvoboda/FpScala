namespace PreludeLib;

public class Result
{
    private Exception? _failure;

    public bool IsFailure => _failure != null;
    public bool IsSuccess => !IsFailure;

    private Result()
    {
    }

    public static Result Success()
        => new Result();

    public static Result Fail(Exception exception)
        => new Result() {_failure = exception};
}

/*
public class Result<T> where T : class
{
    private Exception? _failure;

    public T? Value { get; }
    public bool IsFailure => _failure != null;
    public bool IsSuccess => !IsFailure;

    private Result(T? value)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new Result<T>(value);

    public static Result<T> Fail(Exception exception)
        => new Result<T>(null) {_failure = exception};
}
*/