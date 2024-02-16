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
        if (!_inputs.Any()) throw new Exception("No input in the console");

        var line = _inputs.First();
        _inputs.RemoveAt(0);
        return line;
    }

    public void WriteLine(string message)
    {
        _outputs.Add(message);
    }
}