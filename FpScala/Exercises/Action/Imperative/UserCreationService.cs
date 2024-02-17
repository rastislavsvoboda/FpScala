namespace FpScala.Exercises.Action.Imperative;

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

    public User ReadUser()
    {
        var name = ReadName();
        var dateOfBirth = ReadDateOfBirthRetry(maxAttempt: 3);
        var subscribed = ReadSubscribeToMailingListRetry(maxAttempt: 3);
        var now = _clock.Now;
        var user = new User(name, dateOfBirth, subscribed, now);
        WriteLine($"User is ${user}");
        return user;
    }

    private string ReadName()
    {
        _console.WriteLine("What's your name?");
        var name = _console.ReadLine();
        return name;
    }

    public DateOnly ReadDateOfBirth()
    {
        _console.WriteLine("What's your date of birth? [dd-mm-yyyy]");
        var line = _console.ReadLine();
        var dateOfBirth = ParseDate(line);
        return dateOfBirth;
    }

    public DateOnly ReadDateOfBirthRetry(int maxAttempt) =>
        Retry(maxAttempt, () =>
        {
            _console.WriteLine("What's your date of birth? [dd-mm-yyyy]");
            var line = _console.ReadLine();
            return OnError(
                func: () => ParseDate(line),
                cleanup: _ => _console.WriteLine("""Incorrect format, for example enter "18-03-2001" for 18th of March 2001"""));
        });

    public bool ReadSubscribeToMailingList()
    {
        _console.WriteLine("Would like to subscribe to our mailing list? [Y/N]");
        var line = _console.ReadLine();
        return ParseYesNo(line);
    }

    public bool ReadSubscribeToMailingListRetry(int maxAttempt) =>
        Retry(maxAttempt, () =>
        {
            _console.WriteLine("Would like to subscribe to our mailing list? [Y/N]");
            var line = _console.ReadLine();
            return OnError(
                func: () => ParseYesNo(line),
                cleanup: _ => _console.WriteLine("""Incorrect format, enter "Y" for "Yes", "N" for "No" """));
        });

    public static bool ParseYesNo(string? line) =>
        line switch
        {
            "Y" => true,
            "N" => false,
            _ => throw new ArgumentException($"""Expected "Y" or "N" but received {line}""")
        };

    public static string FormatYesNo(bool value)
        => value
            ? "Y"
            : "N";

    public static DateOnly ParseDate(string line) =>
        DateOnly.ParseExact(line, "dd-MM-yyyy");
}