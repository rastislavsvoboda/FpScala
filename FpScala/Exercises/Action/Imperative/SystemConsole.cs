namespace FpScala.Exercises.Action.Imperative;

public class SystemConsole : IConsole
{
    public string ReadLine()
    {
        return Console.ReadLine();
    }

    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
}