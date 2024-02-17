using static PreludeLib.Utils;

namespace FpScala.Tests.Exercises.Action.FP;

using FpScala.Exercises.Action.FP;
using FluentAssertions;
using Xunit;

public class UserCreationExercisesTest
{
    [Fact]
    public void Test_ParseYesNo()
    {
        UserCreationService.ParseYesNo("Y").Should().BeTrue();
        UserCreationService.ParseYesNo("N").Should().BeFalse();
        Assert.Throws<ArgumentException>(() => UserCreationService.ParseYesNo("Never"));
    }

    [Fact]
    public void Test_ReadName()
    {
        var console = new MockConsole(new List<string> {"Rastislav"}, new List<string>());
        var service = new UserCreationService(console, DefaultClock());

        var result = service.ReadName().UnsafeRun();

        result.Should().Be("Rastislav");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadSubscriberToMailingList_example_success(bool expectedValue)
    {
        var inputs = new List<string> {UserCreationService.FormatYesNo(expectedValue)};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = service.ReadSubscribeToMailingList().UnsafeRun();

        result.Should().Be(expectedValue);
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"));
    }

    [Fact]
    public void ReadSubscriberToMailingList_example_failure()
    {
        var inputs = new List<string> {"incorrect"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadSubscribeToMailingList().UnsafeRun());

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));
    }

    [Fact]
    public void ReadDateOfBirth_example_success()
    {
        var date = new DateOnly(1975, 9, 21);
        var inputs = new List<string> {UserCreationService.FormatDate(date)};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = service.ReadDateOfBirth().UnsafeRun();

        result.Should().Be(date);
        Assert.Collection(outputs,
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"));
    }

    [Fact]
    public void ReadDateOfBirth_example_failure()
    {
        var inputs = new List<string> {"21/09/1975"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadDateOfBirth().UnsafeRun());

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));
    }

    [Fact]
    public void Test_ReadUser_with_correct_inputs()
    {
        var inputs = new List<string> {"Rastislav", "21-09-1975", "Y"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var now = new DateTime(2006, 1, 2, 3, 4, 5);
        var clock = new MockClock(now);
        var service = new UserCreationService(console, clock);
        var expected = new User("Rastislav", new DateOnly(1975, 9, 21), true, now);

        var result = service.ReadUser().UnsafeRun();

        result.Should().Be(expected);
        Assert.Collection(outputs,
            item => item.Should().Be("What's your name?"),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be($"User is {expected}"));
    }

    // PART 2: Error handling

    [Fact]
    public void Test_ReadUser_with_retry()
    {
        var inputs = new List<string> {"Rastislav", "September 21th 1975", "1975-09-21", "21-09-1975", "No", "Never", "N"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var now = new DateTime(2006, 1, 2, 3, 4, 5);
        var clock = new MockClock(now);
        var service = new UserCreationService(console, clock);
        var expected = new User(
            Name: "Rastislav",
            DateOfBirth: new DateOnly(1975, 9, 21),
            SubscribedToMailingList: false,
            CreatedAt: now);

        var result = service.ReadUser().UnsafeRun();

        result.Should().Be(expected);
        Assert.Collection(outputs,
            item => item.Should().Be("What's your name?"),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""),
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """),
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be($"User is {expected}"));
    }

    private IClock DefaultClock() => new MockClock(new DateTime(2000, 1, 1));
}