using FpScala.Exercises.Action.Imperative;

namespace FpScala.Tests.Exercises.Action.Imperative;

public class MockConsole : IConsole
{
    private readonly List<string> _inputs;
    private readonly List<string> _outputs;

    public MockConsole(List<string> inputs, List<string> outputs)
    {
        _inputs = inputs;
        _outputs = outputs;
    }

    public string ReadLine()
    {
        throw new NotImplementedException();
    }

    public void WriteLine(string message)
    {
        throw new NotImplementedException();
    }
}