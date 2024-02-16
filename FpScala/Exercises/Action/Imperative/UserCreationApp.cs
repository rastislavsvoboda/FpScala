namespace FpScala.Exercises.Action.Imperative;

using static Console;

public static class UserCreationApp
{
    public static User ReadUser(IConsole console, IClock clock)
    {
        var name = ReadName(console);
        var dateOfBirth = ReadDateOfBirth(console);
        var subscribed = ReadSubscribeToMailingList(console);
        var now = clock.Now;
        var user = new User(name, dateOfBirth, subscribed, now);
        WriteLine($"User is ${user}");
        return user;
    }

    private static string ReadName(IConsole console)
    {
        console.WriteLine("What's your name?");
        var name = console.ReadLine();
        return name;
    }

    public static bool ReadSubscribeToMailingList(IConsole console)
    {
        console.WriteLine("Would like to subscribe to our mailing list? [Y/N]");
        var line = console.ReadLine();
        return ParseYesNo(line);
    }

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

    public static DateOnly ReadDateOfBirth(IConsole console)
    {
        console.WriteLine("What's your date of birth? [dd-mm-yyyy]");
        var line = console.ReadLine();
        var dateOfBirth = DateOnly.ParseExact(line, "dd-MM-yyyy");
        return dateOfBirth;
    }

}