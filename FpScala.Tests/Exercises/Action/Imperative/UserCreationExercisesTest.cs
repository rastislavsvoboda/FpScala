using PreludeLib;
using static FpScala.Exercises.Action.Imperative.UserCreationApp;
using static PreludeLib.Utils;

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
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"));
    }

    // PART 2: Error handling

    [Fact]
    public void ReadSubscribeToMailingListRetry_negative_maxAttempt()
    {
        var console = new MockConsole(new List<string>(), new List<string>());

        var result = Try(() => ReadSubscribeToMailingListRetry(console, maxAttempt: -1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReadSubscribeToMailingListRetry_example_success()
    {
        var outputs = new List<string>();
        var console = new MockConsole(new List<string> {"Never", "N"}, outputs);

        var result = ReadSubscribeToMailingListRetry(console, maxAttempt: 2);

        result.Should().BeFalse();
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"));
    }

    [Fact]
    public void ReadSubscribeToMailingListRetry_example_invalid_input()
    {
        var outputs = new List<string>();
        var console = new MockConsole(new List<string> {"Never"}, outputs);

        var result = Try(() => ReadSubscribeToMailingListRetry(console, maxAttempt: 1));

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));

        // check that the error message is the same as 'ReadSubscribeToMailingList'
        var console2 = new MockConsole(new List<string> {"Never"}, new List<string>());
        var result2 = Try(() => ReadSubscribeToMailingList(console2));
        Assert.Equal(((Failure<bool>) result).Exception.Message, ((Failure<bool>) result2).Exception.Message);
    }

    [Fact]
    public void ReadDateOfBirthRetry_negative_maxAttempt()
    {
        var console = new MockConsole(new List<string>(), new List<string>());

        var result = Try(() => ReadDateOfBirthRetry(console, maxAttempt: -1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReadDateOfBirthRetry_example_success()
    {
        var outputs = new List<string>();
        var console = new MockConsole(new List<string> {"September 21st 1975", "21-09-1975"}, outputs);

        var result = ReadDateOfBirthRetry(console, maxAttempt: 2);

        result.Should().Be(new DateOnly(1975, 9, 21));
        Assert.Collection(outputs,
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"));
    }

    [Fact]
    public void ReadDateOfBirthRetry_example_invalid_input()
    {
        var outputs = new List<string>();
        const string invalidAttempt = "September 21st 1975";
        var console = new MockConsole(new List<string> {invalidAttempt}, outputs);

        var result = Try(() => ReadDateOfBirthRetry(console, maxAttempt: 1));

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));

        // check that the error message is the same as 'ReadDateOfBirth'
        var console2 = new MockConsole(new List<string> {invalidAttempt}, new List<string>());
        var result2 = Try(() => ReadDateOfBirth(console2));
        Assert.Equal(((Failure<DateOnly>) result).Exception.Message, ((Failure<DateOnly>) result2).Exception.Message);
    }

    [Fact]
    public void TestReadUser_with_retry()
    {
        var inputs = new List<string> {"Rastislav", "September 21th 1975", "1975-09-21", "21-09-1975", "No", "Never", "N"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var now = new DateTime(2006, 1, 2, 3, 4, 5);
        var clock = new MockClock(now);
        var expected = new User(
            Name: "Rastislav",
            DateOfBirth: new DateOnly(1975, 9, 21),
            SubscribedToMailingList: false,
            CreatedAt: now);

        var result = ReadUser(console, clock);

        result.Should().Be(expected);
    }


}