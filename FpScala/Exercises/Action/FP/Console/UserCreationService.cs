using PreludeLib;

namespace FpScala.Exercises.Action.FP.Console;

public class UserCreationService
{
    private readonly IConsole _console;
    private readonly IClock _clock;

    public UserCreationService(IConsole console, IClock clock)
    {
        _console = console;
        _clock = clock;
    }

    // ugly syntax via nested FlatMap
    public IO<User> ReadUserEx() =>
        ReadName()
            .FlatMap(name => ReadDateOfBirth()
                .FlatMap(dateOfBirth => ReadSubscribeToMailingList()
                    .FlatMap(subscribed => _clock.Now.FlatMap(now =>
                    {
                        var user = new User(name, dateOfBirth, subscribed, now);
                        return WriteLine($"User is {user}").Map(_ => user);
                    }))));

    public IO<User?> ReadUser() =>
        from name in ReadName()
        from dateOfBirth in ReadDateOfBirth().Retry(maxAttempt: 3)
        from subscribed in ReadSubscribeToMailingList().Retry(maxAttempt: 3)
        from now in _clock.Now
        let user = new User(name, dateOfBirth, subscribed, now)
        from _ in WriteLine($"User is {user}")
        select user;

    public IO<string> ReadName() =>
        from _ in WriteLine("What's your name?")
        from name in ReadLine()
        select name;

    public IO<DateOnly> ReadDateOfBirth()
    {
        var printError = WriteLine("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001""");

        return from _ in WriteLine("What's your date of birth? [dd-mm-yyyy]")
            from line in ReadLine()
            from dateOfBirth in ParseDateOfBirth(line).OnError(_ => printError)
            select dateOfBirth;
    }

    public IO<DateOnly> ParseDateOfBirth(string line) =>
        new(() => ParseDate(line));

    public IO<bool> ReadSubscribeToMailingList()
    {
        var printError = WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """);

        return from _ in WriteLine("Would like to subscribe to our mailing list? [Y/N]")
            from line in ReadLine()
            from yesNo in ParseLineToBoolean(line).OnError(_ => printError)
            select yesNo;
    }

    public IO<bool> ParseLineToBoolean(string line) =>
        new(() => ParseYesNo(line));

    public static bool ParseYesNo(string? line) =>
        line switch
        {
            "Y" => true,
            "N" => false,
            _ => throw new ArgumentException($"""Expected "Y" or "N" but received {line}""")
        };

    public static string FormatYesNo(bool value) =>
        value
            ? "Y"
            : "N";

    public static DateOnly ParseDate(string line) =>
        DateOnly.ParseExact(line, "dd-MM-yyyy");

    public static string FormatDate(DateOnly date) =>
        date.ToString("dd-MM-yyyy");

    // helpers using _console simulating import in Scala
    // or "using static" in C# if then would be static methods
    private IO<string> ReadLine() =>
        _console.ReadLine();

    private IO<Unit> WriteLine(string message) =>
        _console.WriteLine(message);
}