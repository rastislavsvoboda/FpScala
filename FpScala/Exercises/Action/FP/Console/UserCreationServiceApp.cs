using PreludeLib;

namespace FpScala.Exercises.Action.FP.Console;

public class UserCreationServiceApp
{
    public void Run()
    {
        var console = new SystemConsole();
        var clock = new SystemClock();
        var service = new UserCreationService(console, clock);

        var user = service.ReadUser()
            .HandleErrorWith(e => IO<Unit>.Debug($"Oops an error occured: {e.Message}").AndThen(new IO<User?>(() => default)))
            .UnsafeRun();

        System.Diagnostics.Debug.WriteLine($"user={user}");
    }
}