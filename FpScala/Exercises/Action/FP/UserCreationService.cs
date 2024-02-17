using PreludeLib;

namespace FpScala.Exercises.Action.FP;

using static PreludeLib.Utils;

public class UserCreationService
{
    private readonly IConsole _console;
    private readonly IClock _clock;

    public UserCreationService(IConsole console, IClock clock)
    {
        _console = console;
        _clock = clock;
    }

//     public IO<User> ReadUser() =>
//         new(() =>
//         {
//             // TODO: do with retry
//
//             var name = ReadName().UnsafeRun();
// //        var dateOfBirth = Retry(3, ReadDateOfBirth);
//             var dateOfBirth = ReadDateOfBirth().UnsafeRun();
// //        var subscribed = Retry(3, ReadSubscribeToMailingList);
//             var subscribed = ReadSubscribeToMailingList().UnsafeRun();
//             var now = _clock.Now;
//             var user = new User(name, dateOfBirth, subscribed, now);
//             _console.WriteLine($"User is {user}");
//             return user;
//         });

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

    public IO<User> ReadUser() =>
        from name in ReadName()
        from dateOfBirth in ReadDateOfBirth()
        from subscribed in ReadSubscribeToMailingList()
        from now in _clock.Now
        let user = new User(name, dateOfBirth, subscribed, now)
        from _ in WriteLine($"User is {user}")
        select user;

    public IO<string> ReadName() =>
        from _ in WriteLine("What's your name?")
        from name in ReadLine()
        select name;

    public IO<DateOnly> ReadDateOfBirth() =>
        from _ in WriteLine("What's your date of birth? [dd-mm-yyyy]")
        from line in ReadLine()
        from dateOfBirth in ParseDateOfBirth(line).OnError(_ => WriteLine("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""))
        select dateOfBirth;

    public IO<DateOnly> ParseDateOfBirth(string line) =>
        new(() => ParseDate(line));

    // public DateOnly ReadDateOfBirthRetry(int maxAttempt) =>
    //     Retry(maxAttempt, () =>
    //     {
    //         _console.WriteLine("What's your date of birth? [dd-mm-yyyy]");
    //         var line = _console.ReadLine();
    //         return OnError(
    //             func: () => ParseDate(line),
    //             cleanup: _ => _console.WriteLine("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));
    //     });

    public IO<bool> ReadSubscribeToMailingList() =>
        from _ in WriteLine("Would like to subscribe to our mailing list? [Y/N]")
        from line in ReadLine()
        from yesNo in ParseLineToBoolean(line).OnError(_ => WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """))
        select yesNo;

    // TODO

    // public bool ReadSubscribeToMailingListRetry(int maxAttempt) =>
    //     Retry(maxAttempt, () =>
    //     {
    //         _console.WriteLine("Would like to subscribe to our mailing list? [Y/N]");
    //         var line = _console.ReadLine();
    //         return OnError(
    //             func: () => ParseYesNo(line),
    //             cleanup: _ => _console.WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));
    //     });

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