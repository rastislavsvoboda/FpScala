using FluentAssertions;
using FsCheck;
using Xunit;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class FsCheckExploratoryTest
{
    [Fact]
    public void Elements_pick_one_item_from_given_sequence()
    {
        var elements = new[] {"a", "b", "c", "d", "e"};
        var elementsGen = Gen.Elements(elements);

        Prop.ForAll(
                elementsGen.ToArbitrary(),
                elm =>
                {
                    elm.Should().NotBeEmpty();
                    elements.Contains(elm).Should().BeTrue();
                })
            .QuickCheckThrowOnFailure();
    }
}