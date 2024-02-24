using FluentAssertions;
using FpScala.Exercises.Action.FP;
using FpScala.Exercises.Action.FP.Search;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class SearchFlightServiceTest
{
    [Fact]
    public void FromTwoClients_example()
    {
        var now = DateTime.Now;
        var today = DateTime.Today;

        var flight1 = new Flight("1", "BA", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(100), 0, 89.5m, "");
        var flight2 = new Flight("2", "LH", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(105), 0, 96.5m, "");
        var flight3 = new Flight("3", "BA", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(140), 1, 234.0m, "");
        var flight4 = new Flight("4", "LH", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(210), 2, 55.5m, "");

        var client1 = new MockSearchFlightClient(new IO<IEnumerable<Flight>>(() => new[] {flight3, flight1}));
        var client2 = new MockSearchFlightClient(new IO<IEnumerable<Flight>>(() => new[] {flight2, flight4}));

        var service = new FromTwoClientsSearchFlightService(client1, client2);

        var result = service.Search(Airport.ParisOrly, Airport.LondonGatwick, today).UnsafeRun();

        Assert.Collection(result.Flights,
            item => item.Should().Be(flight1),
            item => item.Should().Be(flight2),
            item => item.Should().Be(flight3),
            item => item.Should().Be(flight4));
    }

    [Property(Arbitrary = new[] {typeof(MockSearchFlightClientGenerator)})]
    public void FromTwoClients_should_handle_errors_gracefully(MockSearchFlightClient client1, MockSearchFlightClient client2)
    {
        var today = DateTime.Today;
        var service = new FromTwoClientsSearchFlightService(client1, client2);

        var result = service.Search(Airport.ParisOrly, Airport.LondonGatwick, today).Attempt().UnsafeRun();

        result.IsSuccess.Should().BeTrue();
    }

    [Property]
    public void Always_passing_client()
    {
        var client = MockSearchFlightClientGenerator.PassingClientGen.Generate();
        var today = DateTime.Today;

        var result = client.Search(Airport.ParisOrly, Airport.LondonGatwick, today).Attempt().UnsafeRun();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Always_passing_client2()
    {
        Prop.ForAll(
            MockSearchFlightClientGenerator.PassingClientGen.ToArbitrary(),
            client =>
            {
                var today = DateTime.Today;

                var result = client.Search(Airport.ParisOrly, Airport.LondonGatwick, today).Attempt().UnsafeRun();

                return result.IsSuccess.ToProperty();
            }
        ).VerboseCheckThrowOnFailure();
    }

    [Fact]
    public void Always_failing_client()
    {
        Prop.ForAll(
            MockSearchFlightClientGenerator.FailingClientGen.ToArbitrary(),
            client =>
            {
                var today = DateTime.Today;

                var result = client.Search(Airport.ParisOrly, Airport.LondonGatwick, today).Attempt().UnsafeRun();

                return result.IsFailure.ToProperty();
            }
        ).VerboseCheckThrowOnFailure();
    }

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