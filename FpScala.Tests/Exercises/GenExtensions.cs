using FsCheck;
using Random = FsCheck.Random;

namespace FpScala.Tests.Exercises.Action.FP.Search;

public static class GenExtensions
{
    public static T Generate<T>(this Gen<T> gen) =>
        gen.Eval(100, Random.newSeed());
}