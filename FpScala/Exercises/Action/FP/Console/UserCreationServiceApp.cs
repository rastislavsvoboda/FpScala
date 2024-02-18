namespace FpScala.Exercises.Action.FP.Console;

public class UserCreationServiceApp
{
    public void Run()
    {
        var console = new SystemConsole();
        var clock = new SystemClock();
        var service = new UserCreationService(console, clock);

        var user = service.ReadUser();

        user.UnsafeRun();
    }
}