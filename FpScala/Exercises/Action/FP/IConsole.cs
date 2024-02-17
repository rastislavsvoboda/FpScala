using PreludeLib;

namespace FpScala.Exercises.Action.FP;

public interface IConsole
{
    IO<string> ReadLine();
    IO<Unit> WriteLine(string message);
}