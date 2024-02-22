namespace FpScala.Exercises.Action.FP.Search;

public interface ISearchFlightService
{
    IO<SearchResult> Search(Airport from, Airport to, DateTime date);
}