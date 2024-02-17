using FluentAssertions;
using FpScala.Exercises.Action.FP;
using PreludeLib;
using Xunit;
using static PreludeLib.Utils;

namespace FpScala.Tests.Exercises.Action.FP;

public class IOTest
{
    [Fact]
    public void Apply_is_lazy_and_repeatable()
    {
        var counter = 0;

        var action = new IO<int>(() => counter += 1);
        counter.Should().Be(0, because: "nothing happened");

        action.UnsafeRun();
        counter.Should().Be(1);

        action.UnsafeRun();
        counter.Should().Be(2);
    }

    [Fact]
    public void AndThen_is_lazy_and_call_first_and_then_second_IO_action()
    {
        var counter = 0;

        var first = new IO<int>(() => counter += 1);
        var second = new IO<int>(() => counter *= 2);

        var action = first.AndThen(second);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        action.UnsafeRun();
        counter.Should().Be(2, because: "first and second were executed in the expected order");
    }

    [Fact]
    public void Map_is_lazy_and_call_first_IO_action_and_then_just_second_function()
    {
        var counter = 0;

        var first = new IO<int>(() => counter += 1);
        var action = first.Map(_ => "Hello");
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        action.UnsafeRun();
        counter.Should().Be(1, because: "first was executed");
    }

    [Fact]
    public void FlatMap()
    {
        var counter = 0;

        var first = new IO<int>(() => counter += 1);
        var second = new IO<int>(() => counter *= 2);

        var action = first.FlatMap(_ => second);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        action.UnsafeRun();
        counter.Should().Be(2, because: "first and second were executed in the expected order");
    }

    // PART 3: Error handling

    [Fact]
    public void OnError_success()
    {
        var counter = 0;

        var action = new IO<string>(() =>
            {
                counter += 1;
                return "OK";
            })
            .OnError(_ => new IO<int>(() => counter *= 2));
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => action.UnsafeRun());
        counter.Should().Be(1, because: "first action was executed but not the callback");
        result.Should().Be(new Success<string>("OK"));
    }

    [Fact]
    public void OnError_failure()
    {
        var counter = 0;
        var error1 = new Exception("Boom 1");

        var action = new IO<string>(() => throw error1)
            .OnError(_ => new IO<int>(() => counter += 1));
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => action.UnsafeRun());
        counter.Should().Be(1, because: "callback was executed");
        result.Should().Be(new Failure<string>(error1));
    }
}