namespace FpScala.Exercises.Action.FP;

using static Console;
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

    public IO<User> ReadUser() =>
        new(() =>
        {
            // TODO: do with retry
            var name = ReadName().UnsafeRun();
//        var dateOfBirth = Retry(3, ReadDateOfBirth);
            var dateOfBirth = ReadDateOfBirth().UnsafeRun();
//        var subscribed = Retry(3, ReadSubscribeToMailingList);
            var subscribed = ReadSubscribeToMailingList().UnsafeRun();
            var now = _clock.Now;
            var user = new User(name, dateOfBirth, subscribed, now);
            _console.WriteLine($"User is {user}");
            return user;
        });

    public IO<string> ReadName() =>
        new(() =>
        {
            _console.WriteLine("What's your name?").UnsafeRun();
            var name = _console.ReadLine().UnsafeRun();
            return name;
        });

    public IO<DateOnly> ReadDateOfBirth() =>
        new(() =>
        {
            _console.WriteLine("What's your date of birth? [dd-mm-yyyy]").UnsafeRun();
            var line = _console.ReadLine().UnsafeRun();
            // TODO
            var r = ParseDate(line);
            return r;
            // return OnError(
            //     func: () => ParseDate(line),
            //     cleanup: _ => _console.WriteLine("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));
        });

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
        new(() =>
        {
            _console.WriteLine("Would like to subscribe to our mailing list? [Y/N]").UnsafeRun();
            var line = _console.ReadLine().UnsafeRun();
            var r = ParseYesNo(line);
            return r;
            // TODO
            // return OnError(
            //     func: () => ParseYesNo(line),
            //     cleanup: _ => _console.WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));
        });

    // public bool ReadSubscribeToMailingListRetry(int maxAttempt) =>
    //     Retry(maxAttempt, () =>
    //     {
    //         _console.WriteLine("Would like to subscribe to our mailing list? [Y/N]");
    //         var line = _console.ReadLine();
    //         return OnError(
    //             func: () => ParseYesNo(line),
    //             cleanup: _ => _console.WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));
    //     });

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
}