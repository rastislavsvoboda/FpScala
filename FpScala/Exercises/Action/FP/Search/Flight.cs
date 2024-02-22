namespace FpScala.Exercises.Action.FP.Search;

public record Flight(
    string FlightId,
    string Airline,
    Airport From,
    Airport To,
    DateTime DepartureAt,
    TimeSpan Duration,
    int NumberOfStops, // 0 for direct flight
    Decimal UnitPrice, // in Dollars
    string RedirectLink)
{
    private DateOnly DepartureDate =>
        new DateOnly(DepartureAt.Date.Year, DepartureAt.Date.Month, DepartureAt.Date.Day);
}