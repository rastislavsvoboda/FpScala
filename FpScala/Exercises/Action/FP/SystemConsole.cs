using PreludeLib;

namespace FpScala.Exercises.Action.FP;

public class SystemConsole : IConsole
{
    public IO<string> ReadLine() =>
        new(() => Console.ReadLine());

    public IO<Unit> WriteLine(string message) =>
        new(() =>
        {
            Console.WriteLine(message);
            return Unit.Default;
        });
}