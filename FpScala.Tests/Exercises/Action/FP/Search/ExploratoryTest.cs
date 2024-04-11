using System.Diagnostics;
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

    [Fact]
    public void Test_sequential()
    {
        var items = Enumerable.Range(1, 100_000_000).Select(n => $"this is my item {n}").ToList();

        var sw = new Stopwatch();
        sw.Start();
        var processed = items
            .Select((x, i) => $"{i}. {x.ToUpper()}")
            .ToList();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Test_parallel()
    {
        var items = Enumerable.Range(1, 100_000_000).Select(n => $"this is my item {n}").ToList();

        var sw = new Stopwatch();
        sw.Start();
        var processed = items
            .AsParallel()
            .Select((x, i) => $"{i}. {x.ToUpper()}")
            .ToList();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void Test_parallel_with_zip()
    {
        var items = Enumerable.Range(1, 100_000_000).Select(n => $"this is my item {n}").ToList();
    
        var sw = new Stopwatch();
        sw.Start();
        var processed = items
            .AsParallel()
            .Zip(Enumerable.Range(1, items.Count).AsParallel(), (x, i) => $"{i}. {x.ToUpper()}")
            .ToList();
        sw.Stop();
    
        sw.ElapsedMilliseconds.Should().BeGreaterThan(0);
    }
}