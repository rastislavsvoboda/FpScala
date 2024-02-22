namespace FpScala.Exercises.Action.FP.Search;

public interface ISearchFlightClient
{
    IO<IEnumerable<Flight>> Search(Airport from, Airport to, DateTime date);
}