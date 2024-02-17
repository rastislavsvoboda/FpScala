using FluentAssertions;
using FpScala.Exercises.Action.FP;
using PreludeLib;
using Xunit;

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
    public void AndThen()
    {
        var counter = 0;

        var first = new IO<int>(() => counter += 1);
        var second = new IO<int>(() => counter *= 2);

        var action = first.AndThen(second);
        counter.Should().Be(0, because: "nothing happened before UnsafeRun"); 

        action.UnsafeRun();
        counter.Should().Be(2);
    }
}