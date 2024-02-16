using static FpScala.Exercises.Action.Imperative.UserCreationApp;
using static PreludeLib.Prelude;

namespace FpScala.Tests.Exercises.Action.Imperative;

using FpScala.Exercises.Action.Imperative;
using FluentAssertions;
using Xunit;

public class UserCreationExercisesTest
{
    [Fact]
    public void TestParseYesNo()
    {
        ParseYesNo("Y").Should().BeTrue();
        ParseYesNo("N").Should().BeFalse();
        Assert.Throws<ArgumentException>(() => ParseYesNo("Never"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadSubscriberToMailingList_example_success(bool expectedValue)
    {
        var inputs = new List<string> {FormatYesNo(expectedValue)};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);

        var result = ReadSubscribeToMailingList(console);

        result.Should().Be(expectedValue);
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"));
    }

    [Fact]
    public void ReadSubscriberToMailingList_example_failure()
    {
        var console = new MockConsole(new List<string> {"Never"}, new List<string>());

        var result = Try(() => ReadSubscribeToMailingList(console));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReadDateOfBirth_example_success()
    {
        var console = new MockConsole(new List<string> {"21-09-1975"}, new List<string>());

        var result = ReadDateOfBirth(console);

        result.Should().Be(new DateOnly(1975, 9, 21));
    }

    [Fact]
    public void ReadDateOfBirth_example_failure()
    {
        var console = new MockConsole(new List<string> {"21/09/1975"}, new List<string>());

        var result = Try(() => ReadDateOfBirth(console));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TestReadUser()
    {
        var inputs = new List<string> {"Rastislav", "21-09-1975", "Y"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var now = new DateTime(2006, 1, 2, 3, 4, 5);
        var clock = new MockClock(now);
        var expected = new User("Rastislav", new DateOnly(1975, 9, 21), true, now);

        var result = ReadUser(console, clock);

        result.Should().Be(expected);
        Assert.Collection(outputs,
            item => item.Should().Be("What's your name?"),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]")
        );
    }
}