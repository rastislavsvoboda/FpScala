namespace PreludeLib;

public static class Prelude
{
    public static Result Try(Action action)
    {
        try
        {
            action();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail(e);
        }
    }
}