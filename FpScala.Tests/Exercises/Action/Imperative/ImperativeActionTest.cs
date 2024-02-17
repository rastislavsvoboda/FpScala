using FluentAssertions;
using PreludeLib;
using Xunit;
using static PreludeLib.Utils;

namespace FpScala.Tests.Exercises.Action.Imperative;

public class ImperativeActionTest
{
    [Fact]
    public void Retry_when_attempt_is_0()
    {
        var result = Try(() => Retry(0, () => ""));

        result.IsFailure.Should().BeTrue();
        ((Failure<string>) result).Exception.Message.Should().Be("Must be greater than 0 (Parameter 'maxAttempt')");
    }

    [Fact]
    public void Retry_when_action_fails()
    {
        var counter = 0;
        var error = new Exception("Boom");

        var result = Try(() => Retry<Unit>(5, () =>
        {
            counter += 1;
            throw error;
        }));

        result.Should().Be(new Failure<Unit>(error));
        counter.Should().Be(5);
    }

    [Fact]
    public void Retry_until_action_succeeds()
    {
        var counter = 0;

        var result = Try(() => Retry<string>(5, () =>
        {
            counter += 1;
            Require(counter >= 3, "Counter is too low");
            return "Hello";
        }));

        result.Should().Be(new Success<string>("Hello"));
        counter.Should().Be(3);
    }

    [Fact]
    public void OnError_failure()
    {
        var counter = 0;
        var exception = new Exception("Boom");

        var result = Try(() => OnError<Unit>(
            func: () => throw exception,
            cleanup: _ => counter += 1));

        result.Should().Be(new Failure<Unit>(exception));
        counter.Should().Be(1);
    }

    [Fact]
    public void OnError_success()
    {
        var counter = 0;

        var result = Try(() => OnError<string>(
            func: () => "Hello",
            cleanup: _ => counter += 1));

        result.Should().Be(new Success<string>("Hello"));
        counter.Should().Be(0);
    }
}