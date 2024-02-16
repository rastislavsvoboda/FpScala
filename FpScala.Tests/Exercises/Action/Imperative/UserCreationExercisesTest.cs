namespace FpScala.Tests.Exercises.Action.Imperative;

using FluentAssertions;
using Xunit;
using static FpScala.Exercises.Action.Imperative.UserCreationApp;

public class UserCreationExercisesTest
{
    [Fact]
    public void TestParseYesNo()
    {
        ParseYesNo("Y").Should().BeTrue();
        ParseYesNo("N").Should().BeFalse();
        Assert.Throws<ArgumentException>(() => ParseYesNo("Never"));
    }


    [Fact(Skip = "ignore")]
    public void ReadSubscriberToMailingList_Example()
    {
        var inputs = new List<string> {"N"};
        var outputs = new List<string>();
        var console = new MockConsole(inputs, outputs);
        var result = ReadSubscribeToMailingList();

        result.Should().BeFalse();
        Assert.Collection(outputs,
            item => item.Should().Be("Would like to subscribe to our mailing list? [Y/N]"));
    }
}