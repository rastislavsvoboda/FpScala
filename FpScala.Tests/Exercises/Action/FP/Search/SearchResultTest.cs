using FluentAssertions;
using FpScala.Exercises.Action.FP.Search;
using Xunit;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public class SearchResultTest
{
    [Fact]
    public void SearchResult_constructor_sorts_and_removes_duplicates()
    {
        var now = DateTime.Now;

        var flight1a = new Flight("1", "BA", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(100), 0, 89.5m, "");
        var flight1b = new Flight("1", "BA", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(100), 0, 91.5m, "");
        var flight2 = new Flight("2", "LH", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(105), 0, 96.5m, "");
        var flight3 = new Flight("3", "BA", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(140), 1, 234.0m, "");
        var flight4 = new Flight("4", "LH", Airport.ParisOrly, Airport.LondonGatwick, now, TimeSpan.FromMinutes(210), 2, 55.5m, "");

        var result = new SearchResult(flight2, flight4, flight3, flight1b, flight1a);

        Assert.Collection(result.Flights,
            item => item.Should().Be(flight1a),
            item => item.Should().Be(flight2),
            item => item.Should().Be(flight3),
            item => item.Should().Be(flight4));
    }
}