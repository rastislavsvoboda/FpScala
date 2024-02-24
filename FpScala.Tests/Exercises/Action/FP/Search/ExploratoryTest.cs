using FluentAssertions;
using FpScala.Exercises.Action.FP;
using FsCheck;
using Xunit;
using Random = System.Random;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class ExploratoryTest
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

    [Fact]
    public void Comparing_sequence_with_shuffled_version()
    {
        var items1 = new[] {"a", "1", "3", "c", "2", "b"};
        var items2 = items1.Shuffle(new Random()).ToArray();

        items1.SequenceEqual(items2).Should().BeFalse(because: "it compares also order"); // assume shuffle will not produce same order
        items1.Should().BeEquivalentTo(items2, because: "it compares only elements");
    }
    
    [Fact]
    public void Shuffle()
    {
        Prop.ForAll(
            Arb.Generate<int[]>().ToArbitrary(),
            items =>
            {
                var shuffledItems = items.Shuffle(new Random());
                items.Should().BeEquivalentTo(shuffledItems, because: "it compares only elements");
                //  var items = xs.To
            }).QuickCheckThrowOnFailure();
    }
}