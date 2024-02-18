using PreludeLib;

namespace FpScala.Exercises.Action.FP.Console;

public class SystemConsole : IConsole
{
    public IO<string> ReadLine() =>
        new(() => System.Console.ReadLine());

    public IO<Unit> WriteLine(string message) =>
        new(() =>
        {
            System.Console.WriteLine(message);
            return Unit.Default;
        });
}