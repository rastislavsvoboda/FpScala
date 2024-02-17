using FpScala.Exercises.Action.FP;
using PreludeLib;

namespace FpScala.Tests.Exercises.Action.FP;

public class MockConsole : IConsole
{
    private readonly List<string> _inputs;
    private readonly List<string> _outputs;

    public MockConsole(List<string> inputs, List<string> outputs)
    {
        _inputs = inputs;
        _outputs = outputs;
    }

    public IO<string> ReadLine() =>
        new(() =>
        {
            if (!_inputs.Any()) throw new Exception("No input in the console");

            var line = _inputs.First();
            _inputs.RemoveAt(0);
            return line;
        });

    public IO<Unit> WriteLine(string message) =>
        new(() =>
        {
            _outputs.Add(message);
            return Unit.Default;
        });
}