using PreludeLib;
using static PreludeLib.Utils;

namespace FpScala.Tests.Exercises.Action.FP;

using FpScala.Exercises.Action.FP;
using FluentAssertions;
using Xunit;

public class UserCreationExercisesTest
{
    [Fact]
    public void TestParseYesNo()
    {
        UserCreationService.ParseYesNo("Y").Should().BeTrue();
        UserCreationService.ParseYesNo("N").Should().BeFalse();
        Assert.Throws<ArgumentException>(() => UserCreationService.ParseYesNo("Never"));
    }

    [Fact]
    public void TestReadName()
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
    public void TestReadUser()
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

/*
    // PART 2: Error handling

    [Fact]
    public void ReadSubscribeToMailingListRetry_negative_maxAttempt()
    {
        var console = new MockConsole(new List<string>(), new List<string>());
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadSubscribeToMailingListRetry(maxAttempt: -1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReadSubscribeToMailingListRetry_example_success()
    {
        var outputs = new List<string>();
        var console = new MockConsole(new List<string> {"Never", "N"}, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = service.ReadSubscribeToMailingListRetry(maxAttempt: 2);

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
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadSubscribeToMailingListRetry(maxAttempt: 1));

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"),
            item => item.Should().Be("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));

        // check that the error message is the same as 'ReadSubscribeToMailingList'
        var console2 = new MockConsole(new List<string> {"Never"}, new List<string>());
        var service2 = new UserCreationService(console2, DefaultClock());
        var result2 = Try(() => service2.ReadSubscribeToMailingList());
        Assert.Equal(((Failure<bool>) result).Exception.Message, ((Failure<bool>) result2).Exception.Message);
    }

    [Fact]
    public void ReadDateOfBirthRetry_negative_maxAttempt()
    {
        var console = new MockConsole(new List<string>(), new List<string>());
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadDateOfBirthRetry(maxAttempt: -1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReadDateOfBirthRetry_example_success()
    {
        var outputs = new List<string>();
        var console = new MockConsole(new List<string> {"September 21st 1975", "21-09-1975"}, outputs);
        var service = new UserCreationService(console, DefaultClock());

        var result = service.ReadDateOfBirthRetry(maxAttempt: 2);

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
        var service = new UserCreationService(console, DefaultClock());

        var result = Try(() => service.ReadDateOfBirthRetry(maxAttempt: 1));

        result.IsFailure.Should().BeTrue();
        Assert.Collection(outputs,
            item => item.Should().Be("What's your date of birth? [dd-mm-yyyy]"),
            item => item.Should().Be("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));

        // check that the error message is the same as 'ReadDateOfBirth'
        var console2 = new MockConsole(new List<string> {invalidAttempt}, new List<string>());
        var service2 = new UserCreationService(console2, DefaultClock());
        var result2 = Try(() => service2.ReadDateOfBirth());
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
        var service = new UserCreationService(console, clock);
        var expected = new User(
            Name: "Rastislav",
            DateOfBirth: new DateOnly(1975, 9, 21),
            SubscribedToMailingList: false,
            CreatedAt: now);

        var result = service.ReadUser();

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
*/
    private IClock DefaultClock() => new MockClock(new DateTime(2000, 1, 1));
}