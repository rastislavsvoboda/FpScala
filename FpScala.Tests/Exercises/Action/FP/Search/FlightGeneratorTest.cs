using FluentAssertions;
using FsCheck;
using Xunit;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class FlightGeneratorTest
{
    [Fact]
    public void Flight_has_defined_airline_and_different_from_and_to_airports_properties()
    {
        var flightGen = FlightGenerator.Generate();

        Prop.ForAll(
                flightGen,
                flight =>
                {
                    flight.Airline.Should().NotBeEmpty();
                    flight.Duration.Should().BeGreaterThan(TimeSpan.Zero);
                    Assert.NotEqual(flight.From, flight.To);
                })
            .VerboseCheckThrowOnFailure();
    }
}