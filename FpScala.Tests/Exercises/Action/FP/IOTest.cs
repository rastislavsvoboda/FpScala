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

    [Fact]
    public void Retry_when_attempt_is_0()
    {
        var counter = 0;
        var action = new IO<int>(() => counter += 1).Retry(0);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => action.UnsafeRun());

        result.IsFailure.Should().BeTrue();
        ((Failure<int>) result).Exception.Message.Should().Be("Must be greater than 0 (Parameter 'maxAttempt')");
        counter.Should().Be(0, because: "callback was executed");
    }

    [Fact]
    public void Retry_when_action_fails()
    {
        var counter = 0;
        var error = new Exception("Boom");
        var action = new IO<int>(() =>
        {
            counter += 1;
            throw error;
        });
        var retryAction = action.Retry(5);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => retryAction.UnsafeRun());

        result.Should().Be(new Failure<int>(error));
        counter.Should().Be(5);
    }

    [Fact]
    public void Retry_fails_if_max_attempt_is_too_low()
    {
        var counter = 0;
        var error = new Exception("Boom");
        var action = new IO<string>(() =>
        {
            counter += 1;
            if (counter < 3) throw error;
            return "Hello";
        });
        var retryAction = action.Retry(2);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => retryAction.UnsafeRun());

        result.Should().Be(new Failure<string>(error));
        counter.Should().Be(2);
    }

    [Fact]
    public void Retry_until_action_succeeds()
    {
        var counter = 0;
        var error = new Exception("Boom");
        var action = new IO<string>(() =>
        {
            counter += 1;
            if (counter < 3) throw error;
            return "Hello";
        });
        var retryAction = action.Retry(5);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = Try(() => retryAction.UnsafeRun());

        result.Should().Be(new Success<string>("Hello"));
        counter.Should().Be(3);
    }

    [Fact]
    public void Attempt_success()
    {
        var counter = 0;

        var action = new IO<int>(() => counter += 1).Attempt();
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        counter.Should().Be(1);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Attempt_failure()
    {
        var counter = 0;
        var error = new Exception("Boom");
        var action = new IO<int>(() =>
        {
            counter += 1;
            throw error;
        }).Attempt();
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        counter.Should().Be(1);
        result.Should().Be(new Failure<int>(error));
    }

    [Fact]
    public void HandleWithError_success()
    {
        var counter = 0;
        var first = new IO<int>(() => counter += 1).AndThen(new IO<string>(() => "A"));
        var second = new IO<int>(() => counter *= 2).AndThen(new IO<string>(() => "B"));
        var action = first.HandleErrorWith(_ => second);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        result.Should().Be("A");
        counter.Should().Be(1, because: "Only first is executed");
    }

    [Fact]
    public void HandleWithError_failure()
    {
        var counter = 0;
        var first = new IO<int>(() => counter += 1).AndThen(IO<int>.Fail(new Exception("Boom")));
        var second = new IO<int>(() => counter *= 2).AndThen(new IO<int>(() => -1));
        var action = first.HandleErrorWith(_ => second);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        counter.Should().Be(2, because: "first and second were executed in the expected order");
        result.Should().Be(-1);
    }

    // Search Flight Exercises

    [Fact]
    public void Sequence()
    {
        var counter = 0;
        var action = new List<IO<int>>
        {
            new(() => counter += 2),
            new(() => counter *= 3),
            new(() => counter -= 1),
        }.Sequence();
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        result.Should().BeEquivalentTo(new[] {2, 6, 5});
        counter.Should().Be(5);
    }

    [Fact]
    public void Traverse()
    {
        var counter = 0;
        var data = new[] {1, 2, 3, 4, 5};
        var action = data.Traverse(n => new IO<char>(() =>
        {
            counter += 1;
            var ordNum = Convert.ToUInt16('a') + n - 1;
            return Convert.ToChar(ordNum);
        }));
        counter.Should().Be(0, because: "nothing happened before UnsafeRun");

        var result = action.UnsafeRun();

        result.Should().BeEquivalentTo(new[] {'a', 'b', 'c', 'd', 'e'});
        counter.Should().Be(5);
    }
}